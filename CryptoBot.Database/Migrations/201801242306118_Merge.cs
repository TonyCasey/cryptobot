namespace CryptoBot.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Merge : DbMigration
    {
        public override void Up()
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
            
        }
        
        public override void Down()
        {
            DropTable("Lookup.Enumerations");
        }
    }
}
