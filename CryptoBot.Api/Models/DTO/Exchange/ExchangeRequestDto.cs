using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Api.CryptoBot.Models.DTO.Exchange
{
    public class ExchangeRequestDto : RequestBaseDto // must extend RequestBaseDto
    {
        public int ExchangeId { get; set; }
       
        public string Name { get; set; }
        
        public string Code { get; set; }
    }
}
