using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using CryptoBot.Model.Common;
using CryptoBot.Model.Domain.Account;
using CryptoBot.Model.Domain.Market;
using CryptoBot.Model.Domain.Trading;

namespace CryptoBot.Model.Domain.Bot
{
    [Table("Bots", Schema = "Bot")]
    public class Bot : BaseEntity
    {
        public long BotId { get; set; }
        
        public string Name { get; set; }

        public long UserId { get; set; }

        [Required]
        [ForeignKey("UserId")]
        public User User { get; set; }

        public long CoinId { get; set; }
        [Required]
        [ForeignKey("CoinId")]
        public Coin Coin { get; set; }

        public long BaseCoinId { get; set; }
        [Required]
        [ForeignKey("BaseCoinId")]
        public Coin BaseCoin { get; set; }

        public int ExchangeId { get; set; }

        [Required]
        [ForeignKey("ExchangeId")]
        public Exchange Exchange { get; set; }
        
        public bool Active { get; set; } = true;

        public ICollection<Indicator> Indicators { get; set; }

        public ICollection<Safety> Safeties { get; set; }
        
        public virtual IList<Position> Positions { get; set; }
        
        public long? CurrentPositionId { get; set; }
        [ForeignKey("CurrentPositionId")]
        [NotMapped] // DO NOT REMOVE THIS -- this object caused a serious problem in the API and it had to be un-mapped. If you need to access, you will have to do a separate call. It was that or a complete drop of it.
        public virtual Position CurrentPosition { get; set; }

        /// <summary>
        /// Amount to trade with
        /// </summary>
        public decimal Amount { get; set; }

        public Enumerations.OrderTypeEnum OrderType{ get; set; }

        public Enumerations.CandleSizeEnum CandleSize { get; set; }

        public ICollection<Order> Orders { get; set; }

        /// <summary>
        /// When true, the Amount field will be updated with profit/loss of each trade
        /// </summary>
        public bool Accumulator { get; set; }

    }
}
