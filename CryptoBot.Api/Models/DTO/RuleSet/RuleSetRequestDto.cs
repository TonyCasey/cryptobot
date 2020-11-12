using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Api.CryptoBot.Models.DTO.RuleSet
{
    public class RuleSetRequestDto : RequestBaseDto // must extend RequestBaseDto
    {
        public long RulesetId { get; set; }
        public string Description { get; set; }
    }
}
