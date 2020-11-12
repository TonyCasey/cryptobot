

using System;

namespace CryptoBot.Model.Exchanges
{
    /// <summary>
    /// Result of exchange order
    /// </summary>
    public enum ExchangeAPIOrderResult
    {
        /// <summary>
        /// Order status is unknown
        /// </summary>
        Unknown,

        /// <summary>
        /// Order has been filled completely
        /// </summary>
        Filled,

        /// <summary>
        /// Order partially filled
        /// </summary>
        FilledPartially,

        /// <summary>
        /// Order is pending or open but no amount has been filled yet
        /// </summary>
        Pending,

        /// <summary>
        /// Error
        /// </summary>
        Error,

        /// <summary>
        /// Order was cancelled
        /// </summary>
        Canceled
    }

    /// <summary>
    /// Result of an exchange order
    /// </summary>
    public class ExchangeOrderResult
    {
        /// <summary>
        /// Order id
        /// </summary>
        public string OrderId { get; set; }

        /// <summary>
        /// Result of the order
        /// </summary>
        public ExchangeAPIOrderResult Result { get; set; }

        /// <summary>
        /// Message if any
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// Original order amount
        /// </summary>
        public decimal Amount { get; set; }

        /// <summary>
        /// Amount filled
        /// </summary>
        public decimal AmountFilled { get; set; }

        /// <summary>
        /// Average price
        /// </summary>
        public decimal AveragePrice { get; set; }

        /// <summary>
        /// Order date
        /// </summary>
        public DateTime OrderDate { get; set; }

        /// <summary>
        /// Symbol
        /// </summary>
        public string Symbol { get; set; }

        /// <summary>
        /// Whether the order is a buy or sell
        /// </summary>
        public bool IsBuy { get; set; }

        /// <summary>
        /// Exchange
        /// </summary>
        public long EsxhangeId { get; set; }

        /// <summary>
        /// Fees that were charged normally in the purchased coin
        /// </summary>
        public decimal Fees { get; set; }
    }
}
