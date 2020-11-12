using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CryptoBot.Database;
using CryptoBot.Model.Common;
using CryptoBot.Model.Domain.Account;
using CryptoBot.Model.Domain.Bot;
using CryptoBot.Model.Domain.Trading;
using NLog;
using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Types.Enums;

namespace CryptoBot.Core.Integrations.MessagingApps
{
    public class TelegramMessagingApp : IMessagingApp
    {
        private readonly CryptoBotDbContext _dbContext;
        private long? _chatId;
        private readonly MessagingApp _app;
        private readonly Logger _logger;
        private static ITelegramBotClient _telegramBotClient;

        public TelegramMessagingApp(MessagingApp app, Logger logger, CryptoBotDbContext dbContext)
        {

            _app = app;
            _logger = logger;
            _dbContext = dbContext;

            if (!_app.MessagingAppSettings.Any() || _app.MessagingAppSettings.All(x => x.Key != Constants.Token))
                throw new Exception("Missing settings for Telegram messaging app");

            var token = _app.MessagingAppSettings?.Last(x => x.Key == Constants.Token)?.Value;

            // chat ID could be null. It needs a message to be sent from the user first, to activate it
            if (_app.MessagingAppSettings.Find(x => x.Key == Constants.ChatId) != null)
            {
                _chatId = Convert.ToInt32(_app.MessagingAppSettings.Last(x => x.Key == Constants.ChatId).Value);
            }

            _telegramBotClient = new TelegramBotClient(token);
            _telegramBotClient.OnMessage += OnMessage;
            _telegramBotClient.StartReceiving();

        }

        public void SendMessage(string message)
        {
            if (_chatId != null)
            {
                _telegramBotClient.SendTextMessageAsync(_chatId, message);
            }
            else
            {
                Log("No chatid set up yet. Please go to telegram and send a hello world message to the bot");
            }

        }

        public void OnMessage(object sender, EventArgs messageEventArg)
        {
            var message = ((MessageEventArgs)messageEventArg).Message;

            if (message == null || message.Type != MessageType.TextMessage) return;

            if (_chatId == null)
            {
                _chatId = message.Chat.Id;

                var newMessagingAppSettings =
                _dbContext.MessagingAppSettings.Add(new MessagingAppSettings
                {
                    MessagingApp = _app,
                    Key = Constants.ChatId,
                    Value = _chatId.ToString()
                });
                _dbContext.SaveChanges();

                _app.MessagingAppSettings.Add(newMessagingAppSettings); // Add to the user in memory

                Log("Hello world received & chatId saved. Your bot is now set up");
            }


            List<Bot> bots;

            switch (message.Text.Split(' ').First())
            {
                case "/profit":


                    _telegramBotClient.SendTextMessageAsync(_chatId,
                        $"Net profit: {_dbContext.Positions.Where(x => x.Bot.Active).Sum(y => y.NetProfit)}\n" +
                        $"Percent {_dbContext.Positions.Where(x => x.Bot.Active).Sum(y => y.NetProfitPercent)}%"
                    );

                    break;

                case "/positions":

                    StringBuilder sb = new StringBuilder();

                    bots = _dbContext.Bots.Where(x => x.Active).ToList();
                    IEnumerable<long?> openPositionIds = bots.Select(x => x.CurrentPositionId);

                    sb.Append(
                                $"{bots.Count(x => x.CurrentPositionId != null)} bots with open positions from {bots.Count} active bots\n\n");

                    List<Position> positions = _dbContext
                        .Positions
                        .Include("Bot")
                        .Include("Bot.Coin")
                        .Where(x => openPositionIds.Contains(x.PositionId))
                        .ToList();

                    foreach (Position position in positions)
                    {
                        sb.AppendLine($"{position.Bot.Name}\n \tcoin: {position.Bot.Coin.Code}\n \tbuy price: {position.BuyPrice}\n \tquantity: {position.Quantity}\n \tbuy timestamp: {position.BuyTimeStamp}");
                        sb.AppendLine("\n-------------------\n");
                    }

                    _telegramBotClient.SendTextMessageAsync(_chatId, sb.ToString());

                    break;
            }

        }

        private void Log(string message)
        {
            if (String.IsNullOrEmpty(message)) { _logger.Info(""); return; }
            _logger.Info($"[Telegram Messaging App] - {message}");
        }
    }
}
