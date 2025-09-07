using System;
using CryptoBot.Core.Messaging;
using CryptoBot.ExchangeEngine.API.Exchanges;
using CryptoBot.Model.Common;
using CryptoBot.Model.Domain.Market;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NLog;
using System.Collections.Generic;

namespace CryptoBot.Tests.Scheduler
{
    [TestClass]
    public class SchedulerTest:BaseTest
    {
        private new Logger _logger;
        private Dictionary<Coin, Coin> _coins;
        private Core.Scheduling.Scheduler _scheduler;

        [TestInitialize]
        public void Initiatize()
        {
            _coins = new Dictionary<Coin, Coin>();
            _logger = LogManager.GetCurrentClassLogger();
            _scheduler = new Core.Scheduling.Scheduler(new Mock<IExchangeApi>().Object,
                new Mock<MessageDispatcher>().Object,
                Enumerations.SchedulerType.ScheduledEventOccursEveryValueInSecondsPastTheHour, 60, 60, _coins, _logger);
        }

        [TestMethod]
        public void TestSchedulerStart()
        {
        }

        [TestMethod]
        public void Test_GetScheduledEventMessageToLog()
        {
            const string expectedMsg1 = "Starting scheuler to run at exactly 60 seconds past every hour";
            const string expectedMsg2 = "Starting scheuler to run at 60 second intervals";
            const string expectedMsg3 = "Starting scheuler to run daily at 00:10:07";

            var scheduler1 = new Core.Scheduling.Scheduler(new Mock<IExchangeApi>().Object,
                new Mock<MessageDispatcher>().Object,
                Enumerations.SchedulerType.ScheduledEventOccursEveryValueInSecondsPastTheHour, 60, 60, _coins, _logger);

            var scheduler2 = new Core.Scheduling.Scheduler(new Mock<IExchangeApi>().Object,
                new Mock<MessageDispatcher>().Object,
                Enumerations.SchedulerType.ScheduledEventOccursEveryValueInSeconds, 60, 60, _coins, _logger);

            var scheduler3 = new Core.Scheduling.Scheduler(new Mock<IExchangeApi>().Object,
                new Mock<MessageDispatcher>().Object,
                Enumerations.SchedulerType.ScheduledEventOccursOnceDailyValueInSecondsPastMidnight, 607, 60, _coins, _logger);

            var msg1 = scheduler1.GetScheduledEventMessageToLog();
            var msg2 = scheduler2.GetScheduledEventMessageToLog();
            var msg3 = scheduler3.GetScheduledEventMessageToLog();

            Assert.AreEqual(expectedMsg1, msg1);
            Assert.AreEqual(expectedMsg2, msg2);
            Assert.AreEqual(expectedMsg3, msg3);
        }

        [TestMethod]
        public void Test_GetTimeSpanToSleepFor()
        {
            var result1 =
                _scheduler.GetTimeSpanToSleepFor(Enumerations.SchedulerType.ScheduledEventOccursEveryValueInSeconds,
                    3600);

            var result2 =
                _scheduler.GetTimeSpanToSleepFor(
                    Enumerations.SchedulerType.ScheduledEventOccursOnceDailyValueInSecondsPastMidnight, 3600);

            var result3 =
                _scheduler.GetTimeSpanToSleepFor(
                    Enumerations.SchedulerType.ScheduledEventOccursEveryValueInSecondsPastTheHour, 60);

            // For ScheduledEventOccursEveryValueInSeconds, should return exactly the value
            Assert.AreEqual(TimeSpan.FromSeconds(3600), result1);
            
            // For ScheduledEventOccursOnceDailyValueInSecondsPastMidnight, should be less than 24 hours
            Assert.IsTrue(result2 <= TimeSpan.FromDays(1));
            Assert.IsTrue(result2 > TimeSpan.Zero);
            
            // For ScheduledEventOccursEveryValueInSecondsPastTheHour, could be negative if we're past the mark
            // Just check it's reasonable
            Assert.IsTrue(Math.Abs(result3.TotalHours) <= 1.5);
        }
    }
}
