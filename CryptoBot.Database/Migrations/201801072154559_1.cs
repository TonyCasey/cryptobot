namespace CryptoBot.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _1 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "Account.ApiSettings",
                c => new
                    {
                        ApiSettingId = c.Long(nullable: false, identity: true),
                        Url = c.String(nullable: false),
                        Key = c.String(),
                        Secret = c.String(),
                        Passphrase = c.String(),
                        ComissionRate = c.Double(nullable: false),
                        SocketUrl = c.String(),
                        Simulated = c.Boolean(nullable: false),
                        Exchange_ExchangeId = c.Int(nullable: false),
                        User_UserId = c.Long(nullable: false),
                    })
                .PrimaryKey(t => t.ApiSettingId)
                .ForeignKey("Market.Exchanges", t => t.Exchange_ExchangeId, cascadeDelete: true)
                .ForeignKey("Account.Users", t => t.User_UserId, cascadeDelete: true)
                .Index(t => t.Exchange_ExchangeId)
                .Index(t => t.User_UserId);
            
            CreateTable(
                "Market.Exchanges",
                c => new
                    {
                        ExchangeId = c.Int(nullable: false, identity: true),
                        Name = c.String(maxLength: 50, unicode: false),
                        Code = c.String(maxLength: 10, unicode: false),
                        CreationTime = c.DateTime(nullable: false),
                        CreationUser = c.String(nullable: false, maxLength: 250),
                        LastUpdateTime = c.DateTime(),
                        LastUpdateUser = c.String(maxLength: 250),
                    })
                .PrimaryKey(t => t.ExchangeId);
            
            CreateTable(
                "Account.Users",
                c => new
                    {
                        UserId = c.Long(nullable: false, identity: true),
                        Name = c.String(maxLength: 250, unicode: false),
                    })
                .PrimaryKey(t => t.UserId);
            
            CreateTable(
                "Bot.Bots",
                c => new
                    {
                        BotId = c.Long(nullable: false, identity: true),
                        Name = c.String(),
                        Active = c.Boolean(nullable: false),
                        CurrentPositionId = c.Long(),
                        Amount = c.Decimal(nullable: false, precision: 38, scale: 18),
                        OrderType = c.Int(nullable: false),
                        CandleSize = c.Int(nullable: false),
                        CreationTime = c.DateTime(nullable: false),
                        CreationUser = c.String(nullable: false, maxLength: 250),
                        LastUpdateTime = c.DateTime(),
                        LastUpdateUser = c.String(maxLength: 250),
                        BaseCoin_CoinId = c.Long(),
                        Coin_CoinId = c.Long(nullable: false),
                        Exchange_ExchangeId = c.Int(nullable: false),
                        User_UserId = c.Long(nullable: false),
                    })
                .PrimaryKey(t => t.BotId)
                .ForeignKey("Market.Coins", t => t.BaseCoin_CoinId)
                .ForeignKey("Market.Coins", t => t.Coin_CoinId, cascadeDelete: true)
                .ForeignKey("Trading.Positions", t => t.CurrentPositionId)
                .ForeignKey("Market.Exchanges", t => t.Exchange_ExchangeId, cascadeDelete: true)
                .ForeignKey("Account.Users", t => t.User_UserId, cascadeDelete: true)
                .Index(t => t.CurrentPositionId)
                .Index(t => t.BaseCoin_CoinId)
                .Index(t => t.Coin_CoinId)
                .Index(t => t.Exchange_ExchangeId)
                .Index(t => t.User_UserId);
            
            CreateTable(
                "Market.Coins",
                c => new
                    {
                        CoinId = c.Long(nullable: false, identity: true),
                        Code = c.String(),
                        CreationTime = c.DateTime(nullable: false),
                        CreationUser = c.String(nullable: false, maxLength: 250),
                        LastUpdateTime = c.DateTime(),
                        LastUpdateUser = c.String(maxLength: 250),
                    })
                .PrimaryKey(t => t.CoinId);
            
            CreateTable(
                "Trading.Positions",
                c => new
                    {
                        PositionId = c.Long(nullable: false, identity: true),
                        Side = c.Int(nullable: false),
                        BuyPrice = c.Double(nullable: false),
                        SellPrice = c.Double(),
                        Quantity = c.Double(nullable: false),
                        Commission = c.Double(),
                        NetProfit = c.Double(),
                        NetProfitPercent = c.Double(),
                        GrossProfit = c.Double(),
                        GrossProfitPercent = c.Double(),
                        BuyTimeStamp = c.DateTime(nullable: false),
                        BuyRequestExchangeReference = c.String(),
                        SellTimeStamp = c.DateTime(),
                        SellRequestExchangeReference = c.String(),
                        Status = c.Int(nullable: false),
                        BotId = c.Long(nullable: false),
                        CreationTime = c.DateTime(nullable: false),
                        CreationUser = c.String(nullable: false, maxLength: 250),
                        LastUpdateTime = c.DateTime(),
                        LastUpdateUser = c.String(maxLength: 250),
                        Bot_BotId = c.Long(),
                    })
                .PrimaryKey(t => t.PositionId)
                .ForeignKey("Bot.Bots", t => t.BotId, cascadeDelete: true)
                .ForeignKey("Bot.Bots", t => t.Bot_BotId)
                .Index(t => t.BotId)
                .Index(t => t.Bot_BotId);
            
            CreateTable(
                "Trading.Orders",
                c => new
                    {
                        OrderId = c.Long(nullable: false, identity: true),
                        OurRef = c.Guid(nullable: false),
                        Symbol = c.String(maxLength: 10, unicode: false),
                        ExchangeOrderId = c.String(),
                        Price = c.Double(nullable: false),
                        Quantity = c.Double(nullable: false),
                        QuantityFilled = c.Double(nullable: false),
                        OrderDate = c.DateTime(nullable: false),
                        Status = c.Int(nullable: false),
                        Side = c.Int(nullable: false),
                        Type = c.Int(nullable: false),
                        TimeStamp = c.DateTime(),
                        PositionId = c.Long(),
                        BotId = c.Long(nullable: false),
                        CreationTime = c.DateTime(nullable: false),
                        CreationUser = c.String(nullable: false, maxLength: 250),
                        LastUpdateTime = c.DateTime(),
                        LastUpdateUser = c.String(maxLength: 250),
                    })
                .PrimaryKey(t => t.OrderId)
                .ForeignKey("Bot.Bots", t => t.BotId, cascadeDelete: true)
                .ForeignKey("Trading.Positions", t => t.PositionId)
                .Index(t => t.PositionId)
                .Index(t => t.BotId);
            
            CreateTable(
                "Bot.Indicators",
                c => new
                    {
                        IndicatorId = c.Long(nullable: false, identity: true),
                        IndicatorType = c.Int(nullable: false),
                        CreationTime = c.DateTime(nullable: false),
                        CreationUser = c.String(nullable: false, maxLength: 250),
                        LastUpdateTime = c.DateTime(),
                        LastUpdateUser = c.String(maxLength: 250),
                        Bot_BotId = c.Long(nullable: false),
                    })
                .PrimaryKey(t => t.IndicatorId)
                .ForeignKey("Bot.Bots", t => t.Bot_BotId, cascadeDelete: true)
                .Index(t => t.Bot_BotId);
            
            CreateTable(
                "Bot.RuleSets",
                c => new
                    {
                        RuleSetId = c.Long(nullable: false, identity: true),
                        RuleSide = c.Int(nullable: false),
                        CreationTime = c.DateTime(nullable: false),
                        CreationUser = c.String(nullable: false, maxLength: 250),
                        LastUpdateTime = c.DateTime(),
                        LastUpdateUser = c.String(maxLength: 250),
                        Indicator_IndicatorId = c.Long(),
                        Safety_SafetyId = c.Long(),
                    })
                .PrimaryKey(t => t.RuleSetId)
                .ForeignKey("Bot.Indicators", t => t.Indicator_IndicatorId)
                .ForeignKey("Bot.Safeties", t => t.Safety_SafetyId)
                .Index(t => t.Indicator_IndicatorId)
                .Index(t => t.Safety_SafetyId);
            
            CreateTable(
                "Bot.Rules",
                c => new
                    {
                        RuleId = c.Long(nullable: false, identity: true),
                        IndicatorRule = c.Int(nullable: false),
                        Value = c.Decimal(precision: 38, scale: 18),
                        CreationTime = c.DateTime(nullable: false),
                        CreationUser = c.String(nullable: false, maxLength: 250),
                        LastUpdateTime = c.DateTime(),
                        LastUpdateUser = c.String(maxLength: 250),
                        RuleSet_RuleSetId = c.Long(),
                    })
                .PrimaryKey(t => t.RuleId)
                .ForeignKey("Bot.RuleSets", t => t.RuleSet_RuleSetId)
                .Index(t => t.RuleSet_RuleSetId);
            
            CreateTable(
                "Bot.Safeties",
                c => new
                    {
                        SafetyId = c.Long(nullable: false, identity: true),
                        SafetyType = c.Int(nullable: false),
                        CreationTime = c.DateTime(nullable: false),
                        CreationUser = c.String(nullable: false, maxLength: 250),
                        LastUpdateTime = c.DateTime(),
                        LastUpdateUser = c.String(maxLength: 250),
                        Bot_BotId = c.Long(),
                    })
                .PrimaryKey(t => t.SafetyId)
                .ForeignKey("Bot.Bots", t => t.Bot_BotId)
                .Index(t => t.Bot_BotId);
            
            CreateTable(
                "Market.Candles",
                c => new
                    {
                        CandleId = c.Long(nullable: false, identity: true),
                        ExchangeName = c.String(),
                        Name = c.String(),
                        Timestamp = c.DateTime(nullable: false),
                        PeriodSeconds = c.Int(nullable: false),
                        OpenPrice = c.Decimal(nullable: false, precision: 38, scale: 18),
                        HighPrice = c.Decimal(nullable: false, precision: 38, scale: 18),
                        LowPrice = c.Decimal(nullable: false, precision: 38, scale: 18),
                        ClosePrice = c.Decimal(nullable: false, precision: 38, scale: 18),
                        VolumePrice = c.Double(nullable: false),
                        VolumeQuantity = c.Double(nullable: false),
                        WeightedAverage = c.Decimal(nullable: false, precision: 38, scale: 18),
                        CreationTime = c.DateTime(nullable: false),
                        CreationUser = c.String(nullable: false, maxLength: 250),
                        LastUpdateTime = c.DateTime(),
                        LastUpdateUser = c.String(maxLength: 250),
                    })
                .PrimaryKey(t => t.CandleId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("Bot.Bots", "User_UserId", "Account.Users");
            DropForeignKey("Bot.Safeties", "Bot_BotId", "Bot.Bots");
            DropForeignKey("Trading.Positions", "Bot_BotId", "Bot.Bots");
            DropForeignKey("Bot.RuleSets", "Safety_SafetyId", "Bot.Safeties");
            DropForeignKey("Bot.Rules", "RuleSet_RuleSetId", "Bot.RuleSets");
            DropForeignKey("Bot.RuleSets", "Indicator_IndicatorId", "Bot.Indicators");
            DropForeignKey("Bot.Indicators", "Bot_BotId", "Bot.Bots");
            DropForeignKey("Bot.Bots", "Exchange_ExchangeId", "Market.Exchanges");
            DropForeignKey("Bot.Bots", "CurrentPositionId", "Trading.Positions");
            DropForeignKey("Trading.Orders", "PositionId", "Trading.Positions");
            DropForeignKey("Trading.Orders", "BotId", "Bot.Bots");
            DropForeignKey("Trading.Positions", "BotId", "Bot.Bots");
            DropForeignKey("Bot.Bots", "Coin_CoinId", "Market.Coins");
            DropForeignKey("Bot.Bots", "BaseCoin_CoinId", "Market.Coins");
            DropForeignKey("Account.ApiSettings", "User_UserId", "Account.Users");
            DropForeignKey("Account.ApiSettings", "Exchange_ExchangeId", "Market.Exchanges");
            DropIndex("Bot.Safeties", new[] { "Bot_BotId" });
            DropIndex("Bot.Rules", new[] { "RuleSet_RuleSetId" });
            DropIndex("Bot.RuleSets", new[] { "Safety_SafetyId" });
            DropIndex("Bot.RuleSets", new[] { "Indicator_IndicatorId" });
            DropIndex("Bot.Indicators", new[] { "Bot_BotId" });
            DropIndex("Trading.Orders", new[] { "BotId" });
            DropIndex("Trading.Orders", new[] { "PositionId" });
            DropIndex("Trading.Positions", new[] { "Bot_BotId" });
            DropIndex("Trading.Positions", new[] { "BotId" });
            DropIndex("Bot.Bots", new[] { "User_UserId" });
            DropIndex("Bot.Bots", new[] { "Exchange_ExchangeId" });
            DropIndex("Bot.Bots", new[] { "Coin_CoinId" });
            DropIndex("Bot.Bots", new[] { "BaseCoin_CoinId" });
            DropIndex("Bot.Bots", new[] { "CurrentPositionId" });
            DropIndex("Account.ApiSettings", new[] { "User_UserId" });
            DropIndex("Account.ApiSettings", new[] { "Exchange_ExchangeId" });
            DropTable("Market.Candles");
            DropTable("Bot.Safeties");
            DropTable("Bot.Rules");
            DropTable("Bot.RuleSets");
            DropTable("Bot.Indicators");
            DropTable("Trading.Orders");
            DropTable("Trading.Positions");
            DropTable("Market.Coins");
            DropTable("Bot.Bots");
            DropTable("Account.Users");
            DropTable("Market.Exchanges");
            DropTable("Account.ApiSettings");
        }
    }
}
