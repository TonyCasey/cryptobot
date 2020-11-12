using CryptoBot.Model.Common;
using CryptoBot.Model.Domain.Market;

namespace CryptoBot.SafetyEngine
{
    public interface ISafetyEngine
    {
        Enumerations.SafetyPositionEnum GetSafetyPosition(Candle candle);
    }
}
