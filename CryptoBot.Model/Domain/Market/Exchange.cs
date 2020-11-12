using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CryptoBot.Model.Domain.Market
{
    [Table("Exchanges", Schema = "Market")]
    public class Exchange : BaseEntity
    {
        public int ExchangeId { get; set; }
        [StringLength(50)]
        public string Name { get; set; }
        [StringLength(10)]
        public string Code { get; set; }
    }
}
