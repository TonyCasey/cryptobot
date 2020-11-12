using System.Collections.Generic;
using System.Linq;
using CryptoBot.Model.Common;
using CryptoBot.Model.Domain.Market;

namespace CryptoBot.Core.Utils
{
    public static class DataParsing
    {
        public static List<Candle> GetCandles(List<Ticker> ticks, Enumerations.CandleSizeEnum candleSize, int candleCount)
        {
            List<Candle> candles = new List<Candle>();

            // Candles should be listed as newest to oldest

            // what date range is in the ohlcs?
            //DateTime ohlcStart = ticks.Min(x => x.Time);
            //DateTime ohlcEnd = ticks.Max(x => x.Time);

            //// whats the gap between each ohlc?
            //long ohlcTimeDiff = ((TimeSpan)(ticks.ElementAt(1).Time - ticks.ElementAt(2).Time)).Ticks / 10000000;

            //// how many Candles are in the ohlcs?
            //TimeSpan ohlcPeriod = ohlcEnd - ohlcStart;

            //long Candlecount = ticks.Count / (int)CandleSize;

            //Candle currentCandle = null;
            
            // on the assumption that the timepsan between ticks is 60 seconds

            for(var i = 0; i <= candleCount; i++)
            {
                List<Ticker> candleTicks = ticks.Skip(i * (int) candleSize).Take((int) candleSize).ToList();
                //Candles.Add(new Candle
                //{
                //    Start = CandleTicks.Min(x => x.Time),
                //    End = CandleTicks.Max(x => x.Time),
                //    Ohlc = new OHLC
                //    {
                //        High = CandleTicks.Max(x => x.Last),
                //        Low = CandleTicks.Min(x => x.Last),
                //        Open = CandleTicks.First().Last,
                //        Close = CandleTicks.Last().Last,
                //    },
                //   Volume = CandleTicks.Sum(x=>x.Volume)
                //});
            }
            

            return candles;
        }

        public static List<Candle> AdjustCandleSize(List<Candle> candles, Enumerations.CandleSizeEnum candleSize)
        {
            List<Candle> adjustedCandles = new List<Candle>();

            // Candles should be listed as newest to oldest
            //if (Candles.Any() && Candles.Count > 2)
            //{
            //    if (Candles.ElementAt(0).Start > Candles.ElementAt(1).Start)
            //    {
            //        Candles.Reverse();
            //    }
            //}
            //// what date range is in the current Candles?
            //DateTime start = Candles.Min(x => x.Start);
            //DateTime end = Candles.Max(x => x.End.GetValueOrDefault());

            //// whats the gap between each ohlc?
            //long CandleTimeDiff = ((TimeSpan)(Candles.ElementAt(1).Start - Candles.ElementAt(2).Start)).Ticks / 10000000;

            // what is the timespan of the current Candles?
            //TimeSpan CandlePeriod = start - end;

            long candleCount = candles.Count / (int)candleSize;                        

            for (var i = 0; i <= candleCount; i++)
            {
                List<Candle> tmpCandles = candles.Skip(i * (int)candleSize).Take((int)candleSize).ToList();

                if (tmpCandles.Any())
                {
                    //adjustedCandles.Add(new Candle
                    //{
                    //    Start = tmpCandles.Min(x => x.Start),
                    //    End = tmpCandles.Max(x => x.End),
                    //    Ohlc = new OHLC
                    //    {
                    //        High = tmpCandles.Max(x => x.Ohlc.High),
                    //        Low = tmpCandles.Min(x => x.Ohlc.Low),
                    //        Open = tmpCandles.First().Ohlc.Open,
                    //        Close = tmpCandles.Last().Ohlc.Close,
                    //    },
                    //    Volume = tmpCandles.Sum(x => x.Volume)
                    //});
                }
            }

            return adjustedCandles;
        }
    }
}
