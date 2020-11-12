using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using CryptoBot.Core.Integrations.MessagingApps;
using CryptoBot.Core.Trading;
using CryptoBot.Core.Utils;
using CryptoBot.Database;
using CryptoBot.Model.Common;
using CryptoBot.Model.Domain.Bot;
using CryptoBot.Model.Domain.Market;
using CryptoBot.Model.Domain.Trading;
using CryptoBot.Model.Exchanges;
using Newtonsoft.Json;
using NLog;
using Telegram.Bot;

namespace CryptoBot.Core.Ordering
{
    public class OrderManager : IOrderManager
    {
        private readonly Logger _logger;
        private readonly CryptoBotDbContext _dbContext;
        private readonly Bot _inMemoryBot;
        private readonly ITrader _trader;
        private readonly IMapper _mapper;
        private readonly IMessagingApp _messagingApp;
        private readonly bool _simulated;

        public OrderManager(Bot bot, ITrader trader, CryptoBotDbContext dbContext, Logger logger, IMapper mapper, IMessagingApp messagingApp)
        {
            _logger = logger;
            _mapper = mapper;
            _messagingApp = messagingApp;
            _dbContext = dbContext;
            _inMemoryBot = bot;
            _trader = trader;
            _simulated = _inMemoryBot.User.ApiSettings.Single(x => x.Exchange.ExchangeId == _inMemoryBot.Exchange.ExchangeId).Simulated;
        }

        public async void Buy(Candle candle)
        {
            Log($"Placing order.. {_inMemoryBot.Name} for  {candle.ClosePrice}");

            try
            {
                ExchangeOrderResult orderResult = await _trader.Buy(_inMemoryBot, candle.ClosePrice);


                if (orderResult != null)
                    await ProcessOrderResult(orderResult, candle);
            }
            catch (Exception e)
            {
                _logger.Error(e);
            }
        }

        public async void Sell(Candle candle)
        {
            try
            {
                var orderResult = await _trader.Sell(_inMemoryBot, candle.ClosePrice);

                if (orderResult == null) return;
                await ProcessOrderResult(orderResult, candle);

                Log($"SELL order placed for {orderResult.AveragePrice}");
            }
            catch (Exception e)
            {
                _logger.Error(e);
            }
        }

        private async Task ProcessOrderResult(ExchangeOrderResult orderResult, Candle candle)
        {
            // if its filled straight away, nice, save it & return
            if (orderResult.Result == ExchangeAPIOrderResult.Filled)
            {
                Log("Order filled, saving position");

                SavePosition(SaveOrder(orderResult, GetTimeStamp(candle.Timestamp)));
            }

            // GDAX and possibly other exchanges will return a "Pending" state to give thm time to fill the order
            // we need to check back with them and deal with any issues
            if (orderResult.Result != ExchangeAPIOrderResult.Filled)
            {
                if (!_simulated)
                {
                    Log("Waiting 3 seconds to check order status...");
                    Pause(3);
                }

                ExchangeOrderResult checkOrderResult = CheckOrderStatus(orderResult.OrderId).Result;
                    // had to do .Result for debugging tests

                Log($"Result .. {checkOrderResult.Result}");

                // most likely filled by this stage
                if (checkOrderResult.Result == ExchangeAPIOrderResult.Filled)
                {
                    Log("Order filled, saving position");
                    SavePosition(
                        SaveOrder(checkOrderResult, GetTimeStamp(candle.Timestamp))
                    );
                }
                else // if not need to handle it
                {
                    Log(null);
                    Log("Order not filled...");
                    Log(null);

                    HandleNonFilledOrder(checkOrderResult, candle);
                }
            }
        }

        private void Log(string message)
        {
            if (String.IsNullOrEmpty(message)) { _logger.Info(""); return; }

            var body =
                $"[OrderManager] - Bot: {_inMemoryBot.Name}, Pair: {_inMemoryBot.Coin.Code}-{_inMemoryBot.BaseCoin.Code} - {message}";

            _logger.Info(body);

            
        }

        private Order SaveOrder(ExchangeOrderResult orderResult, DateTime timestamp)
        {
            var order = _mapper.Map(orderResult, new Order());

            order.TimeStamp = timestamp;
            order.BotId = _inMemoryBot.BotId;
            order.OurRef = Guid.NewGuid();

            try
            {
                _dbContext.Orders.Add(order);
                _dbContext.SaveChanges();
            }
            catch (Exception e)
            {
                _logger.Error(e);
            }

            Log($"Saved order to db order number {order.OrderId}");

            return order;
        }
        
        private void SavePosition(Order order)
        {
            // Get a fresh bot from db with its own context 
            var dbBot = _dbContext
                .Bots
                .Single(x => x.BotId == _inMemoryBot.BotId);

            Position currentPosition = null;

            if (dbBot.CurrentPositionId != null)
                currentPosition = _dbContext.Positions.Single(x => x.PositionId == dbBot.CurrentPositionId);

            if (order.Side == Enumerations.OrderSideEnum.Buy) // create a new position
            {
                if (currentPosition == null)
                {
                    var newPosition = _dbContext.Positions.Add(new Position
                    {
                        BotId = _inMemoryBot.BotId,
                        Side = Enumerations.OrderSideEnum.Buy,
                        BuyPrice = order.Price,
                        BuyTimeStamp = order.TimeStamp.GetValueOrDefault(),
                        BuyRequestExchangeReference = order.ExchangeOrderId,
                        Status = Enumerations.PositionStatusEnum.Bought,                        
                        Quantity = order.QuantityFilled,
                        Orders = new List<Order>
                        {
                            order
                        }
                    });

                    SetPositionCommission(order, newPosition);

                    _dbContext.SaveChanges();
                    dbBot.CurrentPositionId = newPosition.PositionId;
                    _dbContext.SaveChanges();

                    _inMemoryBot.CurrentPosition = newPosition;
                    _inMemoryBot.CurrentPositionId = newPosition.PositionId;

                    Log(null);
                    Log($"Created new position {newPosition.PositionId}");
                    Log(null);
                    SendMessageApp(
                        $"Buy order filled for {_inMemoryBot.Name} & new position created Id {newPosition.PositionId}. Filled {order.QuantityFilled} of {_inMemoryBot.Coin.Code} at a price of {order.Price}");
                }
                else // bot is in a Buy position and already has an order against it, what's the problem
                {
                    Log($"{_inMemoryBot.Name} is in a BUY state, but already has a position against it");
                }
            }
            else // update current position
            {
                if (dbBot.CurrentPositionId == null)
                {
                    throw new Exception($"Bot {_inMemoryBot.Name} is trying to close a current position, but it is null");
                }

                currentPosition =
                    _dbContext
                    .Positions
                    .Include("Orders")
                    .Single(x => x.PositionId == dbBot.CurrentPositionId);
                
                Log($"Updating current position {currentPosition.PositionId}");
                
                currentPosition.Status = Enumerations.PositionStatusEnum.Sold;
                currentPosition.Side = Enumerations.OrderSideEnum.Sell; // Side field probably needs to be removed, a position is in a state Bought/Sold, not a side. An order has a side Buy/Sell
                currentPosition.SellPrice = order.Price;
                currentPosition.SellTimeStamp = order.TimeStamp;
                currentPosition.SellRequestExchangeReference = order.ExchangeOrderId;

                SetPositionCommission(order, currentPosition);
                SetCurrentPositionGrossProfit(order.Price, currentPosition);
                SetCurrentPositionNetProfit(order.Price, currentPosition);

                Log(null);
                Log($"Closing a position, for a net profit of {currentPosition.NetProfit} {currentPosition.NetProfitPercent}%");
                Log(null);

                if (dbBot.Accumulator)
                {
                    dbBot.Amount += (decimal)currentPosition.GrossProfit.GetValueOrDefault();
                    Log($"Increasing bot amount for next trade by {currentPosition.GrossProfit.GetValueOrDefault()}");
                }

                currentPosition.Orders.Add(order);

                dbBot.CurrentPosition = null;
                dbBot.CurrentPositionId = null;
                // also the in memory bot
                _inMemoryBot.CurrentPosition = null;
                _inMemoryBot.CurrentPositionId = null;

                _dbContext.SaveChanges();

                SendMessageApp(
                    $"Sell order filled for {_inMemoryBot.Name}, Filled {order.QuantityFilled} of {_inMemoryBot.Coin.Code} at a price of {order.Price}. Net profit {currentPosition.NetProfit}, {currentPosition.NetProfitPercent}%");
            }
        }

        private async Task<ExchangeOrderResult> CheckOrderStatus(string exchangeOrderId)
        {
            return await _trader.GetOrderDetails(exchangeOrderId);
        }

        private async void HandleNonFilledOrder(ExchangeOrderResult orderResult, Candle candle)
        {

            ExchangeOrderResult result = null;

            Ticker ticker;

            switch (orderResult.Result)
            {
                case ExchangeAPIOrderResult.FilledPartially:

                    Log($"Partially filled order quantity filled {orderResult.AmountFilled} of {orderResult.Amount}");

                    /*
                     
                     PARTIALLY FILLED
                     
                     1. Save the initial order and what was filled
                     2. Check in thrity seconds for any more filling of it
                     3. Add a new order against this position 
                     4. If not filled after two more queries, cancel it
                                          
                     */

                    // 1. Save 

                    SavePosition(SaveOrder(orderResult, GetTimeStamp(candle.Timestamp)));

                    // 2. Check in 30 seconds twice

                    int counter = 0;

                    while (counter < 3)
                    {
                        Log("Waiting 30 seconds..");

                        Pause(30);
                        ++counter;

                        result = await CheckOrderStatus(orderResult.OrderId);

                        if (result.Result == ExchangeAPIOrderResult.Filled) // if status now is now filled great
                        {
                            Log($"Remaining quanity of order filled");
                            SaveOrder(orderResult, GetTimeStamp(candle.Timestamp));
                            UpdatePositionQuantity();
                            return;
                        }
                        else
                        {
                            if (counter == 2) // this is the last time & it wasn't filled
                            {
                                Log($"Remaining quanity of order not filled after waiting 90 seconds, cancelling the order, exchange order id {orderResult.OrderId}, message: {orderResult.Message}");

                                _trader.CancelOrder(orderResult.OrderId);

                                // TODO: Could look at putting in another order for the outstanding quantity
                                // Get the minimum quantity for the currency
                                // if the outstanding quantity is more than the minimum quantity 
                                // get the current price
                                // create a new order for the remaining quantity
                                // update the position with the new quantity received
                            }
                        }

                    }


                    break;

                case ExchangeAPIOrderResult.Canceled:

                    /*
                     
                     CANCELLED by exchange
                     
                     1. Wait 10 seconds
                     3. Check price now
                     4. Issue a new Buy
                                          
                     */

                    Log($"Order cancelled by exchange for orderId {orderResult.OrderId}, waiting 10 seconds and retrying, message: {orderResult.Message}");

                    Log("Waiting 10 seconds");
                    Pause(10);

                    ticker = _trader.GetLatestTicker(_inMemoryBot.BaseCoin, _inMemoryBot.Coin);

                    if (orderResult.IsBuy)
                        Buy(new Candle() { Timestamp = DateTime.Now, ClosePrice = ticker.Ask });
                    else
                        Sell(new Candle() { Timestamp = DateTime.Now, ClosePrice = ticker.Bid });

                    break;


                case ExchangeAPIOrderResult.Error:

                    /*
                     
                     ERROR
                     
                     1. Wait 10 seconds and check again
                     2. If still error, cancel it
                     3. Check price now
                     4. Issue a new Buy
                                          
                     */

                    Log($"Error reported on exchange for orderId {orderResult.OrderId}, message: {orderResult.Message}");
                    Log("Waiting 10 seconds to check again");
                    Pause(10);

                    result = await CheckOrderStatus(orderResult.OrderId);
                    Log($"Result.. {result.Result}");

                    // 2. If not filled, cancel it

                    if (result.Result != ExchangeAPIOrderResult.Filled)
                    {
                        Log($"Not filled, cancelling order {orderResult.OrderId}");
                        _trader.CancelOrder(orderResult.OrderId);

                        // 3. Check the price now
                        ticker = _trader.GetLatestTicker(_inMemoryBot.BaseCoin, _inMemoryBot.Coin);
                        Log($"Going to order again, latest ticker {JsonConvert.SerializeObject(ticker)} ");

                        if (orderResult.IsBuy)
                            Buy(new Candle() { Timestamp = DateTime.Now, ClosePrice = ticker.Ask });
                        else
                            Sell(new Candle() { Timestamp = DateTime.Now, ClosePrice = ticker.Bid });
                    }

                    break;


                case ExchangeAPIOrderResult.Pending:

                    /*
                     *
                     PENDING
                     
                     1. Wait 30 seconds and check again
                     2. If still pending, cancel it
                     3. Check price now
                     4. Issue a new Buy
                                          
                     */

                    Log($"Pending reported on exchange for orderId {orderResult.OrderId}, message: {orderResult.Message}");

                    // 1. Wait thirty seconds & check

                    Pause(30);

                    result = await CheckOrderStatus(orderResult.OrderId);

                    // 2. If not filled, cancel it

                    if (result.Result == ExchangeAPIOrderResult.Pending)
                    {

                        _trader.CancelOrder(orderResult.OrderId);

                        // 3. Check the price now
                        ticker = _trader.GetLatestTicker(_inMemoryBot.BaseCoin, _inMemoryBot.Coin);

                        Log($"Order still pending after 30 seconds for orderId {orderResult.OrderId}, cancelling and re-ordering at new price {ticker.Ask}");

                        if (orderResult.IsBuy)
                            Buy(new Candle() { Timestamp = DateTime.Now, ClosePrice = ticker.Ask });
                        else
                            Sell(new Candle() { Timestamp = DateTime.Now, ClosePrice = ticker.Bid });
                    }


                    break;


                case ExchangeAPIOrderResult.Unknown:

                    /*
                     *
                     UNKNOWN
                     
                     1. Wait thirty seconds and check again
                     2. If still unknow, cancel it
                     3. Check price now
                     4. Issue a new Buy
                                          
                     */

                    Log($"UNKNOWN reported by exchange for orderId {orderResult.OrderId}, message: {orderResult.Message}. Waiting 30 seconds to try again");

                    // 1. Wait thirty seconds & check

                    Pause(30);

                    result = await CheckOrderStatus(orderResult.OrderId);

                    // 2. If not filled, cancel it

                    if (result.Result == ExchangeAPIOrderResult.Unknown)
                    {
                        _trader.CancelOrder(orderResult.OrderId);

                        // 3. Check the price now
                        ticker = _trader.GetLatestTicker(_inMemoryBot.BaseCoin, _inMemoryBot.Coin);

                        Log($"Order still UNKNOWN after 30 seconds for orderId {orderResult.OrderId}, cancelling and re-ordering at new price {ticker.Ask}");

                        if (orderResult.IsBuy)
                            Buy(new Candle() { Timestamp = DateTime.Now, ClosePrice = ticker.Ask });
                        else
                            Sell(new Candle() { Timestamp = DateTime.Now, ClosePrice = ticker.Bid });
                    }

                    break;
            }
        }

        private void UpdatePositionQuantity()
        {
            long? positionId = _inMemoryBot.CurrentPositionId;
            Position position = null;

            // the bot could have a closed position so it will be null
            if (positionId == null)
            {
                // get the last closed position for this bot
                position = _dbContext
                    .Positions
                    .Last(x => x.BotId == _inMemoryBot.BotId && x.Status == Enumerations.PositionStatusEnum.Sold);
            }
            else
            {
                try
                {
                    position = _dbContext
                   .Positions
                   .Single(x => x.PositionId == positionId);
                }
                catch (Exception)
                {
                    Log($"Position not found for {positionId}");
                }
               
            }

            if (position != null)
            {
                List<Order> orders =
                    _dbContext
                    .Orders
                    .Where(
                        x => x.PositionId == _inMemoryBot.CurrentPosition.PositionId
                        && x.Side == Enumerations.OrderSideEnum.Buy
                        && (x.Status == Enumerations.OrderStatusEnum.Filled || x.Status == Enumerations.OrderStatusEnum.FilledPartially))
                        .ToList();

                position.Quantity = orders.Sum(x => x.QuantityFilled);

                _dbContext.SaveChanges();
            }
            
        }

        [DebuggerStepThrough]
        private void Pause(int seconds)
        {
            Thread.Sleep(seconds * 1000);
        }

        private void SetPositionCommission(Order order, Position position)
        {
            position.Commission = position.Commission.GetValueOrDefault() + (_simulated
                                      ? ((order.Price * order.Quantity) / 100) * .25
                                      : order.Fees);
        }

        /// <summary>
        /// If in a positive profit, set to Sell - Buy, else set to Sell - Buy - Commission
        /// </summary>
        /// <param name="sellPrice"></param>
        /// <param name="currentPosition"></param>
        internal void SetCurrentPositionGrossProfit(double sellPrice, Position currentPosition)
        {
            var grossProfit = (sellPrice - currentPosition.BuyPrice) * currentPosition.Quantity;
            currentPosition.GrossProfit = grossProfit;
            currentPosition.GrossProfitPercent = Calculations.Percentage(grossProfit,
                (currentPosition.BuyPrice * currentPosition.Quantity));
        }

        /// <summary>
        /// Always set to Sell - Buy - Commission
        /// </summary>
        /// <param name="sellPrice"></param>
        /// <param name="currentPosition"></param>
        internal void SetCurrentPositionNetProfit(double sellPrice, Position currentPosition)
        {
            var netProfit = (sellPrice - currentPosition.BuyPrice) * currentPosition.Quantity -
                               currentPosition.Commission.GetValueOrDefault();
            currentPosition.NetProfit = netProfit;
            currentPosition.NetProfitPercent = Calculations.Percentage(netProfit,
                (currentPosition.BuyPrice * currentPosition.Quantity));
        }

        private DateTime GetTimeStamp(DateTime candleTimeStampTime)
        {
            return _simulated ? candleTimeStampTime : DateTime.Now;
        }

        private void SendMessageApp(string body)
        {
            if (_messagingApp != null)
                _messagingApp.SendMessage(body);
        }
    }
}
