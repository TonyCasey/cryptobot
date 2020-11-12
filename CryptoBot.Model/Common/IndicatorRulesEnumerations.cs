using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CryptoBot.Model.Common
{
    public static class IndicatorRulesEnumerations
    {
        public enum RuleSideEnum
        {
            Buy =1,
            Sell
        }
        
        

        public enum MacdRulesEnum
        {
            /// <summary>
            /// Macd is crossing the signal line in an upward trend
            /// </summary>
            IsCrossingUp,
            /// <summary>
            /// Macd is crossing the signal line in an downward trend
            /// </summary>
            IsCrossingDown,
            IsHistogramBelowZero,
            IsHistogramTurningDown,
            IsHistogramTurningUp,
            IsMacdCrossingZeroUpwards,
            IsMacdCrossingZeroDownwards,
            IsMacdAboveZero,
            IsMacdBelowZero,
            /// <summary>
            /// make sure there is good momentum in the macd
            /// when the histogram is taller there is a bigger gap between macd & signal
            /// so it would suggest the price is rising at a speed
            /// </summary>
            IsHistogramAboveMinimumHeight,
            /// <summary>
            /// if it is becomming overbought
            /// </summary>
            IsHistogramBelowAMinimumHeight,
            /// <summary>
            /// indicates a bullish run
            /// </summary>
            IsMacdAboveSignal,
            /// <summary>
            /// indicates a bearish run
            /// </summary>
            IsMacdBelowSignal,
            /// <summary>
            /// could indicate a bearish change 
            /// </summary>
            IsMacdTurningDown,
            /// <summary>
            /// could indicate a bullish change
            /// </summary>
            IdMacdTurningUp,
            /// <summary>
            /// stops the bot from buying too high in case it was restarted
            /// </summary>
            IsMacdBelowAValue,

            IsMacdAboveValue,

            /// <summary>
            /// determines if a crossover has happend in the previous three candles
            /// </summary>
            HasMacdCrossedSignalUpwards

        }

        public enum VolumeRulesEnum
        {
            /// <summary>
            /// Is above a volume level
            /// </summary>
            IsAboveVolume,
            /// <summary>
            /// Is below a volume level
            /// </summary>
            IsBelowVolume
        }

        public enum CrossingRulesEnum
        {
            /// <summary>
            /// Crossing over
            /// </summary>
            IsCrossingOver,
            /// <summary>
            /// Crossing under
            /// </summary>
            IsCrossingUnder
        }

        public enum OverBoughtSoldRulesEnum
        {
            /// <summary>
            /// When over bought
            /// </summary>
            IsOverBought,
            /// <summary>
            /// When over sold
            /// </summary>
            IsOverSold
        }
    }
}
