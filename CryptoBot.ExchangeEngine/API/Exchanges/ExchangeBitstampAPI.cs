

using CryptoBot.Model.Exchanges;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using CryptoBot.Model.Domain.Market;
using CryptoBot.Model.Domain.Trading;

namespace CryptoBot.ExchangeEngine.API.Exchanges
{
    public class ExchangeBitstampAPI : ExchangeAPI
    {
        public override string BaseUrl { get; set; } = "https://www.bitstamp.net/api/v2";
        public override string Name => ExchangeName.Bitstamp;

        private JToken MakeBitstampRequest(string subUrl)
        {
            JToken token = MakeJsonRequest<JToken>(subUrl);
            if (!(token is JArray) && token["error"] != null)
            {
                throw new APIException(token["error"].ToString());
            }
            return token;
        }

        public override string NormalizeSymbol(string symbol)
        {
            return symbol?.Replace("/", string.Empty).Replace("-", string.Empty).Replace("_", string.Empty).ToLowerInvariant();
        }

        public override IEnumerable<string> GetSymbols()
        {
            foreach (JToken token in MakeBitstampRequest("/trading-pairs-info"))
            {
                yield return (string)token["url_symbol"];
            }
        }

        public override Ticker GetTicker(Coin baseCoin, Coin coin)
        {
            string symbol = $"{baseCoin.Code}-{coin.Code}";
            // {"high": "0.10948945", "last": "0.10121817", "timestamp": "1513387486", "bid": "0.10112165", "vwap": "0.09958913", "volume": "9954.37332614", "low": "0.09100000", "ask": "0.10198408", "open": "0.10250028"}
            symbol = NormalizeSymbol(symbol);
            JToken token = MakeBitstampRequest("/ticker/" + symbol);
            return new Ticker
            {
                Ask = (decimal)token["ask"],
                Bid = (decimal)token["bid"],
                Last = (decimal)token["last"],
                Volume = new ExchangeVolume
                {
                    PriceAmount = (decimal)token["volume"],
                    PriceSymbol = symbol,
                    QuantityAmount = (decimal)token["volume"] * (decimal)token["last"],
                    QuantitySymbol = symbol,
                    Timestamp = CryptoUtility.UnixTimeStampToDateTimeSeconds((long)token["timestamp"])
                }
            };
        }

        public override ExchangeOrderBook GetOrderBook(Coin baseCoin, Coin coin, int maxCount = 100)
        {
            string symbol = NormalizeSymbol($"{baseCoin.Code}-{coin.Code}");
            symbol = NormalizeSymbol(symbol);
            JToken token = MakeBitstampRequest("/order_book/" + symbol);
            ExchangeOrderBook book = new ExchangeOrderBook();
            foreach (JArray ask in token["asks"])
            {
                book.Asks.Add(new ExchangeOrderPrice { Amount = (decimal)ask[1], Price = (decimal)ask[0] });
            }
            foreach (JArray bid in token["bids"])
            {
                book.Bids.Add(new ExchangeOrderPrice { Amount = (decimal)bid[1], Price = (decimal)bid[0] });
            }
            return book;
        }

        public override IEnumerable<Trade> GetHistoricalTrades(Coin baseCoin, Coin coin, DateTime? sinceDateTime = null)
        {
            string symbol = $"{baseCoin.Code}-{coin.Code}";
            // [{"date": "1513387997", "tid": "33734815", "price": "0.01724547", "type": "1", "amount": "5.56481714"}]
            symbol = NormalizeSymbol(symbol);
            JToken token = MakeBitstampRequest("/transactions/" + symbol);
            foreach (JToken trade in token)
            {
                yield return new Trade
                {
                    Amount = (decimal)trade["amount"],
                    Id = (long)trade["tid"],
                    IsBuy = (string)trade["type"] == "0",
                    Price = (decimal)trade["price"],
                    Timestamp = CryptoUtility.UnixTimeStampToDateTimeSeconds((long)trade["date"])
                };
            }
        }
    }
}
