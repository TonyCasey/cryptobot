using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Api.CryptoBot.Models.DTO.Coin
{
    public class CoinResponseDto : ResponseBaseDto // must extend ResponseBaseDto
    {
        public long CoinId { get; set; }
        public string Code { get; set; }
        public int OrderRoundingExponent { get; set; }
    }
}
