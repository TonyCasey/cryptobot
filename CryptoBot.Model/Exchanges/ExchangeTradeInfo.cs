
using System.Linq;
using CryptoBot.Model.Domain.Market;
using CryptoBot.Model.Domain.Trading;

namespace CryptoBot.Model.Exchanges
{
    /// <summary>
    /// Latest trade info for an exchange
    /// </summary>
    public class ExchangeTradeInfo
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="info">Exchange info</param>
        /// <param name="symbol">The symbol to trade</param>
        public ExchangeTradeInfo(ExchangeInfo info, Coin baseCoin, Coin coin)
        {
            ExchangeInfo = info;
            Symbol = $"{baseCoin.Code}-{coin.Code}";
            BaseCoin = baseCoin;
            Coin = coin;
        }

        public Coin Coin { get; set; }

        public Coin BaseCoin { get; set; }

        /// <summary>
        /// Update the trade info via API
        /// </summary>
        public void Update()
        {
            Ticker = ExchangeInfo.API.GetTicker(BaseCoin, Coin);
            RecentTrades = ExchangeInfo.API.GetRecentTrades(BaseCoin, Coin).ToArray();
            if (RecentTrades.Length == 0)
            {
                Trade = new Trade();
            }
            else
            {
                //Trade = new Trade { Amount = (float)RecentTrades[RecentTrades.Length - 1].Amount, Price = (float)RecentTrades[RecentTrades.Length - 1].Price, Ticks = (long)CryptoUtility.UnixTimestampFromDateTimeMilliseconds(RecentTrades[RecentTrades.Length - 1].Timestamp) };
            }
            Orders = ExchangeInfo.API.GetOrderBook(BaseCoin, Coin);
        }

        /// <summary>
        /// Exchange info
        /// </summary>
        public ExchangeInfo ExchangeInfo { get; private set; }

        /// <summary>
        /// Ticker for the exchange
        /// </summary>
        public Ticker Ticker { get; private set; }

        /// <summary>
        /// Recent trades in ascending order
        /// </summary>
        public Trade[] RecentTrades { get; private set; }

        /// <summary>
        /// Pending orders on the exchange
        /// </summary>
        public ExchangeOrderBook Orders { get; private set; }

        /// <summary>
        /// The last trade made, allows setting to facilitate fast testing of traders based on price alone
        /// </summary>
        public Trade Trade { get; set; }

        /// <summary>
        /// The current symbol being traded
        /// </summary>
        public string Symbol { get; set; }
    }
}
