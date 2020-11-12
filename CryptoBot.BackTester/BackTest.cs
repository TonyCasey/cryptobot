using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CryptoBot.Core.Bots;
using CryptoBot.Core.Integrations.MessagingApps;
using CryptoBot.Core.Mapping;
using CryptoBot.Core.Messaging;
using CryptoBot.Core.Ordering;
using CryptoBot.Core.Trading;
using CryptoBot.Database;
using CryptoBot.ExchangeEngine;
using CryptoBot.ExchangeEngine.API.Exchanges;
using CryptoBot.ExchangeEngine.API.Services;
using CryptoBot.Model.Common;
using CryptoBot.Model.Domain.Account;
using CryptoBot.Model.Domain.Bot;
using CryptoBot.Model.Domain.Market;
using CryptoBot.Model.Domain.Trading;
using CryptoBot.Model.Exchanges;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NLog;
using Logger = Microsoft.VisualStudio.TestTools.UnitTesting.Logging.Logger;

namespace CryptoBot.BackTester
{
    [TestClass]
    public class BackTest : BaseTest
    {
        private CryptoBotDbContext _dbContext;
        private NLog.Logger _logger;
        private static MessageDispatcher _messageDispatcher;
        private static IMessagingApp _messagingApp;

        [TestInitialize]
        public void Initiatize()
        {
            _dbContext = new CryptoBotDbContext();
            _logger = LogManager.GetCurrentClassLogger();
            new MappingConfigurator();
            _messageDispatcher = new MessageDispatcher();
        }

        [TestMethod]
        public async Task BackTestAllBots()
        {
            // get user 
            User user = _dbContext
                .Users
                .Include(x => x.ApiSettings.Select(y => y.Exchange))
                .Include(x => x.MessagingApps)
                .Include(x => x.MessagingApps.Select(y => y.MessagingAppSettings))
                .Single(x => x.UserId == 1);

            // get watches, get coins, get exchange

            List<Bot> bots =
                _dbContext
                    .Bots
                    .Include("User")
                    .Include("User.ApiSettings")
                    .Include("User.ApiSettings.Exchange")
                    .Include("User.MessagingApps")
                    .Include("User.MessagingApps.MessagingAppSettings")
                    .Include("Exchange")
                    .Include("Coin")
                    .Include("BaseCoin")
                    .Include("Positions")
                    .Include("Indicators")
                    .Include("Indicators.RuleSets")
                    .Include("Indicators.RuleSets.Rules")
                    .AsNoTracking()
                    .Where(x => x.User.UserId == user.UserId)
                    .Where(x => x.Active)
                    .ToList();


            // get the messaging app if any
            if (user.MessagingApps.Any(x => x.Active))
            {
                var app = user.MessagingApps.First(x => x.Active);

                _messagingApp = MessagingAppFactory.GetMessagingApp(app, _logger, _dbContext);
            }

            int oneMinute = 60;
            int fifteenMinute = oneMinute * 15;
            int thirtyMinute = oneMinute * 30;
            int sixtyMinute = oneMinute * 60;




            foreach (Bot bot in bots)
            {
                IExchangeApi exchangeApi = ExchangeEngine.ExchangeFactory.GetExchangeApi(
                    (Enumerations.ExchangesEnum) bot.Exchange.ExchangeId,
                    _mapper.Map<ApiSetting, ExchangeSettings>(
                        user.ApiSettings.Single(x => x.Exchange.ExchangeId == bot.Exchange.ExchangeId),
                        new ExchangeSettings()));


                /*
                 *  Note from GDAX...
                 *  
                 *  The maximum number of data points for a single request is 200 candles. 
                 *  If your selection of start/end time and granularity will result in more than 200 data points, your request will be rejected. 
                 *  If you wish to retrieve fine granularity data over a larger time range, you will need to make multiple requests with new start/end ranges.
                 * 
                 */

                int requiredWeeks = 1;

                List<Candle> candles = new List<Candle>();

               

                
                var requiredHours = 36;

                for (int k = requiredHours; k > 0; k--)
                {
                    var fromDate = DateTime.UtcNow.Subtract(TimeSpan.FromMinutes( 60 * k  ));
                    fromDate = new DateTime(fromDate.Year, fromDate.Month, fromDate.Day, fromDate.Hour, 0, 0); // the start of the hour
                    var toDate = fromDate.AddHours(1);
                    toDate = new DateTime(toDate.Year, toDate.Month, toDate.Day, toDate.Hour, 0, 0);

                    int candleSize = 60 * 5; // 60 second x ?

                    try
                    {
                        candles.AddRange(exchangeApi.GetCandles(bot.BaseCoin, bot.Coin, candleSize, fromDate, toDate));
                    }
                    catch (Exception e)
                    {
                        _logger.Error(e);
                    }
                    


                    if (k % 2 == 0) // only three requests allowed per second on GDAX
                        Thread.Sleep(1000);
                }

                

                

                candles = candles.OrderBy(x => x.Timestamp).ToList();



                TradeBot tradeBot = new TradeBot(bot, _logger, _dbContext, _mapper, _IndicatorFactoryWrapper, _messagingApp);

                _messageDispatcher.AddMessageHandler(tradeBot);


                foreach (Candle candle in candles)
                {
                    _messageDispatcher.Dispatch(new CandleMessage
                    {
                        Candles = new List<Candle>() {candle},
                        BaseCoin = bot.BaseCoin,
                        Coin = bot.Coin
                    });

                }

                // close last order in case its open
                if (bot.CurrentPosition?.Status == Enumerations.PositionStatusEnum.Bought)
                {
                    Position position =
                        _dbContext.Positions.Single(x => x.PositionId == bot.CurrentPosition.PositionId);
                    position.Status = Enumerations.PositionStatusEnum.Sold;
                    _dbContext.SaveChanges();
                }


                tradeBot.OnStop();
                tradeBot = null;

            }

        }
    }

}
