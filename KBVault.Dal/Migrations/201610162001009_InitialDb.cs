namespace KBVault.Dal.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialDb : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Activities",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        UserId = c.Long(nullable: false),
                        ActivityDate = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                        Operation = c.String(nullable: false, maxLength: 50),
                        Information = c.String(maxLength: 500),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.KbUser", t => t.UserId)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.KbUser",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        UserName = c.String(nullable: false, maxLength: 50),
                        Password = c.String(nullable: false, maxLength: 500),
                        Name = c.String(maxLength: 50),
                        LastName = c.String(maxLength: 50),
                        Email = c.String(maxLength: 200),
                        Role = c.String(nullable: false, maxLength: 50),
                        Author = c.Long(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.KbUser", t => t.Author)
                .Index(t => t.Author);
            
            CreateTable(
                "dbo.Article",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        Title = c.String(nullable: false, maxLength: 200),
                        Content = c.String(),
                        Views = c.Int(nullable: false),
                        Likes = c.Int(nullable: false),
                        Created = c.DateTime(),
                        Edited = c.DateTime(),
                        IsDraft = c.Int(nullable: false),
                        PublishStartDate = c.DateTime(),
                        PublishEndDate = c.DateTime(),
                        Author = c.Long(nullable: false),
                        CategoryId = c.Int(nullable: false),
                        SefName = c.String(nullable: false, maxLength: 200, unicode: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Category", t => t.CategoryId)
                .ForeignKey("dbo.KbUser", t => t.Author)
                .Index(t => t.Author)
                .Index(t => t.CategoryId);
            
            CreateTable(
                "dbo.ArticleTag",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        TagId = c.Long(nullable: false),
                        ArticleId = c.Long(nullable: false),
                        Author = c.Long(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Tag", t => t.TagId)
                .ForeignKey("dbo.Article", t => t.ArticleId)
                .ForeignKey("dbo.KbUser", t => t.Author)
                .Index(t => t.TagId)
                .Index(t => t.ArticleId)
                .Index(t => t.Author);
            
            CreateTable(
                "dbo.Tag",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 50),
                        Author = c.Long(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.KbUser", t => t.Author)
                .Index(t => t.Author);
            
            CreateTable(
                "dbo.Attachment",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        ArticleId = c.Long(nullable: false),
                        Path = c.String(nullable: false, maxLength: 2048),
                        FileName = c.String(nullable: false, maxLength: 2048),
                        Extension = c.String(nullable: false, maxLength: 5),
                        Downloads = c.Int(nullable: false),
                        Hash = c.String(maxLength: 256),
                        HashTime = c.DateTime(),
                        MimeType = c.String(maxLength: 50, unicode: false),
                        Author = c.Long(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Article", t => t.ArticleId)
                .ForeignKey("dbo.KbUser", t => t.Author)
                .Index(t => t.ArticleId)
                .Index(t => t.Author);
            
            CreateTable(
                "dbo.Category",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 200),
                        IsHot = c.Boolean(nullable: false),
                        Parent = c.Int(),
                        SefName = c.String(nullable: false, maxLength: 200, unicode: false),
                        Author = c.Long(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Category", t => t.Parent)
                .ForeignKey("dbo.KbUser", t => t.Author)
                .Index(t => t.Parent)
                .Index(t => t.Author);
            
            CreateTable(
                "dbo.Settings",
                c => new
                    {
                        CompanyName = c.String(nullable: false, maxLength: 100),
                        TagLine = c.String(maxLength: 500),
                        JumbotronText = c.String(maxLength: 100),
                        ArticleCountPerCategoryOnHomePage = c.Short(nullable: false),
                        ShareThisPublicKey = c.String(maxLength: 50),
                        DisqusShortName = c.String(maxLength: 150),
                        IndexFileExtensions = c.String(maxLength: 2000, unicode: false),
                        ArticlePrefix = c.String(maxLength: 50),
                        AnalyticsAccount = c.String(maxLength: 50),
                        Author = c.Long(nullable: false),
                        BackupPath = c.String(maxLength: 2000),
                    })
                .PrimaryKey(t => t.CompanyName)
                .ForeignKey("dbo.KbUser", t => t.Author)
                .Index(t => t.Author);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Tag", "Author", "dbo.KbUser");
            DropForeignKey("dbo.Settings", "Author", "dbo.KbUser");
            DropForeignKey("dbo.KbUser", "Author", "dbo.KbUser");
            DropForeignKey("dbo.Category", "Author", "dbo.KbUser");
            DropForeignKey("dbo.Attachment", "Author", "dbo.KbUser");
            DropForeignKey("dbo.ArticleTag", "Author", "dbo.KbUser");
            DropForeignKey("dbo.Article", "Author", "dbo.KbUser");
            DropForeignKey("dbo.Category", "Parent", "dbo.Category");
            DropForeignKey("dbo.Article", "CategoryId", "dbo.Category");
            DropForeignKey("dbo.Attachment", "ArticleId", "dbo.Article");
            DropForeignKey("dbo.ArticleTag", "ArticleId", "dbo.Article");
            DropForeignKey("dbo.ArticleTag", "TagId", "dbo.Tag");
            DropForeignKey("dbo.Activities", "UserId", "dbo.KbUser");
            DropIndex("dbo.Settings", new[] { "Author" });
            DropIndex("dbo.Category", new[] { "Author" });
            DropIndex("dbo.Category", new[] { "Parent" });
            DropIndex("dbo.Attachment", new[] { "Author" });
            DropIndex("dbo.Attachment", new[] { "ArticleId" });
            DropIndex("dbo.Tag", new[] { "Author" });
            DropIndex("dbo.ArticleTag", new[] { "Author" });
            DropIndex("dbo.ArticleTag", new[] { "ArticleId" });
            DropIndex("dbo.ArticleTag", new[] { "TagId" });
            DropIndex("dbo.Article", new[] { "CategoryId" });
            DropIndex("dbo.Article", new[] { "Author" });
            DropIndex("dbo.KbUser", new[] { "Author" });
            DropIndex("dbo.Activities", new[] { "UserId" });
            DropTable("dbo.Settings");
            DropTable("dbo.Category");
            DropTable("dbo.Attachment");
            DropTable("dbo.Tag");
            DropTable("dbo.ArticleTag");
            DropTable("dbo.Article");
            DropTable("dbo.KbUser");
            DropTable("dbo.Activities");
        }
    }
}
