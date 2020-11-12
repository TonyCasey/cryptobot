using System.ComponentModel.DataAnnotations.Schema;

namespace CryptoBot.Model.Domain.Market
{
    [Table("Coins", Schema = "Market")]
    public class Coin : BaseEntity
    {
        public long CoinId { get; set; }
        public string Code { get; set; }
        /// <summary>
        /// When placing an order using this currency as base currency
        /// how many decimal places to round and avoid over precision errors
        /// </summary>
        public int OrderRoundingExponent { get; set; } = 2;
    }
}
