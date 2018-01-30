namespace KBVault.Dal.Migrations
{
    using System;
    using System.Data.Entity.Migrations;

#pragma warning disable SA1300 // Element must begin with upper-case letter
    public partial class addTotalArticleCountToSettings : DbMigration
#pragma warning restore SA1300 // Element must begin with upper-case letter
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
