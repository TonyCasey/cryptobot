using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Api.CryptoBot.Models.DTO.Coin
{
    public class CoinSearchRequestDto : SearchBase // must extend SearchBase
    {
        public string Code { get; set; }
    }
}
