namespace CryptoBot.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CompoundAmount : DbMigration
    {
        public override void Up()
        {
            AddColumn("Bot.Bots", "CompoundAmount", c => c.Boolean(nullable: false));
            Sql("Update Bot.Bots Set CompoundAmount = 1");
        }
        
        public override void Down()
        {
            DropColumn("Bot.Bots", "CompoundAmount");
        }
    }
}
