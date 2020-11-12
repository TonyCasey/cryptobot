using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CryptoBot.Model.Common;

namespace Api.CryptoBot.Models.DTO.Order
{
    public class OrderSearchRequestDto : SearchBase // must extend SearchBase
    {
        public long? OrderId { get; set; }

        public long? BotId { get; set; }

        public long? PositionId { get; set; }

        public Guid OurRef { get; set; }

        // string, e.g. BTCUSD
        public string Symbol { get; set; }

        // Trade ID on the exchange
        public string ExchangeOrderId { get; set; }

        public long? ExchangeId { get; set; }

        public Enumerations.OrderSideEnum? Side { get; set; }
    }
}
