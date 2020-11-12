using CryptoBot.Model.Exchanges;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using CryptoBot.ExchangeEngine.API.Exchanges;
using CryptoBot.Model.Domain.Market;

namespace CryptoBot.ExchangeEngine.API.Services
{
    /// <summary>
    /// Contains functions to query cryptowatch API
    /// </summary>
    public class CryptowatchAPI : ExchangeBaseAPI
    {
        public override string BaseUrl { get; set; } = "https://api.cryptowat.ch";
        public override string Name => "Cryptowatch";

        private JToken MakeCryptowatchRequest(string subUrl)
        {
            JToken token = MakeJsonRequest<JToken>(subUrl);
            if (token["result"] == null)
            {
                throw new APIException("Unexpected result from API");
            }
            return token["result"];
        }

        /// <summary>
        /// Get market candles
        /// </summary>
        /// <param name="exchange">Exchange name</param>
        /// <param name="marketName">Market name</param>
        /// <param name="before">Optional date to restrict data to before this date</param>
        /// <param name="after">Optional date to restrict data to after this date</param>
        /// <param name="periods">Periods</param>
        /// <returns>Market candles</returns>
        public IEnumerable<Candle> GetMarketCandles(string exchange, string marketName, DateTime? before, DateTime? after, params int[] periods)
        {
            string periodString = string.Join(",", periods);
            string beforeDateString = (before == null ? string.Empty : "&before=" + (long)before.Value.UnixTimestampFromDateTimeSeconds());
            string afterDateString = (after == null ? string.Empty : "&after=" + (long)after.Value.UnixTimestampFromDateTimeSeconds());
            string url = "/markets/" + exchange + "/" + marketName + "/ohlc?periods=" + periodString + beforeDateString + afterDateString;
            JToken token = MakeCryptowatchRequest(url);
            foreach (JProperty prop in token)
            {
                foreach (JArray array in prop.Value)
                {
                    yield return new Candle
                    {
                        ExchangeName = exchange,
                        Name = marketName,
                        ClosePrice = (decimal)array[4],
                        Timestamp = CryptoUtility.UnixTimeStampToDateTimeSeconds((long)array[0]),
                        HighPrice = (decimal)array[2],
                        LowPrice = (decimal)array[3],
                        OpenPrice = (decimal)array[1],
                        PeriodSeconds = int.Parse(prop.Name),
                        VolumePrice = (double)array[5],
                        VolumeQuantity = (double)array[5] * (double)array[4]
                    };
                }
            }
        }

        /// <summary>
        /// Retrieve all market summaries
        /// </summary>
        /// <returns>Market summaries</returns>
        public IEnumerable<MarketSummary> GetMarketSummaries()
        {
            JToken token = MakeCryptowatchRequest("/markets/summaries");
            foreach (JProperty prop in token)
            {
                string[] pieces = prop.Name.Split(':');
                if (pieces.Length != 2)
                {
                    continue;
                }
                yield return new MarketSummary
                {
                    ExchangeName = pieces[0],
                    Name = pieces[1],
                    HighPrice = (decimal)prop.Value["price"]["high"],
                    LastPrice = (decimal)prop.Value["price"]["last"],
                    LowPrice = (decimal)prop.Value["price"]["low"],
                    PriceChangeAmount = (decimal)prop.Value["price"]["change"]["absolute"],
                    PriceChangePercent = (float)prop.Value["price"]["change"]["percentage"],
                    Volume = (double)prop.Value["volume"]
                };
            }
        }
    }
}
