namespace steamStats.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class newModelAdded1 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.UserGamesModels",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        username = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            AddColumn("dbo.Games", "UserGamesModel_Id", c => c.Int());
            CreateIndex("dbo.Games", "UserGamesModel_Id");
            AddForeignKey("dbo.Games", "UserGamesModel_Id", "dbo.UserGamesModels", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Games", "UserGamesModel_Id", "dbo.UserGamesModels");
            DropIndex("dbo.Games", new[] { "UserGamesModel_Id" });
            DropColumn("dbo.Games", "UserGamesModel_Id");
            DropTable("dbo.UserGamesModels");
        }
    }
}
