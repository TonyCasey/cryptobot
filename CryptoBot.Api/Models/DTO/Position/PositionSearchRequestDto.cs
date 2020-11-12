using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CryptoBot.Model.Common;

namespace Api.CryptoBot.Models.DTO.Position
{
    public class PositionSearchRequestDto : SearchBase // must extend SearchBase
    {
        public long? PositionId { get; set; }
        
        public long? BotId { get; set; }

        // buy or sell
        public Enumerations.OrderSideEnum? Side { get; set; }
       
        public double? BuyPrice { get; set; }

        public double? SellPrice { get; set; }

        public double? Quantity { get; set; }

        public double? Commission { get; set; }

        public double? NetProfit { get; set; }

        public double? NetProfitPercent { get; set; }

        public double? GrossProfit { get; set; }

        public double? GrossProfitPercent { get; set; }

        public DateTime? BuyTimeStamp { get; set; }
        public string BuyRequestExchangeReference { get; set; }  
        
        public DateTime? SellTimeStamp { get; set; }
        public string SellRequestExchangeReference { get; set; }
    }
}
