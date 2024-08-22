namespace ClientDatabaseApp.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialMigration : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Activity",
                c => new
                    {
                        ActivityId = c.Int(nullable: false, identity: true),
                        ClientId = c.Int(nullable: false),
                        Note = c.String(maxLength: 2000),
                        DateOfCreation = c.DateTime(nullable: false),
                        DateOfAction = c.DateTime(),
                    })
                .PrimaryKey(t => t.ActivityId)
                .ForeignKey("dbo.Client", t => t.ClientId, cascadeDelete: true)
                .Index(t => t.ClientId);
            
            CreateTable(
                "dbo.Client",
                c => new
                    {
                        ClientId = c.Int(nullable: false, identity: true),
                        ClientName = c.String(maxLength: 200),
                        Phonenumber = c.String(maxLength: 30),
                        Email = c.String(maxLength: 40),
                        City = c.String(maxLength: 50),
                        Facebook = c.String(maxLength: 1000),
                        Instagram = c.String(maxLength: 1000),
                        PageURL = c.String(maxLength: 1000),
                        Data = c.DateTime(),
                        Owner = c.String(maxLength: 50),
                        Note = c.String(maxLength: 2000),
                        Status = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.ClientId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Activity", "ClientId", "dbo.Client");
            DropIndex("dbo.Activity", new[] { "ClientId" });
            DropTable("dbo.Client");
            DropTable("dbo.Activity");
        }
    }
}
