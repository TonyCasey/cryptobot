using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using CryptoBot.IndicatrorEngine.Extensions;
using CryptoBot.IndicatrorEngine.Model;
using CryptoBot.Model.Common;
using CryptoBot.Model.Domain.Bot;
using CryptoBot.Model.Domain.Market;
using Newtonsoft.Json;
using NLog;
using TALib;

namespace CryptoBot.IndicatrorEngine.Macd
{
    public class MacdIndicator : IIndicatorEngine
    {
        private Logger _logger;

        public MacdIndicator(Logger logger)
        {
            _logger = logger;
        }

        public Enumerations.IndicatorSignalEnum GetIndicatorPosition(List<Candle> candles, List<RuleSet> ruleSets)
        {
            

            var closingPrices = candles.Select(x => (double) x.ClosePrice).ToArray();

            /**
             *  The initial values given by the MACD here always seem to be off.
             *  Give yourself plenty of previous candles so as it smooths out and catches up.
             *  They are very accurate once they do.
             */

            double[] outMacd = new double[closingPrices.Length];
            double[] outMacdSignal = new double[closingPrices.Length];
            double[] outMacdHistogram = new double[closingPrices.Length];
            int fastLength = 12, slowLength = 26, signalSmoothing = 9;

            var response = Functions.Macd(closingPrices.AsSpan(), new Range(0, closingPrices.Length), 
                outMacd.AsSpan(), outMacdSignal.AsSpan(), outMacdHistogram.AsSpan(), out var outRange, 
                fastLength, slowLength, signalSmoothing);
                
            int outBegIndex = outRange.Start.Value;
            int outNbElement = outRange.End.Value - outRange.Start.Value;            

            List<Model.Macd> macds = new List<Model.Macd>();

            if (response == Core.RetCode.Success)
            {
                if (candles.Any() && outBegIndex > 0 && candles.Count() > outBegIndex)
                {
                    int index = candles.Count() - 1 - outBegIndex;

                    for (int i = 0; i <= index; i++)
                    {
                        macds.Add(new Model.Macd
                        {
                            MacdSignalValue = outMacdSignal[i],
                            MacdValue = outMacd[i],
                            HistogramValue = outMacdHistogram[i],
                            Candle = candles.ElementAt(outBegIndex + i)
                        });
                    }

                }

            }


            if (macds.Any())
            {
                // Check for signals
                bool sell =
                    CheckRuleSets(ruleSets.Where(x => x.RuleSide == IndicatorRulesEnumerations.RuleSideEnum.Sell), macds);

                bool buy =
                    CheckRuleSets(ruleSets.Where(x => x.RuleSide == IndicatorRulesEnumerations.RuleSideEnum.Buy), macds);

                if (sell)
                    return Enumerations.IndicatorSignalEnum.Sell;

                if (buy)
                    return Enumerations.IndicatorSignalEnum.Buy;

            }

            return Enumerations.IndicatorSignalEnum.None;
        }

        


        private MacdIndicatorResult GetInidcatorResult(Rule rule,  List<Model.Macd> macds)
        {

            switch ((IndicatorRulesEnumerations.MacdRulesEnum)rule.IndicatorRuleTypeId)
            {
                case IndicatorRulesEnumerations.MacdRulesEnum.IsMacdAboveSignal:
                    return macds.IsMacdAboveSignal();

                case IndicatorRulesEnumerations.MacdRulesEnum.IsHistogramAboveMinimumHeight:
                    return macds.IsHistogramAboveMinimumHeight(Convert.ToDouble(rule.Value));

                case IndicatorRulesEnumerations.MacdRulesEnum.IsHistogramBelowAMinimumHeight:
                    return macds.IsHistogramBelowAMinimumHeight(Convert.ToDouble(rule.Value));

                case IndicatorRulesEnumerations.MacdRulesEnum.IsCrossingDown:
                    return macds.IsCrossingDown();

                case IndicatorRulesEnumerations.MacdRulesEnum.IsCrossingUp:
                    return macds.IsCrossingUp();

                case IndicatorRulesEnumerations.MacdRulesEnum.IsMacdBelowZero:
                    return macds.IsMacdBelowZero();

                case IndicatorRulesEnumerations.MacdRulesEnum.IsMacdTurningDown:
                    return macds.IsMacdTurningDown();
                    
                case IndicatorRulesEnumerations.MacdRulesEnum.IsHistogramTurningDown:
                    return macds.IsHistogramTurningDown();
                    
                case IndicatorRulesEnumerations.MacdRulesEnum.IsMacdBelowSignal:
                    return macds.IsMacdBelowSignal();

                case IndicatorRulesEnumerations.MacdRulesEnum.IsMacdBelowAValue:
                    return macds.IsMacdBelowAValue(Convert.ToDouble(rule.Value));

                case IndicatorRulesEnumerations.MacdRulesEnum.IsMacdAboveValue:
                    return macds.IsMacdAboveValue(Convert.ToDouble(rule.Value));

                case IndicatorRulesEnumerations.MacdRulesEnum.HasMacdCrossedSignalUpwards:
                    return macds.HasMacdCrossedSignalUpwards();


                default:
                    return new MacdIndicatorResult{Message = "Error", Result = false};
            }
            
        }

       

        public bool CheckRuleSets(IEnumerable<RuleSet> ruleSets, List<Model.Macd> macds )
        {
            int passedRuleSets = 0; // if one of the rulesets pass then we got a signal

            foreach (RuleSet ruleSet in ruleSets)
            {
                int passedRules = 0;

                List<IIndicatorResult> passedResults = new List<IIndicatorResult>();

                foreach (Rule rule in ruleSet.Rules)
                {
                    IIndicatorResult result = GetInidcatorResult(rule, macds);

                    if (result.Result)
                    {
                        passedResults.Add(result);
                        ++passedRules;
                    }
                }

                if (passedRules == ruleSet.Rules.Count)
                {
                    _logger.Info("");
                    ++passedRuleSets;
                    
                    Log($"All rules for {ruleSet.RuleSide.ToString().ToUpper()} ruleset passed { ruleSet.Indicator.IndicatorType.ToString() }, {ruleSet.Indicator.Bot.Name} ..");

                    foreach (IIndicatorResult indicatorResult in passedResults)
                    {
                        Log(indicatorResult.Message);
                    }
                    _logger.Info("");
                }
                    
            }

            return passedRuleSets > 0;
        }

        private void Log(string message)
        {
            _logger.Info($"[MacdIndicator] - {message}");
        }

    }
    
}
