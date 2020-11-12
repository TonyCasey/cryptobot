namespace CryptoBot.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RuleSet_ForeignKeys : DbMigration
    {
        public override void Up()
        {
            DropIndex("Bot.RuleSets", new[] { "Indicator_IndicatorId" });
            DropIndex("Bot.RuleSets", new[] { "Safety_SafetyId" });
            RenameColumn(table: "Bot.RuleSets", name: "Indicator_IndicatorId", newName: "IndicatorId");
            RenameColumn(table: "Bot.RuleSets", name: "Safety_SafetyId", newName: "SafetyId");
            AlterColumn("Bot.RuleSets", "IndicatorId", c => c.Long(nullable: true));
            AlterColumn("Bot.RuleSets", "SafetyId", c => c.Long(nullable: true));
            CreateIndex("Bot.RuleSets", "IndicatorId");
            CreateIndex("Bot.RuleSets", "SafetyId");
        }
        
        public override void Down()
        {
            DropIndex("Bot.RuleSets", new[] { "SafetyId" });
            DropIndex("Bot.RuleSets", new[] { "IndicatorId" });
            AlterColumn("Bot.RuleSets", "SafetyId", c => c.Long());
            AlterColumn("Bot.RuleSets", "IndicatorId", c => c.Long());
            RenameColumn(table: "Bot.RuleSets", name: "SafetyId", newName: "Safety_SafetyId");
            RenameColumn(table: "Bot.RuleSets", name: "IndicatorId", newName: "Indicator_IndicatorId");
            CreateIndex("Bot.RuleSets", "Safety_SafetyId");
            CreateIndex("Bot.RuleSets", "Indicator_IndicatorId");
        }
    }
}
