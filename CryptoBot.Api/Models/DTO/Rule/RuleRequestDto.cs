using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Api.CryptoBot.Models.DTO.Rule
{
    public class RuleRequestDto : RequestBaseDto // must extend RequestBaseDto
    {
        public long RuleId { get; set; }

        public int IndicatorRuleTypeId { get; set; }

        public decimal? Value { get; set; }
    }
}
