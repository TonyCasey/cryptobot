using System;

namespace CryptoBot.Core.Utils
{
    public static class Calculations
    {
        /// <summary>
        /// Percentage firstdouble is of seconddouble
        /// </summary>
        /// <param name="figure"></param>
        /// <param name="asPercentageOfThisFigure"></param>
        /// <returns></returns>
        public static double Percentage(double figure, double asPercentOfThisFigure)
        {
            return Math.Round(((100 * figure) / asPercentOfThisFigure), 2);
        }
    }
}
