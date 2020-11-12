namespace CryptoBot.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class NotMapped : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("Bot.Bots", "CurrentPositionId", "Trading.Positions");
            DropIndex("Bot.Bots", new[] { "CurrentPositionId" });
            DropIndex("Trading.Positions", new[] { "BotId" });
            DropIndex("Trading.Positions", new[] { "Bot_BotId" });
            //DropColumn("Trading.Positions", "BotId");
            //RenameColumn(table: "Trading.Positions", name: "Bot_BotId", newName: "BotId");
            AlterColumn("Trading.Positions", "BotId", c => c.Long(nullable: false));
            CreateIndex("Trading.Positions", "BotId");
        }
        
        public override void Down()
        {
            DropIndex("Trading.Positions", new[] { "BotId" });
            AlterColumn("Trading.Positions", "BotId", c => c.Long());
            RenameColumn(table: "Trading.Positions", name: "BotId", newName: "Bot_BotId");
            AddColumn("Trading.Positions", "BotId", c => c.Long(nullable: false));
            CreateIndex("Trading.Positions", "Bot_BotId");
            CreateIndex("Trading.Positions", "BotId");
            CreateIndex("Bot.Bots", "CurrentPositionId");
            AddForeignKey("Bot.Bots", "CurrentPositionId", "Trading.Positions", "PositionId");
        }
    }
}
