using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using CryptoBot.Model.Common;
using CryptoBot.Model.Domain.Market;

namespace CryptoBot.Model.Domain.Trading
{
    [Table("Positions", Schema = "Trading")]
    public class Position : BaseEntity
    {
        public long PositionId { get; set; }
        
        // buy or sell
        public Enumerations.OrderSideEnum Side { get; set; }
       
        public double BuyPrice { get; set; }

        public double? SellPrice { get; set; }

        public double Quantity { get; set; }

        public double? Commission { get; set; }

        public double? NetProfit { get; set; }

        public double? NetProfitPercent { get; set; }

        public double? GrossProfit { get; set; }

        public double? GrossProfitPercent { get; set; }

        public DateTime BuyTimeStamp { get; set; }
        public string BuyRequestExchangeReference { get; set; }  
        
        public DateTime? SellTimeStamp { get; set; }
        public string SellRequestExchangeReference { get; set; }
        
        public Enumerations.PositionStatusEnum Status { get; set; }

        public virtual List<Order> Orders { get; set; }

        [Required]
        public long BotId { get; set; }
        [ForeignKey("BotId")]
        public Bot.Bot Bot { get; set; }

    }
}
