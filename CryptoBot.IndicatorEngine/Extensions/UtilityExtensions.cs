namespace CryptoBot.IndicatrorEngine.Extensions
{
    public static class UtilityExtensions
    {
        /// <summary>
        /// Stops the scientific notation shortening of doubles
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string ToStringPreservePrecision(this double input)
        {
            return input.ToString("0." + new string('#', 339));
        }
    }
}
