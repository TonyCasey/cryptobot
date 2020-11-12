namespace CryptoBot.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class precision : DbMigration
    {
        public override void Up()
        {
            AddColumn("Bot.Bots", "Accumulator", c => c.Boolean(nullable: false));
            AddColumn("Market.Coins", "OrderRoundingExponent", c => c.Int(nullable: false));
            DropColumn("Bot.Bots", "CompoundAmount");

            Sql("Update Market.Coins Set OrderRoundingExponent = 2 Where Code in ('USDT', 'EUR')");
        }
        
        public override void Down()
        {
            AddColumn("Bot.Bots", "CompoundAmount", c => c.Boolean(nullable: false));
            DropColumn("Market.Coins", "OrderRoundingExponent");
            DropColumn("Bot.Bots", "Accumulator");
        }
    }
}
