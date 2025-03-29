namespace steamStats.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddLastUpdatedToGame : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Games", "lastUpdated", c => c.DateTime(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Games", "lastUpdated");
        }
    }
}
