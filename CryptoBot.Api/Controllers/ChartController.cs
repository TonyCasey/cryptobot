using Api.CryptoBot.Data;
using Api.CryptoBot.Models.DTO;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Data;
using System.Net;
using System.Threading.Tasks;
using Api.CryptoBot.Models.DTO.Chart;
using CryptoBot.Model.Domain;
using Microsoft.AspNetCore.Server.Kestrel.Internal.System.Text.Encodings.Web.Utf8;
using Newtonsoft.Json;
using System.IO;
using System.Linq;
using System.Reflection;
using Api.CryptoBot.Models.DTO.Chart.Entities;
using CryptoBot.Core.Utils;
using CryptoBot.ExchangeEngine;
using CryptoBot.ExchangeEngine.API.Exchanges;
using CryptoBot.Model.Common;
using CryptoBot.Model.Domain.Market;
using CryptoBot.Model.Exchanges;
using Microsoft.AspNetCore.Cors;
using CryptoUtility = CryptoBot.ExchangeEngine.API.Exchanges.CryptoUtility;

namespace Api.CryptoBot.Controllers
{
    /// <summary>
    /// Chart controller
    ///
    ///</summary>
    //[Microsoft.AspNetCore.Authorization.Authorize]
    [EnableCors("CorsPolicy")]
    [Produces("application/json")]
    [ApiVersion("1.0")]
    [Microsoft.AspNetCore.Mvc.Route("api/v{version:apiVersion}/[controller]")]
    public class ChartController : Controller
    {

        private readonly IMapper _mapper;
        private readonly CryptoBotApiDbContext _dbContext;
        private IExchangeApi _exchangeApi;

        /// <summary>
        /// Main constructor to inject mapper and IService
        ///
        /// </summary>
        /// <param name="mapper"></param>
        /// <param name="service"></param>
        public ChartController (IMapper mapper,  CryptoBotApiDbContext dbContext)
        {
            _mapper = mapper;
            _dbContext = dbContext;
        }


        [Microsoft.AspNetCore.Mvc.HttpGet("config")]
        public async Task<ConfigResponseDto> Config()
        {
            var json = System.IO.File.ReadAllText("D:\\Development\\CryptoBot\\CryptoBot.Api\\Data\\Sample\\crypto\\config.json");
            return JsonConvert.DeserializeObject<ConfigResponseDto>(json);
        }

        [Microsoft.AspNetCore.Mvc.HttpGet("symbols")]
        public async Task<SymbolsResponseDto> Symbols(string symbol)
        {
            if (symbol.Contains(":"))
                symbol = symbol.Split(':')[1];

            var json = System.IO.File.ReadAllText("D:\\Development\\CryptoBot\\CryptoBot.Api\\Data\\Sample\\crypto\\symbols.json");
            var searchResults = JsonConvert.DeserializeObject<IEnumerable<Symbol>>(json);

            return _mapper.Map(searchResults.FirstOrDefault(x => x.Name == symbol), new SymbolsResponseDto());
        }

        [Microsoft.AspNetCore.Mvc.HttpGet("symbol_info")]
        public async Task<SymbolInfoResponseDto> SymbolInfo(string group)
        {
            return new SymbolInfoResponseDto()
            {
                Symbol = new string[]{"AAPL", "MSFT", "SPX"},
                Description = new string[]{"Apple Inc", "Microsoft corp", "S&P 500 index"},
                Exchangelisted = "NYSE",
                Exchangetraded = "NYSE",
                Minmovement = 1,
                Minmovement2 = 2,
                Pricescale = new int[]{1, 1, 100},
                Hasdwm = true,
                Hasnovolume = new bool[]{false, false, true},
                Type = new string[]{"stock", "stock", "index"},
                Ticker = new string[]{"AAPL~0", "MSFT~0", "$SPX500"},
                Timezone = "America / New_York",
                Sessionregular = "0900 - 1600"
            };
        }

        [Microsoft.AspNetCore.Mvc.HttpGet("search")]
        public async Task<List<SymbolSearchResponseDto>> Search(string query, int limit, string exchange, string type)
        {
            // Need to convert this into SymbolInfo objects
            var json = System.IO.File.ReadAllText("D:\\Development\\CryptoBot\\CryptoBot.Api\\Data\\Sample\\crypto\\symbols.json");

            var searchResults = JsonConvert.DeserializeObject<IEnumerable<Symbol>>(json);
            if (query != null)
                searchResults = searchResults.Where(x => x.Name.Contains(query));

            List<SymbolSearchResponseDto> results = _mapper.Map(searchResults, new List<SymbolSearchResponseDto>());

            return results;

        }

        [Microsoft.AspNetCore.Mvc.HttpGet("history")]
        public async Task<HistoryResponseDto> History(string symbol, Int32 from, Int32 to, string resolution)
        {
            _exchangeApi = ExchangeFactory.GetExchangeApi(Enumerations.ExchangesEnum.Gdax, null);

            IEnumerable<Candle> candles =
                _exchangeApi.GetCandles(new Coin {Code = "EUR"}, new Coin {Code = "BTC"}, 60 * 60 * 24, UnixTimeStampToDateTime(from), UnixTimeStampToDateTime(to));

            candles = candles.OrderBy(x => x.Timestamp);

            //var asTable = ToDataTable(candles.ToList());
            
            var response = new HistoryResponseDto
            {
                TimeStamps = candles.Select(x =>  (long) CryptoUtility.UnixTimestampFromDateTimeSeconds(x.Timestamp)).ToArray(),
                Opens = candles.Select(x => x.OpenPrice).ToArray(), 
                Highs = candles.Select(x => x.HighPrice).ToArray(),
                Lows = candles.Select(x => x.LowPrice).ToArray(),
                Closes = candles.Select(x => x.ClosePrice).ToArray(),
                Volumes = candles.Select(x => Convert.ToDecimal(x.VolumeQuantity)).ToArray(),
                Status = "ok"
            };

            return response;

            //var json = System.IO.File.ReadAllText("D:\\Development\\CryptoBot\\CryptoBot.Api\\Data\\Sample\\history.json");
            //var result = JsonConvert.DeserializeObject<HistoryResponseDto>(json);
            //var fromDates = result.TimeStamps.Where(x => x > from);
            //var toDates = result.TimeStamps.Where(x => x < to);

            //if(!fromDates.Any() || !toDates.Any())
            //    return new HistoryResponseDto{ Status = "no_data"};

            //return JsonConvert.DeserializeObject<HistoryResponseDto>(json);
        }

        [Microsoft.AspNetCore.Mvc.HttpGet("marks")]
        public async Task<MarksResponseDto> Marks(string symbol, Int32 from, Int32 to, string resolution)
        {
            //var json = System.IO.File.ReadAllText("D:\\Development\\CryptoBot\\CryptoBot.Api\\Data\\Sample\\marks.json");
            //return JsonConvert.DeserializeObject<MarksResponseDto>(json);
            // Not in use at the moment
            return new MarksResponseDto();
        }

        [Microsoft.AspNetCore.Mvc.HttpGet("time")]
        public async Task<Int32> Time()
        {
            return (Int32)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
        }
        

        private DateTime UnixTimeStampToDateTime(long unixTimeStamp) 
        {
            System.DateTime dtDateTime = new System.DateTime(1970, 1, 1, 0, 0, 0, 0);
            dtDateTime = dtDateTime.AddSeconds(unixTimeStamp);
            return dtDateTime;
        }
        
    }

    
            
}
