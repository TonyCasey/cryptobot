using CryptoBot.Core.Ordering;
using CryptoBot.Core.Trading;
using CryptoBot.Model.Common;
using CryptoBot.Model.Domain.Account;
using CryptoBot.Model.Domain.Bot;
using CryptoBot.Model.Domain.Market;
using CryptoBot.Model.Exchanges;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NLog;
using System;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using CryptoBot.Core.Integrations.MessagingApps;
using CryptoBot.Database;
using CryptoBot.ExchangeEngine.API.Exchanges;
using Logger = NLog.Logger;

namespace CryptoBot.Tests.Orders
{
    [TestClass]
    public class ExchangeIntegrationTest : BaseTest
    {
        private Logger _logger;
        private CryptoBotDbContext _dbContext;        
        private User _user;
        private IExchangeApi _exchangeApi;
        private static IMessagingApp _messagingApp;

        [TestInitialize]
        public void Initiatize()
        {
            var optionsBuilder = new DbContextOptionsBuilder<CryptoBotDbContext>();
            optionsBuilder.UseSqlServer("Data Source=.;Initial Catalog=CryptoBot_Dev;Integrated Security=true");
            _dbContext = new CryptoBotDbContext(optionsBuilder.Options);                        
            _logger = LogManager.GetCurrentClassLogger();
            _user = _dbContext
                .Users
                .Include(x => x.ApiSettings.Select(y => y.Exchange))
                .Include(x => x.MessagingApps)
                .Include(x => x.MessagingApps.Select(y => y.MessagingAppSettings))
                .Single(x => x.UserId == 1);

            // get the messaging app if any
            if (_user.MessagingApps.Any(x => x.Active))
            {
                var app = _user.MessagingApps.First(x => x.Active);

                _messagingApp = MessagingAppFactory.GetMessagingApp(app, _logger, _dbContext);
            }
        }

        [TestMethod]
        public void GdaxIntegrationBuyTest()
        {
            Bot bot = _dbContext
                .Bots
                .Include("BaseCoin")
                .Include("Coin")
                .Single(x => x.BotId == 1);                        

            ExchangeSettings exchangeSettings = _mapper.Map<ApiSetting, ExchangeSettings>(
                _user.ApiSettings.Single(x => x.Exchange.ExchangeId == bot.Exchange.ExchangeId),
                new ExchangeSettings());

            exchangeSettings.Simulate = false;

            _exchangeApi = ExchangeEngine.ExchangeFactory.GetExchangeApi(
                (Enumerations.ExchangesEnum) bot.Exchange.ExchangeId,exchangeSettings);

            Ticker ticker = _exchangeApi.GetTicker(bot.BaseCoin, bot.Coin);
            
            bot.Amount = 10m; // 3 euros

            OrderManager orderManager = new OrderManager(bot, new Trader(_logger), _dbContext, _logger, _mapper, _messagingApp);

            orderManager.Buy(new Candle(){ClosePrice = ticker.Ask, Timestamp = DateTime.Now});
            
        }

        [TestMethod]
        public void GdaxIntegrationSellTest()
        {

            Bot bot = _dbContext
                .Bots
                .Include("BaseCoin")
                .Include("Coin")
                .Include("CurrentPosition")
                .Single(x => x.BotId == 1);

            if (bot == null || bot.CurrentPosition == null ||
                bot.CurrentPosition.Status != Enumerations.PositionStatusEnum.Bought)
            {
                throw new Exception("No current open position");
            }
            
            ExchangeSettings exchangeSettings = _mapper.Map<ApiSetting, ExchangeSettings>(
                _user.ApiSettings.Single(x => x.Exchange.ExchangeId == bot.Exchange.ExchangeId),
                new ExchangeSettings());

            exchangeSettings.Simulate = false;

            _exchangeApi = ExchangeEngine.ExchangeFactory.GetExchangeApi(
                (Enumerations.ExchangesEnum)bot.Exchange.ExchangeId, exchangeSettings);


            Ticker ticker = _exchangeApi.GetTicker(bot.BaseCoin, bot.Coin);


            bot.Amount = 10M; // €2

            OrderManager orderManager = new OrderManager(bot, new Trader(_logger), _dbContext, _logger, _mapper, _messagingApp);

            orderManager.Sell(new Candle() { ClosePrice = ticker.Ask, Timestamp = DateTime.Now });

        }

        [TestMethod]
        public void BittrexIntegrationBuyTest()
        {

            Bot bot = _dbContext
                .Bots
                .Include("BaseCoin")
                .Include("Coin")
                .Single(x => x.BotId == 4);



            ExchangeSettings exchangeSettings = _mapper.Map<ApiSetting, ExchangeSettings>(
                _user.ApiSettings.Single(x => x.Exchange.ExchangeId == bot.Exchange.ExchangeId),
                new ExchangeSettings());

            exchangeSettings.Simulate = false;

            _exchangeApi = ExchangeEngine.ExchangeFactory.GetExchangeApi(
                (Enumerations.ExchangesEnum)bot.Exchange.ExchangeId, exchangeSettings);


            Ticker ticker = _exchangeApi.GetTicker(bot.BaseCoin, bot.Coin);

            bot.Amount = .0005M; // BTC

            OrderManager orderManager = new OrderManager(bot, new Trader(_logger), _dbContext, _logger, _mapper, _messagingApp);

            orderManager.Buy(new Candle() { ClosePrice = ticker.Ask, Timestamp = DateTime.Now });

        }


        [TestMethod]
        public void BittrexIntegrationSellTest()
        {

            Bot bot = _dbContext
                .Bots
                .Include("BaseCoin")
                .Include("Coin")
                .Include("CurrentPosition")
                .Single(x => x.BotId == 4);

            if (bot == null || bot.CurrentPosition == null ||
                bot.CurrentPosition.Status != Enumerations.PositionStatusEnum.Bought)
            {
                throw new Exception("No current open position");
            }

            ExchangeSettings exchangeSettings = _mapper.Map<ApiSetting, ExchangeSettings>(
                _user.ApiSettings.Single(x => x.Exchange.ExchangeId == bot.Exchange.ExchangeId),
                new ExchangeSettings());

            exchangeSettings.Simulate = false;

            _exchangeApi = ExchangeEngine.ExchangeFactory.GetExchangeApi(
                (Enumerations.ExchangesEnum)bot.Exchange.ExchangeId, exchangeSettings);


            Ticker ticker = _exchangeApi.GetTicker(bot.BaseCoin, bot.Coin);


            bot.Amount = .0005M; // BTC

            OrderManager orderManager = new OrderManager(bot, new Trader(_logger), _dbContext, _logger, _mapper, _messagingApp);

            orderManager.Sell(new Candle() { ClosePrice = ticker.Ask, Timestamp = DateTime.Now });

        }
    }
}
