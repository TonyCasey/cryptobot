using System.Collections.Generic;
using CryptoBot.Model.Common;
using CryptoBot.Model.Domain.Market;
using NLog;

namespace CryptoBot.SafetyEngine
{
    public class StopLoss : ISafetyEngine
    {
        private List<Candle> _candles;
        private readonly Logger _logger;

        public StopLoss()
        {
            _candles = new List<Candle>();
        }

        public StopLoss(List<Candle> candles, Logger logger)
        {
            _candles = candles;
            _logger = logger;
        }
        public Enumerations.SafetyPositionEnum GetSafetyPosition(Candle candle)
        {
            _candles.Add(candle);
            // TODO; Logic to figure out safety

            return Enumerations.SafetyPositionEnum.Ok;
        }
    }
}
