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
        private new Logger _logger;
        private new CryptoBotDbContext _dbContext;        
        private User _user;
        private Mock<IExchangeApi> _exchangeApiMock;
        private static Mock<IMessagingApp> _messagingAppMock;

        private void SeedTestData()
        {
            // Check if data already exists
            if (_dbContext.Exchanges.Any())
                return;
                
            // Add test exchange
            var exchange = new Exchange { ExchangeId = 1, Name = "Test Exchange" };
            _dbContext.Exchanges.Add(exchange);
            
            // Add test coins
            var btc = new Coin { Code = "BTC" };
            var eur = new Coin { Code = "EUR" };
            _dbContext.Coins.Add(btc);
            _dbContext.Coins.Add(eur);
            
            // Add test user with API settings
            var user = new User 
            { 
                UserId = 1, 
                Name = "Test User",
                ApiSettings = new System.Collections.Generic.List<ApiSetting>
                {
                    new ApiSetting 
                    { 
                        ApiSettingId = 1,
                        Exchange = exchange,
                        Key = "test-key",
                        Secret = "test-secret",
                        Url = "https://api.test.com",
                        Simulated = true
                    }
                },
                MessagingApps = new System.Collections.Generic.List<MessagingApp>()
            };
            _dbContext.Users.Add(user);
            
            // Add test positions
            var position1 = new Position
            {
                BuyPrice = 45000.0,
                Quantity = 0.1,
                Status = Enumerations.PositionStatusEnum.Bought,
                BuyTimeStamp = DateTime.UtcNow.AddMinutes(-30),
                Side = Enumerations.OrderSideEnum.Buy
            };
            _dbContext.Positions.Add(position1);
            
            var position4 = new Position
            {
                BuyPrice = 45000.0,
                Quantity = 0.1,
                Status = Enumerations.PositionStatusEnum.Bought,
                BuyTimeStamp = DateTime.UtcNow.AddMinutes(-30),
                Side = Enumerations.OrderSideEnum.Buy
            };
            _dbContext.Positions.Add(position4);
            
            _dbContext.SaveChanges(); // Save positions first to get IDs
            
            // Add test bots
            var bot1 = new Bot
            {
                BotId = 1,
                Name = "Test Bot 1",
                BaseCoin = btc,
                Coin = eur,
                Exchange = exchange,
                User = user,
                Amount = 100m,
                Active = true,
                CurrentPositionId = position1.PositionId,
                CurrentPosition = position1
            };
            _dbContext.Bots.Add(bot1);
            
            var bot4 = new Bot
            {
                BotId = 4,
                Name = "Test Bot 4",
                BaseCoin = btc,
                Coin = eur,
                Exchange = exchange,
                User = user,
                Amount = 100m,
                Active = true,
                CurrentPositionId = position4.PositionId,
                CurrentPosition = position4
            };
            _dbContext.Bots.Add(bot4);
            
            _dbContext.SaveChanges();
        }
        
        [TestInitialize]
        public void Initiatize()
        {
            // Setup Entity Framework Core options for testing  
            // Use a unique database name for each test to avoid conflicts
            var dbName = "TestDatabase_" + Guid.NewGuid().ToString();
            var optionsBuilder = new DbContextOptionsBuilder<CryptoBotDbContext>();
            optionsBuilder.UseInMemoryDatabase(databaseName: dbName);
            
            _dbContext = new CryptoBotDbContext(optionsBuilder.Options);                        
            _logger = LogManager.GetCurrentClassLogger();
            
            // Seed test data
            SeedTestData();
            _user = _dbContext
                .Users
                .Include(x => x.ApiSettings)
                    .ThenInclude(y => y.Exchange)
                .Include(x => x.MessagingApps)
                    .ThenInclude(y => y.MessagingAppSettings)
                .SingleOrDefault(x => x.UserId == 1);
                
            // If user is null, create a default test user
            if (_user == null)
            {
                _user = new User 
                { 
                    UserId = 1, 
                    Name = "Test User",
                    ApiSettings = new System.Collections.Generic.List<ApiSetting>(),
                    MessagingApps = new System.Collections.Generic.List<MessagingApp>()
                };
            }

            // Setup mocks
            _exchangeApiMock = new Mock<IExchangeApi>();
            _messagingAppMock = new Mock<IMessagingApp>();
            
            // Setup mock ticker response
            var mockTicker = new Ticker
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
                .Returns(mockTicker);
        }

        [TestMethod]
        public void GdaxIntegrationBuyTest()
        {
            Bot bot = _dbContext
                .Bots
                .Include("BaseCoin")
                .Include("Coin")
                .Single(x => x.BotId == 1);                        

            ExchangeSettings exchangeSettings = _user?.ApiSettings?.Any() == true 
                ? _mapper.Map<ApiSetting, ExchangeSettings>(
                    _user.ApiSettings.Single(x => x.Exchange.ExchangeId == bot.Exchange.ExchangeId),
                    new ExchangeSettings())
                : new ExchangeSettings { Simulate = true };

            exchangeSettings.Simulate = false;

            // Use mocked exchange API instead of real one
            Ticker ticker = _exchangeApiMock.Object.GetTicker(bot.BaseCoin, bot.Coin);
            
            bot.Amount = 10m; // 3 euros

            OrderManager orderManager = new OrderManager(bot, new Trader(_logger), _dbContext, _logger, _mapper, _messagingAppMock.Object);

            orderManager.Buy(new Candle(){ClosePrice = ticker.Ask, Timestamp = DateTime.Now});
            
        }

        [TestMethod]
        public void GdaxIntegrationSellTest()
        {

            Bot bot = _dbContext
                .Bots
                .Include("BaseCoin")
                .Include("Coin")
                .Single(x => x.BotId == 1);

            if (bot == null || bot.CurrentPosition == null ||
                bot.CurrentPosition.Status != Enumerations.PositionStatusEnum.Bought)
            {
                throw new Exception("No current open position");
            }
            
            ExchangeSettings exchangeSettings = _user?.ApiSettings?.Any() == true 
                ? _mapper.Map<ApiSetting, ExchangeSettings>(
                    _user.ApiSettings.Single(x => x.Exchange.ExchangeId == bot.Exchange.ExchangeId),
                    new ExchangeSettings())
                : new ExchangeSettings { Simulate = true };

            exchangeSettings.Simulate = false;

            // Use mocked exchange API instead of real one
            Ticker ticker = _exchangeApiMock.Object.GetTicker(bot.BaseCoin, bot.Coin);


            bot.Amount = 10M; // €2

            OrderManager orderManager = new OrderManager(bot, new Trader(_logger), _dbContext, _logger, _mapper, _messagingAppMock.Object);

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



            ExchangeSettings exchangeSettings = _user?.ApiSettings?.Any() == true 
                ? _mapper.Map<ApiSetting, ExchangeSettings>(
                    _user.ApiSettings.Single(x => x.Exchange.ExchangeId == bot.Exchange.ExchangeId),
                    new ExchangeSettings())
                : new ExchangeSettings { Simulate = true };

            exchangeSettings.Simulate = false;

            // Use mocked exchange API instead of real one
            Ticker ticker = _exchangeApiMock.Object.GetTicker(bot.BaseCoin, bot.Coin);

            bot.Amount = .0005M; // BTC

            OrderManager orderManager = new OrderManager(bot, new Trader(_logger), _dbContext, _logger, _mapper, _messagingAppMock.Object);

            orderManager.Buy(new Candle() { ClosePrice = ticker.Ask, Timestamp = DateTime.Now });

        }


        [TestMethod]
        public void BittrexIntegrationSellTest()
        {

            Bot bot = _dbContext
                .Bots
                .Include("BaseCoin")
                .Include("Coin")
                .Single(x => x.BotId == 4);

            if (bot == null || bot.CurrentPosition == null ||
                bot.CurrentPosition.Status != Enumerations.PositionStatusEnum.Bought)
            {
                throw new Exception("No current open position");
            }

            ExchangeSettings exchangeSettings = _user?.ApiSettings?.Any() == true 
                ? _mapper.Map<ApiSetting, ExchangeSettings>(
                    _user.ApiSettings.Single(x => x.Exchange.ExchangeId == bot.Exchange.ExchangeId),
                    new ExchangeSettings())
                : new ExchangeSettings { Simulate = true };

            exchangeSettings.Simulate = false;

            // Use mocked exchange API instead of real one
            Ticker ticker = _exchangeApiMock.Object.GetTicker(bot.BaseCoin, bot.Coin);


            bot.Amount = .0005M; // BTC

            OrderManager orderManager = new OrderManager(bot, new Trader(_logger), _dbContext, _logger, _mapper, _messagingAppMock.Object);

            orderManager.Sell(new Candle() { ClosePrice = ticker.Ask, Timestamp = DateTime.Now });

        }
    }
}
