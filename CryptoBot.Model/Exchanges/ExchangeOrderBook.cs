
using System.Collections.Generic;
using System.IO;

namespace CryptoBot.Model.Exchanges
{
    /// <summary>
    /// A price entry in an exchange order book
    /// </summary>
    public struct ExchangeOrderPrice
    {
        /// <summary>
        /// Price
        /// </summary>
        public decimal Price { get; set; }

        /// <summary>
        /// Amount
        /// </summary>
        public decimal Amount { get; set; }

        /// <summary>
        /// ToString
        /// </summary>
        /// <returns>String</returns>
        public override string ToString()
        {
            return "Price: " + Price + ", Amount: " + Amount;
        }

        /// <summary>
        /// Write to a binary writer
        /// </summary>
        /// <param name="writer">Binary writer</param>
        public void ToBinary(BinaryWriter writer)
        {
            writer.Write((double)Price);
            writer.Write((double)Amount);
        }

        /// <summary>
        /// Constructor from a binary reader
        /// </summary>
        /// <param name="reader">Binary reader to read from</param>
        public ExchangeOrderPrice(BinaryReader reader)
        {
            Price = (decimal)reader.ReadDouble();
            Amount = (decimal)reader.ReadDouble();
        }
    }

    /// <summary>
    /// Represents all the asks (sells) and bids (buys) for an exchange asset
    /// </summary>
    public class ExchangeOrderBook
    {
        /// <summary>
        /// List of asks (sells)
        /// </summary>
        public List<ExchangeOrderPrice> Asks { get; } = new List<ExchangeOrderPrice>();

        /// <summary>
        /// List of bids (buys)
        /// </summary>
        public List<ExchangeOrderPrice> Bids { get; } = new List<ExchangeOrderPrice>();

        /// <summary>
        /// ToString
        /// </summary>
        /// <returns>String</returns>
        public override string ToString()
        {
            return string.Format("Asks: {0}, Bids: {1}", Asks.Count, Bids.Count);
        }

        /// <summary>
        /// Write to a binary writer
        /// </summary>
        /// <param name="writer">Binary writer</param>
        public void ToBinary(BinaryWriter writer)
        {
            writer.Write(Asks.Count);
            writer.Write(Bids.Count);
            foreach (ExchangeOrderPrice price in Asks)
            {
                price.ToBinary(writer);
            }
            foreach (ExchangeOrderPrice price in Bids)
            {
                price.ToBinary(writer);
            }
        }

        /// <summary>
        /// Read from a binary reader
        /// </summary>
        /// <param name="reader">Binary reader</param>
        public void FromBinary(BinaryReader reader)
        {
            Asks.Clear();
            Bids.Clear();
            int askCount = reader.ReadInt32();
            int bidCount = reader.ReadInt32();
            while (askCount-- > 0)
            {
                Asks.Add(new ExchangeOrderPrice(reader));
            }
            while (bidCount-- > 0)
            {
                Bids.Add(new ExchangeOrderPrice(reader));
            }
        }
    }
}
