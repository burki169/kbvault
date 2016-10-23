namespace KBVault.Dal.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddeDIconForCategory : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Category", "Icon", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Category", "Icon");
        }
    }
}
