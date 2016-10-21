namespace KBVault.Dal.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addTotalArticleCountToSettings : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Settings", "ShowTotalArticleCountOnFrontPage", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Settings", "ShowTotalArticleCountOnFrontPage");
        }
    }
}
