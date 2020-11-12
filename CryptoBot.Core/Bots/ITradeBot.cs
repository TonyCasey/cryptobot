using System.Collections.Generic;
using CryptoBot.Model.Domain.Market;

namespace CryptoBot.Core.Bots
{
    public interface ITradeBot
    {
        void Start();
        void OnStart();
        void OnTick(Ticker ticker);
        /// <summary>
        /// On each candle, returns a boolean on whether to change position or not
        /// </summary>
        /// <param name="candle"></param>
        /// <returns>Change position?</returns>
        bool OnCandle(Candle candle);
        void OnStop();
        /// <summary>
        /// For some indicators like the MACD you need to have some preloaded data
        /// </summary>
        /// <param name="candles"></param>
        void PreLoadCandles(List<Candle> candles);
    }
}
