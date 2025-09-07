using CryptoBot.Core.Messaging;
using CryptoBot.ExchangeEngine.API.Exchanges;
using CryptoBot.Model.Common;
using CryptoBot.Model.Domain.Market;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace CryptoBot.Core.Scheduling
{
    public class Scheduler
    {
        private readonly IExchangeApi _exchangeApi;
        private readonly MessageDispatcher _messageispatcher;
        private readonly int _schedulerValueInSeconds;
        private readonly Dictionary<Coin, Coin> _coins;
        private bool _hasFinished = false;
        private readonly Logger _logger;
        private readonly Enumerations.SchedulerType _schedulerType;
        private readonly int _candlePeriodInSeconds;


        public Scheduler(IExchangeApi exchangeApi, MessageDispatcher messageispatcher, 
            Enumerations.SchedulerType schedulerType, int schedulerValueInSeconds, 
            int candlePeriodInSeconds, Dictionary<Coin, Coin> coins, Logger logger)
        {
            _exchangeApi = exchangeApi;
            _messageispatcher = messageispatcher;
            _schedulerValueInSeconds = schedulerValueInSeconds;
            _coins = coins;
            _logger = logger;
            _schedulerType = schedulerType;
            _candlePeriodInSeconds = candlePeriodInSeconds;
        }

        public void Start()
        {
            _logger.Info(GetScheduledEventMessageToLog());            

            while (!_hasFinished)
            {
                foreach (KeyValuePair<Coin, Coin> pair in _coins)
                {
                    // NO HISTORICAL DATA NEEDED AS WE GET THIS SEPARATLEY IN THE PRELOAD WHEN BOT STARTS
                    
                    try
                    {
                        // set the minutes to :00
                        DateTime fromDate = new DateTime(DateTime.UtcNow.AddMinutes(-5).Year, DateTime.UtcNow.AddMinutes(-5).Month, DateTime.UtcNow.AddMinutes(-5).Day, DateTime.UtcNow.AddMinutes(-5).Hour, DateTime.UtcNow.AddMinutes(-5).Minute, 0);
                        if (fromDate.Minute % 5 > 0)
                        {
                            int diff = fromDate.Minute % 5;
                            fromDate = new DateTime(fromDate.AddMinutes(-diff).Year,fromDate.AddMinutes(-diff).Month, fromDate.AddMinutes(-diff).Day, fromDate.AddMinutes(-diff).Hour, fromDate.AddMinutes(-diff).Minute, 0);
                        }
                            
                        DateTime toDate = new DateTime(fromDate.AddMinutes(5).Year, fromDate.AddMinutes(5).Month, fromDate.AddMinutes(5).Day, fromDate.AddMinutes(5).Hour, fromDate.AddMinutes(5).Minute, 0);
                        

                        IEnumerable<Candle> candles = _exchangeApi.GetCandles(pair.Key, pair.Value, _candlePeriodInSeconds, fromDate, toDate);
                        
                        
                        if (candles.Any())
                        {
                            var candleMessage = new CandleMessage
                            {
                                Candles = candles.Skip(candles.Count() - 2), // sometimes the request brings back more records than are needed
                                BaseCoin = pair.Key,
                                Coin = pair.Value
                            };
                            _messageispatcher.Dispatch(candleMessage);
                        }
                    }
                    catch (Exception e)
                    {
                        _logger.Error(e.Message);
                        _logger.Error(e.StackTrace);
                    }
                }

                var nextCandle = new DateTime(DateTime.UtcNow.AddMinutes(5).Year, DateTime.UtcNow.AddMinutes(5).Month, DateTime.UtcNow.AddMinutes(5).Day, DateTime.UtcNow.AddMinutes(5).Hour, DateTime.UtcNow.AddMinutes(5).Minute, 0);
                if (nextCandle.Minute % 5 > 0)
                {
                    int diff = nextCandle.Minute % 5;
                    nextCandle = new DateTime(nextCandle.AddMinutes(-diff).Year,nextCandle.AddMinutes(-diff).Month, nextCandle.AddMinutes(-diff).Day, nextCandle.AddMinutes(-diff).Hour, nextCandle.AddMinutes(-diff).Minute, 0);
                }
                nextCandle = new DateTime(nextCandle.AddMinutes(1).Year,nextCandle.AddMinutes(1).Month, nextCandle.AddMinutes(1).Day, nextCandle.AddMinutes(1).Hour, nextCandle.AddMinutes(1).Minute, 0);


                var timeToSleepFor = nextCandle.Subtract(DateTime.UtcNow);

                //var timeToSleepFor = DateTime.UtcNow.AddMinutes()  GetTimeSpanToSleepFor(_schedulerType, _schedulerValueInSeconds);
                _logger.Info($"Going to sleep until the next run on {nextCandle.ToLocalTime()}");
                Thread.Sleep(timeToSleepFor);
            }
        }

        public void Stop()
        {
            _logger.Info("Stopping timer...");
            _hasFinished = true;
        }

        internal string GetScheduledEventMessageToLog()
        {
            var timeSpan = TimeSpan.FromSeconds(_schedulerValueInSeconds);
            switch (_schedulerType)
            {
                case Enumerations.SchedulerType.ScheduledEventOccursEveryValueInSeconds:
                    return $"Starting scheuler to run at {_schedulerValueInSeconds} second intervals";
                case Enumerations.SchedulerType.ScheduledEventOccursEveryValueInSecondsPastTheHour:
                    return $"Starting scheuler to run at exactly {_schedulerValueInSeconds} seconds past every hour";
                case Enumerations.SchedulerType.ScheduledEventOccursOnceDailyValueInSecondsPastMidnight:
                    return $"Starting scheuler to run daily at {timeSpan.Hours:00}:{timeSpan.Minutes:00}:{timeSpan.Seconds:00}";
                default:
                    return string.Empty;
            }
        }

        internal TimeSpan GetTimeSpanToSleepFor(Enumerations.SchedulerType schedulerType, int schedulerValueInSeconds)
        {
            switch (schedulerType)
            {
                case Enumerations.SchedulerType.ScheduledEventOccursEveryValueInSecondsPastTheHour:
                    {
                        var currentDateTimePlusOneHour = DateTime.UtcNow.AddHours(1);
                        var theNextDateTimeToRunAt =
                            new DateTime(currentDateTimePlusOneHour.Year, currentDateTimePlusOneHour.Month,
                                currentDateTimePlusOneHour.Day, currentDateTimePlusOneHour.Hour, 0, 0).AddSeconds(
                                schedulerValueInSeconds).ToUniversalTime();
                        return theNextDateTimeToRunAt.Subtract(DateTime.UtcNow);
                    }
                case Enumerations.SchedulerType.ScheduledEventOccursOnceDailyValueInSecondsPastMidnight:
                    {
                        var currentDateTimePlusOneDay = DateTime.UtcNow.AddDays(1);
                        var theNextDateTimeToRunAt =
                            new DateTime(currentDateTimePlusOneDay.Year, currentDateTimePlusOneDay.Month,
                                currentDateTimePlusOneDay.Day, 0, 0, 0).AddSeconds(
                                schedulerValueInSeconds).ToUniversalTime();
                        return theNextDateTimeToRunAt.Subtract(DateTime.UtcNow);
                    }
                case Enumerations.SchedulerType.ScheduledEventOccursEveryValueInSeconds:
                {
                    // Simply return the interval specified
                    return TimeSpan.FromSeconds(schedulerValueInSeconds);
                }
                default:
                    {
                        return TimeSpan.FromSeconds(schedulerValueInSeconds);
                    }
            }
        }


    }
}
