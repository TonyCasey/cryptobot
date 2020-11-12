using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Api.CryptoBot.Models.DTO.Rule
{
    public class RuleSearchRequestDto : SearchBase // must extend SearchBase
    {
        public long RuleId { get; set; }

        public int IndicatorRuleTypeId { get; set; }
        
    }
}
