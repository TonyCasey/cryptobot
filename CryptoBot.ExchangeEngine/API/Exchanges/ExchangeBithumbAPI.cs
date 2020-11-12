

using CryptoBot.Model.Exchanges;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using CryptoBot.Model.Domain.Market;
using CryptoBot.Model.Domain.Trading;

namespace CryptoBot.ExchangeEngine.API.Exchanges
{
    public class ExchangeBithumbAPI : ExchangeAPI
    {
        public override string BaseUrl { get; set; } = "https://api.bithumb.com";
        public override string Name => ExchangeName.Bithumb;

        private static readonly char[] normalizeSeps = new char[] { '-', '_' };

        public override string NormalizeSymbol(string symbol)
        {
            if (symbol != null)
            {
                int pos = symbol.IndexOfAny(normalizeSeps);
                if (pos >= 0)
                {
                    symbol = symbol.Substring(0, pos).ToLowerInvariant();
                }
            }
            return symbol;
        }

        private string StatusToError(string status)
        {
            switch (status)
            {
                case "5100": return "Bad Request";
                case "5200": return "Not Member";
                case "5300": return "Invalid Apikey";
                case "5302": return "Method Not Allowed";
                case "5400": return "Database Fail";
                case "5500": return "Invalid Parameter";
                case "5600": return "Custom Notice";
                case "5900": return "Unknown Error";
                default: return status;
            }
        }

        private void CheckError(JToken result)
        {
            if (result != null && !(result is JArray) && result["status"] != null && result["status"].Value<string>() != "0000")
            {
                throw new APIException(result["status"].Value<string>() + ": " + result["message"].Value<string>());
            }
        }
        
        private JToken MakeRequestBithumb(ref string symbol, string subUrl)
        {
            symbol = NormalizeSymbol(symbol);
            JObject obj = MakeJsonRequest<JObject>(subUrl.Replace("$SYMBOL$", symbol ?? string.Empty));
            CheckError(obj);
            return obj["data"];
        }

        private Ticker ParseTicker(string symbol, JToken data, DateTime? date)
        {
            return new Ticker
            {
                Ask = (decimal)data["sell_price"],
                Bid = (decimal)data["buy_price"],
                Last = (decimal)data["buy_price"], // Silly Bithumb doesn't provide the last actual trade value in the ticker,
                Volume = new ExchangeVolume
                {
                    PriceAmount = (decimal)data["average_price"],
                    PriceSymbol = "KRW",
                    QuantityAmount = (decimal)data["units_traded"],
                    QuantitySymbol = symbol,
                    Timestamp = date ?? CryptoUtility.UnixTimeStampToDateTimeMilliseconds((long)data["date"])
                }
            };
        }

        private ExchangeOrderBook ParseOrderBook(JToken data)
        {
            ExchangeOrderBook book = new ExchangeOrderBook();
            foreach (JToken token in data["bids"])
            {
                book.Bids.Add(new ExchangeOrderPrice { Amount = (decimal)token["quantity"], Price = (decimal)token["price"] });
            }
            foreach (JToken token in data["asks"])
            {
                book.Asks.Add(new ExchangeOrderPrice { Amount = (decimal)token["quantity"], Price = (decimal)token["price"] });
            }
            return book;
        }

        public override IEnumerable<string> GetSymbols()
        {
            List<string> symbols = new List<string>();
            string symbol = "all";
            JToken data = MakeRequestBithumb(ref symbol, "/public/ticker/$SYMBOL$");
            foreach (JProperty token in data)
            {
                if (token.Name != "date")
                {
                    symbols.Add(token.Name);
                }
            }
            return symbols;
        }

        public override Ticker GetTicker(Coin baseCoin, Coin coin)
        {
            string symbol = $"{baseCoin.Code}-{coin.Code}";
            JToken data = MakeRequestBithumb(ref symbol, "/public/ticker/$SYMBOL$");
            return ParseTicker(symbol, data, null);
        }

        public override IEnumerable<KeyValuePair<string, Ticker>> GetTickers()
        {
            string symbol = "all";
            List<KeyValuePair<string, Ticker>> tickers = new List<KeyValuePair<string, Ticker>>();
            JToken data = MakeRequestBithumb(ref symbol, "/public/ticker/$SYMBOL$");
            DateTime date = CryptoUtility.UnixTimeStampToDateTimeMilliseconds((long)data["date"]);
            foreach (JProperty token in data)
            {
                if (token.Name != "date")
                {
                    tickers.Add(new KeyValuePair<string, Ticker>(token.Name, ParseTicker(token.Name, token.Value, date)));
                }
            }
            return tickers;
        }

        public override ExchangeOrderBook GetOrderBook(Coin baseCoin, Coin coin, int maxCount = 100)
        {
            string symbol = NormalizeSymbol($"{baseCoin.Code}-{coin.Code}");
            JToken data = MakeRequestBithumb(ref symbol, "/public/orderbook/$SYMBOL$");
            return ParseOrderBook(data);
        }

        public override IEnumerable<KeyValuePair<string, ExchangeOrderBook>> GetOrderBooks(int maxCount = 100)
        {
            string symbol = "all";
            List<KeyValuePair<string, ExchangeOrderBook>> books = new List<KeyValuePair<string, ExchangeOrderBook>>();
            JToken data = MakeRequestBithumb(ref symbol, "/public/orderbook/$SYMBOL$");
            foreach (JProperty book in data)
            {
                if (book.Name != "timestamp" && book.Name != "payment_currency")
                {
                    books.Add(new KeyValuePair<string, ExchangeOrderBook>(book.Name, ParseOrderBook(book.Value)));
                }
            }
            return books;
        }

        public override IEnumerable<Trade> GetHistoricalTrades(Coin baseCoin, Coin coin, DateTime? sinceDateTime = null)
        {
            string symbol = $"{baseCoin.Code}-{coin.Code}";
            JToken data = MakeRequestBithumb(ref symbol, "/public/recent_transactions/$SYMBOL$");
            foreach (JToken token in data)
            {
                yield return new Trade
                {
                    Amount = (decimal)token["units_traded"],
                    Price = (decimal)token["price"],
                    Id = -1,
                    IsBuy = (string)token["type"] == "bid",
                    Timestamp = (DateTime)token["transaction_date"]
                };
            }
        }
    }
}
