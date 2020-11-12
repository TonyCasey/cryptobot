using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CryptoBot.Model.Common;

namespace Api.CryptoBot.Models.DTO.Order
{
    public class OrderRequestDto : RequestBaseDto // must extend RequestBaseDto
    {
        public long OrderId { get; set; }

        public Guid OurRef { get; set; }
        
        public long BotId { get; set; }

        public long? PositionId { get; set; }

        public double Price { get; set; }

        public double Quantity { get; set; }

        public double QuantityFilled { get; set; }
        
        public Enumerations.OrderStatusEnum Status { get; set; }
        
        public Enumerations.OrderSideEnum Side { get; set; }

        public Enumerations.OrderTypeEnum Type { get; set; }
        
        public double Fees { get; set; }

        public DateTime OrderDate { get; set; }
    }
}
