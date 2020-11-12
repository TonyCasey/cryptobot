using System.Collections.Generic;
using System.Linq;
using CryptoBot.IndicatrorEngine.Model;
using CryptoBot.Model.Common;
using CryptoBot.Model.Domain.Bot;
using CryptoBot.Model.Domain.Market;
using CryptoBot.Model.Extensions;
using Newtonsoft.Json;
using NLog;

namespace CryptoBot.IndicatrorEngine.Crossing
{
    public class CloseOpenCrossIndicator : IIndicatorEngine
    {
        private readonly Logger _logger;
        private List<Candle> _groupedCandles;

        public CloseOpenCrossIndicator(Logger logger)
        {
            _logger = logger;
        }

        public Enumerations.IndicatorSignalEnum GetIndicatorPosition(List<Candle> candles, List<RuleSet> ruleSets)
        {
            if (candles.Count > 1) // we need to compare the last two 
            {
                // Check for signals
                bool sell =
                    CheckRuleSets(ruleSets.Where(x => x.RuleSide == IndicatorRulesEnumerations.RuleSideEnum.Sell), candles);

                bool buy =
                    CheckRuleSets(ruleSets.Where(x => x.RuleSide == IndicatorRulesEnumerations.RuleSideEnum.Buy), candles);

                if (sell)
                    return Enumerations.IndicatorSignalEnum.Sell;

                if (buy)
                    return Enumerations.IndicatorSignalEnum.Buy;
            }

            return Enumerations.IndicatorSignalEnum.None;
        }

        private bool CheckRuleSets(IEnumerable<RuleSet> ruleSets, List<Candle> candles)
        {
            int passedRuleSets = 0; // if one of the rulesets pass then we got a signal

            foreach (RuleSet ruleSet in ruleSets)
            {
                int passedRules = 0;

                List<IIndicatorResult> passedResults = new List<IIndicatorResult>();

                foreach (Rule rule in ruleSet.Rules)
                {
                    IIndicatorResult result = GetInidcatorResult(rule, candles);

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

        private IIndicatorResult GetInidcatorResult(Rule rule, List<Candle> candles)
        {
            
            

            // period is the minutes between when the crossing test should be performed
            // tests on tradingview with bitcoin return best results using a 30 min space on a 5 min chart
            decimal period = rule.Value.GetValueOrDefault(30);
            

            if( candles.Count < 19 || (candles.Last().Timestamp.Minute != 0 && candles.Last().Timestamp.Minute % period != 0))
                return new IndicatorResult
                {
                    Message = $"IsCrossingOver - within period",
                    Result = false,
                };


            Candle lastCandle = candles.ElementAtOrDefault(candles.Count - 1);
            Candle secondLastCandle = candles.ElementAtOrDefault(candles.Count - 2);
            
         

            switch ((IndicatorRulesEnumerations.CrossingRulesEnum)rule.IndicatorRuleTypeId)
            {
                case IndicatorRulesEnumerations.CrossingRulesEnum.IsCrossingOver:
                    
                    
                    if (lastCandle.IsBullish() && secondLastCandle.IsBearish())
                    {
                        return new IndicatorResult
                        {
                            Message = $"IsCrossingOver - true lastCandle.IsBullish() && secondLastCandle.IsBearish() {JsonConvert.SerializeObject(lastCandle)} && {JsonConvert.SerializeObject(secondLastCandle)}",
                            Result = true,
                        };
                    }
                    else
                    {
                        return new IndicatorResult
                        {
                            Message = $"IsCrossingOver - false lastCandle.IsBullish() && secondLastCandle.IsBearish() {JsonConvert.SerializeObject(lastCandle)} && {JsonConvert.SerializeObject(secondLastCandle)}",
                            Result = false,
                        };
                    }

                    

                case IndicatorRulesEnumerations.CrossingRulesEnum.IsCrossingUnder:


                    // To check as having crossed under the last bar close is less than the last bar open 
                    // and it was the opposite on the bar immediately preceding it.

                    if (lastCandle.IsBearish() && secondLastCandle.IsBullish())
                    {
                        return new IndicatorResult
                        {
                            Message = $"IsCrossingUnder - true lastCandle.IsBearish() && secondLastCandle.IsBullish() {JsonConvert.SerializeObject(lastCandle)} && {JsonConvert.SerializeObject(secondLastCandle)}",
                            Result = true,
                        };
                    }
                    else
                    {
                        return new IndicatorResult
                        {
                            Message = $"IsCrossingUnder - false lastCandle.IsBearish() && secondLastCandle.IsBullish() {JsonConvert.SerializeObject(lastCandle)} && {JsonConvert.SerializeObject(secondLastCandle)}",
                            Result = false,
                        };
                    }

                default:
                    return new IndicatorResult{Message = "Error", Result = false};
            }
        }

        private void Log(string message)
        {
            _logger.Info($"[CrossIndicator] - {message}");
        }

    }
}
