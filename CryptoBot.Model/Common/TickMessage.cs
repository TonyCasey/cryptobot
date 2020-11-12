using System.Collections;
using System.Runtime.Remoting.Messaging;
using CryptoBot.Model.Domain.Market;

namespace CryptoBot.Model.Common
{
    public class TickMessage : IMessage
    {
        public Ticker Ticker { get; set; }
        public IDictionary Properties { get; }
    }
}
