using System.Collections;
using CryptoBot.Model.Domain.Market;

namespace CryptoBot.Model.Common
{
    public class TickMessage : IMessage
    {
        public Ticker Ticker { get; set; }
        public IDictionary Properties { get; }
    }
}
