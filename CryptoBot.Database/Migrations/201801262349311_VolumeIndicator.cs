namespace CryptoBot.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class VolumeIndicator : DbMigration
    {
        public override void Up()
        {
            AddColumn("Bot.Rules", "IndicatorRuleTypeId", c => c.Int(nullable: false));
            DropColumn("Bot.Rules", "IndicatorRule");
        }
        
        public override void Down()
        {
            AddColumn("Bot.Rules", "IndicatorRule", c => c.Int(nullable: false));
            DropColumn("Bot.Rules", "IndicatorRuleTypeId");
        }
    }
}
