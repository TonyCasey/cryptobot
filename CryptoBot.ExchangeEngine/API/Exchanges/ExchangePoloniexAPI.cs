
using CryptoBot.Model.Exchanges;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using CryptoBot.Model.Common;
using CryptoBot.Model.Domain.Market;
using CryptoBot.Model.Domain.Trading;

namespace CryptoBot.ExchangeEngine.API.Exchanges
{
    public class ExchangePoloniexAPI : ExchangeAPI
    {
        public override string BaseUrl { get; set; } = "https://poloniex.com";
        public override string Name => ExchangeName.Poloniex;

        private void CheckError(JObject json)
        {
            if (json == null)
            {
                throw new APIException("No response from server");
            }
            JToken error = json["error"];
            if (error != null)
            {
                throw new APIException((string)error);
            }
        }

        private Dictionary<string, object> GetNoncePayload()
        {
            return new Dictionary<string, object>
            {
                { "nonce", DateTime.UtcNow.Ticks }
            };
        }

        private void CheckError(JToken result)
        {
            if (result != null && !(result is JArray) && result["error"] != null)
            {
                throw new APIException(result["error"].Value<string>());
            }
        }

        private JToken MakePrivateAPIRequest(string command, params object[] parameters)
        {
            Dictionary<string, object> payload = GetNoncePayload();
            payload["command"] = command;
            if (parameters != null && parameters.Length % 2 == 0)
            {
                for (int i = 0; i < parameters.Length;)
                {
                    payload[parameters[i++].ToString()] = parameters[i++];
                }
            }
            JToken result = MakeJsonRequest<JToken>("/tradingApi", null, payload);
            CheckError(result);
            return result;
        }

        private ExchangeOrderResult ParseOrder(JToken result)
        {
            //result = JToken.Parse("{\"orderNumber\":31226040,\"resultingTrades\":[{\"quantity\":\"338.8732\",\"date\":\"2014-10-18 23:03:21\",\"rate\":\"0.00000173\",\"total\":\"0.00058625\",\"tradeID\":\"16164\",\"type\":\"buy\"}]}");
            ExchangeOrderResult order = new ExchangeOrderResult();
            order.OrderId = result["orderNumber"].ToString();
            JToken trades = result["resultingTrades"];
            decimal tradeCount = (decimal)trades.Children().Count();
            if (tradeCount != 0m)
            {
                foreach (JToken token in trades)
                {
                    order.Amount += (decimal)token["quantity"];
                    order.AmountFilled = order.Amount;
                    order.AveragePrice += (decimal)token["rate"];
                    if ((string)token["type"] == "buy")
                    {
                        order.IsBuy = true;
                    }
                    if (order.OrderDate == DateTime.MinValue)
                    {
                        order.OrderDate = (DateTime)token["date"];
                    }
                }
                order.AveragePrice /= tradeCount;
            }
            return order;
        }

        protected override void ProcessRequest(HttpWebRequest request, Dictionary<string, object> payload)
        {
            if (CanMakeAuthenticatedRequest(payload))
            {
                string form = GetFormForPayload(payload);
                request.Headers["Key"] = PublicApiKey.ToUnsecureString();
                request.Headers["Sign"] = CryptoUtility.SHA512Sign(form, PrivateApiKey.ToUnsecureString());
                request.Method = "POST";
                WriteFormToRequest(request, form);
            }
        }

        public ExchangePoloniexAPI()
        {
            RequestContentType = "application/x-www-form-urlencoded";
        }

        public override string NormalizeSymbol(string symbol)
        {
            return symbol?.ToUpperInvariant().Replace('-', '_');
        }

        public override IEnumerable<string> GetSymbols()
        {
            List<string> symbols = new List<string>();
            var tickers = GetTickers();
            foreach (var kv in tickers)
            {
                symbols.Add(kv.Key);
            }
            return symbols;
        }

        public override Ticker GetTicker(Coin baseCoin, Coin coin)
        {
            string symbol = $"{baseCoin.Code}-{coin.Code}";
            IEnumerable<KeyValuePair<string, Ticker>> tickers = GetTickers();
            foreach (var kv in tickers)
            {
                if (kv.Key == symbol)
                {
                    return kv.Value;
                }
            }
            return null;
        }

        public override IEnumerable<KeyValuePair<string, Ticker>> GetTickers()
        {
            // {"BTC_LTC":{"last":"0.0251","lowestAsk":"0.02589999","highestBid":"0.0251","percentChange":"0.02390438","baseVolume":"6.16485315","quoteVolume":"245.82513926"}
            List<KeyValuePair<string, Ticker>> tickers = new List<KeyValuePair<string, Ticker>>();
            JObject obj = MakeJsonRequest<JObject>("/public?command=returnTicker");
            CheckError(obj);
            foreach (JProperty prop in obj.Children())
            {
                string symbol = prop.Name;
                JToken values = prop.Value;
                tickers.Add(new KeyValuePair<string, Ticker>(symbol, new Ticker
                {
                    Ask = (decimal)values["lowestAsk"],
                    Bid = (decimal)values["highestBid"],
                    Last = (decimal)values["last"],
                    Volume = new ExchangeVolume
                    {
                        PriceAmount = (decimal)values["baseVolume"],
                        PriceSymbol = symbol,
                        QuantityAmount = (decimal)values["quoteVolume"],
                        QuantitySymbol = symbol,
                        Timestamp = DateTime.UtcNow
                    }
                }));
            }
            return tickers;
        }

        public override ExchangeOrderBook GetOrderBook(Coin baseCoin, Coin coin, int maxCount = 100)
        {
            string symbol = NormalizeSymbol($"{baseCoin.Code}-{coin.Code}");
            // {"asks":[["0.01021997",22.83117932],["0.01022000",82.3204],["0.01022480",140],["0.01023054",241.06436945],["0.01023057",140]],"bids":[["0.01020233",164.195],["0.01020232",66.22565096],["0.01020200",5],["0.01020010",66.79296968],["0.01020000",490.19563761]],"isFrozen":"0","seq":147171861}
            symbol = NormalizeSymbol(symbol);
            ExchangeOrderBook book = new ExchangeOrderBook();
            JObject obj = MakeJsonRequest<JObject>("/public?command=returnOrderBook&currencyPair=" + symbol + "&depth=" + maxCount);
            CheckError(obj);
            foreach (JArray array in obj["asks"])
            {
                book.Asks.Add(new ExchangeOrderPrice { Amount = (decimal)array[1], Price = (decimal)array[0] });
            }
            foreach (JArray array in obj["bids"])
            {
                book.Bids.Add(new ExchangeOrderPrice { Amount = (decimal)array[1], Price = (decimal)array[0] });
            }
            return book;
        }

        public override IEnumerable<KeyValuePair<string, ExchangeOrderBook>> GetOrderBooks(int maxCount = 100)
        {
            List<KeyValuePair<string, ExchangeOrderBook>> books = new List<KeyValuePair<string, ExchangeOrderBook>>();
            JObject obj = MakeJsonRequest<JObject>("/public?command=returnOrderBook&currencyPair=all&depth=" + maxCount);
            CheckError(obj);
            foreach (JProperty token in obj.Children())
            {
                ExchangeOrderBook book = new ExchangeOrderBook();
                foreach (JArray array in token.First["asks"])
                {
                    book.Asks.Add(new ExchangeOrderPrice { Amount = (decimal)array[1], Price = (decimal)array[0] });
                }
                foreach (JArray array in token.First["bids"])
                {
                    book.Bids.Add(new ExchangeOrderPrice { Amount = (decimal)array[1], Price = (decimal)array[0] });
                }
                books.Add(new KeyValuePair<string, ExchangeOrderBook>(token.Name, book));
            }
            return books;
        }

        public override IEnumerable<Trade> GetHistoricalTrades(Coin baseCoin, Coin coin, DateTime? sinceDateTime = null)
        {
            string symbol = $"{baseCoin.Code}-{coin.Code}";
            // [{"globalTradeID":245321705,"tradeID":11501281,"date":"2017-10-20 17:39:17","type":"buy","rate":"0.01022188","quantity":"0.00954454","total":"0.00009756"},...]
            // https://poloniex.com/public?command=returnTradeHistory&currencyPair=BTC_LTC&start=1410158341&end=1410499372
            symbol = NormalizeSymbol(symbol);
            string baseUrl = "/public?command=returnTradeHistory&currencyPair=" + symbol;
            string url;
            string dt;
            DateTime timestamp;
            List<Trade> trades = new List<Trade>();
            while (true)
            {
                url = baseUrl;
                if (sinceDateTime != null)
                {
                    url += "&start=" + (long)CryptoUtility.UnixTimestampFromDateTimeSeconds(sinceDateTime.Value) + "&end=" +
                        (long)CryptoUtility.UnixTimestampFromDateTimeSeconds(sinceDateTime.Value.AddDays(1.0));
                }
                JArray obj = MakeJsonRequest<JArray>(url);
                if (obj == null || obj.Count == 0)
                {
                    break;
                }
                if (sinceDateTime != null)
                {
                    sinceDateTime = ((DateTime)obj[0]["date"]).AddSeconds(1.0);
                }
                foreach (JToken child in obj.Children())
                {
                    dt = ((string)child["date"]).Replace(' ', 'T').Trim('Z') + "Z";
                    timestamp = DateTime.Parse(dt).ToUniversalTime();
                    trades.Add(new Trade
                    {
                        Amount = (decimal)child["quantity"],
                        Price = (decimal)child["rate"],
                        Timestamp = timestamp,
                        Id = (long)child["globalTradeID"],
                        IsBuy = (string)child["type"] == "buy"
                    });
                }
                trades.Sort((t1, t2) => t1.Timestamp.CompareTo(t2.Timestamp));
                foreach (Trade t in trades)
                {
                    yield return t;
                }
                trades.Clear();
                if (sinceDateTime == null)
                {
                    break;
                }
                System.Threading.Thread.Sleep(2000);
            }
        }

        public override IEnumerable<Trade> GetRecentTrades(Coin baseCoin, Coin coin)
        {
            string symbol = NormalizeSymbol($"{baseCoin.Code}-{coin.Code}");
            return GetHistoricalTrades(baseCoin, coin);
        }

        public override IEnumerable<Candle> GetCandles(Coin baseCoin, Coin coin, int periodSeconds, DateTime? startDate = null, DateTime? endDate = null)
        {
            // https://poloniex.com/public?command=returnChartData&currencyPair=BTC_XMR&start=1405699200&end=9999999999&period=14400
            // [{"date":1405699200,"high":0.0045388,"low":0.00403001,"open":0.00404545,"close":0.00435873,"volume":44.34555992,"quoteVolume":10311.88079097,"weightedAverage":0.00430043}]
            string symbol = NormalizeSymbol($"{baseCoin.Code}-{coin.Code}");
            string url = "/public?command=returnChartData&currencyPair=" + symbol;
            if (startDate != null)
            {
                url += "&start=" + (long)startDate.Value.UnixTimestampFromDateTimeSeconds();
            }
            url += "&end=" + (endDate == null ? long.MaxValue : (long)endDate.Value.UnixTimestampFromDateTimeSeconds());
            url += "&period=" + periodSeconds;
            JToken token = MakeJsonRequest<JToken>(url);
            CheckError(token);
            foreach (JToken candle in token)
            {
                yield return new Candle
                {
                    ClosePrice = (decimal)candle["close"],
                    ExchangeName = Name,
                    HighPrice = (decimal)candle["high"],
                    LowPrice = (decimal)candle["low"],
                    OpenPrice = (decimal)candle["open"],
                    Name = symbol,
                    PeriodSeconds = periodSeconds,
                    Timestamp = CryptoUtility.UnixTimeStampToDateTimeSeconds((long)candle["date"]),
                    VolumePrice = (double)candle["volume"],
                    VolumeQuantity = (double)candle["quoteVolume"],
                    WeightedAverage = (decimal)candle["weightedAverage"]
                };
            }
        }

        public override Dictionary<string, decimal> GetAmountsAvailableToTrade()
        {
            Dictionary<string, decimal> amounts = new Dictionary<string, decimal>();
            JToken result = MakePrivateAPIRequest("returnBalances");
            foreach (JProperty child in result.Children())
            {
                amounts[child.Name] = (decimal)child.Value;
            }
            return amounts;
        }

        public override ExchangeOrderResult PlaceOrder(Coin baseCoin, Coin coin, decimal quantity, decimal? price, bool buy, Enumerations.OrderTypeEnum type)
        {
            string symbol = NormalizeSymbol($"{baseCoin.Code}-{coin.Code}");
            JToken result = MakePrivateAPIRequest(buy ? "buy" : "sell", "currencyPair", symbol, "rate", price, "quantity", quantity);
            return ParseOrder(result);
        }

        public override ExchangeOrderResult GetOrderDetails(string orderId)
        {
            throw new NotSupportedException("Poloniex does not support getting the details of one order");
        }

        public override IEnumerable<ExchangeOrderResult> GetOpenOrderDetails(Coin baseCoin, Coin coin)
        {
            string symbol = NormalizeSymbol($"{baseCoin.Code}-{coin.Code}");
            ParseOrder(null);
            symbol = NormalizeSymbol(symbol);
            JToken result;
            if (!string.IsNullOrWhiteSpace(symbol))
            {
                result = MakePrivateAPIRequest("getOpenOrders", "currencyPair", symbol);
            }
            else
            {
                result = MakePrivateAPIRequest("getOpenOrders");
            }
            CheckError(result);
            if (result is JArray array)
            {
                foreach (JToken token in array)
                {
                    yield return ParseOrder(token);
                }
            }
        }

        public override void CancelOrder(string orderId)
        {
            JToken token = MakePrivateAPIRequest("cancelOrder", "orderNumber", long.Parse(orderId));
            CheckError(token);
            if (token["success"] == null || (int)token["success"] != 1)
            {
                throw new APIException("Failed to cancel order, success was not 1");
            }
        }
    }
}