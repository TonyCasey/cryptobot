using System;
using System.Linq;
using System.Threading.Tasks;
using CryptoBot.Core.Utils;
using CryptoBot.ExchangeEngine;
using CryptoBot.ExchangeEngine.API.Exchanges;
using CryptoBot.Model.Common;
using CryptoBot.Model.Domain.Account;
using CryptoBot.Model.Domain.Bot;
using CryptoBot.Model.Domain.Market;
using CryptoBot.Model.Exchanges;
using NLog;

namespace CryptoBot.Core.Trading
{
    public class Trader : ITrader
    {
        private IExchangeApi _exchangeApi;
        private readonly Logger _logger;
        private ExchangeOrderResult _simulatedOrderResult;

        public Trader(Logger logger)
        {
            _logger = logger;
        }

        private ExchangeOrderResult Buy(Coin baseCoin, Coin coin, decimal quantity, decimal? price, Enumerations.OrderTypeEnum type)
        {
            
            _logger.Info("");
            Log($"Placing BUY order for {baseCoin.Code}-{coin.Code}, quantity {quantity}, price {price}, type {type}");
            _logger.Info("");

            try
            {
                return _exchangeApi.Simulated
                    ? SimulateOrderResult(baseCoin, coin, quantity, price, true, ExchangeAPIOrderResult.Pending)
                    : _exchangeApi.PlaceOrder(baseCoin, coin, quantity, price, true, type);
            }
            catch (Exception e)
            {
                _logger.Error(e);
                return null;
            }
        }
       
        private ExchangeOrderResult Sell(Coin baseCoin, Coin coin, decimal quantity, decimal? price, Enumerations.OrderTypeEnum type)
        {
            _logger.Info("");
            Log($"Placing SELL order for {baseCoin.Code}-{coin.Code}, quantity {quantity}, price {price}, type {type}");
            _logger.Info("");

            try
            {
                return _exchangeApi.Simulated
                    ? SimulateOrderResult(baseCoin, coin, quantity, price, false, ExchangeAPIOrderResult.Pending)
                    : _exchangeApi.PlaceOrder(baseCoin, coin, quantity, price, false, type);
            }
            catch (Exception e)
            {
                _logger.Error(e);
                return null;
            }
        }
        
        public async Task<ExchangeOrderResult> Buy(Bot bot, decimal price)
        {
            _exchangeApi = GetExchange(bot);
            
            
            switch (bot.OrderType)
            {
                case Enumerations.OrderTypeEnum.Market:
                    return Buy(bot.BaseCoin, bot.Coin, bot.Amount, null, Enumerations.OrderTypeEnum.Market);
                case Enumerations.OrderTypeEnum.Limit:
                    
                   
                    // get the best ask
                    ExchangeOrderBook orderBook = null;

                    if (_exchangeApi.Simulated)
                    {
                        orderBook = SimulateExchangeOrderBook(price);
                    }
                    else
                    {
                       orderBook = _exchangeApi.GetOrderBook(bot.BaseCoin, bot.Coin, 1);
                    }

                    if (orderBook != null)
                    {
                        ExchangeOrderPrice bestAsk = orderBook.Asks.First();

                        decimal quantity = Math.Round( bot.Amount / bestAsk.Price, bot.Coin.OrderRoundingExponent);
                        
                        return Buy(bot.BaseCoin, bot.Coin, quantity, bestAsk.Price, Enumerations.OrderTypeEnum.Market);
                    }

                    throw new Exception("Couldn't place a buy order");

                case Enumerations.OrderTypeEnum.Stop: // to do
                    throw new NotImplementedException();
            }

            return null;
        }

        public async Task<ExchangeOrderResult> Sell(Bot bot, decimal price)
        {
            _exchangeApi = GetExchange(bot);

            if(bot.CurrentPosition == null || bot.CurrentPosition.Status != Enumerations.PositionStatusEnum.Bought)
                throw new Exception("No open current position on bot");

            decimal quantity = (decimal) Math.Round( bot.CurrentPosition.Quantity, bot.Coin.OrderRoundingExponent, MidpointRounding.AwayFromZero);

            switch (bot.OrderType)
            {
                case Enumerations.OrderTypeEnum.Market:
                    return Sell(bot.BaseCoin, bot.Coin, quantity, null, Enumerations.OrderTypeEnum.Market);
                case Enumerations.OrderTypeEnum.Limit:

                    // get the best bid
                    ExchangeOrderBook orderBook = null;

                    if (_exchangeApi.Simulated)
                    {
                        orderBook = SimulateExchangeOrderBook(price);
                    }
                    else
                    {
                        orderBook = _exchangeApi.GetOrderBook(bot.BaseCoin, bot.Coin, 1);
                    }

                    if (orderBook != null)
                    {
                        ExchangeOrderPrice bestBid = orderBook.Bids.First();
                        
                        return Sell(bot.BaseCoin, bot.Coin, quantity, bestBid.Price, Enumerations.OrderTypeEnum.Market);
                    }
                    break;

                case Enumerations.OrderTypeEnum.Stop: // to do
                    throw new NotImplementedException();
                    
            }
            return null;
        }

        private IExchangeApi GetExchange(Bot bot)
        {
            ApiSetting exchangeSetting = bot.User.ApiSettings.FirstOrDefault(x => x.Exchange.ExchangeId == bot.Exchange.ExchangeId);

            return
            ExchangeFactory.GetExchangeApi((Enumerations.ExchangesEnum)bot.Exchange.ExchangeId, new ExchangeSettings
            {
                Url = exchangeSetting.Url,
                SocketUrl = exchangeSetting.SocketUrl,
                PassPhrase = exchangeSetting.Passphrase,
                ApiKey = exchangeSetting.Key,
                Secret = exchangeSetting.Secret,
                CommissionRate = exchangeSetting.ComissionRate,
                Simulate = exchangeSetting.Simulated
            });
            
        }

       

        /// <summary>
        /// Simulate a result from an exchange
        /// </summary>
        /// <param name="symbol"></param>
        /// <param name="quantity"></param>
        /// <param name="price"></param>
        /// <param name="IsBuy"></param>
        /// <param name="simulatedResult">GDAX will send back a pending result on a new order </param>
        /// <returns></returns>
        private ExchangeOrderResult SimulateOrderResult(Coin baseCoin, Coin coin, decimal quantity, decimal? price, bool IsBuy, ExchangeAPIOrderResult simulatedResult)
        {
            Log($"Using simulated exchange {_exchangeApi.Name }");

            _simulatedOrderResult = new ExchangeOrderResult
            {
                Amount = quantity,
                AmountFilled = quantity,
                AveragePrice = price.GetValueOrDefault(0),
                IsBuy = IsBuy,
                Message = "Simulated order",
                OrderDate = DateTime.UtcNow,
                Symbol = $"{baseCoin.Code}-{coin.Code}",
                OrderId = Guid.NewGuid().ToString(),
                Result = simulatedResult
            };

            return _simulatedOrderResult;
            
        }

        private ExchangeOrderBook SimulateExchangeOrderBook(decimal desiredPrice)
        {
            return new ExchangeOrderBook
            {
                Asks = { new ExchangeOrderPrice {Price = desiredPrice}},
                Bids = { new ExchangeOrderPrice{ Price = desiredPrice}}
            };
        }

        private void Log(string message)
        {
            _logger.Info($"[Trader] - {message}");
        }

        public async Task<ExchangeOrderResult> GetOrderDetails(string orderId)
        {
            // if this is simulated, change the status of the simualte order & send back
            if (_exchangeApi.Simulated)
            {
                _simulatedOrderResult.Result = ExchangeAPIOrderResult.Filled;
            }

            return _exchangeApi.Simulated ? _simulatedOrderResult : await _exchangeApi.GetOrderDetailsAsync(orderId);
        }

        public async void CancelOrder(string orderId)
        {
            try
            {
                if(!_exchangeApi.Simulated)
                    await _exchangeApi.CancelOrderAsync(orderId);
            }
            catch (Exception e)
            {
               _logger.Error(e);
            }
        }

        public Ticker GetLatestTicker(Coin baseCoin, Coin coin)
        {
            return _exchangeApi.Simulated ? 
                new Ticker{ Ask = _simulatedOrderResult.AveragePrice, Bid = _simulatedOrderResult.AveragePrice } 
                : _exchangeApi.GetTicker(baseCoin, coin);
        }
    }
}
