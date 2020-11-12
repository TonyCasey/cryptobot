namespace CryptoBot.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Indicator_ForeignKey : DbMigration
    {
        public override void Up()
        {
            DropIndex("Bot.RuleSets", new[] { "IndicatorId" });
            DropIndex("Bot.RuleSets", new[] { "SafetyId" });
            RenameColumn(table: "Bot.Indicators", name: "Bot_BotId", newName: "BotId");
            RenameIndex(table: "Bot.Indicators", name: "IX_Bot_BotId", newName: "IX_BotId");
            AlterColumn("Bot.RuleSets", "IndicatorId", c => c.Long());
            AlterColumn("Bot.RuleSets", "SafetyId", c => c.Long());
            CreateIndex("Bot.RuleSets", "IndicatorId");
            CreateIndex("Bot.RuleSets", "SafetyId");
        }
        
        public override void Down()
        {
            DropIndex("Bot.RuleSets", new[] { "SafetyId" });
            DropIndex("Bot.RuleSets", new[] { "IndicatorId" });
            AlterColumn("Bot.RuleSets", "SafetyId", c => c.Long(nullable: false));
            AlterColumn("Bot.RuleSets", "IndicatorId", c => c.Long(nullable: false));
            RenameIndex(table: "Bot.Indicators", name: "IX_BotId", newName: "IX_Bot_BotId");
            RenameColumn(table: "Bot.Indicators", name: "BotId", newName: "Bot_BotId");
            CreateIndex("Bot.RuleSets", "SafetyId");
            CreateIndex("Bot.RuleSets", "IndicatorId");
        }
    }
}
