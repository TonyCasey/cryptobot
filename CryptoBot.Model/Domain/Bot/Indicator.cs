using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using CryptoBot.Model.Common;

namespace CryptoBot.Model.Domain.Bot
{
    [Table("Indicators", Schema = "Bot")]
    public class Indicator : BaseEntity
    {
        public long IndicatorId { get; set; }
        
        public virtual List<RuleSet> RuleSets { get; set; }

        public long BotId { get; set; }
        [Required]
        [ForeignKey("BotId")]
        public Bot Bot { get; set; }

        [Required]
        public Enumerations.IndicatorTypeEnum IndicatorType { get; set; }

        /// <summary>
        /// When ticked it is used for a BUY signal
        /// </summary>
        public bool UseForBuy { get; set; } = true;

        /// <summary>
        /// When ticked it is used for a SELL signal
        /// </summary>
        public bool UseForSell { get; set; } = true;
    }
}
