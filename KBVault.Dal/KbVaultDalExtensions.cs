using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Infrastructure;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KBVault.Dal
{
    public partial class KbVaultEntities:DbContext
    {
        public override int SaveChanges()
        {
            /* http://stackoverflow.com/a/6157071 */

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
                // if only 
                if (!entry.IsRelationship && !isUserViewAction)
                {
                    string operationDescription = "";

                    Activity act = new Activity();
                    act.ActivityDate = DateTime.Now;

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
                    if( entry.Entity is Article )
                    {
                        Article a = ((Article)entry.Entity);
                        operationDescription += " Article ";
                        act.Information = "Title: " + a.Title + " Id:" + a.Id.ToString();
                        act.UserId = a.Author;
                    }
                    else if (entry.Entity is Category)
                    {
                        Category c = ((Category)entry.Entity);
                        operationDescription += " Category ";
                        act.Information = "Name: " + c.Name + " Id:" + c.Id.ToString();
                        act.UserId = c.Author;
                    }
                    else if (entry.Entity is Tag)
                    {
                        Tag t = ((Tag)entry.Entity);
                        operationDescription += " Tag ";
                        act.Information = "Name: " + t.Name + " Id:" + t.Id.ToString();
                        act.UserId = t.Author;
                    }
                    else if (entry.Entity is ArticleTag)
                    {
                        ArticleTag at = ((ArticleTag)entry.Entity);
                        operationDescription += " ArticleTag ";
                        act.Information = "ArticleId: " + at.ArticleId+ " TagId:" + at.TagId.ToString();
                        act.UserId = at.Author;
                    }
                    else if (entry.Entity is Attachment)
                    {
                        Attachment a = ((Attachment)entry.Entity);
                        operationDescription += " Attachment ";
                        act.Information = "ArticleId: " + a.ArticleId+ " Id:" + a.Id.ToString();
                        act.UserId = a.Author;
                    }
                    else if (entry.Entity is Setting)
                    {
                        Setting s = ((Setting)entry.Entity);
                        operationDescription += " Settings ";
                        act.Information = "Settings updated";
                        act.UserId = s.Author;
                    }
                    act.Operation = operationDescription;
                    List<SqlParameter> procParams = new List<SqlParameter>();
                    procParams.Add( new SqlParameter("1",act.UserId ) );
                    procParams.Add( new SqlParameter("2",act.ActivityDate) );
                    procParams.Add( new SqlParameter("3",act.Operation ) );
                    procParams.Add( new SqlParameter("4",act.Information ) );
                    
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
    }

    public static class KbVaultDalExtensions
    {
        public static IQueryable<Article> PublishedArticles(this KbVaultEntities db)
        {
            try
            {
                DateTime today = DateTime.Now.Date;
                return db.Articles
                        .Include("Category")
                        .Include("ArticleTags.Tag")
                        .Include("Attachments")
                        .Where( a => a.PublishStartDate <= today &&
                                a.PublishEndDate >= today &&
                                a.IsDraft == 0
                        );
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
