using Autofac;
using AutoMapper;
using CryptoBot.Core.Bots;
using CryptoBot.Core.Integrations.MessagingApps;
using CryptoBot.Core.Mapping;
using CryptoBot.Core.Messaging;
using CryptoBot.Core.Scheduling;
using CryptoBot.IndicatrorEngine;
using CryptoBot.Model.Common;
using CryptoBot.Model.Domain.Account;
using CryptoBot.Model.Domain.Bot;
using CryptoBot.Model.Domain.Market;
using CryptoBot.Model.Exchanges;
using NLog;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.ServiceProcess;
using System.Threading;
using System.Threading.Tasks;
using CryptoBot.Database;
using User = CryptoBot.Model.Domain.Account.User;

namespace CryptoBot
{


    public class Program
    {
        private static IContainer Container { get; set; }

        static Logger _logger;
        private static CryptoBotDbContext _dbContext;
        private static MessageDispatcher _messageDispatcher;
        private static IMapper _mapper;
        private static IIndicatorFactoryWrapper _indicatorFactoryWrapper;
        private static IMessagingApp _messagingApp;

        #region Nested classes to support running as service
        public const string ServiceName = "MyService";

        public class Service : ServiceBase
        {
            public Service()
            {
                ServiceName = Program.ServiceName;
            }

            protected override void OnStart(string[] args)
            {
                Program.Start(args);
            }

            protected override void OnStop()
            {
                Program.Stop();
            }
        }
        #endregion

        private static async void Start(string[] args)
        {
            // onstart code here
            _dbContext = new CryptoBotDbContext();
            _logger = LogManager.GetCurrentClassLogger();
            _messageDispatcher = new MessageDispatcher();
            _indicatorFactoryWrapper = new IndicatorFactoryWrapper();


            SetupMapping();


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

            var activeBots = bots.Where(x => x.Active).ToList();

            // get the messaging app if any
            if (user.MessagingApps.Any(x => x.Active))
            {
                var app = user.MessagingApps.First(x => x.Active);

                _messagingApp = MessagingAppFactory.GetMessagingApp(app, _logger, _dbContext);

                _messagingApp.SendMessage("CryptoBot starting..");
            }


            foreach (var bot in activeBots)
            {
                var tradeBot = new TradeBot(bot, _logger, _dbContext, _mapper, _indicatorFactoryWrapper, _messagingApp);

                var exchangeApi = ExchangeEngine.ExchangeFactory.GetExchangeApi(
                    (Enumerations.ExchangesEnum)bot.Exchange.ExchangeId,
                    _mapper.Map<ApiSetting, ExchangeSettings>(
                        user.ApiSettings.Single(x => x.Exchange.ExchangeId == bot.Exchange.ExchangeId),
                        new ExchangeSettings()));
                

                var candles = new List<Candle>();
                
                var requiredHours = 48;

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


                // ***** VERY IMPORTANT *****
                // When candles are preloaded into the bot they do not go through any indicator rules.
                // When the bot starts, it could be starting 10 mins after the hour and it could go asleep unless we tell it to check the last candle.
                //
                // If we do not run the last candle through the indicators then no indicator logic will run until the next hour and we could be missing a move.
                // When we preload, we will not add the last two candles. When the scheduler starts, it asks for the last two candles.
                // So, we will allow it to pass these into the bot which will mean it will pass them through the indicators and take any action it might need to.
                tradeBot.PreLoadCandles(candles.Take(candles.Count() - 1).ToList());

                _messageDispatcher.AddMessageHandler(tradeBot);

                tradeBot.Start();

                _messagingApp.SendMessage($"{bot.Name} started");
            }

            AddSchedulerForEachActiveBotExchange(activeBots, user);

            Console.ReadLine();
        }

        private static void Stop()
        {
            // onstop code here
        }

        public static void Main(string[] args)
        {
            if (!Environment.UserInteractive)
                // running as service
                using (var service = new Service())
                    ServiceBase.Run(service);
            else
            {
                // running as console app
                Start(args);

                Console.WriteLine("Press any key to stop...");
                Console.ReadKey(true);

                Stop();
            }

        }

        private static void AddSchedulerForEachActiveBotExchange(List<Bot> activeBots, User user)
        {
            int oneMinuteInSeconds = 60;
            int fiveMinuteInSeconds = 5 * oneMinuteInSeconds;
            int fifteenMinuteInSeconds = oneMinuteInSeconds * 15;
            int thirtyMinuteInSeconds = oneMinuteInSeconds * 30;
            int sixtyMinuteInSeconds = oneMinuteInSeconds * 60;
            IEnumerable<Exchange> activeExchanges = activeBots.Select(y => y.Exchange);

            // get the individual exchanges and start a scheduler for each one
            foreach (var exchange in activeExchanges)
            {
                var exchangeApi = ExchangeEngine.ExchangeFactory.GetExchangeApi(
                    (Enumerations.ExchangesEnum)exchange.ExchangeId,
                    _mapper.Map<ApiSetting, ExchangeSettings>(
                        user.ApiSettings.Single(x => x.Exchange.ExchangeId == exchange.ExchangeId),
                        new ExchangeSettings()));

                var coins = activeBots.Where(x => x.Exchange == exchange).ToDictionary(bot => bot.BaseCoin, bot => bot.Coin);

                Task.Factory.StartNew(
                    () => { new Scheduler(exchangeApi, _messageDispatcher, Enumerations.SchedulerType.ScheduledEventOccursEveryValueInSeconds, 300, fiveMinuteInSeconds, coins, _logger).Start(); });
            }
        }

        private static void RegisterIoc()
        {
            var builder = new ContainerBuilder();
            builder.RegisterType<Logger>().As<Logger>();
            builder.RegisterType<CryptoBotDbContext>().As<CryptoBotDbContext>();
            Container = builder.Build();
        }

        private static void SetupMapping()
        {
            var mapperConfig = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<MappingConfigurator>();
            });

            _mapper = new Mapper(mapperConfig);
        }
    }
}
