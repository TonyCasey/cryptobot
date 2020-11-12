using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using CryptoBot.Model.Common;
using CryptoBot.Model.Domain.Market;

namespace CryptoBot.Model.Domain.Trading
{
    [Table("Orders", Schema = "Trading")]
    public class Order : BaseEntity
    {
        public long OrderId { get; set; }

        public Guid OurRef { get; set; }

        // string, e.g. BTCUSD
        [StringLength(10)]
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
        [ForeignKey("PositionId")]
        public virtual Position Position { get; set; }

        public long BotId { get; set; }
        [ForeignKey("BotId")]
        public virtual Bot.Bot Bot { get; set; }

        public double Fees { get; set; }

    }
}
