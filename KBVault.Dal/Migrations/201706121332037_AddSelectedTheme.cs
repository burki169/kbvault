namespace KBVault.Dal.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddSelectedTheme : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Settings", "SelectedTheme", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Settings", "SelectedTheme");
        }
    }
}
