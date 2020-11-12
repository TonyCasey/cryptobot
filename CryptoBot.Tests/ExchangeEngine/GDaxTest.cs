using System;
using CryptoBot.ExchangeEngine;
using CryptoBot.ExchangeEngine.API.Exchanges;
using CryptoBot.Model.Common;
using CryptoBot.Model.Domain.Market;
using CryptoBot.Model.Exchanges;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CryptoBot.Tests
{
    [TestClass]
    public class GDaxTest
    {
        private ExchangeSettings _exchangeSettings;

        [TestInitialize]
        public void Initiatize()
        {
            _exchangeSettings = new ExchangeSettings
            {
                Url = "api.gdax.com",
                SocketUrl = "wss://ws-feed.gdax.com",
                PassPhrase = "6i89kxgbwfd",
                ApiKey = "7defb75d4e46be15806188541dc0f425", // read only
                Secret = "Q7asiE0WkMzZeo3Z+QZ0kidQ+rj90cueBMdGsooZ81jgkoEQo43HuWeLnKVUFpkYGx6bQkxFPIPsAG3jcNRj1g==",
                CommissionRate = .25,
                Simulate = true
            };
        }

        [TestMethod]
        public void GetBTCTickerTest()
        {
            // Arrange
            IExchangeApi exchangeApi = ExchangeFactory.GetExchangeApi(Enumerations.ExchangesEnum.Gdax, _exchangeSettings);

            // Act
            var response = exchangeApi.GetTicker(new Coin(){Code = "BTC"}, new Coin(){Code = "EUR"} );


            // Asert
            Assert.IsNotNull(response);
        }

        [TestMethod]
        public void OpenWebSocketTest()
        {
            // Arrange 
            IExchangeApi exchangeApi = ExchangeFactory.GetExchangeApi(Enumerations.ExchangesEnum.Gdax, _exchangeSettings);

            // Act
            exchangeApi.OpenSocket();

        }
    }
}
