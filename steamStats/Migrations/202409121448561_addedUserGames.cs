namespace steamStats.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addedUserGames : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Games", "ApplicationUser_Id", "dbo.AspNetUsers");
            DropIndex("dbo.Games", new[] { "ApplicationUser_Id" });
            CreateTable(
                "dbo.ApplicationUserGames",
                c => new
                    {
                        ApplicationUser_Id = c.String(nullable: false, maxLength: 128),
                        Game_Id = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => new { t.ApplicationUser_Id, t.Game_Id })
                .ForeignKey("dbo.AspNetUsers", t => t.ApplicationUser_Id, cascadeDelete: true)
                .ForeignKey("dbo.Games", t => t.Game_Id, cascadeDelete: true)
                .Index(t => t.ApplicationUser_Id)
                .Index(t => t.Game_Id);
            
            DropColumn("dbo.Games", "ApplicationUser_Id");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Games", "ApplicationUser_Id", c => c.String(maxLength: 128));
            DropForeignKey("dbo.ApplicationUserGames", "Game_Id", "dbo.Games");
            DropForeignKey("dbo.ApplicationUserGames", "ApplicationUser_Id", "dbo.AspNetUsers");
            DropIndex("dbo.ApplicationUserGames", new[] { "Game_Id" });
            DropIndex("dbo.ApplicationUserGames", new[] { "ApplicationUser_Id" });
            DropTable("dbo.ApplicationUserGames");
            CreateIndex("dbo.Games", "ApplicationUser_Id");
            AddForeignKey("dbo.Games", "ApplicationUser_Id", "dbo.AspNetUsers", "Id");
        }
    }
}
