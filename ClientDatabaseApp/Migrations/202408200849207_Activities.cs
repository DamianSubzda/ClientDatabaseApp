namespace ClientDatabaseApp.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Activities : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Event", "ClientId", "dbo.Client");
            DropIndex("dbo.Event", new[] { "ClientId" });
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
            
            DropTable("dbo.Event");
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.Event",
                c => new
                    {
                        EventId = c.Int(nullable: false, identity: true),
                        ClientId = c.Int(nullable: false),
                        Note = c.String(maxLength: 2000),
                        DateOfCreation = c.DateTime(nullable: false),
                        DateOfAction = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.EventId);
            
            DropForeignKey("dbo.Activity", "ClientId", "dbo.Client");
            DropIndex("dbo.Activity", new[] { "ClientId" });
            DropTable("dbo.Activity");
            CreateIndex("dbo.Event", "ClientId");
            AddForeignKey("dbo.Event", "ClientId", "dbo.Client", "ClientId", cascadeDelete: true);
        }
    }
}
