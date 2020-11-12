namespace CryptoBot.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class BuyAndSellIndicatorFlags : DbMigration
    {
        public override void Up()
        {
            AddColumn("Bot.Indicators", "UseForBuy", c => c.Boolean(nullable: false));
            AddColumn("Bot.Indicators", "UseForSell", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("Bot.Indicators", "UseForSell");
            DropColumn("Bot.Indicators", "UseForBuy");
        }
    }
}
