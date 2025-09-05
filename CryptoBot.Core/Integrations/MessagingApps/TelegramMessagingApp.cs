using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using CryptoBot.Database;
using CryptoBot.Model.Common;
using CryptoBot.Model.Domain.Account;
using CryptoBot.Model.Domain.Bot;
using CryptoBot.Model.Domain.Trading;
using Microsoft.EntityFrameworkCore;
using NLog;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Exceptions;

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
            
            // Use new API for receiving messages
            var receiverOptions = new ReceiverOptions
            {
                AllowedUpdates = { } // receive all update types
            };
            
            _telegramBotClient.StartReceiving(
                HandleUpdateAsync,
                HandlePollingErrorAsync,
                receiverOptions,
                cancellationToken: CancellationToken.None
            );

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

        // Legacy interface implementation - keeping for compatibility
        public void OnMessage(object sender, EventArgs messageEventArg)
        {
            // This method is required by IMessagingApp interface but not used with new Telegram.Bot API
            // The new HandleUpdateAsync method handles message processing
        }

        private async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {
            if (update.Type != UpdateType.Message || update.Message?.Text == null)
                return;

            var message = update.Message;

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

                _app.MessagingAppSettings.Add(newMessagingAppSettings.Entity); // Add to the user in memory

                Log("Hello world received & chatId saved. Your bot is now set up");
            }


            List<Bot> bots;

            switch (message.Text.Split(' ').First())
            {
                case "/profit":


                    await _telegramBotClient.SendTextMessageAsync(_chatId,
                        $"Net profit: {_dbContext.Positions.Where(x => x.Bot.Active).Sum(y => y.NetProfit)}\n" +
                        $"Percent {_dbContext.Positions.Where(x => x.Bot.Active).Sum(y => y.NetProfitPercent)}%",
                        cancellationToken: cancellationToken
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
                        .Include(x => x.Bot)
                        .Include(x => x.Bot.Coin)
                        .Where(x => openPositionIds.Contains(x.PositionId))
                        .ToList();

                    foreach (Position position in positions)
                    {
                        sb.AppendLine($"{position.Bot.Name}\n \tcoin: {position.Bot.Coin.Code}\n \tbuy price: {position.BuyPrice}\n \tquantity: {position.Quantity}\n \tbuy timestamp: {position.BuyTimeStamp}");
                        sb.AppendLine("\n-------------------\n");
                    }

                    await _telegramBotClient.SendTextMessageAsync(_chatId, sb.ToString(), cancellationToken: cancellationToken);

                    break;
            }

        }

        private Task HandlePollingErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
        {
            var errorMessage = exception switch
            {
                ApiRequestException apiRequestException
                    => $"Telegram API Error:\n[{apiRequestException.ErrorCode}]\n{apiRequestException.Message}",
                _ => exception.ToString()
            };

            Log($"Telegram polling error: {errorMessage}");
            return Task.CompletedTask;
        }

        private void Log(string message)
        {
            if (String.IsNullOrEmpty(message)) { _logger.Info(""); return; }
            _logger.Info($"[Telegram Messaging App] - {message}");
        }
    }
}
