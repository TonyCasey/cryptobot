using System;
using System.ComponentModel.DataAnnotations.Schema;
using CryptoBot.Model.Common;

namespace CryptoBot.Model.Domain.Market
{
    [Table("PriceLevels", Schema = "Market")]
    public class PriceLevel : BaseEntity
    {
        public long PriceLevelId { get; set; }
        public double Price { get; set; }
        public long Size { get; set; }
        public DateTime? TimeStamp { get; set; }
    }
}
