

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CryptoBot.Model.Common;
using CryptoBot.Model.Domain.Market;
using CryptoBot.Model.Domain.Trading;
using CryptoBot.Model.Exchanges;
using NLog;

namespace CryptoBot.ExchangeEngine.API.Exchanges
{
    /// <summary>
    /// Base class for all exchange API
    /// </summary>
    public abstract class ExchangeAPI : ExchangeBaseAPI, IExchangeApi
    {
        private static readonly Dictionary<string, IExchangeApi> apis = new Dictionary<string, IExchangeApi>(StringComparer.OrdinalIgnoreCase);

        /// <summary>
        /// Static constructor
        /// </summary>
        static ExchangeAPI()
        {
            foreach (Type type in typeof(ExchangeAPI).Assembly.GetTypes().Where(type => type.IsSubclassOf(typeof(ExchangeAPI))))
            {
                ExchangeAPI api = Activator.CreateInstance(type) as ExchangeAPI;
                apis[api.Name] = api;
            }
        }

        /// <summary>
        /// Get an exchange API given an exchange name (see public constants at top of this file)
        /// </summary>
        /// <param name="exchangeName">Exchange name</param>
        /// <returns>Exchange API or null if not found</returns>
        public static IExchangeApi GetExchangeAPI(string exchangeName)
        {
            GetExchangeAPIDictionary().TryGetValue(exchangeName, out IExchangeApi api);
            return api;
        }

        /// <summary>
        /// Get a dictionary of exchange APIs for all exchanges
        /// </summary>
        /// <returns>Dictionary of string exchange name and value exchange api</returns>
        public static IReadOnlyDictionary<string, IExchangeApi> GetExchangeAPIDictionary()
        {
            return apis;
        }

        /// <summary>
        /// Normalize a symbol for use on this exchange
        /// </summary>
        /// <param name="symbol">Symbol</param>
        /// <returns>Normalized symbol</returns>
        public virtual string NormalizeSymbol(string symbol) { return symbol; }

        /// <summary>
        /// Normalize a symbol to a global standard symbol that is the same with all exchange symbols, i.e. btc-usd. This base method standardizes with a hyphen separator.
        /// </summary>
        /// <param name="symbol"></param>
        /// <returns>Normalized global symbol</returns>
        public virtual string NormalizeSymbolGlobal(string symbol)
        {
            return symbol?.Replace("_", "-").Replace("/", "-").ToLowerInvariant();
        }

        /// <summary>
        /// Get exchange symbols
        /// </summary>
        /// <returns>Array of symbols</returns>
        public virtual IEnumerable<string> GetSymbols() { throw new NotImplementedException(); }

        /// <summary>
        /// ASYNC - Get exchange symbols
        /// </summary>
        /// <returns>Array of symbols</returns>
        public Task<IEnumerable<string>> GetSymbolsAsync() => Task.Factory.StartNew(() => GetSymbols());

        /// <summary>
        /// Get exchange ticker
        /// </summary>
        /// <param name="symbol">Symbol to get ticker for</param>
        /// <returns>Ticker</returns>
        public virtual Ticker GetTicker(Coin baseCoin, Coin coin) { throw new NotImplementedException(); }

        /// <summary>
        /// ASYNC - Get exchange ticker
        /// </summary>
        /// <param name="symbol">Symbol to get ticker for</param>
        /// <returns>Ticker</returns>
        public Task<Ticker> GetTickerAsync(Coin baseCoin, Coin coin) => Task.Factory.StartNew(() => GetTicker(baseCoin, coin));

        /// <summary>
        /// Get all tickers. If the exchange does not support this, a ticker will be requested for each symbol.
        /// </summary>
        /// <returns>Key value pair of symbol and tickers array</returns>
        public virtual IEnumerable<KeyValuePair<string, Ticker>> GetTickers()
        {
            throw new NotImplementedException();
            //foreach (string symbol in GetSymbols())
            //{
            //    yield return new KeyValuePair<string, Ticker>(symbol, GetTicker(symbol));
            //}
        }

        /// <summary>
        /// ASYNC - Get all tickers. If the exchange does not support this, a ticker will be requested for each symbol.
        /// </summary>
        /// <returns>Key value pair of symbol and tickers array</returns>
        public Task<IEnumerable<KeyValuePair<string, Ticker>>> GetTickersAsync() => Task.Factory.StartNew(() => GetTickers());

        /// <summary>
        /// Get exchange order book
        /// </summary>
        /// <param name="symbol">Symbol to get order book for</param>
        /// <param name="maxCount">Max count, not all exchanges will honor this parameter</param>
        /// <returns>Exchange order book or null if failure</returns>
        public virtual ExchangeOrderBook GetOrderBook(Coin baseCoin, Coin coin, int maxCount = 100) { throw new NotImplementedException(); }

        /// <summary>
        /// ASYNC - Get exchange order book
        /// </summary>
        /// <param name="symbol">Symbol to get order book for</param>
        /// <param name="maxCount">Max count, not all exchanges will honor this parameter</param>
        /// <returns>Exchange order book or null if failure</returns>
        public Task<ExchangeOrderBook> GetOrderBookAsync(Coin baseCoin, Coin coin, int maxCount = 100) => Task.Factory.StartNew(() => GetOrderBook(baseCoin, coin, maxCount));

        /// <summary>
        /// Get exchange order book all symbols. If the exchange does not support this, an order book will be requested for each symbol. Depending on the exchange, the number of bids and asks will have different counts, typically 50-100.
        /// </summary>
        /// <param name="maxCount">Max count of bids and asks - not all exchanges will honor this parameter</param>
        /// <returns>Symbol and order books pairs</returns>
        public virtual IEnumerable<KeyValuePair<string, ExchangeOrderBook>> GetOrderBooks( int maxCount = 100)
        {
            throw new NotImplementedException();
            //foreach (string symbol in GetSymbols())
            //{
            //    yield return new KeyValuePair<string, ExchangeOrderBook>(symbol, GetOrderBook(symbol, maxCount));
            //}
        }

        /// <summary>
        /// ASYNC - Get exchange order book all symbols. If the exchange does not support this, an order book will be requested for each symbol. Depending on the exchange, the number of bids and asks will have different counts, typically 50-100.
        /// </summary>
        /// <param name="maxCount">Max count of bids and asks - not all exchanges will honor this parameter</param>
        /// <returns>Symbol and order books pairs</returns>
        public Task<IEnumerable<KeyValuePair<string, ExchangeOrderBook>>> GetOrderBooksAsync(int maxCount = 100) => Task.Factory.StartNew(() => GetOrderBooks(maxCount));

        /// <summary>
        /// Get historical trades for the exchange
        /// </summary>
        /// <param name="symbol">Symbol to get historical data for</param>
        /// <param name="sinceDateTime">Optional date time to start getting the historical data at, null for the most recent data</param>
        /// <returns>An enumerator that iterates all historical data, this can take quite a while depending on how far back the sinceDateTime parameter goes</returns>
        public virtual IEnumerable<Trade> GetHistoricalTrades(Coin baseCoin, Coin coin, DateTime? sinceDateTime = null) { throw new NotImplementedException(); }

        /// <summary>
        /// ASYNC - Get historical trades for the exchange
        /// </summary>
        /// <param name="symbol">Symbol to get historical data for</param>
        /// <param name="sinceDateTime">Optional date time to start getting the historical data at, null for the most recent data</param>
        /// <returns>An enumerator that iterates all historical data, this can take quite a while depending on how far back the sinceDateTime parameter goes</returns>
        public Task<IEnumerable<Trade>> GetHistoricalTradesAsync(Coin baseCoin, Coin coin, DateTime? sinceDateTime = null) => Task.Factory.StartNew(() => GetHistoricalTrades(baseCoin, coin, sinceDateTime));

        /// <summary>
        /// Get recent trades on the exchange - this implementation simply calls GetHistoricalTrades with a null sinceDateTime.
        /// </summary>
        /// <param name="symbol">Symbol to get recent trades for</param>
        /// <returns>An enumerator that loops through all trades</returns>
        public virtual IEnumerable<Trade> GetRecentTrades(Coin baseCoin, Coin coin) { return GetHistoricalTrades(baseCoin, coin, null); }

        /// <summary>
        /// ASYNC - Get recent trades on the exchange - this implementation simply calls GetHistoricalTradesAsync with a null sinceDateTime.
        /// </summary>
        /// <param name="symbol">Symbol to get recent trades for</param>
        /// <returns>An enumerator that loops through all trades</returns>
        public Task<IEnumerable<Trade>> GetRecentTradesAsync(Coin baseCoin, Coin coin) => Task.Factory.StartNew(() => GetRecentTrades(baseCoin, coin));

        /// <summary>
        /// Get candles (open, high, low, close)
        /// </summary>
        /// <param name="symbol">Symbol to get candles for</param>
        /// <param name="periodsSeconds">Period in seconds to get candles for</param>
        /// <param name="startDate">Optional start date to get candles for</param>
        /// <param name="endDate">Optional end date to get candles for</param>
        /// <returns>Candles</returns>
        public virtual IEnumerable<Candle> GetCandles(Coin baseCoin, Coin coin, int periodSeconds, DateTime? startDate = null, DateTime? endDate = null) { throw new NotSupportedException(); }

        /// <summary>
        /// ASYNC - Get candles (open, high, low, close)
        /// </summary>
        /// <param name="symbol">Symbol to get candles for</param>
        /// <param name="periodsSeconds">Period in seconds to get candles for</param>
        /// <param name="startDate">Optional start date to get candles for</param>
        /// <param name="endDate">Optional end date to get candles for</param>
        /// <returns>Candles</returns>
        public Task<IEnumerable<Candle>> GetCandlesAsync(Coin baseCoin, Coin coin, int periodSeconds, DateTime? startDate = null, DateTime? endDate = null) => Task.Factory.StartNew(() => GetCandles(baseCoin, coin, periodSeconds, startDate, endDate));

        /// <summary>
        /// Get amounts available to trade, symbol / quantity dictionary
        /// </summary>
        /// <returns>Symbol / quantity dictionary</returns>
        public virtual Dictionary<string, decimal> GetAmountsAvailableToTrade() { throw new NotImplementedException(); }

        /// <summary>
        /// ASYNC - Get amounts available to trade, symbol / quantity dictionary
        /// </summary>
        /// <returns>Symbol / quantity dictionary</returns>
        public Task<Dictionary<string, decimal>> GetAmountsAvailableToTradeAsync() => Task.Factory.StartNew<Dictionary<string, decimal>>(() => GetAmountsAvailableToTrade());

        /// <summary>
        /// Place a limit order
        /// </summary>
        /// <param name="coin"></param>
        /// <param name="quantity">Amount</param>
        /// <param name="price">Price</param>
        /// <param name="buy">True to buy, false to sell</param>
        /// <param name="baseCoin"></param>
        /// <returns>Result</returns>
        public virtual ExchangeOrderResult PlaceOrder(Coin baseCoin, Coin coin, decimal quantity, decimal? price, bool buy, Enumerations.OrderTypeEnum type = Enumerations.OrderTypeEnum.Market) { throw new NotImplementedException(); }

        /// <summary>
        /// ASYNC - Place a limit order
        /// </summary>
        /// <param name="coin"></param>
        /// <param name="amount">Amount</param>
        /// <param name="price">Price</param>
        /// <param name="buy">True to buy, false to sell</param>
        /// <param name="baseCoin"></param>
        /// <returns>Result</returns>
        public Task<ExchangeOrderResult> PlaceOrderAsync(Coin baseCoin, Coin coin, decimal amount, decimal price, bool buy) => Task.Factory.StartNew(() => PlaceOrder(baseCoin, coin, amount, price, buy));

        /// <summary>
        /// Get order details
        /// </summary>
        /// <param name="orderId">Order id to get details for</param>
        /// <returns>Order details</returns>
        public virtual ExchangeOrderResult GetOrderDetails(string orderId) { throw new NotImplementedException(); }

        /// <summary>
        /// ASYNC - Get order details
        /// </summary>
        /// <param name="orderId">Order id to get details for</param>
        /// <returns>Order details</returns>
        public Task<ExchangeOrderResult> GetOrderDetailsAsync(string orderId) => Task.Factory.StartNew(() => GetOrderDetails(orderId));

        /// <summary>
        /// Get the details of all open orders
        /// </summary>
        /// <param name="symbol">Symbol to get open orders for or null for all</param>
        /// <returns>All open order details</returns>
        public virtual IEnumerable<ExchangeOrderResult> GetOpenOrderDetails(Coin baseCoin, Coin coin) { throw new NotImplementedException(); }

        /// <summary>
        /// ASYNC - Get the details of all open orders
        /// </summary>
        /// <param name="symbol">Symbol to get open orders for or null for all</param>
        /// <returns>All open order details</returns>
        public Task<IEnumerable<ExchangeOrderResult>> GetOpenOrderDetailsAsync(Coin baseCoin, Coin coin) => Task.Factory.StartNew(() => GetOpenOrderDetails(baseCoin, coin));

        /// <summary>
        /// Cancel an order, an exception is thrown if error
        /// </summary>
        /// <param name="orderId">Order id of the order to cancel</param>
        public virtual void CancelOrder(string orderId) { throw new NotImplementedException(); }

        /// <summary>
        /// ASYNC - Cancel an order, an exception is thrown if error
        /// </summary>
        /// <param name="orderId">Order id of the order to cancel</param>
        public Task CancelOrderAsync(string orderId) => Task.Factory.StartNew(() => CancelOrder(orderId));

        public virtual Task OpenSocket()
        {
            throw new NotImplementedException();
        }

        public virtual bool Simulated { get; }
    }


        
    /// <summary>
    /// List of exchange names
    /// </summary>
    public static class ExchangeName
    {
        /// <summary>
        /// Binance
        /// </summary>
        public const string Binance = "Binance";

        /// <summary>
        /// Bitfinex
        /// </summary>
        public const string Bitfinex = "Bitfinex";

        /// <summary>
        /// Bithumb
        /// </summary>
        public const string Bithumb = "Bithumb";

        /// <summary>
        /// Bitstamp
        /// </summary>
        public const string Bitstamp = "Bitstamp";

        /// <summary>
        /// Bittrex
        /// </summary>
        public const string Bittrex = "Bittrex";

        /// <summary>
        /// GDAX
        /// </summary>
        public const string GDAX = "GDAX";

        /// <summary>
        /// Gemini
        /// </summary>
        public const string Gemini = "Gemini";

        /// <summary>
        /// Kraken
        /// </summary>
        public const string Kraken = "Kraken";

        /// <summary>
        /// Poloniex
        /// </summary>
        public const string Poloniex = "Poloniex";
    }
}
