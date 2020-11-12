using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CryptoBot.Model.Domain.Market;

namespace CryptoBot.Core.Ordering
{
    public interface IOrderManager
    {
        void Buy(Candle candle);
        void Sell(Candle candle);
    }
}
