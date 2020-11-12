using CryptoBot.IndicatrorEngine.Crossing;
using CryptoBot.IndicatrorEngine.Macd;
using CryptoBot.IndicatrorEngine.Volume;
using CryptoBot.Model.Common;
using NLog;

namespace CryptoBot.IndicatrorEngine
{
    public static class IndicatorFactory
    {
        public static IIndicatorEngine GetIndicator(Enumerations.IndicatorTypeEnum indicatorType, Logger logger)
        {
            switch (indicatorType)
            {
                case Enumerations.IndicatorTypeEnum.Macd:
                    return new MacdIndicator(logger);
                case Enumerations.IndicatorTypeEnum.Volume:
                    return new VolumeIndicator(logger);
                case Enumerations.IndicatorTypeEnum.CloseOpenCrossing:
                    return new CloseOpenCrossIndicator(logger);
                case Enumerations.IndicatorTypeEnum.Stocastic:
                    return new Stocastic.Stocastic(logger);
                default:
                    return null;
            }
        }
    }
}
