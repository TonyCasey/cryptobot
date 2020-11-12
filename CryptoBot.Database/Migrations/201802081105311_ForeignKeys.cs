namespace CryptoBot.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ForeignKeys : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("Account.ApiSettings", "Exchange_ExchangeId", "Market.Exchanges");
            DropForeignKey("Account.ApiSettings", "User_UserId", "Account.Users");
            DropForeignKey("Account.MessagingApps", "User_UserId", "Account.Users");
            DropForeignKey("Account.MessagingAppSettings", "MessagingApp_MessagingAppId", "Account.MessagingApps");
            DropForeignKey("Bot.Bots", "Coin_CoinId", "Market.Coins");
            DropForeignKey("Bot.Bots", "Exchange_ExchangeId", "Market.Exchanges");
            DropForeignKey("Bot.Indicators", "Bot_BotId", "Bot.Bots");
            DropForeignKey("Trading.Orders", "BotId", "Bot.Bots");
            DropForeignKey("Bot.Bots", "User_UserId", "Account.Users");
            DropForeignKey("Trading.Positions", "BotId", "Bot.Bots");
            DropIndex("Bot.Bots", new[] { "BaseCoin_CoinId" });
            RenameColumn(table: "Bot.Bots", name: "BaseCoin_CoinId", newName: "BaseCoinId");
            RenameColumn(table: "Bot.Bots", name: "Coin_CoinId", newName: "CoinId");
            RenameColumn(table: "Bot.Bots", name: "Exchange_ExchangeId", newName: "ExchangeId");
            RenameColumn(table: "Bot.Bots", name: "User_UserId", newName: "UserId");
            RenameIndex(table: "Bot.Bots", name: "IX_User_UserId", newName: "IX_UserId");
            RenameIndex(table: "Bot.Bots", name: "IX_Coin_CoinId", newName: "IX_CoinId");
            RenameIndex(table: "Bot.Bots", name: "IX_Exchange_ExchangeId", newName: "IX_ExchangeId");
            AlterColumn("Market.Exchanges", "Name", c => c.String(maxLength: 50));
            AlterColumn("Market.Exchanges", "Code", c => c.String(maxLength: 10));
            AlterColumn("Account.Users", "Name", c => c.String(maxLength: 250));
            AlterColumn("Bot.Bots", "BaseCoinId", c => c.Long(nullable: false));
            AlterColumn("Trading.Orders", "Symbol", c => c.String(maxLength: 10));
            CreateIndex("Bot.Bots", "BaseCoinId");
            AddForeignKey("Account.ApiSettings", "Exchange_ExchangeId", "Market.Exchanges", "ExchangeId");
            AddForeignKey("Account.ApiSettings", "User_UserId", "Account.Users", "UserId");
            AddForeignKey("Account.MessagingApps", "User_UserId", "Account.Users", "UserId");
            AddForeignKey("Account.MessagingAppSettings", "MessagingApp_MessagingAppId", "Account.MessagingApps", "MessagingAppId");
            AddForeignKey("Bot.Bots", "CoinId", "Market.Coins", "CoinId");
            AddForeignKey("Bot.Bots", "ExchangeId", "Market.Exchanges", "ExchangeId");
            AddForeignKey("Bot.Indicators", "Bot_BotId", "Bot.Bots", "BotId");
            AddForeignKey("Trading.Orders", "BotId", "Bot.Bots", "BotId");
            AddForeignKey("Bot.Bots", "UserId", "Account.Users", "UserId");
            AddForeignKey("Trading.Positions", "BotId", "Bot.Bots", "BotId");
        }
        
        public override void Down()
        {
            DropForeignKey("Trading.Positions", "BotId", "Bot.Bots");
            DropForeignKey("Bot.Bots", "UserId", "Account.Users");
            DropForeignKey("Trading.Orders", "BotId", "Bot.Bots");
            DropForeignKey("Bot.Indicators", "Bot_BotId", "Bot.Bots");
            DropForeignKey("Bot.Bots", "ExchangeId", "Market.Exchanges");
            DropForeignKey("Bot.Bots", "CoinId", "Market.Coins");
            DropForeignKey("Account.MessagingAppSettings", "MessagingApp_MessagingAppId", "Account.MessagingApps");
            DropForeignKey("Account.MessagingApps", "User_UserId", "Account.Users");
            DropForeignKey("Account.ApiSettings", "User_UserId", "Account.Users");
            DropForeignKey("Account.ApiSettings", "Exchange_ExchangeId", "Market.Exchanges");
            DropIndex("Bot.Bots", new[] { "BaseCoinId" });
            AlterColumn("Trading.Orders", "Symbol", c => c.String(maxLength: 10, unicode: false));
            AlterColumn("Bot.Bots", "BaseCoinId", c => c.Long());
            AlterColumn("Account.Users", "Name", c => c.String(maxLength: 250, unicode: false));
            AlterColumn("Market.Exchanges", "Code", c => c.String(maxLength: 10, unicode: false));
            AlterColumn("Market.Exchanges", "Name", c => c.String(maxLength: 50, unicode: false));
            RenameIndex(table: "Bot.Bots", name: "IX_ExchangeId", newName: "IX_Exchange_ExchangeId");
            RenameIndex(table: "Bot.Bots", name: "IX_CoinId", newName: "IX_Coin_CoinId");
            RenameIndex(table: "Bot.Bots", name: "IX_UserId", newName: "IX_User_UserId");
            RenameColumn(table: "Bot.Bots", name: "UserId", newName: "User_UserId");
            RenameColumn(table: "Bot.Bots", name: "ExchangeId", newName: "Exchange_ExchangeId");
            RenameColumn(table: "Bot.Bots", name: "CoinId", newName: "Coin_CoinId");
            RenameColumn(table: "Bot.Bots", name: "BaseCoinId", newName: "BaseCoin_CoinId");
            CreateIndex("Bot.Bots", "BaseCoin_CoinId");
            AddForeignKey("Trading.Positions", "BotId", "Bot.Bots", "BotId", cascadeDelete: true);
            AddForeignKey("Bot.Bots", "User_UserId", "Account.Users", "UserId", cascadeDelete: true);
            AddForeignKey("Trading.Orders", "BotId", "Bot.Bots", "BotId", cascadeDelete: true);
            AddForeignKey("Bot.Indicators", "Bot_BotId", "Bot.Bots", "BotId", cascadeDelete: true);
            AddForeignKey("Bot.Bots", "Exchange_ExchangeId", "Market.Exchanges", "ExchangeId", cascadeDelete: true);
            AddForeignKey("Bot.Bots", "Coin_CoinId", "Market.Coins", "CoinId", cascadeDelete: true);
            AddForeignKey("Account.MessagingAppSettings", "MessagingApp_MessagingAppId", "Account.MessagingApps", "MessagingAppId", cascadeDelete: true);
            AddForeignKey("Account.MessagingApps", "User_UserId", "Account.Users", "UserId", cascadeDelete: true);
            AddForeignKey("Account.ApiSettings", "User_UserId", "Account.Users", "UserId", cascadeDelete: true);
            AddForeignKey("Account.ApiSettings", "Exchange_ExchangeId", "Market.Exchanges", "ExchangeId", cascadeDelete: true);
        }
    }
}
