using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CryptoBot.IndicatrorEngine.Model;
using CryptoBot.Model.Common;
using CryptoBot.Model.Domain.Bot;
using CryptoBot.Model.Domain.Market;
using NLog;

namespace CryptoBot.IndicatrorEngine.Volume
{
    public class VolumeIndicator : IIndicatorEngine
    {
        private Logger _logger;

        public VolumeIndicator(Logger logger)
        {
            _logger = logger;
        }

        public Enumerations.IndicatorSignalEnum GetIndicatorPosition(List<Candle> candles, List<RuleSet> ruleSets)
        {
            bool sell =
                CheckRuleSets(ruleSets.Where(x => x.RuleSide == IndicatorRulesEnumerations.RuleSideEnum.Sell), candles);

            bool buy =
                CheckRuleSets(ruleSets.Where(x => x.RuleSide == IndicatorRulesEnumerations.RuleSideEnum.Buy), candles);

            if (sell)
                return Enumerations.IndicatorSignalEnum.Sell;

            if (buy)
                return Enumerations.IndicatorSignalEnum.Buy;

            return Enumerations.IndicatorSignalEnum.None;
        }

        public bool CheckRuleSets(IEnumerable<RuleSet> ruleSets, List<Candle> candles)
        {
            int passedRuleSets = 0; // if one of the rulesets pass then we got a signal

            foreach (RuleSet ruleSet in ruleSets)
            {
                int passedRules = 0;

                List<IIndicatorResult> passedResults = new List<IIndicatorResult>();

                foreach (Rule rule in ruleSet.Rules)
                {
                    IndicatorResult result = GetInidcatorResult(rule, candles);

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

        private IndicatorResult GetInidcatorResult(Rule rule, List<Candle> candles)
        {
            switch ((IndicatorRulesEnumerations.VolumeRulesEnum)rule.IndicatorRuleTypeId)
            {
                case IndicatorRulesEnumerations.VolumeRulesEnum.IsAboveVolume:

                        return new IndicatorResult { Message = $"Is above volume is {candles.LastOrDefault().VolumeQuantity} > {rule.Value}", Result = candles.LastOrDefault().VolumeQuantity > (double)rule.Value };
                    
                case IndicatorRulesEnumerations.VolumeRulesEnum.IsBelowVolume:

                    return new IndicatorResult { Message = $"Is below Volume  {candles.LastOrDefault().VolumeQuantity} < {rule.Value}", Result = candles.LastOrDefault().VolumeQuantity < (double)rule.Value };

                default:
                    return new IndicatorResult { Message = "Error", Result = false };

            }
        }

        private void Log(string message)
        {
            _logger.Info($"[VolumeIndicator] - {message}");
        }
    }


}
