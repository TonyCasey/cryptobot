using CryptoBot.Model.Domain.Market;

namespace CryptoBot.Model.Extensions
{
    public static class CandleExtensions
    {
        public static bool IsBullish(this Candle candle)
        {
            return candle.ClosePrice > candle.OpenPrice;
        }

        public static bool IsBearish(this Candle candle)
        {
            return candle.ClosePrice < candle.OpenPrice;
        }
    }
}
