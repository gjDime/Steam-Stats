namespace steamStats.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class usergames : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Games", "ApplicationUser_Id", c => c.String(maxLength: 128));
            CreateIndex("dbo.Games", "ApplicationUser_Id");
            AddForeignKey("dbo.Games", "ApplicationUser_Id", "dbo.AspNetUsers", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Games", "ApplicationUser_Id", "dbo.AspNetUsers");
            DropIndex("dbo.Games", new[] { "ApplicationUser_Id" });
            DropColumn("dbo.Games", "ApplicationUser_Id");
        }
    }
}
