using CryptoBot.Model.Common;

namespace CryptoBot.SafetyEngine
{
    public static class SafetyFactory
    {
        public static ISafetyEngine GetSafety(Enumerations.SafetyTypeEnum safetyType)
        {
            switch (safetyType)
            {
                case Enumerations.SafetyTypeEnum.StopLoss:
                    return new StopLoss();
                default:
                    return null;
            }
        }
    }
}
