using System;
using System.Net;
using CryptoBot.Model.Common;
using CryptoBot.Model.Domain.Market;
using Newtonsoft.Json.Linq;
using NLog;
using RestSharp;

namespace CryptoBot.ExchangeEngine.API.Services
{
    public class CoinigyAPI : ExchangeBaseAPI
    {
        private Logger _logger = LogManager.GetCurrentClassLogger();

        private const string _apiUrl = "https://www.coinigy.com";
        private RestClient _restClient;


        public CoinigyAPI()
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            _restClient = new RestClient(_apiUrl);
        }

        public async System.Threading.Tasks.Task<JArray> GetChartFeedAsync(Exchange exchange, Coin baseCoin, Coin coin, Enumerations.CandleSizeEnum candleSize, long fromDateTimeUnixTimestamp,
            long toDateTimeUnixTimeStamp)
        {

            string url =
            $"/getjson/chart_feed/{exchange.Code}/{coin.Code}/{baseCoin.Code}/{(int)candleSize}/{fromDateTimeUnixTimestamp}/{toDateTimeUnixTimeStamp}";

            var request = new RestRequest(url, Method.Get);
      
            try
            {
                var response = await _restClient.ExecuteAsync(request);

                return JArray.Parse(response.Content);
            }
            catch (Exception e)
            {
                _logger.Error(e);

                return null;
            }
            
        }

        public override string BaseUrl { get; set; }
        public override string Name { get; }
    }
}
