using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CryptoBot.Model.Domain.Market
{
    [Table("Ohlcs", Schema = "Market")]
    public class OHLC : BaseEntity
    {
        [Key]
        public long OhlcId { get; set; }
        
        public double Open { get; set; }
        public double High { get; set; }
        public double Low { get; set; }
        public double Close { get; set; }
        
    }
}
