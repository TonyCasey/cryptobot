using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Api.CryptoBot.Models.DTO.Coin
{
    public class CoinSearchResponseDto : SearchResponseBase<CoinResponseDto> // must extend SearchResponseBase
    {
        public string Description { get; set; }
    }
}
