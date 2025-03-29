namespace steamStats.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class timeremoved : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.Games", "lastUpdated");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Games", "lastUpdated", c => c.DateTime(nullable: false));
        }
    }
}
