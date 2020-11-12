using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CryptoBot.IndicatrorEngine.Model;
using CryptoBot.Model.Common;
using CryptoBot.Model.Domain.Bot;
using CryptoBot.Model.Domain.Market;
using Newtonsoft.Json;
using NLog;
using TicTacTec.TA.Library;

namespace CryptoBot.IndicatrorEngine.Stocastic
{
    public class Stocastic : IIndicatorEngine
    {
        private Logger _logger;

        public Stocastic(Logger logger)
        {
            _logger = logger;
        }

        public Enumerations.IndicatorSignalEnum GetIndicatorPosition(List<Candle> candles, List<RuleSet> ruleSets)
        {
            var inHigh = candles.Select(x => (float) x.HighPrice).ToArray();
            var inLow = candles.Select(x => (float) x.LowPrice).ToArray();
            var inClose = candles.Select(x => (float) x.ClosePrice).ToArray();

            int startIndex = 0, endIndex = inHigh.Length - 1;
            int outBegIndex = 0, outNbElement = 0;
            int optInFastKPeriod = 14, optInSlowKPeriod = 1, optInSlowDPeriod = 3;
            

            double[] outSlowK = new double[endIndex]; 
            double[] outSlowD = new double[endIndex];

            var response = TicTacTec.TA.Library.Core.Stoch(startIndex, endIndex, inHigh, inLow, inClose,
                optInFastKPeriod, optInSlowKPeriod, Core.MAType.Sma, optInSlowDPeriod, Core.MAType.Sma, out outBegIndex,
                out outNbElement, outSlowK, outSlowD);

            if (response != Core.RetCode.Success || (!outSlowK.Any() || outSlowK[0] <= 0) ) 
            {
                return Enumerations.IndicatorSignalEnum.None;
            }

            // Check for signals
            bool sell =
                CheckRuleSets(ruleSets.Where(x => x.RuleSide == IndicatorRulesEnumerations.RuleSideEnum.Sell), outSlowK[outSlowK.Length - (optInFastKPeriod+1) ], outSlowD[outSlowD.Length- (optInFastKPeriod+1)]);

            bool buy =
                CheckRuleSets(ruleSets.Where(x => x.RuleSide == IndicatorRulesEnumerations.RuleSideEnum.Buy), outSlowK[outSlowK.Length - (optInFastKPeriod+1)], outSlowD[outSlowD.Length - (optInFastKPeriod+1)]);

            if (sell)
                return Enumerations.IndicatorSignalEnum.Sell;

            if (buy)
                return Enumerations.IndicatorSignalEnum.Buy;


            return Enumerations.IndicatorSignalEnum.None;
        }

        private bool CheckRuleSets(IEnumerable<RuleSet> ruleSets, double slowK, double slowD)
        {
            int passedRuleSets = 0; // if one of the rulesets pass then we got a signal

            foreach (RuleSet ruleSet in ruleSets)
            {
                int passedRules = 0;

                List<IIndicatorResult> passedResults = new List<IIndicatorResult>();

                foreach (Rule rule in ruleSet.Rules)
                {
                    IIndicatorResult result = GetInidcatorResult(rule, slowK, slowD);

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

        private IIndicatorResult GetInidcatorResult(Rule rule, double slowK, double slowD)
        {
            switch ((IndicatorRulesEnumerations.OverBoughtSoldRulesEnum)rule.IndicatorRuleTypeId)
            {
                case IndicatorRulesEnumerations.OverBoughtSoldRulesEnum.IsOverBought:
                    
                    
                    if (Convert.ToDecimal(slowK) > rule.Value.GetValueOrDefault())
                    {
                        return new IndicatorResult
                        {
                            Message = $"IsOverBought - true slowK > rule.Value {slowK} > {rule.Value}",
                            Result = true,
                        };
                    }
                    else
                    {
                        return new IndicatorResult
                        {
                            Message = $"IsOverBought - false ",
                            Result = false,
                        };
                    }

                    

                case IndicatorRulesEnumerations.OverBoughtSoldRulesEnum.IsOverSold:

                    if (Convert.ToDecimal(slowK) < rule.Value.GetValueOrDefault())
                    {
                        return new IndicatorResult
                        {
                            Message = $"IsOverSold - true slowK < rule.Value {slowK} < {rule.Value}",
                            Result = true,
                        };
                    }
                    else
                    {
                        return new IndicatorResult
                        {
                            Message = $"IsOverSold - false",
                            Result = false,
                        };
                    }

                default:
                    return new IndicatorResult{Message = "Error", Result = false};
            }

        }


        private void Log(string message)
        {
            _logger.Info($"[StocasticIndicator] - {message}");
        }
    }
}
