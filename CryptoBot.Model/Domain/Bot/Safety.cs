using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using CryptoBot.Model.Common;

namespace CryptoBot.Model.Domain.Bot
{
    [Table("Safeties", Schema = "Bot")]
    public class Safety : BaseEntity    
    {
        public long SafetyId { get; set; }

        public virtual List<RuleSet> RuleSets { get; set; }

        [Required]
        public Enumerations.SafetyTypeEnum SafetyType { get; set; }
    }
}
