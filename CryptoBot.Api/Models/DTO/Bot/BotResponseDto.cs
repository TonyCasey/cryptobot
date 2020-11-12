using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CryptoBot.Model.Common;

namespace Api.CryptoBot.Models.DTO.Bot
{
    public class BotResponseDto : ResponseBaseDto // must extend ResponseBaseDto
    {
        public long BotId { get; set; }
        public string Name { get; set; }
        public long BaseCoinId { get; set; }
        public long CoinId { get; set; }
        public int ExchangeId { get; set; }
        public bool Active { get; set; } = true;
        public long? CurrentPositionId { get; set; }
        public decimal Amount { get; set; }
        public Enumerations.OrderTypeEnum OrderType{ get; set; }
        public Enumerations.CandleSizeEnum CandleSize { get; set; }
        public bool Accumulator { get; set; }
    }
}
