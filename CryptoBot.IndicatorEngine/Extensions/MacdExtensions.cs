using System;
using System.Collections.Generic;
using System.Linq;
using CryptoBot.IndicatrorEngine.Model;

namespace CryptoBot.IndicatrorEngine.Extensions
{
    public static class MacdExtensions
    {
        public static MacdIndicatorResult IsCrossingUp(this List<Model.Macd> macds)
        {
            // Macd will cross signal, compare the last two records

            if (macds.Count() < 2)
                return new MacdIndicatorResult{ Message = "IsCrossingUp macds.Count < 2", Result = false };

            double secondLastMacd = macds.ElementAt(macds.Count() - 2).MacdValue;
            double lastMacd = macds.ElementAt(macds.Count() - 1).MacdValue;

            double secondLastSignal = macds.ElementAt(macds.Count() - 2).MacdSignalValue;
            double lastSignal = macds.ElementAt(macds.Count() - 1).MacdSignalValue;

            bool result = (secondLastMacd < secondLastSignal && lastMacd > lastSignal);

            if (result)
            {
                return new MacdIndicatorResult
                {
                    Message =
                        $"IsCrossingUp - secondLastMacd {secondLastMacd.ToStringPreservePrecision()} < secondLastSignal {secondLastSignal.ToStringPreservePrecision()} && lastMacd {lastMacd.ToStringPreservePrecision()} > lastSignal {lastSignal.ToStringPreservePrecision()}",
                    Result = true
                };
            }
            else
            {
                return new MacdIndicatorResult
                {
                    Message = "IsCrossingUp - false",
                    Result = false
                };
            }

        }

        public static MacdIndicatorResult IsCrossingDown(this List<Model.Macd> macds)
        {
            // Macd will cross signal, compare the last two records

            if (macds.Count() < 2)
                return new MacdIndicatorResult { Message = "IsCrossingDown macds.Count < 2", Result = false };

            double secondLastMacd = macds.ElementAt(macds.Count() - 2).MacdValue;
            double lastMacd = macds.ElementAt(macds.Count() - 1).MacdValue;

            double secondLastSignal = macds.ElementAt(macds.Count() - 2).MacdSignalValue;
            double lastSignal = macds.ElementAt(macds.Count() - 1).MacdSignalValue;

            bool result = (secondLastMacd > secondLastSignal && lastMacd < lastSignal) || (secondLastMacd == lastSignal);

            if (result)
            {
                return new MacdIndicatorResult
                {
                    Message =
                        $"IsCrossingDown - (secondLastMacd {secondLastMacd.ToStringPreservePrecision()} > secondLastSignal {secondLastSignal.ToStringPreservePrecision()} && lastMacd {lastMacd.ToStringPreservePrecision()} < lastSignal {lastSignal.ToStringPreservePrecision()}) || (secondLastMacd {secondLastMacd.ToStringPreservePrecision()} == lastSignal {lastSignal.ToStringPreservePrecision()})",
                    Result = true

                };
            }
            else
            {
                return new MacdIndicatorResult
                {
                    Message = "IsCrossingDown - false",
                    Result = false
                };
            }

        }

        public static MacdIndicatorResult IsMacdBelowZero(this List<Model.Macd> macds)
        {
            bool result = macds.Any() && macds.Last().MacdValue < 0;

            if (result)
            {
                return new MacdIndicatorResult
                {
                    Message =
                        $"IsMacdBelowZero - macds.Last().MacdValue {macds.Last().MacdValue.ToStringPreservePrecision()} < 0 ",
                    Result = true

                };
            }
            else
            {
                return new MacdIndicatorResult
                {
                    Message = "IsMacdBelowZero - false",
                    Result = false
                };
            }

        }

        public static MacdIndicatorResult IsMacdTurningDown(this List<Model.Macd> macds)
        {
            // Macd will cross signal, compare the last two records

            if (macds.Count() < 2)
                return new MacdIndicatorResult { Message = "IsMacdTurningDown macds.Count < 2", Result = false };

            double secondLastMacd = macds.ElementAt(macds.Count() - 2).MacdValue;
            double lastMacd = macds.ElementAt(macds.Count() - 1).MacdValue;
            

            bool result = lastMacd < secondLastMacd;

            if (result)
            {
                return new MacdIndicatorResult
                {
                    Message =
                        $"IsMacdTurningDown - lastMacd < secondLastMacd {lastMacd.ToStringPreservePrecision()} < {secondLastMacd.ToStringPreservePrecision()}",
                    Result = true

                };
            }
            else
            {
                return new MacdIndicatorResult
                {
                    Message = "IsMacdTurningDown - false",
                    Result = false
                };
            }

        }

        public static MacdIndicatorResult IsHistogramTurningDown(this List<Model.Macd> macds)
        {
            if (macds.Count() < 2)
                return new MacdIndicatorResult { Message = "IsHistogramTurningDown macds.Count < 2", Result = false };

            double seconfLastHistogram = macds.ElementAt(macds.Count() - 2).HistogramValue;
            double lastHistogram = macds.ElementAt(macds.Count() - 1).HistogramValue;

            bool result = lastHistogram < seconfLastHistogram;

            if (result)
            {
                return new MacdIndicatorResult
                {
                    Message =
                        $"IsHistogramTurningDown - lastHistogram < seconfLastHistogram {lastHistogram.ToStringPreservePrecision()} < {seconfLastHistogram.ToStringPreservePrecision()}",
                    Result = true

                };
            }
            else
            {
                return new MacdIndicatorResult
                {
                    Message = "IsHistogramTurningDown - false",
                    Result = false
                };
            }
        }
        
        public static MacdIndicatorResult IsHistogramAboveMinimumHeight(this List<Model.Macd> macds, double height)
        {
            bool result = macds.Any() && macds.Last().HistogramValue > height;

            if (result)
            {
                return new MacdIndicatorResult
                {
                    Message =
                        $"IsHistogramAboveMinimumHeight - macds.Last().HistogramValue { macds.Last().HistogramValue.ToStringPreservePrecision() } > height {height.ToStringPreservePrecision()}",
                    Result = true

                };
            }
            else
            {
                return new MacdIndicatorResult
                {
                    Message = "IsHistogramAboveMinimumHeight - false",
                    Result = false
                };
            }
            
        }

        public static MacdIndicatorResult IsHistogramBelowAMinimumHeight(this List<Model.Macd> macds, double height)
        {
            bool result = macds.Any() && macds.Last().HistogramValue < height;

            if (result)
            {
                return new MacdIndicatorResult
                {
                    Message =
                        $"IsHistogramBelowAMinimumHeight - macds.Last().HistogramValue { macds.Last().HistogramValue.ToStringPreservePrecision() } < height {height.ToStringPreservePrecision()}",
                    Result = true

                };
            }
            else
            {
                return new MacdIndicatorResult
                {
                    Message = "IsHistogramBelowAMinimumHeight - false",
                    Result = false
                };
            }

        }

        public static MacdIndicatorResult IsMacdAboveSignal(this List<Model.Macd> macds)
        {
            // Macd will cross signal, compare the last two records

            if (macds.Count < 2)
                return new MacdIndicatorResult { Message = "IsMacdAboveSignal macds.Count < 2", Result = false }; ;
            
            double lastMacd = macds.Last().MacdValue;
            
            double lastSignal = macds.Last().MacdSignalValue;
            

            bool result = lastMacd > lastSignal;

            if (result)
            {
                return new MacdIndicatorResult
                {
                    Message =
                        $"IsMacdAboveSignal - lastMacd {lastMacd.ToStringPreservePrecision()} > lastSignal {lastSignal.ToStringPreservePrecision()}",
                    Result = true

                };
            }
            else
            {
                return new MacdIndicatorResult
                {
                    Message = "IsMacdAboveSignal - false",
                    Result = false
                };
            }

        }

        public static MacdIndicatorResult IsMacdBelowSignal(this List<Model.Macd> macds)
        {
            // Macd will cross signal, compare the last two records

            if (macds.Count < 2)
                return new MacdIndicatorResult { Message = "IsMacdBelowSignal macds.Count < 2", Result = false }; ;

            double lastMacd = macds.Last().MacdValue;

            double lastSignal = macds.Last().MacdSignalValue;


            bool result = lastMacd < lastSignal;

            if (result)
            {
                return new MacdIndicatorResult
                {
                    Message =
                        $"IsMacdBelowSignal - lastMacd {lastMacd.ToStringPreservePrecision()} < lastSignal {lastSignal.ToStringPreservePrecision()}",
                    Result = true

                };
            }
            else
            {
                return new MacdIndicatorResult
                {
                    Message = "IsMacdBelowSignal - false",
                    Result = false
                };
            }

        }

        public static MacdIndicatorResult IsMacdBelowAValue(this List<Model.Macd> macds, double value)
        {

            if(macds.Count < 2)
                return new MacdIndicatorResult { Message = "IsMacdBelowAValue no macds yet", Result = false };
            
            double lastMacd = macds.Last().MacdValue;

            bool result = lastMacd < value;

            if (result)
            {
                return new MacdIndicatorResult
                {
                    Message =
                        $"IsMacdBelowAValue - lastMacd < value {lastMacd.ToStringPreservePrecision()} < {value.ToStringPreservePrecision()}",
                    Result = true

                };
            }
            else
            {
                return new MacdIndicatorResult
                {
                    Message = "IsMacdBelowAValue - false",
                    Result = false
                };
            }

        }

        public static MacdIndicatorResult IsMacdAboveValue(this List<Model.Macd> macds, double value)
        {
            bool result = macds.Any() && macds.Last().MacdValue > value;

            if (result)
            {
                return new MacdIndicatorResult
                {
                    Message =
                        $"IsMacdAboveValue - macds.Last().MacdValue {macds.Last().MacdValue.ToStringPreservePrecision() } > value {value.ToStringPreservePrecision()}",
                    Result = true
                };
            }
            else
            {
                return new MacdIndicatorResult
                {
                    Message = "IsMacdAboveValue - false",
                    Result = false
                };
            }

        }

        public static MacdIndicatorResult HasMacdCrossedSignalUpwards(this List<Model.Macd> macds)
        {
            // Macd will cross signal, compare the last 6 records

            if (macds.Count < 4)
                return new MacdIndicatorResult { Message = "HasMacdCrossedSignalUpwards macds.Count < 4", Result = false }; ;

            // the current macd needs to be greater than the signal
            // but 1 or two candles previous it would need to have been below it

            double lastMacd = macds.Last().MacdValue;

            double lastSignal = macds.Last().MacdSignalValue;

            double secondLastMacd = macds.ElementAt(macds.Count()-2).MacdValue;

            double secondLastSignal = macds.ElementAt(macds.Count() - 2).MacdSignalValue;

            double thirdLastMacd = macds.ElementAt(macds.Count() - 3).MacdValue;

            double thirdLastSignal = macds.ElementAt(macds.Count() - 3).MacdSignalValue;

            double fourthLastMacd = macds.ElementAt(macds.Count() - 4).MacdValue;

            double fourthLastSignal = macds.ElementAt(macds.Count() - 4).MacdSignalValue;

            bool result = lastMacd > lastSignal && (secondLastMacd < secondLastSignal || thirdLastMacd < thirdLastSignal || fourthLastMacd < fourthLastSignal);

            if (result)
            {
                return new MacdIndicatorResult
                {
                    Message =
                        $"HasMacdCrossedSignalUpwards - lastMacd {lastMacd.ToStringPreservePrecision()} > lastSignal {lastSignal.ToStringPreservePrecision()} && (secondLastMacd < secondLastSignal {secondLastMacd.ToStringPreservePrecision()} < {secondLastSignal.ToStringPreservePrecision()} {secondLastMacd < secondLastSignal} || thirdLastMacd < thirdLastSignal {thirdLastMacd.ToStringPreservePrecision()} < {thirdLastSignal.ToStringPreservePrecision()} {thirdLastMacd < thirdLastSignal})",
                    Result = true

                };
            }
            else
            {
                return new MacdIndicatorResult
                {
                    Message = "HasMacdCrossedSignalUpwards - false",
                    Result = false
                };
            }

        }
    }
}
