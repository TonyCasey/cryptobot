
using System;
using System.IO;

namespace CryptoBot.Model.Domain.Market
{
    /// <summary>
    /// Details of the current price of an exchange asset
    /// </summary>
    public class Ticker
    {
        /// <summary>
        /// The bid is the price to sell at
        /// </summary>
        public decimal Bid { get; set; }

        /// <summary>
        /// The ask is the price to buy at
        /// </summary>
        public decimal Ask { get; set; }

        /// <summary>
        /// The last trade purchase price
        /// </summary>
        public decimal Last { get; set; }

        /// <summary>
        /// Volume info
        /// </summary>
        public ExchangeVolume Volume { get; set; }

        /// <summary>
        /// Get a string for this ticker
        /// </summary>
        /// <returns>String</returns>
        public override string ToString()
        {
            return string.Format("Bid: {0}, Ask: {1}, Last: {2}", Bid, Ask, Last);
        }

        /// <summary>
        /// Write to writer
        /// </summary>
        /// <param name="writer">Writer</param>
        public void ToBinary(BinaryWriter writer)
        {
            writer.Write((double)Bid);
            writer.Write((double)Ask);
            writer.Write((double)Last);
            Volume.ToBinary(writer);
        }

        /// <summary>
        /// Read from reader
        /// </summary>
        /// <param name="reader">Reader</param>
        public void FromBinary(BinaryReader reader)
        {
            Bid = (decimal)reader.ReadDouble();
            Ask = (decimal)reader.ReadDouble();
            Last = (decimal)reader.ReadDouble();
            Volume = (Volume ?? new ExchangeVolume());
            Volume.FromBinary(reader);
        }
    }

    /// <summary>
    /// Info about exchange volume
    /// </summary>
    public class ExchangeVolume
    {
        /// <summary>
        /// Last volume update timestamp
        /// </summary>
        public DateTime Timestamp { get; set; }

        /// <summary>
        /// Price symbol - will equal quantity symbol if exchange doesn't break it out by price unit and quantity unit
        /// </summary>
        public string PriceSymbol { get; set; }

        /// <summary>
        /// Price amount - will equal QuantityAmount if exchange doesn't break it out by price unit and quantity unit
        /// </summary>
        public decimal PriceAmount { get; set; }

        /// <summary>
        /// Quantity symbol (converted into this unit)
        /// </summary>
        public string QuantitySymbol { get; set; }

        /// <summary>
        /// Quantity amount (this many units total)
        /// </summary>
        public decimal QuantityAmount { get; set; }

        /// <summary>
        /// Write to a binary writer
        /// </summary>
        /// <param name="writer">Binary writer</param>
        public void ToBinary(BinaryWriter writer)
        {
            writer.Write(Timestamp.ToUniversalTime().Ticks);
            writer.Write(PriceSymbol);
            writer.Write((double)PriceAmount);
            writer.Write(QuantitySymbol);
            writer.Write((double)QuantityAmount);
        }

        /// <summary>
        /// Read from a binary reader
        /// </summary>
        /// <param name="reader">Binary reader</param>
        public void FromBinary(BinaryReader reader)
        {
            Timestamp = new DateTime(reader.ReadInt64(), DateTimeKind.Utc);
            PriceSymbol = reader.ReadString();
            PriceAmount = (decimal)reader.ReadDouble();
            QuantitySymbol = reader.ReadString();
            QuantityAmount = (decimal)reader.ReadDouble();
        }
    }
}
