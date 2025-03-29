namespace steamStats.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addedBool : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Games", "IsInPopular", c => c.Boolean(nullable: false));
            AddColumn("dbo.Games", "IsInFavorite", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Games", "IsInFavorite");
            DropColumn("dbo.Games", "IsInPopular");
        }
    }
}
