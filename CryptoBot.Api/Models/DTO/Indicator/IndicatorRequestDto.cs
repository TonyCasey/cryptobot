using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CryptoBot.Model.Common;

namespace Api.CryptoBot.Models.DTO.Indicator
{
    public class IndicatorRequestDto : RequestBaseDto // must extend RequestBaseDto
    {
        public long IndicatorId { get; set; }
        
        public Enumerations.IndicatorTypeEnum IndicatorType { get; set; }
        
        public bool UseForBuy { get; set; }
        
        public bool UseForSell { get; set; }
    }
}
