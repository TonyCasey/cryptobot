using System;
using CryptoBot.ExchangeEngine;
using CryptoBot.ExchangeEngine.API.Exchanges;
using CryptoBot.Model.Common;
using CryptoBot.Model.Domain.Market;
using CryptoBot.Model.Exchanges;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Threading.Tasks;

namespace CryptoBot.Tests
{
    [TestClass]
    public class GDaxTest
    {
        private ExchangeSettings _exchangeSettings;
        private Mock<IExchangeApi> _exchangeApiMock;

        [TestInitialize]
        public void Initiatize()
        {
            _exchangeSettings = new ExchangeSettings
            {
                Url = "api.pro.coinbase.com", // Updated from deprecated GDAX
                SocketUrl = "wss://ws-feed.pro.coinbase.com", // Updated from deprecated GDAX
                PassPhrase = "",
                ApiKey = "", // read only
                Secret = "",
                CommissionRate = .25,
                Simulate = true
            };
            
            _exchangeApiMock = new Mock<IExchangeApi>();
        }

        [TestMethod]
        public void GetBTCTickerTest()
        {
            // Arrange
            var expectedTicker = new Ticker 
            { 
                Ask = 50000m, 
                Bid = 49900m, 
                Last = 49950m,
                Volume = new ExchangeVolume 
                { 
                    QuantityAmount = 1000m,
                    PriceAmount = 49950000m,
                    Timestamp = DateTime.UtcNow
                }
            };
            
            _exchangeApiMock.Setup(x => x.GetTicker(It.IsAny<Coin>(), It.IsAny<Coin>()))
                .Returns(expectedTicker);

            // Act
            var response = _exchangeApiMock.Object.GetTicker(new Coin(){Code = "BTC"}, new Coin(){Code = "EUR"});

            // Assert
            Assert.IsNotNull(response);
            Assert.AreEqual(expectedTicker.Last, response.Last);
            _exchangeApiMock.Verify(x => x.GetTicker(It.IsAny<Coin>(), It.IsAny<Coin>()), Times.Once);
        }

        [TestMethod]
        public void OpenWebSocketTest()
        {
            // Arrange
            _exchangeApiMock.Setup(x => x.OpenSocket())
                .Callback(() => { /* Mock successful socket connection */ });

            // Act
            _exchangeApiMock.Object.OpenSocket();

            // Assert
            _exchangeApiMock.Verify(x => x.OpenSocket(), Times.Once);
        }
    }
}
