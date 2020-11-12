

using CryptoBot.Model.Exchanges;
using Newtonsoft.Json.Linq;
using RestSharp.Extensions.MonoHttp;
using System;
using System.Collections.Generic;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using CryptoBot.Model.Common;
using CryptoBot.Model.Domain.Market;
using CryptoBot.Model.Domain.Trading;
using NLog;

namespace CryptoBot.ExchangeEngine.API.Exchanges
{
    public class ExchangeBittrexAPI : ExchangeAPI
    {
        private ExchangeSettings _exchangeSettings;
        public override string BaseUrl { get; set; } = "https://bittrex.com/api/v1.1";
        public string BaseUrl2 { get; set; } = "https://bittrex.com/api/v2.0";
        public override string Name => ExchangeName.Bittrex;


        public ExchangeBittrexAPI()
        {
            
        }

        public ExchangeBittrexAPI(ExchangeSettings exchangeSettings)
        {
            Logger = LogManager.GetCurrentClassLogger();
            RequestContentType = "application/json";
            _exchangeSettings = exchangeSettings;

            LoadSettings();

            RateLimit = new RateGate(2, TimeSpan.FromSeconds(1));
        }

        public void LoadSettings()
        {
            try
            {
                PublicApiKey = _exchangeSettings.ApiKey?.ToSecureString();
                PrivateApiKey = _exchangeSettings.Secret?.ToSecureString();
                Passphrase = _exchangeSettings.PassPhrase?.ToSecureString();
            }
            catch (Exception)
            {
                throw new Exception("Problem loading API information on GDAX");
            }
        }

        public override bool Simulated
        {
            get { return _exchangeSettings.Simulate; }
        }

        public Logger Logger { get; set; }

        private void CheckError(JToken obj)
        {
            if (obj["success"] == null || !obj["success"].Value<bool>())
            {
                throw new APIException(obj["message"].Value<string>());
            }
        }

        private ExchangeOrderResult ParseOrder(JToken token)
        {

            ExchangeOrderResult order = new ExchangeOrderResult();
            decimal amount = token.Value<decimal>("Quantity");
            decimal remaining = token.Value<decimal>("QuantityRemaining");
            decimal amountFilled = amount - remaining;
            order.Amount = amount;
            order.AmountFilled = amountFilled;
            order.AveragePrice = token.Value<decimal>("Price") / (amountFilled == 0 ? 1:amountFilled);
            order.Message = string.Empty;
            order.OrderId = token.Value<string>("OrderUuid");
            order.Result = (amountFilled == amount ? ExchangeAPIOrderResult.Filled : (amountFilled == 0 ? ExchangeAPIOrderResult.Pending : ExchangeAPIOrderResult.FilledPartially));
            order.OrderDate = token["Opened"].Value<DateTime>();
            order.Symbol = token["Exchange"].Value<string>();
            order.Fees = token.Value<decimal>("CommissionPaid");
            string type = (string)token["OrderType"];
            if (string.IsNullOrWhiteSpace(type))
            {
                type = (string)token["Type"] ?? string.Empty;
            }
            order.IsBuy = type.IndexOf("BUY", StringComparison.OrdinalIgnoreCase) >= 0;
            return order;
        }

        private Dictionary<string, object> GetNoncePayload()
        {
            return new Dictionary<string, object>
            {
                { "nonce", DateTime.UtcNow.Ticks }
            };
        }

        protected override Uri ProcessRequestUrl(UriBuilder url, Dictionary<string, object> payload)
        {
            if (CanMakeAuthenticatedRequest(payload))
            {
                // payload is ignored, except for the nonce which is added to the url query - bittrex puts all the "post" parameters in the url query instead of the request body
                var query = HttpUtility.ParseQueryString(url.Query);
                url.Query = "apikey=" + _exchangeSettings.ApiKey + "&nonce=" + payload["nonce"].ToString() + (query.Count == 0 ? string.Empty : "&" + query.ToString());
                return url.Uri;
            }
            return url.Uri;
        }

        protected override void ProcessRequest(HttpWebRequest request, Dictionary<string, object> payload)
        {
            if (CanMakeAuthenticatedRequest(payload))
            {
                string url = request.RequestUri.ToString();
                string sign = CryptoUtility.SHA512Sign(url, _exchangeSettings.Secret);
                
                request.Headers["apisign"] = sign;
                
            }
        }

        public override string NormalizeSymbol(string symbol)
        {
            return symbol?.ToUpperInvariant();
        }

        public override IEnumerable<string> GetSymbols()
        {
            List<string> symbols = new List<string>();
            JObject obj = MakeJsonRequest<JObject>("/public/getmarkets");
            CheckError(obj);
            if (obj["result"] is JArray array)
            {
                foreach (JToken token in array)
                {
                    symbols.Add(token["MarketName"].Value<string>());
                }
            }
            return symbols;
        }

        public override Ticker GetTicker(Coin baseCoin, Coin coin)
        {
            string symbol = $"{baseCoin.Code}-{coin.Code}";
            JObject obj = MakeJsonRequest<JObject>("/public/getmarketsummary?market=" + NormalizeSymbol(symbol));
            CheckError(obj);
            JToken ticker = obj["result"][0];
            if (ticker != null)
            {
                return new Ticker
                {
                    Ask = ticker["Ask"].Value<decimal>(),
                    Bid = ticker["Bid"].Value<decimal>(),
                    Last = ticker["Last"].Value<decimal>(),
                    Volume = new ExchangeVolume
                    {
                        PriceAmount = ticker["Volume"].Value<decimal>(),
                        PriceSymbol = symbol,
                        QuantityAmount = ticker["BaseVolume"].Value<decimal>(),
                        QuantitySymbol = symbol,
                        Timestamp = ticker["TimeStamp"].Value<DateTime>()
                    }
                };
            }
            return null;
        }

        public override IEnumerable<KeyValuePair<string, Ticker>> GetTickers()
        {
            JObject obj = MakeJsonRequest<Newtonsoft.Json.Linq.JObject>("public/getmarketsummaries");
            CheckError(obj);
            JToken tickers = obj["result"];
            if (tickers == null)
            {
                return null;
            }
            string symbol;
            List<KeyValuePair<string, Ticker>> tickerList = new List<KeyValuePair<string, Ticker>>();
            foreach (JToken ticker in tickers)
            {
                symbol = (string)ticker["MarketName"];
                Ticker tickerObj = new Ticker
                {
                    Ask = (decimal)ticker["Ask"],
                    Bid = (decimal)ticker["Bid"],
                    Last = (decimal)ticker["Last"],
                    Volume = new ExchangeVolume
                    {
                        PriceAmount = (decimal)ticker["BaseVolume"],
                        PriceSymbol = symbol,
                        QuantityAmount = (decimal)ticker["Volume"],
                        QuantitySymbol = symbol,
                        Timestamp = (DateTime)ticker["TimeStamp"]
                    }
                };
                tickerList.Add(new KeyValuePair<string, Ticker>(symbol, tickerObj));
            }
            return tickerList;
        }

        public override ExchangeOrderBook GetOrderBook(Coin baseCoin, Coin coin, int maxCount = 100)
        {
            string symbol = NormalizeSymbol($"{baseCoin.Code}-{coin.Code}");
            JObject obj = MakeJsonRequest<Newtonsoft.Json.Linq.JObject>("public/getorderbook?market=" + symbol + "&type=both&limit_bids=" + maxCount + "&limit_asks=" + maxCount);
            CheckError(obj);
            JToken book = obj["result"];
            if (book == null)
            {
                return null;
            }
            ExchangeOrderBook orders = new ExchangeOrderBook();
            JToken bids = book["buy"];
            foreach (JToken token in bids)
            {
                ExchangeOrderPrice order = new ExchangeOrderPrice { Amount = token["Quantity"].Value<decimal>(), Price = token["Rate"].Value<decimal>() };
                orders.Bids.Add(order);
            }
            JToken asks = book["sell"];
            foreach (JToken token in asks)
            {
                ExchangeOrderPrice order = new ExchangeOrderPrice { Amount = token["Quantity"].Value<decimal>(), Price = token["Rate"].Value<decimal>() };
                orders.Asks.Add(order);
            }
            return orders;
        }

        public override IEnumerable<Trade> GetHistoricalTrades(Coin baseCoin, Coin coin, DateTime? sinceDateTime = null)
        {
            string symbol = $"{baseCoin.Code}-{coin.Code}";

            // TODO: sinceDateTime is ignored
            // https://bittrex.com/Api/v2.0/pub/market/GetTicks?marketName=BTC-WAVES&tickInterval=oneMin&_=1499127220008
            symbol = NormalizeSymbol(symbol);
            string baseUrl = "/pub/market/GetTicks?marketName=" + symbol + "&tickInterval=oneMin";
            string url;
            List<Trade> trades = new List<Trade>();
            while (true)
            {
                url = baseUrl;
                if (sinceDateTime != null)
                {
                    url += "&_=" + DateTime.UtcNow.Ticks;
                }
                JObject obj = MakeJsonRequest<JObject>(url, BaseUrl2);
                CheckError(obj);
                JArray array = obj["result"] as JArray;
                if (array == null || array.Count == 0)
                {
                    break;
                }
                if (sinceDateTime != null)
                {
                    sinceDateTime = array.Last["T"].Value<DateTime>();
                }
                foreach (JToken trade in array)
                {
                    // {"O":0.00106302,"H":0.00106302,"L":0.00106302,"C":0.00106302,"V":80.58638589,"T":"2017-08-18T17:48:00","BV":0.08566493}
                    trades.Add(new Trade
                    {
                        Amount = trade["V"].Value<decimal>(),
                        Price = trade["C"].Value<decimal>(),
                        Timestamp = trade["T"].Value<DateTime>(),
                        Id = -1,
                        IsBuy = true
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
                System.Threading.Thread.Sleep(1000);
            }
        }

        public override IEnumerable<Trade> GetRecentTrades(Coin baseCoin, Coin coin)
        {
            string symbol = NormalizeSymbol($"{baseCoin.Code}-{coin.Code}");
            string baseUrl = "/public/getmarkethistory?market=" + symbol;
            JObject obj = MakeJsonRequest<JObject>(baseUrl);
            CheckError(obj);
            JArray array = obj["result"] as JArray;
            if (array != null && array.Count != 0)
            {
                foreach (JToken token in array)
                {
                    yield return new Trade
                    {
                        Amount = token.Value<decimal>("Quantity"),
                        IsBuy = token.Value<string>("OrderType").Equals("BUY", StringComparison.OrdinalIgnoreCase),
                        Price = token.Value<decimal>("Price"),
                        Timestamp = token.Value<DateTime>("TimeStamp"),
                        Id = token.Value<long>("Id")
                    };
                }
            }
        }
        

        public override IEnumerable<Candle> GetCandles(Coin baseCoin, Coin coin, int periodSeconds, DateTime? startDate = null, DateTime? endDate = null)
        {
            // https://bittrex.com/Api/v2.0/pub/market/GetTicks?marketName=BTC-WAVES&tickInterval=day
            // "{"success":true,"message":"","result":[{"O":0.00011000,"H":0.00060000,"L":0.00011000,"C":0.00039500,"V":5904999.37958770,"T":"2016-06-20T00:00:00","BV":2212.16809610} ] }"
            string periodString;
            switch (periodSeconds)
            {
                case 60: periodString = "oneMin"; break;
                case 300: periodString = "fiveMin"; break;
                case 1800: periodString = "thirtyMin"; break;
                case 3600: periodString = "hour"; break;
                case 86400: periodString = "day"; break;
                case 259200: periodString = "threeDay"; break;
                case 604800: periodString = "week"; break;
                default:
                    if (periodSeconds > 604800)
                    {
                        periodString = "month";
                    }
                    else
                    {
                        throw new ArgumentOutOfRangeException("Period seconds must be 60,300,1800,3600,86400, 259200 or 604800");
                    }
                    break;
            }
            string symbol = NormalizeSymbol($"{baseCoin.Code}-{coin.Code}");
            startDate = startDate ?? DateTime.UtcNow;
            endDate = endDate ?? startDate.Value.Subtract(TimeSpan.FromDays(1.0));
            JToken result = MakeJsonRequest<JToken>("pub/market/GetTicks?marketName=" + symbol + "&tickInterval=" + periodString, BaseUrl2);
            CheckError(result);
            JArray array = result["result"] as JArray;
            foreach (JToken jsonCandle in array)
            {
                Candle candle = new Candle
                {
                    ClosePrice = (decimal)jsonCandle["C"],
                    ExchangeName = Name,
                    HighPrice = (decimal)jsonCandle["H"],
                    LowPrice = (decimal)jsonCandle["L"],
                    Name = symbol,
                    OpenPrice = (decimal)jsonCandle["O"],
                    PeriodSeconds = periodSeconds,
                    Timestamp = (DateTime)jsonCandle["T"],
                    VolumePrice = (double)jsonCandle["BV"],
                    VolumeQuantity = (double)jsonCandle["V"]
                };
                if (candle.Timestamp >= startDate && candle.Timestamp <= endDate)
                {
                    yield return candle;
                }
            }
        }

        public override Dictionary<string, decimal> GetAmountsAvailableToTrade()
        {
            Dictionary<string, decimal> currencies = new Dictionary<string, decimal>(StringComparer.OrdinalIgnoreCase);
            string url = "/account/getbalances";
            JObject obj = MakeJsonRequest<JObject>(url, null, GetNoncePayload());
            CheckError(obj);
            if (obj["result"] is JArray array)
            {
                foreach (JToken token in array)
                {
                    currencies.Add(token["Currency"].Value<string>(), token["Available"].Value<decimal>());
                }
            }
            return currencies;
        }

        public override ExchangeOrderResult PlaceOrder(Coin baseCoin, Coin coin, decimal quantity, decimal? price, bool buy, Enumerations.OrderTypeEnum type)
        {
            // No market or stop type orders available here, see https://bittrex.com/home/api
            string symbol = NormalizeSymbol($"{baseCoin.Code}-{coin.Code}");
            string url = (buy ? "/market/buylimit" : "/market/selllimit") + "?market=" + symbol + "&quantity=" + quantity + "&rate=" + price;
            JObject obj = MakeJsonRequest<JObject>(url, null, GetNoncePayload());
            CheckError(obj);
            string orderId = obj["result"]["uuid"].Value<string>();
            return GetOrderDetails(orderId);
        }

        public override ExchangeOrderResult GetOrderDetails(string orderId)
        {
            if (string.IsNullOrWhiteSpace(orderId))
            {
                return null;
            }

            string url = "/account/getorder?uuid=" + orderId;
            JObject obj = MakeJsonRequest<JObject>(url, null, GetNoncePayload());
            CheckError(obj);
            JToken result = obj["result"];
            if (result == null)
            {
                return null;
            }
            return ParseOrder(result);
        }

        public override IEnumerable<ExchangeOrderResult> GetOpenOrderDetails(Coin baseCoin, Coin coin)
        {
            string symbol = NormalizeSymbol($"{baseCoin.Code}-{coin.Code}");
            string url = "/market/getopenorders" + (string.IsNullOrWhiteSpace(symbol) ? string.Empty : "?market=" + NormalizeSymbol(symbol));
            JObject obj = MakeJsonRequest<JObject>(url, null, GetNoncePayload());
            CheckError(obj);
            JToken result = obj["result"];
            if (result != null)
            {
                foreach (JToken token in result.Children())
                {
                    yield return ParseOrder(token);
                }
            }
        }

        public override void CancelOrder(string orderId)
        {
            JObject obj = MakeJsonRequest<JObject>("/market/cancel?uuid=" + orderId, null, GetNoncePayload());
            CheckError(obj);
        }
    }
}
