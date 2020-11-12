

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
    public class ExchangeGeminiAPI : ExchangeAPI
    {
        public override string BaseUrl { get; set; } = "https://api.gemini.com/v1";
        public override string Name => ExchangeName.Gemini;

        private ExchangeVolume ParseVolume(JToken token)
        {
            ExchangeVolume vol = new ExchangeVolume();
            JProperty[] props = token.Children<JProperty>().ToArray();
            if (props.Length == 3)
            {
                vol.PriceSymbol = props[0].Name;
                vol.PriceAmount = (decimal)props[0].Value;
                vol.QuantitySymbol = props[1].Name;
                vol.QuantityAmount = (decimal)props[1].Value;
                vol.Timestamp = CryptoUtility.UnixTimeStampToDateTimeMilliseconds((long)props[2].Value);
            }

            return vol;
        }

        private ExchangeOrderResult ParseOrder(JToken result)
        {
            decimal amount = result["original_amount"].Value<decimal>();
            decimal amountFilled = result["executed_amount"].Value<decimal>();
            return new ExchangeOrderResult
            {
                Amount = amount,
                AmountFilled = amountFilled,
                AveragePrice = result["price"].Value<decimal>(),
                Message = string.Empty,
                OrderId = result["id"].Value<string>(),
                Result = (amountFilled == amount ? ExchangeAPIOrderResult.Filled : (amountFilled == 0 ? ExchangeAPIOrderResult.Pending : ExchangeAPIOrderResult.FilledPartially)),
                OrderDate = CryptoUtility.UnixTimeStampToDateTimeMilliseconds(result["timestampms"].Value<double>()),
                Symbol = result["symbol"].Value<string>(),
                IsBuy = result["side"].Value<string>() == "buy"
            };
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
                { "nonce", DateTime.UtcNow.Ticks }
            };
        }

        protected override void ProcessRequest(HttpWebRequest request, Dictionary<string, object> payload)
        {
            if (CanMakeAuthenticatedRequest(payload))
            {
                payload.Add("request", request.RequestUri.AbsolutePath);
                string json = JsonConvert.SerializeObject(payload);
                string json64 = System.Convert.ToBase64String(Encoding.ASCII.GetBytes(json));
                string hexSha384 = CryptoUtility.SHA384Sign(json64, CryptoUtility.SecureStringToString(PrivateApiKey));
                request.Headers["X-GEMINI-PAYLOAD"] = json64;
                request.Headers["X-GEMINI-SIGNATURE"] = hexSha384;
                request.Headers["X-GEMINI-APIKEY"] = CryptoUtility.SecureStringToString(PublicApiKey);
                request.Method = "POST";

                // gemini doesn't put the payload in the post body it puts it in as a http header, so no need to write to request stream
            }
        }

        public override string NormalizeSymbol(string symbol)
        {
            return symbol?.Replace("-", string.Empty).ToLowerInvariant();
        }

        public override IEnumerable<string> GetSymbols()
        {
            return MakeJsonRequest<string[]>("/symbols");
        }

        public override Ticker GetTicker(Coin baseCoin, Coin coin)
        {
            string symbol = NormalizeSymbol("{baseCoin.Code}-{coin.Code}");
            JObject obj = MakeJsonRequest<Newtonsoft.Json.Linq.JObject>("/pubticker/" + symbol);
            if (obj == null || obj.Count == 0)
            {
                return null;
            }
            Ticker t = new Ticker
            {
                Ask = obj.Value<decimal>("ask"),
                Bid = obj.Value<decimal>("bid"),
                Last = obj.Value<decimal>("last")
            };
            t.Volume = ParseVolume(obj["volume"]);
            return t;
        }

        public override ExchangeOrderBook GetOrderBook(Coin baseCoin, Coin coin, int maxCount = 100)
        {
            string symbol = NormalizeSymbol($"{baseCoin.Code}-{coin.Code}");
            JObject obj = MakeJsonRequest<Newtonsoft.Json.Linq.JObject>("/book/" + symbol + "?limit_bids=" + maxCount + "&limit_asks=" + maxCount);
            if (obj == null || obj.Count == 0)
            {
                return null;
            }

            ExchangeOrderBook orders = new ExchangeOrderBook();
            JToken bids = obj["bids"];
            foreach (JToken token in bids)
            {
                ExchangeOrderPrice order = new ExchangeOrderPrice { Amount = token["quantity"].Value<decimal>(), Price = token["price"].Value<decimal>() };
                orders.Bids.Add(order);
            }
            JToken asks = obj["asks"];
            foreach (JToken token in asks)
            {
                ExchangeOrderPrice order = new ExchangeOrderPrice { Amount = token["quantity"].Value<decimal>(), Price = token["price"].Value<decimal>() };
                orders.Asks.Add(order);
            }
            return orders;
        }

        public override IEnumerable<Trade> GetHistoricalTrades(Coin baseCoin, Coin coin, DateTime? sinceDateTime = null)
        {
            string symbol = $"{baseCoin.Code}-{coin.Code}";
            const int maxCount = 100;
            symbol = NormalizeSymbol(symbol);
            string baseUrl = "/trades/" + symbol + "?limit_trades=" + maxCount;
            string url;
            List<Trade> trades = new List<Trade>();
            while (true)
            {
                url = baseUrl;
                if (sinceDateTime != null)
                {
                    url += "&timestamp=" + CryptoUtility.UnixTimestampFromDateTimeMilliseconds(sinceDateTime.Value).ToString();
                }
                JArray obj = MakeJsonRequest<Newtonsoft.Json.Linq.JArray>(url);
                if (obj == null || obj.Count == 0)
                {
                    break;
                }
                if (sinceDateTime != null)
                {
                    sinceDateTime = CryptoUtility.UnixTimeStampToDateTimeMilliseconds(obj.First["timestampms"].Value<long>());
                }
                foreach (JToken token in obj)
                {
                    trades.Add(new Trade
                    {
                        Amount = token["quantity"].Value<decimal>(),
                        Price = token["price"].Value<decimal>(),
                        Timestamp = CryptoUtility.UnixTimeStampToDateTimeMilliseconds(token["timestampms"].Value<long>()),
                        Id = token["tid"].Value<long>(),
                        IsBuy = token["type"].Value<string>() == "buy"
                    });
                }
                trades.Sort((t1, t2) => t1.Timestamp.CompareTo(t2.Timestamp));
                foreach (Trade t in trades)
                {
                    yield return t;
                }
                trades.Clear();
                if (obj.Count < maxCount || sinceDateTime == null)
                {
                    break;
                }
                System.Threading.Thread.Sleep(1000);
            }
        }

        public override Dictionary<string, decimal> GetAmountsAvailableToTrade()
        {
            Dictionary<string, decimal> lookup = new Dictionary<string, decimal>(StringComparer.OrdinalIgnoreCase);
            JArray obj = MakeJsonRequest<Newtonsoft.Json.Linq.JArray>("/balances", null, GetNoncePayload());
            CheckError(obj);
            var q = from JToken token in obj
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
                { "nonce", DateTime.UtcNow.Ticks },
                { "client_order_id", "ExchangeSharp_" + DateTime.UtcNow.ToString("s", System.Globalization.CultureInfo.InvariantCulture) },
                { "symbol", symbol },
                { "quantity", quantity.ToString(CultureInfo.InvariantCulture.NumberFormat) },
                { "price", price.ToString() },
                { "side", (buy ? "buy" : "sell") },
                { "type", "exchange limit" } // only "exchange limit" supported through API see https://docs.gemini.com/rest-api/#new-order
            };
            JToken obj = MakeJsonRequest<JToken>("/order/new", null, payload);
            CheckError(obj);
            return ParseOrder(obj);
        }

        public override ExchangeOrderResult GetOrderDetails(string orderId)
        {
            if (string.IsNullOrWhiteSpace(orderId))
            {
                return null;
            }

            JToken result = MakeJsonRequest<JToken>("/order/status", null, new Dictionary<string, object> { { "nonce", DateTime.UtcNow.Ticks }, { "order_id", orderId } });
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
            JToken result = MakeJsonRequest<JToken>("/orders", null, new Dictionary<string, object> { { "nonce", DateTime.UtcNow.Ticks } });
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
            JObject result = MakeJsonRequest<JObject>("/order/cancel", null, new Dictionary<string, object>{ { "nonce", DateTime.UtcNow.Ticks }, { "order_id", orderId } });
            CheckError(result);
        }
    }
}
