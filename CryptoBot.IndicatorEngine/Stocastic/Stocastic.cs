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
using TALib;

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
            var highs = candles.Select(x => (double)x.HighPrice).ToArray();
            var lows = candles.Select(x => (double)x.LowPrice).ToArray();
            var closes = candles.Select(x => (double)x.ClosePrice).ToArray();

            var inHigh = new ReadOnlySpan<double>(highs);
            var inLow = new ReadOnlySpan<double>(lows);
            var inClose = new ReadOnlySpan<double>(closes);
            var inRange = Range.EndAt(closes.Length);

            int optInFastKPeriod = 14, optInSlowKPeriod = 1, optInSlowDPeriod = 3;
            
            var outSlowK = new double[closes.Length].AsSpan();
            var outSlowD = new double[closes.Length].AsSpan();

            var response = Functions.Stoch(inHigh, inLow, inClose, inRange, outSlowK, outSlowD,
                out Range outRange, optInFastKPeriod, optInSlowKPeriod, Core.MAType.Sma, optInSlowDPeriod, Core.MAType.Sma);

            if (response != Core.RetCode.Success || outRange.End.Value <= outRange.Start.Value) 
            {
                return Enumerations.IndicatorSignalEnum.None;
            }

            var length = outRange.End.Value - outRange.Start.Value;
            if (length == 0)
            {
                return Enumerations.IndicatorSignalEnum.None;
            }

            // Get the last values for comparison
            var lastSlowK = outSlowK[length - 1];
            var lastSlowD = outSlowD[length - 1];

            // Check for signals
            bool sell = CheckRuleSets(ruleSets.Where(x => x.RuleSide == IndicatorRulesEnumerations.RuleSideEnum.Sell), lastSlowK, lastSlowD);
            bool buy = CheckRuleSets(ruleSets.Where(x => x.RuleSide == IndicatorRulesEnumerations.RuleSideEnum.Buy), lastSlowK, lastSlowD);

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
