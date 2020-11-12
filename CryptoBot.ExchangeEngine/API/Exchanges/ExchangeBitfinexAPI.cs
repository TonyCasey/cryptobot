﻿
using CryptoBot.Model.Exchanges;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Text;
using CryptoBot.Model.Common;
using CryptoBot.Model.Domain.Market;
using CryptoBot.Model.Domain.Trading;

namespace CryptoBot.ExchangeEngine.API.Exchanges
{
    public class ExchangeBitfinexAPI : ExchangeAPI
    {
        public override string BaseUrl { get; set; } = "https://api.bitfinex.com/v2";
        public string BaseUrlV1 { get; set; } = "https://api.bitfinex.com/v1";
        public override string Name => ExchangeName.Bitfinex;

        public override string NormalizeSymbol(string symbol)
        {
            return symbol?.Replace("-", string.Empty).ToUpperInvariant();
        }

        /// <summary>
        /// Normalize a symbol to a global standard symbol that is the same with all exchange symbols, i.e. btcusd
        /// </summary>
        /// <param name="symbol"></param>
        /// <returns>Normalized global symbol</returns>
        public override string NormalizeSymbolGlobal(string symbol)
        {
            if (symbol != null && symbol.Length > 1 && symbol[0] == 't' && char.IsUpper(symbol[1]))
            {
                symbol = symbol.Substring(1);
            }
            return symbol.ToLowerInvariant();
        }

        public string NormalizeSymbolV1(string symbol)
        {
            return symbol?.Replace("-", string.Empty).ToLowerInvariant();
        }

        private void CheckError(JToken result)
        {
            if (result != null && !(result is JArray) && result["result"] != null && result["result"].Value<string>() == "error")
            {
                throw new APIException(result["reason"].Value<string>());
            }
        }

        private Dictionary<string, object> GetNoncePayload()
        {
            return new Dictionary<string, object>
            {
                { "nonce", DateTime.UtcNow.Ticks.ToString() }
            };
        }

        private ExchangeOrderResult ParseOrder(JToken order)
        {
            decimal amount = order["original_amount"].Value<decimal>();
            decimal amountFilled = order["executed_amount"].Value<decimal>();
            return new ExchangeOrderResult
            {
                Amount = amount,
                AmountFilled = amountFilled,
                AveragePrice = order["price"].Value<decimal>(),
                Message = string.Empty,
                OrderId = order["id"].Value<string>(),
                Result = (amountFilled == amount ? ExchangeAPIOrderResult.Filled : (amountFilled == 0 ? ExchangeAPIOrderResult.Pending : ExchangeAPIOrderResult.FilledPartially)),
                OrderDate = CryptoUtility.UnixTimeStampToDateTimeSeconds(order["timestamp"].Value<double>()),
                Symbol = order["symbol"].Value<string>(),
                IsBuy = order["side"].Value<string>() == "buy"
            };
        }

        protected override void ProcessRequest(HttpWebRequest request, Dictionary<string, object> payload)
        {
            if (CanMakeAuthenticatedRequest(payload))
            {
                payload.Add("request", request.RequestUri.AbsolutePath);
                string json = JsonConvert.SerializeObject(payload);
                string json64 = System.Convert.ToBase64String(Encoding.ASCII.GetBytes(json));
                string hexSha384 = CryptoUtility.SHA384Sign(json64, PrivateApiKey.ToUnsecureString());
                request.Headers["X-BFX-PAYLOAD"] = json64;
                request.Headers["X-BFX-SIGNATURE"] = hexSha384;
                request.Headers["X-BFX-APIKEY"] = PublicApiKey.ToUnsecureString();
                request.Method = "POST";

                // bitfinex doesn't put the payload in the post body it puts it in as a http header, so no need to write to request stream
            }
        }

        public override IEnumerable<string> GetSymbols()
        {
            if (ReadCache("GetSymbols", out string[] symbols))
            {
                return symbols;
            }
            symbols = MakeJsonRequest<string[]>("/symbols", BaseUrlV1);
            for (int i = 0; i < symbols.Length; i++)
            {
                symbols[i] = NormalizeSymbol(symbols[i]);
            }
            WriteCache("GetSymbols", TimeSpan.FromMinutes(60.0), symbols);
            return symbols;
        }

        public override Ticker GetTicker(Coin baseCoin, Coin coin)
        {
            string symbol = $"{baseCoin.Code}-{coin.Code}";
            decimal[] ticker = MakeJsonRequest<decimal[]>("/ticker/t" + symbol);
            return new Ticker { Bid = ticker[0], Ask = ticker[2], Last = ticker[6], Volume = new ExchangeVolume { PriceAmount = ticker[7], PriceSymbol = symbol, QuantityAmount = ticker[7] * ticker[6], QuantitySymbol = symbol, Timestamp = DateTime.UtcNow } };
        }

        public override IEnumerable<KeyValuePair<string, Ticker>> GetTickers()
        {
            List<KeyValuePair<string, Ticker>> tickers = new List<KeyValuePair<string, Ticker>>();
            IReadOnlyCollection<string> symbols = GetSymbols().ToArray();
            if (symbols != null && symbols.Count != 0)
            {
                StringBuilder symbolString = new StringBuilder();
                foreach (string symbol in symbols)
                {
                    symbolString.Append('t');
                    symbolString.Append(symbol.ToUpperInvariant());
                    symbolString.Append(',');
                }
                symbolString.Length--;
                JToken token = MakeJsonRequest<JToken>("/tickers?symbols=" + symbolString);
                DateTime now = DateTime.UtcNow;
                foreach (JArray array in token)
                {
                    tickers.Add(new KeyValuePair<string, Ticker>(((string)array[0]).Substring(1), new Ticker
                    {
                        Ask = (decimal)array[3],
                        Bid = (decimal)array[1],
                        Last = (decimal)array[7],
                        Volume = new ExchangeVolume
                        {
                            PriceAmount = (decimal)array[8],
                            PriceSymbol = (string)array[0],
                            QuantityAmount = (decimal)array[8] * (decimal)array[7],
                            QuantitySymbol = (string)array[0],
                            Timestamp = now
                        }
                    }));
                }
            }
            return tickers;
        }

        public override ExchangeOrderBook GetOrderBook(Coin baseCoin, Coin coin, int maxCount = 100)
        {
            string symbol = NormalizeSymbol($"{baseCoin.Code}-{coin.Code}");
            ExchangeOrderBook orders = new ExchangeOrderBook();
            decimal[][] books = MakeJsonRequest<decimal[][]>("/book/t" + symbol + "/P0?len=" + maxCount);
            foreach (decimal[] book in books)
            {
                if (book[2] > 0m)
                {
                    orders.Bids.Add(new ExchangeOrderPrice { Amount = book[2], Price = book[0] });
                }
                else
                {
                    orders.Asks.Add(new ExchangeOrderPrice { Amount = -book[2], Price = book[0] });
                }
            }
            return orders;
        }

        public override IEnumerable<Trade> GetHistoricalTrades(Coin baseCoin, Coin coin, DateTime? sinceDateTime = null)
        {
            string symbol = $"{baseCoin.Code}-{coin.Code}";
            const int maxCount = 100;
            symbol = NormalizeSymbol(symbol);
            string baseUrl = "/trades/t" + symbol + "/hist?sort=" + (sinceDateTime == null ? "-1" : "1") + "&limit=" + maxCount;
            string url;
            List<Trade> trades = new List<Trade>();
            decimal[][] tradeChunk;
            while (true)
            {
                url = baseUrl;
                if (sinceDateTime != null)
                {
                    url += "&start=" + (long)CryptoUtility.UnixTimestampFromDateTimeMilliseconds(sinceDateTime.Value);
                }
                tradeChunk = MakeJsonRequest<decimal[][]>(url);
                if (tradeChunk == null || tradeChunk.Length == 0)
                {
                    break;
                }
                if (sinceDateTime != null)
                {
                    sinceDateTime = CryptoUtility.UnixTimeStampToDateTimeMilliseconds((double)tradeChunk[tradeChunk.Length - 1][1]);
                }
                foreach (decimal[] tradeChunkPiece in tradeChunk)
                {
                    trades.Add(new Trade { Amount = Math.Abs(tradeChunkPiece[2]), IsBuy = tradeChunkPiece[2] > 0m, Price = tradeChunkPiece[3], Timestamp = CryptoUtility.UnixTimeStampToDateTimeMilliseconds((double)tradeChunkPiece[1]), Id = (long)tradeChunkPiece[0] });
                }
                trades.Sort((t1, t2) => t1.Timestamp.CompareTo(t2.Timestamp));
                foreach (Trade t in trades)
                {
                    yield return t;
                }
                trades.Clear();
                if (tradeChunk.Length < 500 || sinceDateTime == null)
                {
                    break;
                }
                System.Threading.Thread.Sleep(5000);
            }
        }

        public override IEnumerable<Candle> GetCandles(Coin baseCoin, Coin coin, int periodSeconds, DateTime? startDate = null, DateTime? endDate = null)
        {
            string symbol = NormalizeSymbol($"{baseCoin.Code}-{coin.Code}");
            // https://api.bitfinex.com/v2/candles/trade:1d:btcusd/hist?start=ms_start&end=ms_end
            symbol = NormalizeSymbol(symbol);
            endDate = endDate ?? DateTime.UtcNow;
            startDate = startDate ?? endDate.Value.Subtract(TimeSpan.FromDays(1.0));
            string periodString = CryptoUtility.SecondsToPeriodString(periodSeconds).Replace("d", "D"); // WTF Bitfinex, capital D???
            string url = "/candles/trade:" + periodString + ":t" + symbol + "/hist?sort=1&start=" +
                (long)startDate.Value.UnixTimestampFromDateTimeMilliseconds() + "&end=" + (long)endDate.Value.UnixTimestampFromDateTimeMilliseconds();
            JToken token = MakeJsonRequest<JToken>(url);
            CheckError(token);

            /* MTS, OPEN, CLOSE, HIGH, LOW, VOL */
            foreach (JArray candle in token)
            {
                yield return new Candle
                {
                    ClosePrice = (decimal)candle[2],
                    ExchangeName = Name,
                    HighPrice = (decimal)candle[3],
                    LowPrice = (decimal)candle[4],
                    Name = symbol,
                    OpenPrice = (decimal)candle[1],
                    PeriodSeconds = periodSeconds,
                    Timestamp = CryptoUtility.UnixTimeStampToDateTimeMilliseconds((long)candle[0]),
                    VolumePrice = (double)candle[5],
                    VolumeQuantity = (double)candle[5] * (double)candle[2]
                };
            }
        }

        public override Dictionary<string, decimal> GetAmountsAvailableToTrade()
        {
            Dictionary<string, decimal> lookup = new Dictionary<string, decimal>(StringComparer.OrdinalIgnoreCase);
            JArray obj = MakeJsonRequest<Newtonsoft.Json.Linq.JArray>("/balances", BaseUrlV1, GetNoncePayload());
            CheckError(obj);
            var q = from JToken token in obj
                    where token["type"].Equals("trading")
                    select new { Currency = token["currency"].Value<string>(), Available = token["available"].Value<decimal>() };
            foreach (var kv in q)
            {
                lookup[kv.Currency] = kv.Available;
            }
            return lookup;
        }

        public override ExchangeOrderResult PlaceOrder(Coin baseCoin, Coin coin, decimal quantity, decimal? price, bool buy, Enumerations.OrderTypeEnum type)
        {
            string symbol = NormalizeSymbol($"{baseCoin.Code}-{coin.Code}");
            Dictionary<string, object> payload = new Dictionary<string, object>
            {
                { "nonce", DateTime.UtcNow.Ticks.ToString() },
                { "symbol", symbol },
                { "quantity", quantity.ToString(CultureInfo.InvariantCulture.NumberFormat) },
                { "price", price.ToString() },
                { "side", (buy ? "buy" : "sell") },
                { "type", $"exchange {type.ToString().ToLower()}" }
            };
            JToken obj = MakeJsonRequest<JToken>("/order/new", BaseUrlV1, payload);
            CheckError(obj);
            return ParseOrder(obj);
        }

        public override ExchangeOrderResult GetOrderDetails(string orderId)
        {
            if (string.IsNullOrWhiteSpace(orderId))
            {
                return null;
            }

            JToken result = MakeJsonRequest<JToken>("/order/status", BaseUrlV1, new Dictionary<string, object> { { "nonce", DateTime.UtcNow.Ticks.ToString() }, { "order_id", long.Parse(orderId) } });
            CheckError(result);
            return ParseOrder(result);
        }

        /// <summary>
        /// Get the details of all open orders
        /// </summary>
        /// <param name="symbol">Symbol to get open orders for or null for all</param>
        /// <returns>All open order details</returns>
        public override IEnumerable<ExchangeOrderResult> GetOpenOrderDetails(Coin baseCoin, Coin coin)
        {
            string symbol = NormalizeSymbol($"{baseCoin.Code}-{coin.Code}");
            symbol = NormalizeSymbolV1(symbol);
            JToken result = MakeJsonRequest<JToken>("/orders", BaseUrlV1, new Dictionary<string, object> { { "nonce", DateTime.UtcNow.Ticks.ToString() } });
            CheckError(result);
            if (result is JArray array)
            {
                foreach (JToken token in array)
                {
                    if (symbol == null || (string)token["symbol"] == symbol)
                    {
                        yield return ParseOrder(token);
                    }
                }
            }
        }

        public override void CancelOrder(string orderId)
        {
            JObject result = MakeJsonRequest<JObject>("/order/cancel", BaseUrlV1, new Dictionary<string, object> { { "nonce", DateTime.UtcNow.Ticks.ToString() }, { "order_id", long.Parse(orderId) } });
            CheckError(result);
        }
    }
}
