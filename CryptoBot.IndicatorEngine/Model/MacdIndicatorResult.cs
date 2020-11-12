using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CryptoBot.IndicatrorEngine.Model
{
    public class MacdIndicatorResult : IIndicatorResult
    {
        public bool Result { get; set; }
        public string Message { get; set; }
    }
}
