using System.Collections.Generic;
using CryptoBot.Model.Common;
using CryptoBot.Model.Domain.Bot;
using CryptoBot.Model.Domain.Market;
using NLog;

namespace CryptoBot.IndicatrorEngine
{
    public interface IIndicatorEngine
    {
        Enumerations.IndicatorSignalEnum GetIndicatorPosition(List<Candle> candles, List<RuleSet> ruleSets);
    }
}
