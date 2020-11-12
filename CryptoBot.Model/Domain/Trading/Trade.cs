using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.IO;

namespace CryptoBot.Model.Domain.Trading
{
    /// <summary>
    /// Details of an exchangetrade
    /// </summary>
    [Table("Trades", Schema = "Trading")]
    public class Trade : BaseEntity
    {
        public long TradeId { get; set; }

        /// <summary>
        /// Timestamp
        /// </summary>
        public DateTime Timestamp { get; set; }

        /// <summary>
        /// Trade id
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// Price
        /// </summary>
        public decimal Price { get; set; }

        /// <summary>
        /// Amount
        /// </summary>
        public decimal Amount { get; set; }

        /// <summary>
        /// True if buy, false if sell - for some exchanges (Binance) the meaning can be different, i.e. is the buyer the maker
        /// </summary>
        public bool IsBuy { get; set; }

        /// <summary>
        /// ToString
        /// </summary>
        /// <returns>String</returns>
        public override string ToString()
        {
            return string.Format("{0:s},{1},{2},{3}", Timestamp, Price, Amount, IsBuy ? "Buy" : "Sell");
        }

        /// <summary>
        /// Write to binary writer
        /// </summary>
        /// <param name="writer">Binary writer</param>
        public void ToBinary(BinaryWriter writer)
        {
            writer.Write(Timestamp.ToUniversalTime().Ticks);
            writer.Write(Id);
            writer.Write((double)Price);
            writer.Write((double)Amount);
            writer.Write(IsBuy);
        }

        /// <summary>
        /// Read from binary reader
        /// </summary>
        /// <param name="reader">Binary reader</param>
        public void FromBinary(BinaryReader reader)
        {
            Timestamp = new DateTime(reader.ReadInt64(), DateTimeKind.Utc);
            Id = reader.ReadInt64();
            Price = (decimal)reader.ReadDouble();
            Amount = (decimal)reader.ReadDouble();
            IsBuy = reader.ReadBoolean();
        }
    }
}
