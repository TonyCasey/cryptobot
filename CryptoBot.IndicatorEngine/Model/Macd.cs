using CryptoBot.Model.Domain.Market;

namespace CryptoBot.IndicatrorEngine.Model
{
    public class Macd
    {
        public double MacdValue { get; set; }
        public double MacdSignalValue { get; set; }
        public double HistogramValue { get; set; }
        public Candle Candle { get; set; }
    }
}
