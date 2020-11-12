using CryptoBot.Database;
using CryptoBot.Model.Domain.Account;
using NLog;
using Enumerations = CryptoBot.Model.Common.Enumerations;

namespace CryptoBot.Core.Integrations.MessagingApps
{
    public static class MessagingAppFactory 
    {
        public static IMessagingApp GetMessagingApp(MessagingApp messagingApp, Logger logger, CryptoBotDbContext dbContext)
        {
            switch (messagingApp.MessagingAppType)
            {
                case Enumerations.MessagingAppEnum.Telegram:
                    return new TelegramMessagingApp(messagingApp, logger, dbContext);
                case Enumerations.MessagingAppEnum.Slack:
                    // TODO:
                    break;
            }
            return null;
        }
    }
}
