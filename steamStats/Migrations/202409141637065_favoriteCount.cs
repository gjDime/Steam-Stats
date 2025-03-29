namespace steamStats.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class favoriteCount : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Games", "InFavoriteCount", c => c.Int(nullable: false));
            DropColumn("dbo.Games", "IsInFavorite");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Games", "IsInFavorite", c => c.Boolean(nullable: false));
            DropColumn("dbo.Games", "InFavoriteCount");
        }
    }
}
