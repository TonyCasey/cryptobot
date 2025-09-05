using CryptoBot.Core.Ordering;
using CryptoBot.Core.Trading;
using CryptoBot.Model.Common;
using CryptoBot.Model.Domain.Account;
using CryptoBot.Model.Domain.Bot;
using CryptoBot.Model.Domain.Market;
using CryptoBot.Model.Domain.Trading;
using CryptoBot.Model.Exchanges;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NLog;
using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Castle.Components.DictionaryAdapter;
using CryptoBot.Tests.Helpers;
using Logger = NLog.Logger;

namespace CryptoBot.Tests.Orders
{
    [TestClass]
    public class OrderManagerTests : BaseTest
    {
        
        
        private Mock<ITrader> _traderMock;
        private Mock<OrderManager> _orderManagerMock;
        private Bot _bot;
         
       
        [TestInitialize]
        public void Initiatize()
        {
            _bot = GetBot();

            _traderMock = new Mock<ITrader>();
            
            _dbContext.Setup(x => x.Bots).ReturnsDbSet(new List<Bot>() {_bot});
            _dbContext.Setup(x => x.Orders).ReturnsDbSet(new List<Order>());

            var positions = new List<Position>().AsQueryable();
            var positionsMock = new Mock<DbSet<Position>>();
            positionsMock.As<IQueryable<Position>>().Setup(m => m.Provider).Returns(positions.Provider);
            positionsMock.As<IQueryable<Position>>().Setup(m => m.Expression).Returns(positions.Expression);
            positionsMock.As<IQueryable<Position>>().Setup(m => m.ElementType).Returns(positions.ElementType);
            positionsMock.As<IQueryable<Position>>().Setup(m => m.GetEnumerator()).Returns(positions.GetEnumerator());
            _dbContext.Setup(x => x.Positions).Returns(positionsMock.Object);


            //_dbContext.Setup(x => x.Positions).ReturnsDbSet(positions);

            //_dbContext.Setup(x => x.Positions.Add(It.IsAny<Position>()))
            //    .Callback<Position>(position => positions.Add(position));

            //var context = new Mock<CryptoBotDbContext>();

            _orderManagerMock = new Mock<OrderManager>(_bot, _traderMock.Object, _dbContext.Object, _logger, _mapper);
            
        }

        [TestMethod]
        public void TestFilledBuy()
        {

            _traderMock.Setup(x => x.Buy(It.IsAny<Bot>(), It.IsAny<decimal>()))
                .Returns(Task.FromResult(SimulateOrderResult(_bot.BaseCoin, _bot.Coin, _bot.Amount, ExchangeAPIOrderResult.Filled)));

            // if it throws an exception then the test will fail
            _orderManagerMock.Object.Buy(new Candle{ ClosePrice = 100, Timestamp = DateTime.Now });
            
        }

        [TestMethod]
        public void TestPartiallyFilledBuy()
        {
            Bot bot = GetBot();

            _traderMock.Setup(x => x.Buy(It.IsAny<Bot>(), It.IsAny<decimal>()))
                .Returns(Task.FromResult(SimulateOrderResult(bot.BaseCoin, bot.Coin, bot.Amount, ExchangeAPIOrderResult.FilledPartially)));

            // if it throws an exception then the test will fail
            _orderManagerMock.Object.Buy(new Candle { ClosePrice = 100, Timestamp = DateTime.Now });

        }


        private ExchangeOrderResult SimulateOrderResult(Coin baseCoin, Coin coin, decimal amount, ExchangeAPIOrderResult result)
        {
            return new ExchangeOrderResult
            {
                Amount = amount,
                AmountFilled = amount,
                AveragePrice = 9.99M,
                IsBuy = true,
                Message = "Simulated order",
                OrderDate = DateTime.UtcNow,
                Symbol = $"{baseCoin.Code}-{coin.Code}",
                OrderId = Guid.NewGuid().ToString(),
                Result = result,
            };
        }

        private Bot GetBot()
        {
            return new Bot
            {
                BotId = 1,
                Name = "GDAX BTC-EUR 60 min Bot",
                Coin = new Coin() { Code = "EUR" },
                BaseCoin = new Coin() { Code = "BTC" },
                User = new User() { Name = "Test User", UserId = 999, ApiSettings = new List<ApiSetting> {new ApiSetting{ApiSettingId = 1, Exchange = new Exchange{ExchangeId = 1}}}},
                Exchange = new Exchange{ExchangeId = 1},
                Active = true,
                Amount = 2000M,
                OrderType = Enumerations.OrderTypeEnum.Limit,
                Indicators = new List<Indicator>
                {
                    new Indicator
                    {
                        IndicatorType = Enumerations.IndicatorTypeEnum.Macd,
                        RuleSets = new List<RuleSet>
                        {
                            new RuleSet
                            {
                                // buy rules
                                RuleSide = IndicatorRulesEnumerations.RuleSideEnum.Buy,
                                Rules = new List<Rule>
                                {
                                    new Rule
                                    {
                                        IndicatorRuleTypeId = (int)IndicatorRulesEnumerations.MacdRulesEnum.IsMacdAboveSignal
                                    },
                                    new Rule
                                    {
                                        IndicatorRuleTypeId = (int)IndicatorRulesEnumerations.MacdRulesEnum.IsHistogramAboveMinimumHeight,
                                        Value = 30
                                    }
                                }
                            },
                            new RuleSet
                            {
                                // single sell trigger MACD crossing down
                                RuleSide = IndicatorRulesEnumerations.RuleSideEnum.Sell,
                                Rules = new List<Rule>
                                {
                                    new Rule
                                    {
                                        IndicatorRuleTypeId = (int)IndicatorRulesEnumerations.MacdRulesEnum.IsCrossingDown
                                    }
                                }
                            }
                        }
                    }
                },
                Safeties = new List<Safety>
                {
                    new Safety
                    {
                        SafetyType = Enumerations.SafetyTypeEnum.StopLoss
                    }
                },
                CurrentPosition =  new Position
                {
                    Status = Enumerations.PositionStatusEnum.Sold,
                    BuyTimeStamp = DateTime.Now,
                    CreationTime = DateTime.Now,
                    LastUpdateTime = DateTime.Now,
                    SellTimeStamp = DateTime.Now,
                }
                

            };
        }


        [TestMethod]
        public void Tet_SetCurrentPositionGrossProfit_Profit()
        {
            var sellPrice = 10103d;
            var position = new Position { BuyPrice = 10000, Quantity = 0.1, Commission = 1000};
            var orderManager = _orderManagerMock.Object;

            orderManager.SetCurrentPositionGrossProfit(sellPrice, position);

            //Don't include the commission
            Assert.IsTrue(position.GrossProfit.GetValueOrDefault() > 0d);            
        }

        [TestMethod]
        public void Tet_SetCurrentPositionGrossProfit_Loss()
        {
            var sellPrice = 9001;
            var position = new Position { BuyPrice = 10000, Quantity = 0.1, Commission = 1000 };
            var orderManager = _orderManagerMock.Object;

            orderManager.SetCurrentPositionGrossProfit(sellPrice, position);

            //Include the commission
            Assert.IsTrue(position.GrossProfit.GetValueOrDefault() < 0d);
            Assert.IsTrue(position.GrossProfit.GetValueOrDefault() < position.Commission);
        }

        [TestMethod]
        public void Tet_SetCurrentPositionNetProfit_Profit()
        {
            var sellPrice = 10103d;
            var position = new Position { BuyPrice = 10000, Quantity = 0.1, Commission = 1000 };
            var orderManager = _orderManagerMock.Object;

            orderManager.SetCurrentPositionNetProfit(sellPrice, position);

            //Include the commission
            Assert.IsTrue(position.NetProfit.GetValueOrDefault() < 0d);
        }

        [TestMethod]
        public void Tet_SetCurrentPositionNetProfit_Loss()
        {
            var sellPrice = 10103d;
            var position = new Position { BuyPrice = 10000, Quantity = 0.1, Commission = 1000 };
            var orderManager = _orderManagerMock.Object;

            orderManager.SetCurrentPositionNetProfit(sellPrice, position);

            //Include the commission
            Assert.IsTrue(position.NetProfit.GetValueOrDefault() < position.Commission);
        }

    }
}
