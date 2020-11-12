namespace CryptoBot.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class MessagingApps : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "Account.MessagingApps",
                c => new
                    {
                        MessagingAppId = c.Long(nullable: false, identity: true),
                        MessagingAppType = c.Int(nullable: false),
                        Active = c.Boolean(nullable: false),
                        CreationTime = c.DateTime(nullable: false),
                        CreationUser = c.String(nullable: false, maxLength: 250),
                        LastUpdateTime = c.DateTime(),
                        LastUpdateUser = c.String(maxLength: 250),
                        User_UserId = c.Long(nullable: false),
                    })
                .PrimaryKey(t => t.MessagingAppId)
                .ForeignKey("Account.Users", t => t.User_UserId, cascadeDelete: true)
                .Index(t => t.User_UserId);
            
            CreateTable(
                "Account.MessagingAppSettings",
                c => new
                    {
                        MessagingAppSettingsId = c.Long(nullable: false, identity: true),
                        Key = c.String(),
                        Value = c.String(),
                        MessagingApp_MessagingAppId = c.Long(nullable: false),
                    })
                .PrimaryKey(t => t.MessagingAppSettingsId)
                .ForeignKey("Account.MessagingApps", t => t.MessagingApp_MessagingAppId, cascadeDelete: true)
                .Index(t => t.MessagingApp_MessagingAppId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("Account.MessagingApps", "User_UserId", "Account.Users");
            DropForeignKey("Account.MessagingAppSettings", "MessagingApp_MessagingAppId", "Account.MessagingApps");
            DropIndex("Account.MessagingAppSettings", new[] { "MessagingApp_MessagingAppId" });
            DropIndex("Account.MessagingApps", new[] { "User_UserId" });
            DropTable("Account.MessagingAppSettings");
            DropTable("Account.MessagingApps");
        }
    }
}
