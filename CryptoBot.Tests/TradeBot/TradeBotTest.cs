using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using CryptoBot.IndicatrorEngine;
using CryptoBot.IndicatrorEngine.Macd;
using CryptoBot.Model.Common;
using CryptoBot.Model.Domain.Account;
using CryptoBot.Model.Domain.Bot;
using CryptoBot.Model.Domain.Market;
using CryptoBot.Model.Domain.Trading;
using CryptoBot.Tests.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace CryptoBot.Tests.TradeBot
{
    /// <summary>
    /// Summary description for TradeBotTest
    /// </summary>
    [TestClass]
    public class TradeBotTest : BaseTest
    {
        private Bot _bot;
        private Mock<Core.Bots.TradeBot> _tradeBotMock;


        [TestInitialize]
        public void Initiatize()
        {
            _bot = GetBot();
            
            _dbContext.Setup(x => x.Bots).ReturnsDbSet(new List<Bot>() { _bot });
            
            _tradeBotMock = new Mock<Core.Bots.TradeBot>(_bot, _logger, _dbContext.Object, _mapper, _iIndicatorFactoryWrapper);
            
        }
        

        // TODO: From Monday night. Opposing indicators can be ok, actually, just think about it :-)


        [TestMethod]
        public void Test_Opposing_Indicators()
        {
            // Arrange
            Candle candle = new Candle
            {
                VolumeQuantity = 20
            };

            // MACD Buy
            Mock<IIndicatorEngine> macdIndicator = new Mock<IIndicatorEngine>();
            macdIndicator.Setup(x => x.GetIndicatorPosition(It.IsAny<List<Candle>>(), It.IsAny<List<RuleSet>>()))
                .Returns(Enumerations.IndicatorSignalEnum.Sell);
            _iIndicatorFactoryWrapper.Setup(x => x.GetIndicator(It.Is<Enumerations.IndicatorTypeEnum>(y => y == Enumerations.IndicatorTypeEnum.Macd), _logger))
                .Returns(macdIndicator.Object);

            // Volume Buy
            Mock<IIndicatorEngine> volumeIndicator = new Mock<IIndicatorEngine>();
            volumeIndicator.Setup(x => x.GetIndicatorPosition(It.IsAny<List<Candle>>(), It.IsAny<List<RuleSet>>()))
                .Returns(Enumerations.IndicatorSignalEnum.Buy);
            _iIndicatorFactoryWrapper.Setup(x => x.GetIndicator(It.Is<Enumerations.IndicatorTypeEnum>(y => y == Enumerations.IndicatorTypeEnum.Volume), _logger))
                .Returns(volumeIndicator.Object);

            _tradeBotMock = new Mock<Core.Bots.TradeBot>(_bot, _logger, _dbContext.Object, _mapper, _iIndicatorFactoryWrapper.Object);

            var tradeBot = _tradeBotMock.Object;

            // Action
            bool changePosition = tradeBot.OnCandle(candle);

            // Assert
            Assert.IsFalse(changePosition);
        }


        [TestMethod]
        public void Test_Agreeing_Buy_Indicators_NO_Current_Position()
        {
            // Arrange
            Candle candle = new Candle
            {
                VolumeQuantity = 20
            };
            
            // MACD Buy
            Mock<IIndicatorEngine> macdIndicator = new Mock<IIndicatorEngine>();
            macdIndicator.Setup(x => x.GetIndicatorPosition(It.IsAny<List<Candle>>(), It.IsAny<List<RuleSet>>()))
                .Returns(Enumerations.IndicatorSignalEnum.Buy);
            _iIndicatorFactoryWrapper.Setup(x => x.GetIndicator(It.Is<Enumerations.IndicatorTypeEnum>(y => y == Enumerations.IndicatorTypeEnum.Macd), _logger))
                .Returns(macdIndicator.Object);

            // Volume Buy
            Mock<IIndicatorEngine> volumeIndicator = new Mock<IIndicatorEngine>();
            volumeIndicator.Setup(x => x.GetIndicatorPosition(It.IsAny<List<Candle>>(), It.IsAny<List<RuleSet>>()))
                .Returns(Enumerations.IndicatorSignalEnum.Buy);
            _iIndicatorFactoryWrapper.Setup(x => x.GetIndicator(It.Is<Enumerations.IndicatorTypeEnum>(y => y == Enumerations.IndicatorTypeEnum.Volume), _logger))
                .Returns(volumeIndicator.Object);

            _tradeBotMock = new Mock<Core.Bots.TradeBot>(_bot, _logger, _dbContext.Object, _mapper, _iIndicatorFactoryWrapper.Object);

            var tradeBot = _tradeBotMock.Object;

            // Action
            bool changePosition = tradeBot.OnCandle(candle);

            // Assert
            Assert.IsTrue(changePosition);
        }

        [TestMethod]
        public void Test_Disagreeing_Buy_Indicators_NO_Current_Position()
        {
            // Arrange
            Candle candle = new Candle
            {
                VolumeQuantity = 20
            };

            // MACD Buy
            Mock<IIndicatorEngine> macdIndicator = new Mock<IIndicatorEngine>();
            macdIndicator.Setup(x => x.GetIndicatorPosition(It.IsAny<List<Candle>>(), It.IsAny<List<RuleSet>>()))
                .Returns(Enumerations.IndicatorSignalEnum.Sell);
            _iIndicatorFactoryWrapper.Setup(x => x.GetIndicator(It.Is<Enumerations.IndicatorTypeEnum>(y => y == Enumerations.IndicatorTypeEnum.Macd), _logger))
                .Returns(macdIndicator.Object);

            // Volume Buy
            Mock<IIndicatorEngine> volumeIndicator = new Mock<IIndicatorEngine>();
            volumeIndicator.Setup(x => x.GetIndicatorPosition(It.IsAny<List<Candle>>(), It.IsAny<List<RuleSet>>()))
                .Returns(Enumerations.IndicatorSignalEnum.Buy);
            _iIndicatorFactoryWrapper.Setup(x => x.GetIndicator(It.Is<Enumerations.IndicatorTypeEnum>(y => y == Enumerations.IndicatorTypeEnum.Volume), _logger))
                .Returns(volumeIndicator.Object);

            _tradeBotMock = new Mock<Core.Bots.TradeBot>(_bot, _logger, _dbContext.Object, _mapper, _iIndicatorFactoryWrapper.Object);

            var tradeBot = _tradeBotMock.Object;

            // Action
            bool changePosition = tradeBot.OnCandle(candle);

            // Assert
            Assert.IsFalse(changePosition);
        }

        [TestMethod]
        public void Test_Agreeing_Sell_Indicators_WITH_Current_Position()
        {
            // Arrange
            Candle candle = new Candle
            {
                VolumeQuantity = 20
            };

            _bot.CurrentPosition = 
                new Position
                {
                    Side = Enumerations.OrderSideEnum.Buy,
                    BuyPrice = 100,
                    Quantity = 1,
                    BuyTimeStamp = DateTime.Now,
                    Status = Enumerations.PositionStatusEnum.Bought
                
            };

            // MACD Buy
            Mock<IIndicatorEngine> macdIndicator = new Mock<IIndicatorEngine>();
            macdIndicator.Setup(x => x.GetIndicatorPosition(It.IsAny<List<Candle>>(), It.IsAny<List<RuleSet>>()))
                .Returns(Enumerations.IndicatorSignalEnum.Sell);
            _iIndicatorFactoryWrapper.Setup(x => x.GetIndicator(It.Is<Enumerations.IndicatorTypeEnum>(y => y == Enumerations.IndicatorTypeEnum.Macd), _logger))
                .Returns(macdIndicator.Object);

            // Volume Buy, should not be listened to
            Mock<IIndicatorEngine> volumeIndicator = new Mock<IIndicatorEngine>();
            volumeIndicator.Setup(x => x.GetIndicatorPosition(It.IsAny<List<Candle>>(), It.IsAny<List<RuleSet>>()))
                .Returns(Enumerations.IndicatorSignalEnum.Buy);
            _iIndicatorFactoryWrapper.Setup(x => x.GetIndicator(It.Is<Enumerations.IndicatorTypeEnum>(y => y == Enumerations.IndicatorTypeEnum.Volume), _logger))
                .Returns(volumeIndicator.Object);

            _tradeBotMock = new Mock<Core.Bots.TradeBot>(_bot, _logger, _dbContext.Object, _mapper, _iIndicatorFactoryWrapper.Object);

            var tradeBot = _tradeBotMock.Object;

            // Action
            bool changePosition = tradeBot.OnCandle(candle);

            // Assert
            Assert.IsTrue(changePosition);
        }


        private Bot GetBot()
        {
            return new Bot
            {
                BotId = 1,
                Name = "GDAX BTC-EUR 60 min Bot",
                Coin = new Coin() { Code = "EUR" },
                BaseCoin = new Coin() { Code = "BTC" },
                User = new User() { Name = "Test User", UserId = 999, ApiSettings = new List<ApiSetting> { new ApiSetting { ApiSettingId = 1, Exchange = new Exchange { ExchangeId = 1 } } } },
                Exchange = new Exchange { ExchangeId = 1 },
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
                                },
                                Indicator  = new Indicator
                                {
                                    IndicatorType = Enumerations.IndicatorTypeEnum.Macd,
                                    Bot = new Bot{Name = "Test Bot"}
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
                                },
                                Indicator  = new Indicator
                                {
                                    IndicatorType = Enumerations.IndicatorTypeEnum.Macd,
                                    Bot = new Bot{Name = "Test Bot"}
                                }
                            }
                        }
                    },
                    new Indicator // Volume Indicator
                    {
                        IndicatorType = Enumerations.IndicatorTypeEnum.Volume,
                        UseForSell = false,
                        UseForBuy = true,
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
                                        IndicatorRuleTypeId = (int)IndicatorRulesEnumerations.VolumeRulesEnum.IsAboveVolume,
                                        Value = 10M
                                    }
                                },
                                Indicator  = new Indicator
                                {
                                    IndicatorType = Enumerations.IndicatorTypeEnum.Volume,
                                    Bot = new Bot{Name = "Test Bot"}
                                }
                            }
                        }
                    }
                }

            };
        }

    }
}
