using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Infrastructure;
using System.Data.SqlClient;
using System.Linq;
using KBVault.Dal.Entities;

namespace KBVault.Dal
{
    public partial class KbVaultContext : DbContext
    {
        public KbVaultContext()
            : base("name=KbVaultContext")
        {
        }

        public virtual DbSet<Activities> Activities { get; set; }
        public virtual DbSet<Article> Articles { get; set; }
        public virtual DbSet<ArticleTag> ArticleTags { get; set; }
        public virtual DbSet<Attachment> Attachments { get; set; }
        public virtual DbSet<Category> Categories { get; set; }
        public virtual DbSet<Entities.KbUser> KbUsers { get; set; }
        public virtual DbSet<Settings> Settings { get; set; }
        public virtual DbSet<Tag> Tags { get; set; }

        public override int SaveChanges()
        {
            ChangeTracker.DetectChanges(); // Important!
            ObjectContext ctx = ((IObjectContextAdapter)this).ObjectContext;

            List<ObjectStateEntry> objectStateEntryList =
                ctx.ObjectStateManager.GetObjectStateEntries(EntityState.Added
                                                           | EntityState.Modified
                                                           | EntityState.Deleted)
                .ToList();

            foreach (ObjectStateEntry entry in objectStateEntryList)
            {
                var props = entry.GetModifiedProperties();
                string modifiedProperty = props.FirstOrDefault();
                bool isUserViewAction = props.Count() == 1 && modifiedProperty == "Views";
                bool isProfileUpdateAction = entry.Entity is KbUser;
                if (!entry.IsRelationship && !isUserViewAction && !isProfileUpdateAction)
                {
                    var operationDescription = string.Empty;

                    var act = new Activities
                    {
                        ActivityDate = DateTime.Now
                    };

                    switch (entry.State)
                    {
                        case EntityState.Added:
                            operationDescription = "Added ";
                            break;
                        case EntityState.Deleted:
                            operationDescription = "Deleted ";
                            break;
                        case EntityState.Modified:
                            operationDescription = "Modified ";
                            break;
                        default:
                            break;
                    }

                    if (entry.Entity is Article)
                    {
                        var a = (Article)entry.Entity;
                        operationDescription += " Article ";
                        act.Information = "Title: " + a.Title + " Id:" + a.Id.ToString();
                        act.UserId = a.Author;
                    }
                    else if (entry.Entity is Category)
                    {
                        var c = (Category)entry.Entity;
                        operationDescription += " Category ";
                        act.Information = "Name: " + c.Name + " Id:" + c.Id.ToString();
                        act.UserId = c.Author;
                    }
                    else if (entry.Entity is Tag)
                    {
                        var t = (Tag)entry.Entity;
                        operationDescription += " Tag ";
                        act.Information = "Name: " + t.Name + " Id:" + t.Id.ToString();
                        act.UserId = t.Author;
                    }
                    else if (entry.Entity is ArticleTag)
                    {
                        var at = (ArticleTag)entry.Entity;
                        operationDescription += " ArticleTag ";
                        act.Information = "ArticleId: " + at.ArticleId + " TagId:" + at.TagId.ToString();
                        act.UserId = at.Author;
                    }
                    else if (entry.Entity is Attachment)
                    {
                        var a = (Attachment)entry.Entity;
                        operationDescription += " Attachment ";
                        act.Information = "ArticleId: " + a.ArticleId + " Id:" + a.Id.ToString();
                        act.UserId = a.Author;
                    }
                    else if (entry.Entity is Settings)
                    {
                        var s = (Settings)entry.Entity;
                        operationDescription += " Settings ";
                        act.Information = "Settings updated";
                        act.UserId = s.Author;
                    }

                    act.Operation = operationDescription;
                    List<SqlParameter> procParams = new List<SqlParameter>
                    {
                        new SqlParameter("1", act.UserId),
                        new SqlParameter("2", act.ActivityDate),
                        new SqlParameter("3", act.Operation),
                        new SqlParameter("4", act.Information)
                    };

                    this.Database.ExecuteSqlCommand(
                        "Insert Into Activities(UserId,ActivityDate,Operation,Information) " +
                        "Values(@1,@2,@3,@4 )",
                        procParams.ToArray());
                    /*
                    switch (entry.State)
                    {
                        case EntityState.Added:
                            // write log...
                            break;
                        case EntityState.Deleted:
                            // write log...
                            break;
                        case EntityState.Modified:
                            {
                                foreach (string propertyName in
                                             entry.GetModifiedProperties())
                                {
                                    DbDataRecord original = entry.OriginalValues;
                                    string oldValue = original.GetValue(
                                        original.GetOrdinal(propertyName))
                                        .ToString();

                                    CurrentValueRecord current = entry.CurrentValues;
                                    string newValue = current.GetValue(
                                        current.GetOrdinal(propertyName))
                                        .ToString();

                                    if (oldValue != newValue) // probably not necessary
                                    {
                                        Log.WriteAudit(
                                            "Entry: {0} Original :{1} New: {2}",
                                            entry.Entity.GetType().Name,
                                            oldValue, newValue);
                                    }
                                }
                                break;
                            }
                    }*/
                }
            }

            return base.SaveChanges();
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Article>()
                .Property(e => e.SefName)
                .IsUnicode(false);

            modelBuilder.Entity<Article>()
                .HasMany(e => e.ArticleTags)
                .WithRequired(e => e.Article)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Article>()
                .HasMany(e => e.Attachments)
                .WithRequired(e => e.Article)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Attachment>()
                .Property(e => e.MimeType)
                .IsUnicode(false);

            modelBuilder.Entity<Category>()
                .Property(e => e.SefName)
                .IsUnicode(false);

            modelBuilder.Entity<Category>()
                .HasMany(e => e.Articles)
                .WithRequired(e => e.Category)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Category>()
                .HasMany(e => e.ChildCategories)
                .WithOptional(e => e.ParentCategory)
                .HasForeignKey(e => e.Parent);

            modelBuilder.Entity<Entities.KbUser>()
                .HasMany(e => e.Activities)
                .WithRequired(e => e.KbUser)
                .HasForeignKey(e => e.UserId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Entities.KbUser>()
                .HasMany(e => e.Article)
                .WithRequired(e => e.KbUser)
                .HasForeignKey(e => e.Author)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Entities.KbUser>()
                .HasMany(e => e.ArticleTag)
                .WithRequired(e => e.KbUser)
                .HasForeignKey(e => e.Author)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Entities.KbUser>()
                .HasMany(e => e.Attachment)
                .WithRequired(e => e.KbUser)
                .HasForeignKey(e => e.Author)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Entities.KbUser>()
                .HasMany(e => e.Category)
                .WithRequired(e => e.KbUser)
                .HasForeignKey(e => e.Author)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Entities.KbUser>()
                .HasMany(e => e.KbUser1)
                .WithRequired(e => e.KbUser2)
                .HasForeignKey(e => e.Author);

            modelBuilder.Entity<Entities.KbUser>()
                .HasMany(e => e.Settings)
                .WithRequired(e => e.KbUser)
                .HasForeignKey(e => e.Author)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Entities.KbUser>()
                .HasMany(e => e.Tag)
                .WithRequired(e => e.KbUser)
                .HasForeignKey(e => e.Author)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Settings>()
                .Property(e => e.IndexFileExtensions)
                .IsUnicode(false);

            modelBuilder.Entity<Tag>()
                .HasMany(e => e.ArticleTag)
                .WithRequired(e => e.Tag)
                .WillCascadeOnDelete(false);
        }
    }
}
