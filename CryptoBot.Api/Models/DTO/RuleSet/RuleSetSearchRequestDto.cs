using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Api.CryptoBot.Models.DTO.RuleSet
{
    public class RuleSetSearchRequestDto : SearchBase // must extend SearchBase
    {
        public long IndicatorId { get; set; }
    }
}
