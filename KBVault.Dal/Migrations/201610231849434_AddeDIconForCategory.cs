using System.Data.Entity.Migrations;

namespace KBVault.Dal.Migrations
{
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
