using System.ComponentModel.DataAnnotations.Schema;
using CryptoBot.Model.Common;

namespace CryptoBot.Model.Domain.Trading
{
    [Table("TradeSettings", Schema = "Trading")]
    public class TradeSetting : BaseEntity
    {
        public long TradeSettingId { get; set; }

        /// <summary>
        /// Amount
        /// </summary>
        public decimal Amount { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public Enumerations.BoughtSoldPositionEnum BoughtSoldPosition { get; set; }
    }
}
