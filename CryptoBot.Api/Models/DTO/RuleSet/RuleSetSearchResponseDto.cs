using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Api.CryptoBot.Models.DTO.RuleSet
{
    public class RuleSetSearchResponseDto : SearchResponseBase< RuleSetResponseDto > // must extend SearchResponseBase
    {
        public string Description { get; set; }
    }
}
