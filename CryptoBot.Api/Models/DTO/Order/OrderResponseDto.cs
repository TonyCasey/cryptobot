using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using CryptoBot.Model.Common;
using CryptoBot.Model.Domain.Trading;

namespace Api.CryptoBot.Models.DTO.Order
{
    public class OrderResponseDto : ResponseBaseDto // must extend ResponseBaseDto
    {
        public long OrderId { get; set; }

        public Guid OurRef { get; set; }

        // string, e.g. BTCUSD
        public string Symbol { get; set; }

        // Trade ID on the exchange
        public string ExchangeOrderId { get; set; }

        public double Price { get; set; }

        public double Quantity { get; set; }

        public double QuantityFilled { get; set; }
        
        public DateTime OrderDate { get; set; }
        
        public Enumerations.OrderStatusEnum Status { get; set; }

        // buy or sell
        public Enumerations.OrderSideEnum Side { get; set; }

        public Enumerations.OrderTypeEnum Type { get; set; }

        // UTC timestamp in milliseconds
        public DateTime? TimeStamp { get; set; }
        
        public long? PositionId { get; set; }

        public long BotId { get; set; }

        public double Fees { get; set; }
    }
}
