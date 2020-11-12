using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CryptoBot.IndicatrorEngine
{
    public interface IIndicatorResult
    {
        bool Result { get; set; }
        string Message { get; set; }
    }
}
