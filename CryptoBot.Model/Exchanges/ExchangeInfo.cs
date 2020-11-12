
using System.Collections.Generic;
using System.Linq;
using CryptoBot.ExchangeEngine.API.Exchanges;
using CryptoBot.Model.Domain.Market;

namespace CryptoBot.Model.Exchanges
{
    /// <summary>
    /// Information about an exchange
    /// </summary>
    public class ExchangeInfo
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="api">Exchange API</param>
        /// <param name="symbol">The symbol to trade by default, can be null</param>
        public ExchangeInfo(IExchangeApi api, Coin baseCoin, Coin coin)
        {
            API = api;
            Symbols = api.GetSymbols().ToArray();
            TradeInfo = new ExchangeTradeInfo(this, baseCoin, coin);
        }

        /// <summary>
        /// Update the exchange info - get new trade info, etc.
        /// </summary>
        public void Update()
        {
            TradeInfo.Update();
        }

        /// <summary>
        /// API to interact with the exchange
        /// </summary>
        public IExchangeApi API { get; private set; }

        /// <summary>
        /// User assigned identifier of the exchange, can be left at zero if not needed
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Symbols of the exchange
        /// </summary>
        public IReadOnlyCollection<string> Symbols { get; private set; }

        /// <summary>
        /// Latest trade info for the exchange
        /// </summary>
        public ExchangeTradeInfo TradeInfo { get; private set; }
    }
}
