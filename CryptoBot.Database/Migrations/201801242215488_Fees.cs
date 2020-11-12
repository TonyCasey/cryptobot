namespace CryptoBot.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Fees : DbMigration
    {
        public override void Up()
        {
            AddColumn("Trading.Orders", "Fees", c => c.Double(nullable: false));
            DropTable("Lookup.Enumerations");
        }
        
        public override void Down()
        {
            CreateTable(
                "Lookup.Enumerations",
                c => new
                    {
                        EnumerationId = c.Int(nullable: false, identity: true),
                        Group = c.String(),
                        Name = c.String(),
                        Value = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.EnumerationId);
            
            DropColumn("Trading.Orders", "Fees");
        }
    }
}
