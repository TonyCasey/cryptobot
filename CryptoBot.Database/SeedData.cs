using CryptoBot.Model.Common;
using CryptoBot.Model.Domain.Account;
using CryptoBot.Model.Domain.Bot;
using CryptoBot.Model.Domain.Lookup;
using CryptoBot.Model.Domain.Market;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;

namespace CryptoBot.Database
{
    public static class SeedData
    {
        
        public static void Seed(CryptoBotDbContext context)
        {
           
            //  This method will be called after migrating to the latest version.
            AddEnumerations(context);
            AddExchanges(context);
            AddCoins(context);
            AddUsers(context);
            AddBots(context);
            AddApiSettings(context);
            AddMessagingApps(context);
        }

        private static void AddEnumerations(CryptoBotDbContext context)
        {
            List<Enumeration> enumerations = new List<Enumeration>();

            foreach (int i in Enum.GetValues(typeof(IndicatorRulesEnumerations.MacdRulesEnum)))
            {
                String name = Enum.GetName(typeof(IndicatorRulesEnumerations.MacdRulesEnum), i);

                enumerations.Add(new Enumeration
                {
                    Name = name,
                    Value = i,
                    Group = "MacdRulesEnum"
                });
            }

            foreach (int i in Enum.GetValues(typeof(IndicatorRulesEnumerations.VolumeRulesEnum)))
            {
                String name = Enum.GetName(typeof(IndicatorRulesEnumerations.VolumeRulesEnum), i);

                enumerations.Add(new Enumeration
                {
                    Name = name,
                    Value = i,
                    Group = "VolumeRulesEnum"
                });
            }

            foreach (int i in Enum.GetValues(typeof(IndicatorRulesEnumerations.OverBoughtSoldRulesEnum)))
            {
                String name = Enum.GetName(typeof(IndicatorRulesEnumerations.OverBoughtSoldRulesEnum), i);

                enumerations.Add(new Enumeration
                {
                    Name = name,
                    Value = i,
                    Group = "OverBoughtSoldRulesEnum"
                });
            }

            foreach (Enumeration enumeration in enumerations)
            {
                if (!context.Enumerations.Any(x => x.Group == enumeration.Group && x.Name == enumeration.Name))
                {
                    context.Enumerations.Add(enumeration);
                }
               
            }
            context.SaveChanges();
        }

        private static void AddExchanges(CryptoBotDbContext context)
        {
            if (!context.Exchanges.Any())
            {
                context.Exchanges.Add(new Exchange
                {
                    Name = "Bittrex",
                    Code = "BTRX"
                });
                context.Exchanges.Add(new Exchange
                {
                    Name = "HitBTC",
                    Code = "HITB"
                });
                context.Exchanges.Add(new Exchange
                {
                    Name = "GDAX",
                    Code = "GDAX"
                });
                context.SaveChanges();
            }
        }
        
        private static void AddCoins(CryptoBotDbContext context)
        {

            List<Coin> coins = new List<Coin>();


            coins.Add(new Coin
            {
                Code = "BTC",
                OrderRoundingExponent = 8
            });
            coins.Add(new Coin
            {
                Code = "ETH"
            });
            coins.Add(new Coin
            {
                Code = "LTC"
            });
            coins.Add(new Coin
            {
                Code = "BAS"
            });
            coins.Add(new Coin
            {
                Code = "USDT",
                OrderRoundingExponent = 2
            });
            coins.Add(new Coin
            {
                Code = "WAVES"
            });
            coins.Add(new Coin
            {
                Code = "PAY"
            });
            coins.Add(new Coin
            {
                Code = "BAT"
            });
            coins.Add(new Coin
            {
                Code = "MCO"
            });
            coins.Add(new Coin
            {
                Code = "NEO"
            });
            coins.Add(new Coin
            {
                Code = "EUR",
                OrderRoundingExponent = 2
            });
            coins.Add(new Coin
            {
                Code = "ETC",
                OrderRoundingExponent = 8
            });
            coins.Add(new Coin
            {
                Code = "XMR",
                OrderRoundingExponent = 8
            });
            coins.Add(new Coin
            {
                Code = "XLM"
            });
            coins.Add(new Coin
            {
                Code = "STRAT"
            });
            coins.Add(new Coin
            {
                Code = "ADA"
            });
            coins.Add(new Coin
            {
                Code = "STEEM"
            });

            foreach (Coin coin in coins)
            {
                if (!context.Coins.Any(x => x.Code == coin.Code))
                {
                    context.Coins.Add(coin);
                }
            }

            context.SaveChanges();
        }

        private static void AddUsers(CryptoBotDbContext context)
        {
            if (!context.Users.Any())
            {
                context.Users.Add(new User
                {
                    Name = "User 1",

                });
                context.SaveChanges();
            }
        }

        private static void AddBots(CryptoBotDbContext context)
        {

            List<Bot> bots = new List<Bot>();

            #region GDAX Bots

            #region BTC-EUR GDAX 60 min

            bots.Add(new Bot
            {
                Name = "GDAX BTC-EUR 60 min Bot",
                Coin = context.Coins.Single(x => x.Code == "BTC"),
                BaseCoin = context.Coins.Single(x => x.Code == "EUR"),
                User = context.Users.Single(x => x.UserId == 1),
                Exchange = context.Exchanges.Single(x => x.Code == "GDAX"),
                Active = false,
                Amount = 500,
                OrderType = Enumerations.OrderTypeEnum.Limit,
                CandleSize = Enumerations.CandleSizeEnum.SixtyMinute,
                Accumulator = true,
                Indicators = new List<Indicator>
                    {
                        new Indicator
                        {
                            IndicatorType = Enumerations.IndicatorTypeEnum.Macd,
                            RuleSets = new List<RuleSet>
                            {
                                new RuleSet
                                {
                                    // buy rules
                                    RuleSide = IndicatorRulesEnumerations.RuleSideEnum.Buy,
                                    Rules = new List<Rule>
                                    {
                                        new Rule
                                        {
                                            IndicatorRuleTypeId = (int)IndicatorRulesEnumerations.MacdRulesEnum.IsMacdAboveSignal
                                        },
                                        new Rule
                                        {
                                            IndicatorRuleTypeId = (int)IndicatorRulesEnumerations.MacdRulesEnum.IsHistogramAboveMinimumHeight,
                                            Value = 36 // best results on BTC-EUR GDAX
                                        },
                                        new Rule
                                        {
                                            // stops the bot from buying too high in case it was restarted
                                            IndicatorRuleTypeId = (int)IndicatorRulesEnumerations.MacdRulesEnum.IsMacdBelowAValue,
                                            Value = 100
                                        }
                                    }
                                },
                                new RuleSet
                                {
                                    // single sell trigger MACD crossing down
                                    RuleSide = IndicatorRulesEnumerations.RuleSideEnum.Sell,
                                    Rules = new List<Rule>
                                    {
                                        new Rule
                                        {
                                            IndicatorRuleTypeId = (int)IndicatorRulesEnumerations.MacdRulesEnum.IsCrossingDown
                                        }
                                    }
                                },
                                new RuleSet
                                {
                                    // SELL Combined trigger, MACD above 200 & turning down
                                    RuleSide = IndicatorRulesEnumerations.RuleSideEnum.Sell,
                                    Rules = new List<Rule>
                                    {
                                        new Rule
                                        {
                                            IndicatorRuleTypeId = (int)IndicatorRulesEnumerations.MacdRulesEnum.IsMacdTurningDown
                                        },
                                        new Rule
                                        {
                                            IndicatorRuleTypeId = (int)IndicatorRulesEnumerations.MacdRulesEnum.IsMacdAboveValue,
                                            Value = 140
                                        }
                                    }
                                },
                                 new RuleSet
                                {
                                    // single sell trigger MACD below trigger
                                    RuleSide = IndicatorRulesEnumerations.RuleSideEnum.Sell,
                                    Rules = new List<Rule>
                                    {
                                        new Rule
                                        {
                                            IndicatorRuleTypeId = (int)IndicatorRulesEnumerations.MacdRulesEnum.IsMacdBelowSignal
                                        }
                                    }
                                },
                                new RuleSet
                                {
                                    /* SELL
                                     * If histogram height is small < 48 
                                     * && MACD is below < -50 
                                     * && Histogram reduces
                                     * 
                                     */
                                    RuleSide = IndicatorRulesEnumerations.RuleSideEnum.Sell,
                                    Rules = new List<Rule>
                                    {
                                        new Rule
                                        {
                                            IndicatorRuleTypeId = (int)IndicatorRulesEnumerations.MacdRulesEnum.IsHistogramBelowAMinimumHeight,
                                            Value = 48
                                        },
                                        new Rule
                                        {
                                            IndicatorRuleTypeId = (int)IndicatorRulesEnumerations.MacdRulesEnum.IsMacdBelowAValue,
                                            Value = -50
                                        },
                                        new Rule
                                        {
                                            IndicatorRuleTypeId = (int)IndicatorRulesEnumerations.MacdRulesEnum.IsHistogramTurningDown
                                        }
                                    }
                                }

                             }
                        },
                        new Indicator // Volume Indicator
                        {
                            IndicatorType = Enumerations.IndicatorTypeEnum.Volume,
                            UseForSell = false,
                            RuleSets = new List<RuleSet>
                            {
                                new RuleSet
                                {
                                    // buy rules
                                    RuleSide = IndicatorRulesEnumerations.RuleSideEnum.Buy,
                                    Rules = new List<Rule>
                                    {
                                        new Rule
                                        {
                                            IndicatorRuleTypeId = (int)IndicatorRulesEnumerations.VolumeRulesEnum.IsAboveVolume,
                                            Value = 1330000M
                                        }
                                    }
                                }
                            }
                        }
                    }
            });

            #endregion BTC-EUR GDAX 60 min

            #region BTC-EUR GDAX 30 min CrossOver

            bots.Add(new Bot
            {
                Name = "BTC-EUR GDAX 30 min CrossOver",
                Coin = context.Coins.Single(x => x.Code == "BTC"),
                BaseCoin = context.Coins.Single(x => x.Code == "EUR"),
                User = context.Users.Single(x => x.UserId == 1),
                Exchange = context.Exchanges.Single(x => x.Code == "GDAX"),
                Active = false,
                Amount = 500,
                OrderType = Enumerations.OrderTypeEnum.Limit,
                CandleSize = Enumerations.CandleSizeEnum.ThirtyMinute,
                Accumulator = true,
                Indicators = new List<Indicator>
                    {
                        new Indicator
                        {
                            IndicatorType = Enumerations.IndicatorTypeEnum.CloseOpenCrossing,
                            RuleSets = new List<RuleSet>
                            {
                                new RuleSet
                                {
                                    // buy rules
                                    RuleSide = IndicatorRulesEnumerations.RuleSideEnum.Buy,
                                    Rules = new List<Rule>
                                    {
                                        new Rule
                                        {
                                            IndicatorRuleTypeId = (int)IndicatorRulesEnumerations.CrossingRulesEnum.IsCrossingOver
                                        }
                                    }
                                },
                                new RuleSet
                                {
                                    // sell rules
                                    RuleSide = IndicatorRulesEnumerations.RuleSideEnum.Sell,
                                    Rules = new List<Rule>
                                    {
                                        new Rule
                                        {
                                            IndicatorRuleTypeId = (int)IndicatorRulesEnumerations.CrossingRulesEnum.IsCrossingUnder
                                        }
                                    }
                                }
                             }
                        }
                    }
            });

            #endregion
            
            #region BTC-EUR GDAX 15 min Stocastic

            bots.Add(new Bot
            {
                Name = "BTC-EUR GDAX 15 min Stocastic",
                Coin = context.Coins.Single(x => x.Code == "BTC"),
                BaseCoin = context.Coins.Single(x => x.Code == "EUR"),
                User = context.Users.Single(x => x.UserId == 1),
                Exchange = context.Exchanges.Single(x => x.Code == "GDAX"),
                Active = false,
                Amount = 100,
                OrderType = Enumerations.OrderTypeEnum.Limit,
                CandleSize = Enumerations.CandleSizeEnum.FifteenMinute,
                Accumulator = true,
                Indicators = new List<Indicator>
                    {
                        new Indicator
                        {
                            IndicatorType = Enumerations.IndicatorTypeEnum.Stocastic,
                            RuleSets = new List<RuleSet>
                            {
                                new RuleSet
                                {
                                    // buy rules
                                    RuleSide = IndicatorRulesEnumerations.RuleSideEnum.Buy,
                                    Rules = new List<Rule>
                                    {
                                        new Rule
                                        {
                                            IndicatorRuleTypeId = (int)IndicatorRulesEnumerations.OverBoughtSoldRulesEnum.IsOverSold,
                                            Value = 12
                                        }
                                    }
                                },
                                new RuleSet
                                {
                                    // sell rules
                                    RuleSide = IndicatorRulesEnumerations.RuleSideEnum.Sell,
                                    Rules = new List<Rule>
                                    {
                                        new Rule
                                        {
                                            IndicatorRuleTypeId = (int)IndicatorRulesEnumerations.OverBoughtSoldRulesEnum.IsOverBought,
                                            Value = 85
                                        }
                                    }
                                }
                             }
                        }
                    }
            });

            #endregion 

            #region ETH-EUR GDAX 60 min

            // ETH on the backtest was returning around 5-6% in week 1 2018, BTC was returning 13% so all money was put on BTC


            bots.Add(new Bot
            {
                Name = "GDAX ETH-EUR 60 min Bot",
                Coin = context.Coins.Single(x => x.Code == "ETH"),
                BaseCoin = context.Coins.Single(x => x.Code == "EUR"),
                User = context.Users.Single(x => x.UserId == 1),
                Exchange = context.Exchanges.Single(x => x.Code == "GDAX"),
                Active = true,
                Amount = 500,
                OrderType = Enumerations.OrderTypeEnum.Limit,
                CandleSize = Enumerations.CandleSizeEnum.SixtyMinute,
                Accumulator = true,
                Indicators = new List<Indicator>
                    {
                        new Indicator // Macd Indicator
                        {
                            IndicatorType = Enumerations.IndicatorTypeEnum.Macd,
                            RuleSets = new List<RuleSet>
                            {
                                new RuleSet
                                {
                                    // buy rules
                                    RuleSide = IndicatorRulesEnumerations.RuleSideEnum.Buy,
                                    Rules = new List<Rule>
                                    {
                                        new Rule
                                        {
                                            IndicatorRuleTypeId = (int)IndicatorRulesEnumerations.MacdRulesEnum.IsMacdAboveSignal
                                        },
                                        new Rule
                                        {
                                            IndicatorRuleTypeId = (int)IndicatorRulesEnumerations.MacdRulesEnum.IsHistogramAboveMinimumHeight,
                                            Value = 3.3M,
                                        },
                                        new Rule
                                        {
                                            // stops the bot from buying too high in case it was restarted
                                            IndicatorRuleTypeId = (int)IndicatorRulesEnumerations.MacdRulesEnum.IsMacdBelowAValue,
                                            Value = 10
                                        }
                                    }
                                },
                                new RuleSet
                                {
                                    // single sell trigger MACD crossing down
                                    RuleSide = IndicatorRulesEnumerations.RuleSideEnum.Sell,
                                    Rules = new List<Rule>
                                    {
                                        new Rule
                                        {
                                            IndicatorRuleTypeId = (int)IndicatorRulesEnumerations.MacdRulesEnum.IsCrossingDown
                                        }
                                    }
                                }
                             }
                        },
                        new Indicator // Volume Indicator
                        {
                            IndicatorType = Enumerations.IndicatorTypeEnum.Volume,
                            UseForSell = false,
                            RuleSets = new List<RuleSet>
                            {
                                new RuleSet
                                {
                                    // buy rules
                                    RuleSide = IndicatorRulesEnumerations.RuleSideEnum.Buy,
                                    Rules = new List<Rule>
                                    {
                                        new Rule
                                        {
                                            IndicatorRuleTypeId = (int)IndicatorRulesEnumerations.VolumeRulesEnum.IsAboveVolume,
                                            Value = 400000M
                                        }
                                    }
                                }
                             }
                        }
                    }
            });

            #endregion ETH-EUR GDAX 60 min

            #region LTC-EUR GDAX 60 min

            // LTC on the backtest was returning around 5-6% in week 1 2018, BTC was returning 13% so all money was put on BTC

            bots.Add(new Bot
            {
                Name = "GDAX LTC-EUR 60 min Bot",
                Coin = context.Coins.Single(x => x.Code == "LTC"),
                BaseCoin = context.Coins.Single(x => x.Code == "EUR"),
                User = context.Users.Single(x => x.UserId == 1),
                Exchange = context.Exchanges.Single(x => x.Code == "GDAX"),
                Active = false,
                Amount = 500,
                OrderType = Enumerations.OrderTypeEnum.Limit,
                CandleSize = Enumerations.CandleSizeEnum.SixtyMinute,
                Accumulator = true,
                Indicators = new List<Indicator>
                    {
                        new Indicator
                        {
                            IndicatorType = Enumerations.IndicatorTypeEnum.Macd,
                            RuleSets = new List<RuleSet>
                            {

                                #region Buy Rules
                                new RuleSet
                                {
                                    // All crossovers that are above the 0 line

                                    RuleSide = IndicatorRulesEnumerations.RuleSideEnum.Buy,
                                    Rules = new List<Rule>
                                    {
                                        new Rule
                                        {
                                            IndicatorRuleTypeId = (int)IndicatorRulesEnumerations.MacdRulesEnum.IsMacdAboveSignal
                                        },
                                        new Rule
                                        {
                                            IndicatorRuleTypeId = (int)IndicatorRulesEnumerations.MacdRulesEnum.IsHistogramAboveMinimumHeight,
                                            Value = 1
                                        },
                                        new Rule
                                        {
                                            IndicatorRuleTypeId = (int)IndicatorRulesEnumerations.MacdRulesEnum.IsMacdAboveValue,
                                            Value = 0
                                        }
                                    }
                                },
                                new RuleSet
                                {

                                    // for any crossovers that are deep under the zero line

                                    RuleSide = IndicatorRulesEnumerations.RuleSideEnum.Buy,
                                    Rules = new List<Rule>
                                    {
                                        new Rule
                                        {
                                            IndicatorRuleTypeId = (int)IndicatorRulesEnumerations.MacdRulesEnum.IsMacdAboveSignal
                                        },
                                        new Rule
                                        {
                                            IndicatorRuleTypeId = (int)IndicatorRulesEnumerations.MacdRulesEnum.IsMacdBelowAValue,
                                            Value = -10
                                        }
                                    }
                                },

                                #endregion Buy Rules

                                #region Sell Rules
                                new RuleSet
                                {
                                    // single sell trigger MACD crossing down
                                    RuleSide = IndicatorRulesEnumerations.RuleSideEnum.Sell,
                                    Rules = new List<Rule>
                                    {
                                        new Rule
                                        {
                                            IndicatorRuleTypeId = (int)IndicatorRulesEnumerations.MacdRulesEnum.IsCrossingDown
                                        }
                                    }
                                },
                                new RuleSet
                                {
                                    // SELL Combined trigger, MACD above 9 & turning down
                                    RuleSide = IndicatorRulesEnumerations.RuleSideEnum.Sell,
                                    Rules = new List<Rule>
                                    {
                                        new Rule
                                        {
                                            IndicatorRuleTypeId = (int)IndicatorRulesEnumerations.MacdRulesEnum.IsMacdTurningDown
                                        },
                                        new Rule
                                        {
                                            IndicatorRuleTypeId = (int)IndicatorRulesEnumerations.MacdRulesEnum.IsMacdAboveValue,
                                            Value = 9
                                        }
                                    }
                                },
                                new RuleSet
                                {
                                    // SELL Combined trigger, MACD below zero and turning down
                                    RuleSide = IndicatorRulesEnumerations.RuleSideEnum.Sell,
                                    Rules = new List<Rule>
                                    {
                                        new Rule
                                        {
                                            IndicatorRuleTypeId = (int)IndicatorRulesEnumerations.MacdRulesEnum.IsMacdTurningDown
                                        },
                                        new Rule
                                        {
                                            IndicatorRuleTypeId = (int)IndicatorRulesEnumerations.MacdRulesEnum.IsMacdBelowZero
                                        }
                                    }
                                }
                             }

                            #endregion Sell Rules
                        },
                        new Indicator // Volume Indicator
                        {
                            IndicatorType = Enumerations.IndicatorTypeEnum.Volume,
                            UseForSell = false,
                            RuleSets = new List<RuleSet>
                            {
                                new RuleSet
                                {
                                    // buy rules
                                    RuleSide = IndicatorRulesEnumerations.RuleSideEnum.Buy,
                                    Rules = new List<Rule>
                                    {
                                        new Rule
                                        {
                                            IndicatorRuleTypeId = (int)IndicatorRulesEnumerations.VolumeRulesEnum.IsAboveVolume,
                                            Value = 1M
                                        }
                                    }
                                }
                            }
                        }
                    }
            });

            #endregion LTC-EUR GDAX 60 min

            #endregion GDAX Bots

            #region BITTREX Bots

            #region ETH-EUR 60 min

            bots.Add(new Bot
            {
                Name = "BTRX BTC-ETC 60 min Bot",
                Coin = context.Coins.Single(x => x.Code == "ETC"),
                BaseCoin = context.Coins.Single(x => x.Code == "BTC"),
                User = context.Users.Single(x => x.UserId == 1),
                Exchange = context.Exchanges.Single(x => x.Code == "BTRX"),
                Active = false,
                Amount = .02M,
                OrderType = Enumerations.OrderTypeEnum.Limit,
                CandleSize = Enumerations.CandleSizeEnum.SixtyMinute,
                Accumulator = true,
                Indicators = new List<Indicator>
                    {
                        new Indicator
                        {
                            IndicatorType = Enumerations.IndicatorTypeEnum.Macd,
                            RuleSets = new List<RuleSet>
                            {
                                new RuleSet
                                {
                                    // buy rules
                                    RuleSide = IndicatorRulesEnumerations.RuleSideEnum.Buy,
                                    Rules = new List<Rule>
                                    {
                                        new Rule
                                        {
                                            IndicatorRuleTypeId = (int)IndicatorRulesEnumerations.MacdRulesEnum.IsMacdAboveSignal
                                        },
                                        new Rule
                                        {
                                            IndicatorRuleTypeId = (int)IndicatorRulesEnumerations.MacdRulesEnum.IsHistogramAboveMinimumHeight,
                                            Value = 0.00001M
                                        },
                                        new Rule
                                        {
                                            // stops the bot from buying too high in case it was restarted
                                            IndicatorRuleTypeId = (int)IndicatorRulesEnumerations.MacdRulesEnum.IsMacdBelowAValue,
                                            Value = 0.0001M
                                        }
                                    }
                                },
                                new RuleSet
                                {
                                    // single sell trigger MACD crossing down
                                    RuleSide = IndicatorRulesEnumerations.RuleSideEnum.Sell,
                                    Rules = new List<Rule>
                                    {
                                        new Rule
                                        {
                                            IndicatorRuleTypeId = (int)IndicatorRulesEnumerations.MacdRulesEnum.IsCrossingDown
                                        }
                                    }
                                }
                             }
                        },
                        //new Indicator // Volume Indicator // Didin't produce good results
                        //{
                        //    IndicatorType = Enumerations.IndicatorTypeEnum.Volume,
                        //    UseForSell = false,
                        //    RuleSets = new List<RuleSet>
                        //    {
                        //        new RuleSet
                        //        {
                        //            // buy rules
                        //            RuleSide = IndicatorRulesEnumerations.RuleSideEnum.Buy,
                        //            Rules = new List<Rule>
                        //            {
                        //                new Rule
                        //                {
                        //                    IndicatorRuleTypeId = (int)IndicatorRulesEnumerations.VolumeRulesEnum.IsAboveVolume,
                        //                    Value = 0M
                        //                }
                        //            }
                        //        }
                        //    }
                        //}
                    }
            });

            #endregion

            #region BTC-USDT 60 min
            
            bots.Add(new Bot
            {
                Name = "BTRX BTC-USDT 60 min Bot",
                Coin = context.Coins.Single(x => x.Code == "BTC"),
                BaseCoin = context.Coins.Single(x => x.Code == "USDT"),
                User = context.Users.Single(x => x.UserId == 1),
                Exchange = context.Exchanges.Single(x => x.Code == "BTRX"),
                Active = false,
                Amount = 1M,
                OrderType = Enumerations.OrderTypeEnum.Limit,
                CandleSize = Enumerations.CandleSizeEnum.SixtyMinute,
                Indicators = new List<Indicator>
                    {
                        new Indicator
                        {
                            IndicatorType = Enumerations.IndicatorTypeEnum.Macd,
                            RuleSets = new List<RuleSet>
                            {
                                new RuleSet
                                {
                                    // buy rules
                                    RuleSide = IndicatorRulesEnumerations.RuleSideEnum.Buy,
                                    Rules = new List<Rule>
                                    {
                                        new Rule
                                        {
                                            IndicatorRuleTypeId = (int)IndicatorRulesEnumerations.MacdRulesEnum.IsMacdAboveSignal
                                        },
                                        new Rule
                                        {
                                            IndicatorRuleTypeId = (int)IndicatorRulesEnumerations.MacdRulesEnum.IsHistogramAboveMinimumHeight,
                                            Value = 60M
                                        },
                                        new Rule
                                        {
                                            // stops the bot from buying too high in case it was restarted
                                            IndicatorRuleTypeId = (int)IndicatorRulesEnumerations.MacdRulesEnum.IsMacdBelowAValue,
                                            Value = 280M
                                        }
                                    }
                                },
                                new RuleSet
                                {
                                    // single sell trigger MACD crossing down
                                    RuleSide = IndicatorRulesEnumerations.RuleSideEnum.Sell,
                                    Rules = new List<Rule>
                                    {
                                        new Rule
                                        {
                                            IndicatorRuleTypeId = (int)IndicatorRulesEnumerations.MacdRulesEnum.IsCrossingDown
                                        }
                                    }
                                }
                             }
                        }
                    }
            });

            #endregion
            
            #region BTC-XMR 60 min

            bots.Add(new Bot
            {
                Name = "BTRX BTC-XMR 60 min Bot",
                Coin = context.Coins.Single(x => x.Code == "XMR"),
                BaseCoin = context.Coins.Single(x => x.Code == "BTC"),
                User = context.Users.Single(x => x.UserId == 1),
                Exchange = context.Exchanges.Single(x => x.Code == "BTRX"),
                Active = false,
                Amount = 0.02M,
                OrderType = Enumerations.OrderTypeEnum.Limit,
                CandleSize = Enumerations.CandleSizeEnum.SixtyMinute,
                Accumulator = true,
                Indicators = new List<Indicator>
                    {
                        new Indicator
                        {
                            IndicatorType = Enumerations.IndicatorTypeEnum.Macd,
                            RuleSets = new List<RuleSet>
                            {
                                new RuleSet
                                {
                                    // buy rules
                                    RuleSide = IndicatorRulesEnumerations.RuleSideEnum.Buy,
                                    Rules = new List<Rule>
                                    {
                                        new Rule
                                        {
                                            IndicatorRuleTypeId = (int)IndicatorRulesEnumerations.MacdRulesEnum.IsMacdAboveSignal
                                        },
                                        new Rule
                                        {
                                            IndicatorRuleTypeId = (int)IndicatorRulesEnumerations.MacdRulesEnum.IsHistogramAboveMinimumHeight,
                                            Value = 0.000065M
                                        },
                                        new Rule
                                        {
                                            // stops the bot from buying too high in case it was restarted
                                            IndicatorRuleTypeId = (int)IndicatorRulesEnumerations.MacdRulesEnum.IsMacdBelowAValue,
                                            Value = 0.0004M
                                        }
                                    }
                                },
                                new RuleSet
                                {
                                    // single sell trigger MACD crossing down
                                    RuleSide = IndicatorRulesEnumerations.RuleSideEnum.Sell,
                                    Rules = new List<Rule>
                                    {
                                        new Rule
                                        {
                                            IndicatorRuleTypeId = (int)IndicatorRulesEnumerations.MacdRulesEnum.IsCrossingDown
                                        }
                                    }
                                }
                             }
                        },
                        new Indicator // Volume Indicator
                        {
                            IndicatorType = Enumerations.IndicatorTypeEnum.Volume,
                            UseForSell = false,
                            RuleSets = new List<RuleSet>
                            {
                                new RuleSet
                                {
                                    // buy rules
                                    RuleSide = IndicatorRulesEnumerations.RuleSideEnum.Buy,
                                    Rules = new List<Rule>
                                    {
                                        new Rule
                                        {
                                            IndicatorRuleTypeId = (int)IndicatorRulesEnumerations.VolumeRulesEnum.IsAboveVolume,
                                            Value = 4000M
                                        }
                                    }
                                }
                            }
                        }
                    }
            });

            #endregion

            #region BTC-NEO 60 min

            bots.Add(new Bot
            {
                Name = "BTRX BTC-NEO 60 min Bot",
                Coin = context.Coins.Single(x => x.Code == "NEO"),
                BaseCoin = context.Coins.Single(x => x.Code == "BTC"),
                User = context.Users.Single(x => x.UserId == 1),
                Exchange = context.Exchanges.Single(x => x.Code == "BTRX"),
                Active = false,
                Amount = 0.02M,
                OrderType = Enumerations.OrderTypeEnum.Limit,
                CandleSize = Enumerations.CandleSizeEnum.SixtyMinute,
                Accumulator = true,
                Indicators = new List<Indicator>
                    {
                        new Indicator
                        {
                            IndicatorType = Enumerations.IndicatorTypeEnum.Macd,
                            RuleSets = new List<RuleSet>
                            {
                                new RuleSet
                                {
                                    // buy rules
                                    RuleSide = IndicatorRulesEnumerations.RuleSideEnum.Buy,
                                    Rules = new List<Rule>
                                    {
                                        new Rule
                                        {
                                            IndicatorRuleTypeId = (int)IndicatorRulesEnumerations.MacdRulesEnum.IsMacdAboveSignal
                                        },
                                        new Rule
                                        {
                                            IndicatorRuleTypeId = (int)IndicatorRulesEnumerations.MacdRulesEnum.IsHistogramAboveMinimumHeight,
                                            Value = 0.000065M
                                        },
                                        new Rule
                                        {
                                            // stops the bot from buying too high in case it was restarted
                                            IndicatorRuleTypeId = (int)IndicatorRulesEnumerations.MacdRulesEnum.IsMacdBelowAValue,
                                            Value = 0.0004M
                                        }
                                    }
                                },
                                new RuleSet
                                {
                                    // single sell trigger MACD crossing down
                                    RuleSide = IndicatorRulesEnumerations.RuleSideEnum.Sell,
                                    Rules = new List<Rule>
                                    {
                                        new Rule
                                        {
                                            IndicatorRuleTypeId = (int)IndicatorRulesEnumerations.MacdRulesEnum.IsCrossingDown
                                        }
                                    }
                                }
                             }
                        }
                    }
            });

            #endregion

            #region BTC-XLM 60 min

            bots.Add(new Bot
            {
                Name = "BTRX BTC-XLM 60 min Bot",
                Coin = context.Coins.Single(x => x.Code == "XLM"),
                BaseCoin = context.Coins.Single(x => x.Code == "BTC"),
                User = context.Users.Single(x => x.UserId == 1),
                Exchange = context.Exchanges.Single(x => x.Code == "BTRX"),
                Active = false,
                Amount = 0.02M,
                OrderType = Enumerations.OrderTypeEnum.Limit,
                CandleSize = Enumerations.CandleSizeEnum.SixtyMinute,
                Accumulator = true,
                Indicators = new List<Indicator>
                    {
                        new Indicator
                        {
                            IndicatorType = Enumerations.IndicatorTypeEnum.Macd,
                            RuleSets = new List<RuleSet>
                            {
                                new RuleSet
                                {
                                    // buy rules
                                    RuleSide = IndicatorRulesEnumerations.RuleSideEnum.Buy,
                                    Rules = new List<Rule>
                                    {
                                        new Rule
                                        {
                                            IndicatorRuleTypeId = (int)IndicatorRulesEnumerations.MacdRulesEnum.IsMacdAboveSignal
                                        },
                                        new Rule
                                        {
                                            IndicatorRuleTypeId = (int)IndicatorRulesEnumerations.MacdRulesEnum.IsHistogramAboveMinimumHeight,
                                            Value = 0.0000001M
                                        },
                                        new Rule
                                        {
                                            // stops the bot from buying too high in case it was restarted
                                            IndicatorRuleTypeId = (int)IndicatorRulesEnumerations.MacdRulesEnum.IsMacdBelowAValue,
                                            Value = 0.0000019M
                                        }
                                    }
                                },
                                new RuleSet
                                {
                                    // single sell trigger MACD crossing down
                                    RuleSide = IndicatorRulesEnumerations.RuleSideEnum.Sell,
                                    Rules = new List<Rule>
                                    {
                                        new Rule
                                        {
                                            IndicatorRuleTypeId = (int)IndicatorRulesEnumerations.MacdRulesEnum.IsCrossingDown
                                        }
                                    }
                                }
                             }
                        },
                        new Indicator // Volume Indicator
                        {
                            IndicatorType = Enumerations.IndicatorTypeEnum.Volume,
                            UseForSell = false,
                            RuleSets = new List<RuleSet>
                            {
                                new RuleSet
                                {
                                    // buy rules
                                    RuleSide = IndicatorRulesEnumerations.RuleSideEnum.Buy,
                                    Rules = new List<Rule>
                                    {
                                        new Rule
                                        {
                                            IndicatorRuleTypeId = (int)IndicatorRulesEnumerations.VolumeRulesEnum.IsAboveVolume,
                                            Value = 5000000M
                                        }
                                    }
                                }
                            }
                        }
                    }
            });

            #endregion

            #region BTC-STRAT 60 min

            bots.Add(new Bot
            {
                Name = "BTRX BTC-STRAT 60 min Bot",
                Coin = context.Coins.Single(x => x.Code == "STRAT"),
                BaseCoin = context.Coins.Single(x => x.Code == "BTC"),
                User = context.Users.Single(x => x.UserId == 1),
                Exchange = context.Exchanges.Single(x => x.Code == "BTRX"),
                Active = false,
                Amount = 0.012M,
                OrderType = Enumerations.OrderTypeEnum.Limit,
                CandleSize = Enumerations.CandleSizeEnum.SixtyMinute,
                Accumulator = true,
                Indicators = new List<Indicator>
                    {
                        new Indicator
                        {
                            IndicatorType = Enumerations.IndicatorTypeEnum.Macd,
                            RuleSets = new List<RuleSet>
                            {
                                new RuleSet
                                {
                                    // buy rules
                                    RuleSide = IndicatorRulesEnumerations.RuleSideEnum.Buy,
                                    Rules = new List<Rule>
                                    {
                                        new Rule
                                        {
                                            IndicatorRuleTypeId = (int)IndicatorRulesEnumerations.MacdRulesEnum.IsMacdAboveSignal
                                        },
                                        new Rule
                                        {
                                            IndicatorRuleTypeId = (int)IndicatorRulesEnumerations.MacdRulesEnum.IsHistogramAboveMinimumHeight,
                                            Value = 0.000004M
                                        },
                                        new Rule
                                        {
                                            // stops the bot from buying too high in case it was restarted
                                            IndicatorRuleTypeId = (int)IndicatorRulesEnumerations.MacdRulesEnum.IsMacdBelowAValue,
                                            Value = 0.0004M
                                        }
                                    }
                                },
                                new RuleSet
                                {
                                    // single sell trigger MACD crossing down
                                    RuleSide = IndicatorRulesEnumerations.RuleSideEnum.Sell,
                                    Rules = new List<Rule>
                                    {
                                        new Rule
                                        {
                                            IndicatorRuleTypeId = (int)IndicatorRulesEnumerations.MacdRulesEnum.IsCrossingDown
                                        }
                                    }
                                }
                             }
                        },
                        new Indicator // Volume Indicator
                        {
                            IndicatorType = Enumerations.IndicatorTypeEnum.Volume,
                            UseForSell = false,
                            RuleSets = new List<RuleSet>
                            {
                                new RuleSet
                                {
                                    // buy rules
                                    RuleSide = IndicatorRulesEnumerations.RuleSideEnum.Buy,
                                    Rules = new List<Rule>
                                    {
                                        new Rule
                                        {
                                            IndicatorRuleTypeId = (int)IndicatorRulesEnumerations.VolumeRulesEnum.IsAboveVolume,
                                            Value = 25000M
                                        }
                                    }
                                }
                            }
                        }
                    }
            });

            #endregion

            #region BTC-ADA 60 min

            bots.Add(new Bot
            {
                Name = "BTRX BTC-ADA 60 min Bot",
                Coin = context.Coins.Single(x => x.Code == "ADA"),
                BaseCoin = context.Coins.Single(x => x.Code == "BTC"),
                User = context.Users.Single(x => x.UserId == 1),
                Exchange = context.Exchanges.Single(x => x.Code == "BTRX"),
                Active = false,
                Amount = 0.012M,
                OrderType = Enumerations.OrderTypeEnum.Limit,
                CandleSize = Enumerations.CandleSizeEnum.SixtyMinute,
                Accumulator = true,
                Indicators = new List<Indicator>
                    {
                        new Indicator
                        {
                            IndicatorType = Enumerations.IndicatorTypeEnum.Macd,
                            RuleSets = new List<RuleSet>
                            {
                                new RuleSet
                                {
                                    // buy rules
                                    RuleSide = IndicatorRulesEnumerations.RuleSideEnum.Buy,
                                    Rules = new List<Rule>
                                    {
                                        new Rule
                                        {
                                            IndicatorRuleTypeId = (int)IndicatorRulesEnumerations.MacdRulesEnum.IsMacdAboveSignal
                                        },
                                        new Rule
                                        {
                                            IndicatorRuleTypeId = (int)IndicatorRulesEnumerations.MacdRulesEnum.IsHistogramAboveMinimumHeight,
                                            Value = 0.00000008M
                                        },
                                        new Rule
                                        {
                                            // stops the bot from buying too high in case it was restarted
                                            IndicatorRuleTypeId = (int)IndicatorRulesEnumerations.MacdRulesEnum.IsMacdBelowAValue,
                                            Value = 1M
                                        }
                                    }
                                },
                                new RuleSet
                                {
                                    // single sell trigger MACD crossing down
                                    RuleSide = IndicatorRulesEnumerations.RuleSideEnum.Sell,
                                    Rules = new List<Rule>
                                    {
                                        new Rule
                                        {
                                            IndicatorRuleTypeId = (int)IndicatorRulesEnumerations.MacdRulesEnum.IsCrossingDown
                                        }
                                    }
                                }
                             }
                        },
                        new Indicator // Volume Indicator
                        {
                            IndicatorType = Enumerations.IndicatorTypeEnum.Volume,
                            UseForSell = false,
                            RuleSets = new List<RuleSet>
                            {
                                new RuleSet
                                {
                                    // buy rules
                                    RuleSide = IndicatorRulesEnumerations.RuleSideEnum.Buy,
                                    Rules = new List<Rule>
                                    {
                                        new Rule
                                        {
                                            IndicatorRuleTypeId = (int)IndicatorRulesEnumerations.VolumeRulesEnum.IsAboveVolume,
                                            Value = 5000000M
                                        }
                                    }
                                }
                            }
                        }
                    }
            });

            #endregion
            
            #region BTC-STEEM 60 min

            bots.Add(new Bot
            {
                Name = "BTRX BTC-STEEM 60 min Bot",
                Coin = context.Coins.Single(x => x.Code == "STEEM"),
                BaseCoin = context.Coins.Single(x => x.Code == "BTC"),
                User = context.Users.Single(x => x.UserId == 1),
                Exchange = context.Exchanges.Single(x => x.Code == "BTRX"),
                Active = true,
                Amount = 0.012M,
                OrderType = Enumerations.OrderTypeEnum.Limit,
                CandleSize = Enumerations.CandleSizeEnum.SixtyMinute,
                Accumulator = true,
                Indicators = new List<Indicator>
                    {
                        new Indicator
                        {
                            IndicatorType = Enumerations.IndicatorTypeEnum.Macd,
                            RuleSets = new List<RuleSet>
                            {
                                new RuleSet
                                {
                                    // buy rules
                                    RuleSide = IndicatorRulesEnumerations.RuleSideEnum.Buy,
                                    Rules = new List<Rule>
                                    {
                                        new Rule
                                        {
                                            IndicatorRuleTypeId = (int)IndicatorRulesEnumerations.MacdRulesEnum.IsMacdAboveSignal
                                        },
                                        new Rule
                                        {
                                            IndicatorRuleTypeId = (int)IndicatorRulesEnumerations.MacdRulesEnum.IsHistogramAboveMinimumHeight,
                                            Value = 0.0000015M,
                                        },
                                        new Rule
                                        {
                                            // stops the bot from buying too high in case it was restarted
                                            IndicatorRuleTypeId = (int)IndicatorRulesEnumerations.MacdRulesEnum.IsMacdBelowAValue,
                                            Value = 0.000008M,
                                        }
                                    }
                                },
                                new RuleSet
                                {
                                    // single sell trigger MACD crossing down
                                    RuleSide = IndicatorRulesEnumerations.RuleSideEnum.Sell,
                                    Rules = new List<Rule>
                                    {
                                        new Rule
                                        {
                                            IndicatorRuleTypeId = (int)IndicatorRulesEnumerations.MacdRulesEnum.IsCrossingDown
                                        }
                                    }
                                }
                             }
                        },
                        new Indicator // Volume Indicator
                        {
                            IndicatorType = Enumerations.IndicatorTypeEnum.Volume,
                            UseForSell = false,
                            RuleSets = new List<RuleSet>
                            {
                                new RuleSet
                                {
                                    // buy rules
                                    RuleSide = IndicatorRulesEnumerations.RuleSideEnum.Buy,
                                    Rules = new List<Rule>
                                    {
                                        new Rule
                                        {
                                            IndicatorRuleTypeId = (int)IndicatorRulesEnumerations.VolumeRulesEnum.IsAboveVolume,
                                            Value = 200000M
                                        }
                                    }
                                }
                            }
                        }
                    }
            });

            #endregion

            #endregion
            
            foreach (Bot bot in bots)
            {
                if (!context.Bots.Any(x =>
                    x.BaseCoin.CoinId == bot.BaseCoin.CoinId && x.Coin.CoinId == bot.Coin.CoinId && x.Name == bot.Name))
                {
                    context.Bots.Add(bot);
                }
               
            }

            context.SaveChanges();

        }

        private static void AddApiSettings(CryptoBotDbContext context)
        {
            List<ApiSetting> apiSettings = new List<ApiSetting>();


            // Bittrex
            apiSettings.Add(new ApiSetting()
                {
                    User = context.Users.First(),
                    Exchange = context.Exchanges.Single(x => x.Code == "BTRX"),
                    Url = "https://bittrex.com",
                    ComissionRate = 0.25,
                    Simulated = true
                });

            // Gdax
            apiSettings.Add(new ApiSetting()
                {
                    User = context.Users.First(),
                    Exchange = context.Exchanges.Single(x => x.Code == "GDAX"),
                    Url = "https://api.gdax.com",
                    ComissionRate = 0.25,
                    SocketUrl = "wss://ws-feed.gdax.com",
                    Simulated = true
                });

            foreach (ApiSetting apiSetting in apiSettings)
            {
                if (!context.ApiSettings.Any(x => x.Exchange.ExchangeId == apiSetting.Exchange.ExchangeId && x.User.UserId == 1))
                {
                    context.ApiSettings.Add(apiSetting);
                }
            }

            context.SaveChanges();

            
        }

        private static void AddMessagingApps(CryptoBotDbContext context)
        {

            List<MessagingApp> messagingApps = new List<MessagingApp>();

            messagingApps.Add(new MessagingApp
            {
                User = context.Users.First(),
                Active = true,
                MessagingAppType = Enumerations.MessagingAppEnum.Telegram,
                MessagingAppSettings = new List<MessagingAppSettings>
                {
                    new MessagingAppSettings
                    {
                        Key = Constants.Token,
                        Value = "o"
                    }
                }
            });
            

            foreach (MessagingApp messagingApp in messagingApps)
            {
                if (!context.MessagingApps.Any(x => x.MessagingAppType == messagingApp.MessagingAppType && x.User.UserId == 1))
                {
                    context.MessagingApps.Add(messagingApp);
                }
            }

            context.SaveChanges();
        }
    }

    
}
