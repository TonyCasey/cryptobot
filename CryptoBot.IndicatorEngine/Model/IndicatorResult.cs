using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CryptoBot.Model.Common;
using CryptoBot.Model.Domain.Bot;

namespace CryptoBot.IndicatrorEngine.Model
{
    public class IndicatorResult : IIndicatorResult
    {
        public bool RuleResult { get; set; }
        public string IndicatorRuleName { get; set;  }
        public string Message { get; set; }
        public bool Result { get; set; }
    }
}
