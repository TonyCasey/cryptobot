using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using AutoMapper;
using CryptoBot.Core.Integrations.MessagingApps;
using CryptoBot.Core.Messaging;
using CryptoBot.Core.Ordering;
using CryptoBot.Core.Trading;
using CryptoBot.Database;
using CryptoBot.IndicatrorEngine;
using CryptoBot.Model.Common;
using CryptoBot.Model.Domain.Bot;
using CryptoBot.Model.Domain.Market;
using CryptoBot.SafetyEngine;
using NLog;
using Telegram.Bot;
using Candle = CryptoBot.Model.Domain.Market.Candle;



namespace CryptoBot.Core.Bots
{
    /// <summary>
    /// Main base class for all bots
    /// </summary>
    public class TradeBot : ITradeBot, IMessageHandler
    {
        private readonly Bot _bot;
        private List<Candle> _candles, _ticks;
        private readonly Logger _logger;
        private readonly CryptoBotDbContext _dbContext;
        private readonly IMapper _mapper;
        private readonly IIndicatorFactoryWrapper _indicatorWrapper;
        private readonly IMessagingApp _messagingApp;
        private List<Ticker> _tickers;
        private OrderManager _orderManager;
        public bool Paused { get; set; }
        private DateTime _candleStarTime;
        private DateTime _candleEndTime;


        public TradeBot(Bot bot, Logger logger, CryptoBotDbContext dbContext, IMapper mapper, IIndicatorFactoryWrapper indicatorWrapper, IMessagingApp messagingApp)
        {
            _bot = bot;
            _logger = logger;
            _dbContext = dbContext;
            _mapper = mapper;
            _indicatorWrapper = indicatorWrapper;
            _messagingApp = messagingApp;
            _candles = new List<Candle>();
            _tickers = new List<Ticker>();
            _ticks = new List<Candle>();
            _orderManager = new OrderManager(_bot, new Trader(_logger), _dbContext, _logger, _mapper, _messagingApp);
        }



        public void Start()
        {
            if (Paused)
            {
                Pause();
                return;
            }

            _logger.Info($"Bot {_bot.Name} starting....");


            OnStart();
        }

        #region interfaces

        /// <summary>
        /// When bot starts
        /// </summary>
        public virtual void OnStart()
        {
        }

        /// <summary>
        /// BuyPrice change
        /// </summary>
        public virtual void OnTick(Ticker ticker)
        {
            _tickers.Add(ticker);

            // do we have a candle yet?
            int candleSize = (int)_bot.CandleSize;
        }

        private Candle BuildCandle(Candle candle)
        {
            int candleSize = (int) _bot.CandleSize;

            
            if (candle.PeriodSeconds < candleSize * 60)
            {

                if ( _ticks.All(x => x.Timestamp != candle.Timestamp))
                    {

                        /*
                         *  IMPORTANT - TO MATCH CANDLES ACROSS DIFFERENT TIMESPANS
                         *
                         *  After much testing a candle is constructed in the following way..
                         *  Example:
                         *          Building a 30 min candle from 1 min candles
                         *          Staring candle - 12:00:00
                         *          Ending candle  - 12:29:00
                         */

                        // skip the first, we will start from :00
                        if (!_ticks.Any() && candle.Timestamp.Minute != 0)
                            return null;
                        
                        _ticks.Add(candle);

                        // once we get the first candle set the start & end times
                        if (_ticks.Any() && _ticks.Count == 1)
                        {
                            _candleStarTime = new DateTime(candle.Timestamp.Year, candle.Timestamp.Month, candle.Timestamp.Day, candle.Timestamp.Hour, 0, 0);
                            _candleEndTime = new DateTime(_candleStarTime.AddMinutes(candleSize-1).Year, _candleStarTime.AddMinutes(candleSize-1).Month, 
                                _candleStarTime.AddMinutes(candleSize-1).Day, _candleStarTime.AddMinutes(candleSize-1).Hour, 
                                _candleStarTime.AddMinutes(candleSize-1).Minute, 0);
                        }

                    
                        //if(_ticks.Sum(x => x.PeriodSeconds) < candleSize * 60)
                        //    return null;
                    }
                

                if (candle.Timestamp > _candleEndTime) 
                {
                    _ticks.RemoveAt(_ticks.Count-1);

                    Candle newCandle = new Candle()
                    {
                        Timestamp = _candleStarTime,
                        ClosePrice = _ticks.Last().ClosePrice,
                        OpenPrice = _ticks.First().OpenPrice,
                        HighPrice = _ticks.Max(x => x.HighPrice),
                        LowPrice = _ticks.Min(x => x.LowPrice),
                        PeriodSeconds = candleSize * 60,
                        VolumePrice = _ticks.Sum(x => x.VolumePrice),
                        VolumeQuantity = _ticks.Sum(x => x.VolumeQuantity),
                        WeightedAverage = _ticks.Sum(x => x.WeightedAverage) / _ticks.Count // not sure on this
                    };

                    // clear the ticks list
                    _ticks = new List<Candle>();

                    _candleStarTime = new DateTime(_candleEndTime.AddMinutes(1).Year, _candleEndTime.AddMinutes(1).Month, _candleEndTime.AddMinutes(1).Day, _candleEndTime.AddMinutes(1).Hour, _candleEndTime.AddMinutes(1).Minute, 0);
                    _candleEndTime = new DateTime(_candleStarTime.AddMinutes(candleSize-1).Year, _candleStarTime.AddMinutes(candleSize-1).Month, 
                        _candleStarTime.AddMinutes(candleSize-1).Day, _candleStarTime.AddMinutes(candleSize-1).Hour, 
                        _candleStarTime.AddMinutes(candleSize-1).Minute, 0);

                    _ticks.Add(candle);
                    
                    return newCandle;
                    
                }
            }

            return null;
        }

        /// <summary>
        /// When a Candle has closed
        /// </summary>
        public bool OnCandle(Candle candle)
        {
            // is this the right size candle
            int candleSize = (int)_bot.CandleSize;

            if (candle.PeriodSeconds > candleSize * 60)
            {
                Log($"Larger candle size passed in for {_bot.Name}, required size {_bot.CandleSize}, actual size {candle.PeriodSeconds / 60}");
                return false;
            }


            if (candle.PeriodSeconds < candleSize * 60)
            {
                candle = BuildCandle(candle);
                if (candle == null)
                    return false;
            }
            else
            {
                var a = "yes";
            }

            Log($"*** Adding Candle for Exchange {_bot.Exchange.Name} Coin {_bot.Coin.Code} TimeStamp {candle.Timestamp:F}, OpenPrice {candle.OpenPrice} ClosePrice {candle.ClosePrice}");
            _candles.Add(candle);

            
            
            Enumerations.IndicatorSignalEnum indicatorSignal;

            // if there is no current position open, look for a buy
            if (_bot.CurrentPosition == null)
            {

                indicatorSignal = GetIndicatorSignals(Enumerations.IndicatorSignalEnum.Buy);

                switch (indicatorSignal)
                {
                    case Enumerations.IndicatorSignalEnum.Buy:
                        Log($"Bot - {_bot.Name}, no position open, recommended BUY at Candle - {candle.Timestamp}, Closing Price - {candle.ClosePrice}, Volume {candle.VolumeQuantity}");
                        return true;
                    default:
                        Log($"Bot - {_bot.Name}, no buy signals yet");
                        break;
                }

            }
            else // we have an open position, look for a sell
            {
                indicatorSignal = GetIndicatorSignals(Enumerations.IndicatorSignalEnum.Sell);

                switch (indicatorSignal)
                {
                    case Enumerations.IndicatorSignalEnum.Sell:

                        switch (_bot.CurrentPosition?.Status)
                        {
                            case Enumerations.PositionStatusEnum.Bought:

                                Log($"Bot - {_bot.Name}, recommended SELL at Candle - {candle.Timestamp}, Closing Price - {candle.ClosePrice}, Volume {candle.VolumeQuantity}");
                                return true;

                            // If somehow, it is in a sold position then it needs to be cleared from the _bot
                            case Enumerations.PositionStatusEnum.Sold:
                                _bot.CurrentPosition = null;
                                _bot.CurrentPositionId = null;
                                return false;
                        }

                        break;

                    default:

                        Log($"Bot - {_bot.Name}, no sell signals yet");

                        break;
                }
                
            }

            return false;
        }

        /// <summary>
        /// When the bot stops
        /// </summary>
        public virtual void OnStop()
        {

        }

        #endregion



        public void Process(IMessage msg)
        {
           
            if (msg.GetType() == typeof(CandleMessage))
            {
                List<Candle> candles = ((CandleMessage) msg).Candles.ToList();
                Coin baseCoin = ((CandleMessage)msg).BaseCoin;
                Coin coin = ((CandleMessage)msg).Coin;

                if ( baseCoin.Code != _bot.BaseCoin.Code || coin.Code != _bot.Coin.Code)
                    return;

                foreach (Candle candle in candles)
                {
                    //if (DateTime.UtcNow.Subtract(candle.Timestamp).Hours < 1)
                    //    continue;

                    if (_candles.All(x => x.Timestamp != candle.Timestamp))
                    {
                        if (OnCandle(candle)) // change position
                        {

                            if (_bot.CurrentPosition != null)
                            {
                                switch (_bot.CurrentPosition?.Status)
                                {
                                    case Enumerations.PositionStatusEnum.Bought:
                                        _orderManager.Sell(candle);
                                        break;
                                    case Enumerations.PositionStatusEnum.Sold:
                                        _orderManager.Buy(candle);
                                        break;
                                }
                            }
                            else
                            {
                                _orderManager.Buy(candle);
                            }
                        }
                    }
                }
                
            }
        }

        private void Pause()
        {
            Log("");
            Log($" PAUSED **************** {this.GetType().Name}");
            Log("");
        }

        private void Log(string message)
        {
            _logger.Info("");
            _logger.Info($"[TradeBot] - {message}");
        }

        public void PreLoadCandles(List<Candle> candles)
        {
            if (candles.First().PeriodSeconds < (int) _bot.CandleSize * 60)
            {
                foreach (Candle candle in candles)
                {
                    Candle newCandle = BuildCandle(candle);
                    if (newCandle != null)
                    _candles.Add(newCandle);
                }
            }
            else
            {
                _candles = candles;
            }
        }

        private Enumerations.IndicatorSignalEnum GetIndicatorSignals(Enumerations.IndicatorSignalEnum side)
        {
            List<Enumerations.IndicatorSignalEnum> indicatorPositions = new List<Enumerations.IndicatorSignalEnum>();

            foreach (Indicator indicator in _bot.Indicators.Where(x => side == Enumerations.IndicatorSignalEnum.Buy ? x.UseForBuy:x.UseForSell))
            {
                IIndicatorEngine indicatorEngine = _indicatorWrapper.GetIndicator(indicator.IndicatorType, _logger);

                indicatorPositions.Add(indicatorEngine.GetIndicatorPosition(_candles, indicator.RuleSets));

            }

            switch (side)
            {
                case Enumerations.IndicatorSignalEnum.Sell:
                    
                    if (indicatorPositions.All(x => x == Enumerations.IndicatorSignalEnum.Sell))
                        return Enumerations.IndicatorSignalEnum.Sell;
                    else
                    {
                        return Enumerations.IndicatorSignalEnum.None;
                    }
                case Enumerations.IndicatorSignalEnum.Buy:

                    if (indicatorPositions.All(x => x == Enumerations.IndicatorSignalEnum.Buy))
                        return Enumerations.IndicatorSignalEnum.Buy;
                    else
                    {
                        return Enumerations.IndicatorSignalEnum.None;
                    }
            }

            return Enumerations.IndicatorSignalEnum.None;
            
        }
    }
}
