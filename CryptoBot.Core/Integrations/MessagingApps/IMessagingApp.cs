using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CryptoBot.IndicatrorEngine.Volume;

namespace CryptoBot.Core.Integrations.MessagingApps
{
    public interface IMessagingApp
    {
        void SendMessage(string message);
        void OnMessage(object sender, EventArgs messageEventArg);
    }
}
