using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using CryptoBot.Model.Common;

namespace CryptoBot.Model.Domain.Market
{
    [Table("SnapShots", Schema = "Market")]
    public class SnapShot : BaseEntity
    {
        public SnapShot()
        {
            Asks = new List<PriceLevel>();
            Bids = new List<PriceLevel>();
            Trades = new List<PriceLevel>();
        }
        public long SnapShotId { get; set; }
        public long SeqNo { get; set; }

        [StringLength(10)]
        public string Symbol { get; set; }
        public string ExcahngeStatus { get; set; }
        public DateTime TimeStamp { get; set; }

        [Required]
        [ForeignKey("Coin")]
        public long CoinId { get; set; }
        public virtual Coin Coin { get; set; }

        [ForeignKey("BaseCoin")]
        public long? BaseCoinId { get; set; }
        public virtual Coin BaseCoin { get; set; }

        [Required]
        [ForeignKey("Exchange")]
        public int ExchangeId { get; set; }
        public virtual Exchange Exchange { get; set; }

        
        public virtual List<PriceLevel> Asks { get; set; }
        
        public virtual List<PriceLevel> Bids { get; set; }
        
        public virtual List<PriceLevel> Trades { get; set; }

        public Enumerations.BidAskTradeEnum Type { get; set; }
    }
}
