using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CryptoBot.Model.Common;
using CryptoBot.Model.Domain.Account;

namespace CryptoBot.Core.Integrations.MessagingApps
{
    public interface IMessagingAppFactory
    {
        IMessagingApp GetMessagingApp(Enumerations.MessagingAppEnum messagingApp, User user);
    }
}
