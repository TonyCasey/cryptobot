using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CryptoBot.Model.Common;

namespace CryptoBot.Model.Domain.Bot
{
    [Table("RuleSets", Schema = "Bot")]
    public class RuleSet : BaseEntity
    {
        public long RuleSetId { get; set; }
        
        public long? IndicatorId { get; set; }
        [ForeignKey("IndicatorId")]
        public Indicator Indicator { get; set; }

        public long? SafetyId { get; set; }
        [ForeignKey("SafetyId")]
        public Safety Safety { get; set; }

        /// <summary>
        /// Buy or Sell
        /// </summary>
        [Required]
        public IndicatorRulesEnumerations.RuleSideEnum RuleSide { get; set; }

        public List<Rule> Rules { get; set; }

    }
}
