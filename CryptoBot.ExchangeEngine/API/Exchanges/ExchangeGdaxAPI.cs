
using CryptoBot.Model.Exchanges;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NLog;
using System.Net;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using CryptoBot.Model.Common;
using CryptoBot.Model.Domain.Market;
using CryptoBot.Model.Domain.Trading;

namespace CryptoBot.ExchangeEngine.API.Exchanges
{
    public class ExchangeGdaxAPI : ExchangeAPI
    {
        private readonly ExchangeSettings _exchangeSettings;
        public override string BaseUrl { get; set; } = "https://api.gdax.com";
        public override string Name => ExchangeName.GDAX;
        public Logger Logger { get; set; }

        /// <summary>
        /// The response will also contain a CB-AFTER header which will return the cursor id to use in your next request for the page after this one. The page after is an older page and not one that happened after this one in chronological time.
        /// </summary>
        private string cursorAfter;

        /// <summary>
        /// The response will contain a CB-BEFORE header which will return the cursor id to use in your next request for the page before the current one. The page before is a newer page and not one that happened before in chronological time.
        /// </summary>
        private string cursorBefore;

        

        private ExchangeOrderResult ParseOrder(JToken result)
        {
            decimal filledSize = (decimal)result["filled_size"];

            ExchangeOrderResult order = new ExchangeOrderResult
            {
                Amount = (decimal) result["size"],
                AmountFilled = (decimal) result["filled_size"],
                AveragePrice = (decimal) result["executed_value"] / (filledSize == 0 ? 1:filledSize),
                IsBuy = ((string) result["side"]) == "buy",
                OrderDate = (DateTime) result["created_at"],
                Symbol = (string) result["product_id"],
                OrderId = (string) result["id"],
                Fees = (decimal)result["fill_fees"]
            };
            switch ((string) result["status"])
            {
                case "pending":
                    order.Result = ExchangeAPIOrderResult.Pending;
                    break;
                case "active":
                case "open":
                    if (order.Amount == order.AmountFilled)
                    {
                        order.Result = ExchangeAPIOrderResult.Filled;
                    }
                    else if (order.AmountFilled > 0.0m)
                    {
                        order.Result = ExchangeAPIOrderResult.FilledPartially;
                    }
                    else
                    {
                        order.Result = ExchangeAPIOrderResult.Pending;
                    }
                    break;
                case "done":
                case "settled":
                    order.Result = ExchangeAPIOrderResult.Filled;
                    break;
                case "cancelled":
                case "canceled":
                    order.Result = ExchangeAPIOrderResult.Canceled;
                    break;
                default:
                    order.Result = ExchangeAPIOrderResult.Unknown;
                    break;
            }
            return order;
        }

        private Dictionary<string, object> GetTimestampPayload()
        {
            return new Dictionary<string, object>
            {
                {"nonce", CryptoUtility.UnixTimestampFromDateTimeSeconds(DateTime.UtcNow)}
            };
        }

        protected override bool CanMakeAuthenticatedRequest(IReadOnlyDictionary<string, object> payload)
        {
            return base.CanMakeAuthenticatedRequest(payload) && Passphrase != null;
        }

        protected override void ProcessRequest(HttpWebRequest request, Dictionary<string, object> payload)
        {
            if (!CanMakeAuthenticatedRequest(payload))
            {
                return;
            }

            // gdax is funny and wants a seconds double for the nonce, weird... we convert it to double and back to string invariantly to ensure decimal dot is used and not comma
            string timestamp = double.Parse(payload["nonce"].ToString()).ToString(CultureInfo.InvariantCulture);
            payload.Remove("nonce");
            string form = GetJsonForPayload(payload);
            byte[] secret = CryptoUtility.SecureStringToBytesBase64Decode(PrivateApiKey);
            string toHash = timestamp + request.Method.ToUpper() + request.RequestUri.PathAndQuery + form;
            string signatureBase64String = CryptoUtility.SHA256SignBase64(toHash, secret);
            secret = null;
            toHash = null;
            request.Headers["CB-ACCESS-KEY"] = PublicApiKey.ToUnsecureString();
            request.Headers["CB-ACCESS-SIGN"] = signatureBase64String;
            request.Headers["CB-ACCESS-TIMESTAMP"] = timestamp;
            request.Headers["CB-ACCESS-PASSPHRASE"] = CryptoUtility.SecureStringToString(Passphrase);
            WriteFormToRequest(request, form);
        }

        protected override void ProcessResponse(HttpWebResponse response)
        {
            base.ProcessResponse(response);
            cursorAfter = response.Headers["cb-after"];
            cursorBefore = response.Headers["cb-before"];
        }

        /// <summary>
        /// Constructor
        /// </summary>
        public ExchangeGdaxAPI()
        {
            RequestContentType = "application/json";
        }

        public ExchangeGdaxAPI(ExchangeSettings exchangeSettings)
        {
            Logger = LogManager.GetCurrentClassLogger();
            RequestContentType = "application/json";
            _exchangeSettings = exchangeSettings;

            LoadSettings();

            RateLimit = new RateGate(2, TimeSpan.FromSeconds(1));

        }

        /// <summary>
        /// Normalize GDAX symbol / product id
        /// </summary>
        /// <param name="symbol">Symbol / product id</param>
        /// <returns>Normalized symbol / product id</returns>
        public override string NormalizeSymbol(string symbol)
        {
            return symbol?.Replace('_', '-').ToUpperInvariant();
        }

        /// <summary>
        /// Load API keys from an encrypted file - keys will stay encrypted in memory
        /// </summary>
        /// <param name="encryptedFile">Encrypted file to load keys from</param>
        public override void LoadAPIKeys(string encryptedFile)
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

        public void LoadSettings()
        {
            try
            {
                PublicApiKey = _exchangeSettings?.ApiKey?.ToSecureString();
                PrivateApiKey = _exchangeSettings?.Secret?.ToSecureString();
                Passphrase = _exchangeSettings?.PassPhrase?.ToSecureString();
            }
            catch (Exception)
            {
                throw new Exception("Problem loading API information on GDAX");
            }
        }

        public override IEnumerable<string> GetSymbols()
        {
            Dictionary<string, string>[] symbols = MakeJsonRequest<Dictionary<string, string>[]>("/products");
            List<string> symbolList = new List<string>();
            foreach (Dictionary<string, string> symbol in symbols)
            {
                symbolList.Add(symbol["id"]);
            }
            return symbolList.ToArray();
        }

        public override Ticker GetTicker(Coin baseCoin, Coin coin)
        {
            string symbol = $"{coin.Code}-{baseCoin.Code}";
            Dictionary<string, string> ticker =
                MakeJsonRequest<Dictionary<string, string>>("/products/" + symbol + "/ticker");
            decimal volume = decimal.Parse(ticker["volume"]);
            DateTime timestamp = DateTime.Parse(ticker["time"]);
            decimal price = decimal.Parse(ticker["price"]);
            return new Ticker
            {
                Ask = decimal.Parse(ticker["ask"]),
                Bid = decimal.Parse(ticker["bid"]),
                Last = price,
                Volume =
                    new ExchangeVolume
                    {
                        PriceAmount = volume,
                        PriceSymbol = symbol,
                        QuantityAmount = volume * price,
                        QuantitySymbol = symbol,
                        Timestamp = timestamp
                    }
            };
        }

        public override IEnumerable<Trade> GetHistoricalTrades(Coin baseCoin, Coin coin, DateTime? sinceDateTime = null)
        {
            string symbol = $"{coin.Code}-{baseCoin.Code}";
            string baseUrl = "/products/" + symbol.ToUpperInvariant() + "/candles?granularity=" +
                             (sinceDateTime == null ? "30.0" : "1.0");
            string url;
            List<Trade> trades = new List<Trade>();
            decimal[][] tradeChunk;
            while (true)
            {
                url = baseUrl;
                if (sinceDateTime != null)
                {
                    url += "&start=" +
                           WebUtility.UrlEncode(sinceDateTime.Value.ToString("s",
                               System.Globalization.CultureInfo.InvariantCulture));
                    url += "&end=" +
                           WebUtility.UrlEncode(sinceDateTime.Value.AddMinutes(5.0)
                               .ToString("s", System.Globalization.CultureInfo.InvariantCulture));
                }
                tradeChunk = MakeJsonRequest<decimal[][]>(url);
                if (tradeChunk == null || tradeChunk.Length == 0)
                {
                    break;
                }
                if (sinceDateTime != null)
                {
                    sinceDateTime = CryptoUtility.UnixTimeStampToDateTimeSeconds((double) tradeChunk[0][0]);
                }
                foreach (decimal[] tradeChunkPiece in tradeChunk)
                {
                    trades.Add(new Trade
                    {
                        Amount = tradeChunkPiece[5],
                        IsBuy = true,
                        Price = tradeChunkPiece[3],
                        Timestamp = CryptoUtility.UnixTimeStampToDateTimeSeconds((double) tradeChunkPiece[0]),
                        Id = 0
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
            string symbol = NormalizeSymbol($"{coin.Code}-{baseCoin.Code}");
            string baseUrl = "/products/" + symbol.ToUpperInvariant() + "/trades";
            Dictionary<string, object>[] trades = MakeJsonRequest<Dictionary<string, object>[]>(baseUrl);
            List<Trade> tradeList = new List<Trade>();
            foreach (Dictionary<string, object> trade in trades)
            {
                tradeList.Add(new Trade
                {
                    Amount = decimal.Parse(trade["size"] as string),
                    IsBuy = trade["side"] as string == "buy",
                    Price = decimal.Parse(trade["price"] as string),
                    Timestamp = (DateTime) trade["time"],
                    Id = (long) trade["trade_id"]
                });
            }
            foreach (Trade trade in tradeList)
            {
                yield return trade;
            }
        }

        public override ExchangeOrderBook GetOrderBook(Coin baseCoin, Coin coin, int maxCount = 100)
        {
            string symbol = NormalizeSymbol($"{coin.Code}-{baseCoin.Code}");
            // changing this to level 1 where only the best bid and ask is returned
            // see https://docs.gdax.com/?ruby#get-products for more of an explanation

            string url = "/products/" + symbol.ToUpperInvariant() + "/book";

            if (maxCount > 1)
                url += "?level=2";

            ExchangeOrderBook orders = new ExchangeOrderBook();
            Dictionary<string, object> books = MakeJsonRequest<Dictionary<string, object>>(url);
            JArray asks = books["asks"] as JArray;
            JArray bids = books["bids"] as JArray;
            foreach (JArray ask in asks)
            {
                orders.Asks.Add(new ExchangeOrderPrice {Amount = (decimal) ask[1], Price = (decimal) ask[0]});
            }
            foreach (JArray bid in bids)
            {
                orders.Bids.Add(new ExchangeOrderPrice {Amount = (decimal) bid[1], Price = (decimal) bid[0]});
            }
            return orders;
        }

        public override IEnumerable<Candle> GetCandles(Coin baseCoin, Coin coin, int periodSeconds, DateTime? startDate = null, DateTime? endDate = null)
        {
            // /products/<product-id>/candles
            // https://api.gdax.com/products/LTC-BTC/candles?granularity=86400&start=2017-12-04T18:15:33&end=2017-12-11T18:15:33
            List<Candle> candles = new List<Candle>();
            string symbol = NormalizeSymbol($"{coin.Code}-{baseCoin.Code}");

            if (startDate == null)
            {
                startDate = DateTime.UtcNow.Subtract(TimeSpan.FromDays(1.0));
            }

            if (endDate == null)
            {
                endDate = DateTime.UtcNow;
            }

            StringBuilder sb = new StringBuilder();
            sb.Append("/products/");
            sb.Append(symbol);
            sb.Append("/candles?granularity=" + periodSeconds);
            sb.Append("&start=" +
                      WebUtility.UrlEncode(startDate.Value.ToString("s",
                          System.Globalization.CultureInfo.InvariantCulture)).ToUpper());
            sb.Append("&end=" +
                      WebUtility.UrlEncode(endDate.Value.ToString("s",
                          System.Globalization.CultureInfo.InvariantCulture)).ToUpper());

            // nightmare here with GDAX, they ignore the date in the url and return 350+ records
            // howvwer, if you hover over the sb.ToString() it will acknowledge the dates, wtf? I kid you not

            // time, low, high, open, close, volume
            try
            {
                JToken token = MakeJsonRequest<JToken>(sb.ToString());

                foreach (JArray candle in token)
                {
                    candles.Add(new Candle
                    {
                        ClosePrice = (decimal)candle[4],
                        ExchangeName = Name,
                        HighPrice = (decimal)candle[2],
                        LowPrice = (decimal)candle[1],
                        Name = symbol,
                        OpenPrice = (decimal)candle[3],
                        PeriodSeconds = periodSeconds,
                        Timestamp = CryptoUtility.UnixTimeStampToDateTimeSeconds((long)candle[0]),
                        VolumePrice = (double)candle[5],
                        VolumeQuantity = (double)candle[5] * (double)candle[4]
                    });
                }
                // re-sort in ascending order
                candles.Sort((c1, c2) => c1.Timestamp.CompareTo(c2.Timestamp));
            }
            catch (Exception e)
            {
                throw e;
            }

            
            return candles;
        }

        /// <summary>
        /// Get amounts available to trade, symbol / quantity dictionary
        /// </summary>
        /// <returns>Symbol / quantity dictionary</returns>
        public override Dictionary<string, decimal> GetAmountsAvailableToTrade()
        {
            Dictionary<string, decimal> amounts = new Dictionary<string, decimal>();
            JArray array = MakeJsonRequest<JArray>("/accounts", null, GetTimestampPayload());
            foreach (JToken token in array)
            {
                amounts[(string) token["currency"]] = (decimal) token["available"];
            }
            return amounts;
        }

        /// <summary>
        /// Place a limit order
        /// </summary>
        /// <param name="symbol">Symbol</param>
        /// <param name="quantity">Amount</param>
        /// <param name="price">Price</param>
        /// <param name="buy">True to buy, false to sell</param>
        /// <returns>Result</returns>
        public override ExchangeOrderResult PlaceOrder(Coin baseCoin, Coin coin, decimal quantity, decimal? price, bool buy, Enumerations.OrderTypeEnum type)
        {
            string symbol = NormalizeSymbol($"{coin.Code}-{baseCoin.Code}");
            Dictionary<string, object> payload = new Dictionary<string, object>
            {
                {"nonce", CryptoUtility.UnixTimestampFromDateTimeSeconds(DateTime.UtcNow)},
                {"type", type.ToString().ToLower()},
                {"side", (buy ? "buy" : "sell")},
                {"product_id", symbol},
                {"price", price.GetValueOrDefault().ToString(CultureInfo.InvariantCulture)},
                {"size", quantity.ToString(CultureInfo.InvariantCulture)}
            };

            switch (type)
            {
                    case Enumerations.OrderTypeEnum.Market:
                    break;
                    case Enumerations.OrderTypeEnum.Limit:
                        payload.Add("time_in_force", "GTC");
                    break;
                    case Enumerations.OrderTypeEnum.Stop:
                    break;
            }

            JObject result = MakeJsonRequest<JObject>("/orders", null, payload, "POST");
            return ParseOrder(result);
        }

        /// <summary>
        /// Get order details
        /// </summary>
        /// <param name="orderId">Order id to get details for</param>
        /// <returns>Order details</returns>
        public override ExchangeOrderResult GetOrderDetails(string orderId)
        {
            JObject obj = MakeJsonRequest<JObject>("/orders/" + orderId, null, GetTimestampPayload(), "GET");
            return ParseOrder(obj);
        }

        /// <summary>
        /// Get the details of all open orders
        /// </summary>
        /// <param name="symbol">Symbol to get open orders for or null for all</param>
        /// <returns>All open order details</returns>
        public override IEnumerable<ExchangeOrderResult> GetOpenOrderDetails(Coin baseCoin, Coin coin)
        {
            string symbol = NormalizeSymbol($"{coin.Code}-{baseCoin.Code}");
            symbol = NormalizeSymbol(symbol);
            JArray array =
                MakeJsonRequest<JArray>(
                    "orders?type=all" + (string.IsNullOrWhiteSpace(symbol) ? string.Empty : "&product_id=" + symbol),
                    null, GetTimestampPayload());
            foreach (JToken token in array)
            {
                yield return ParseOrder(token);
            }
        }

        /// <summary>
        /// Cancel an order, an exception is thrown if error
        /// </summary>
        /// <param name="orderId">Order id of the order to cancel</param>
        public override void CancelOrder(string orderId)
        {
            MakeJsonRequest<JArray>("orders/" + orderId, null, GetTimestampPayload(), "DELETE");
        }

        public override Task OpenSocket()
        {

            ClientWebSocket socket = new ClientWebSocket();
            Task task = socket.ConnectAsync(new Uri("wss://ws-feed.gdax.com"), CancellationToken.None);
            task.Wait();
            Thread readThread = new Thread(
                delegate (object obj)
                {
                    byte[] recBytes = new byte[1024];
                    while (true)
                    {
                        ArraySegment<byte> t = new ArraySegment<byte>(recBytes);

                        Task<WebSocketReceiveResult> receiveAsync = socket.ReceiveAsync(t, CancellationToken.None);
                        receiveAsync.Wait();
                        string jsonString = Encoding.UTF8.GetString(recBytes);
                        //Console.Out.WriteLine("jsonString = {0}", jsonString);
                        Logger.Info(jsonString);
                        recBytes = new byte[1024];
                    }

                });

            readThread.Start();
            string json = "{\"product_ids\":[\"btc-usd\"],\"type\":\"subscribe\"}";
            byte[] bytes = Encoding.UTF8.GetBytes(json);

            ArraySegment<byte> subscriptionMessageBuffer = new ArraySegment<byte>(bytes);

            socket.SendAsync(subscriptionMessageBuffer, WebSocketMessageType.Text, true, CancellationToken.None);
           
            return null;
        }


        public async Task SubscribeAsync(Action<string> messageReceived)
        {
            if (messageReceived == null)
                throw new ArgumentNullException("onMessageReceived", "Message received callback must not be null.");

            var webSocketClient = new ClientWebSocket();
            var cancellationToken = new CancellationToken();
            await webSocketClient.ConnectAsync( new Uri( _exchangeSettings.SocketUrl), cancellationToken).ConfigureAwait(false);


            if (webSocketClient.State == WebSocketState.Open)
            {
                var requestString = JsonConvert.SerializeObject(new
                {
                    type = "subscribe",
                    product_ids = "BTC-EUR"
                });

                var requestBytes = UTF8Encoding.UTF8.GetBytes(requestString);
                var subscribeRequest = new ArraySegment<byte>(requestBytes);
                var sendCancellationToken = new CancellationToken();
                await webSocketClient.SendAsync(subscribeRequest, WebSocketMessageType.Text, true, sendCancellationToken).ConfigureAwait(false);

                while (webSocketClient.State == WebSocketState.Open)
                {
                    var receiveCancellationToken = new CancellationToken();
                    using (var stream = new MemoryStream(1024))
                    {
                        var receiveBuffer = new ArraySegment<byte>(new byte[1024 * 8]);
                        WebSocketReceiveResult webSocketReceiveResult;
                        do
                        {
                            webSocketReceiveResult = await webSocketClient.ReceiveAsync(receiveBuffer, receiveCancellationToken).ConfigureAwait(false);
                            await stream.WriteAsync(receiveBuffer.Array, receiveBuffer.Offset, receiveBuffer.Count);
                        } while (!webSocketReceiveResult.EndOfMessage);

                        var message = stream.ToArray().Where(b => b != 0).ToArray();
                        messageReceived(Encoding.ASCII.GetString(message, 0, message.Length));
                    }
                }
            }
        }

        public override bool Simulated
        {
            get { return _exchangeSettings.Simulate; } 
        }
    }
}
