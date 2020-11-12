using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CryptoBot.Model.Common;

namespace CryptoBot.Model.Domain.Bot
{
    [Table("Rules", Schema = "Bot")]
    public class Rule : BaseEntity
    {
        public long RuleId { get; set; }

        public int IndicatorRuleTypeId { get; set; }

        public decimal? Value { get; set; }
    }
}
