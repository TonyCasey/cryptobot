using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;
using CryptoBot.Model.Domain.Market;

namespace CryptoBot.Model.Common
{
    public class CandleMessage : IMessage
    {
        public Coin BaseCoin { get; set; }
        public Coin Coin { get; set; }
        public IEnumerable<Candle> Candles { get; set; }
        public IDictionary Properties { get; }
    }
}
