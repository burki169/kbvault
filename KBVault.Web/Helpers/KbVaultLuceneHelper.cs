using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using NLog;
using Lucene.Net.Index;
using Lucene.Net.Documents;
using Lucene.Net.Store;
using Lucene.Net.Analysis.Standard;
using KBVault.Dal;
using System.IO;
using Lucene.Net.Search;
using Lucene.Net.QueryParsers;
using KBVault.Web.Models.Public;
using System.Text.RegularExpressions;

namespace KBVault.Web.Helpers
{
    public class KbVaultLuceneHelper
    {
        private static Logger Log = LogManager.GetCurrentClassLogger();
        
        private static string LuceneIndexDirectory
        {
            get
            {
                return HttpContext.Current.Server.MapPath("~/Lucene");
            }
        }
        //
        // Taken from
        // http://www.dotnetperls.com/remove-html-tags
        //
        private static string StripTagsCharArray(string source)
        {
            char[] array = new char[source.Length];
            int arrayIndex = 0;
            bool inside = false;

            for (int i = 0; i < source.Length; i++)
            {
                char let = source[i];
                if (let == '<')
                {
                    inside = true;
                    continue;
                }
                if (let == '>')
                {
                    inside = false;
                    continue;
                }
                if (!inside)
                {
                    array[arrayIndex] = let;
                    arrayIndex++;
                }
            }
            return new string(array, 0, arrayIndex);
        }

        private static Query ParseQuery(string searchQuery, QueryParser parser)
        {            
            Query q;
            try
            {
                q = parser.Parse(searchQuery.Trim()+"*");
            }
            catch (ParseException e)
            {
                Log.Error("Query parser exception", e);
                q = null;
            }
            if (q == null || string.IsNullOrEmpty(q.ToString()))
            {
                string cooked = Regex.Replace(searchQuery, @"[^\w\.@-]", " ");
                q = parser.Parse(cooked);
            }            
            return q;
        }

        public static List<KbSearchResultItemViewModel> DoSearch(string text, int page=1,int resultCount = 20)
        {
            try
            {
                List<KbSearchResultItemViewModel> results = new List<KbSearchResultItemViewModel>();
                StandardAnalyzer analyzer = new StandardAnalyzer(Lucene.Net.Util.Version.LUCENE_30);
                IndexSearcher searcher = new IndexSearcher(FSDirectory.Open(LuceneIndexDirectory));
                string[] fields = new string[]{ "Title", "Content" };
                var parser = new MultiFieldQueryParser(Lucene.Net.Util.Version.LUCENE_30, fields, analyzer);
                Query q = ParseQuery(text, parser);
                TopDocs hits = searcher.Search(q, page*resultCount);                
                if (hits.ScoreDocs.Count() > 0)
                {                                    
                    for (int i = ((page-1)*resultCount); i < hits.ScoreDocs.Count(); i++)
                    {
                        Document doc = searcher.Doc(hits.ScoreDocs[i].Doc);
                        KbSearchResultItemViewModel item = new KbSearchResultItemViewModel();
                        item.ArticleId = Convert.ToInt32(doc.Get("Id").ToString().Replace("KB-", "").Replace("AT-", ""));
                        item.IsArticle= doc.Get("Id").ToString().StartsWith("KB-");
                        item.IsAttachment = doc.Get("Id").ToString().StartsWith("AT-");
                        item.ArticleTitle = doc.Get("Title").ToString();                        
                        results.Add(item);
                    }
                }                
                searcher.Dispose();
                return results;
            }
            catch (Exception ex)
            {
                Log.Error(ex);
                throw;
            }
        }

        public static void RemoveArticleFromIndex(Article article)
        {
            try
            {
                StandardAnalyzer analyzer = new StandardAnalyzer(Lucene.Net.Util.Version.LUCENE_30);
                IndexWriter writer;

                try
                {
                    writer = new IndexWriter(FSDirectory.Open(LuceneIndexDirectory), analyzer, false, IndexWriter.MaxFieldLength.UNLIMITED);
                }
                catch (System.IO.FileNotFoundException)
                {
                    //Lucene directory is not there so create ie 
                    writer = new IndexWriter(FSDirectory.Open(LuceneIndexDirectory), analyzer, true, IndexWriter.MaxFieldLength.UNLIMITED);
                }
                IndexSearcher searcher = new IndexSearcher(FSDirectory.Open(LuceneIndexDirectory));
                Term term = new Term("Id", "KB-" + article.Id.ToString());
                TermQuery q = new TermQuery(term);
                TopDocs docs = searcher.Search(q, 10);
                if (docs.ScoreDocs.Count() == 1)
                {
                    int i = 0;
                }
                writer.DeleteDocuments(term);
                writer.Optimize();
                writer.Commit();
                writer.Dispose();
            }
            
            catch (Exception ex)
            {
                Log.Error(ex);
                throw;
            }
        }

        public static void AddArticleToIndex(Article article)
        {
            try
            {
                RemoveArticleFromIndex(article);  
                StandardAnalyzer analyzer = new StandardAnalyzer(Lucene.Net.Util.Version.LUCENE_30);
                IndexWriter writer = new IndexWriter(FSDirectory.Open(LuceneIndexDirectory),analyzer ,false,IndexWriter.MaxFieldLength.UNLIMITED);
                Document doc = new Document();                
                string decodedHtml = HttpUtility.HtmlDecode(article.Content);
                doc.Add(new Field("Id", "KB-" + article.Id.ToString(), Field.Store.YES, Field.Index.NOT_ANALYZED));
                doc.Add(new Field("Title", article.Title, Field.Store.YES, Field.Index.ANALYZED));
                doc.Add(new Field("Content", StripTagsCharArray(decodedHtml), Field.Store.YES, Field.Index.ANALYZED));
                
                writer.AddDocument(doc);
                writer.Optimize();  
                writer.Commit();                              
                writer.Dispose();
            }
            catch (Exception ex)
            {
                Log.Error(ex);
                throw;
            }
        }

        public static void RemoveAttachmentFromIndex(Attachment attachment)
        {
            try
            {
                StandardAnalyzer analyzer = new StandardAnalyzer(Lucene.Net.Util.Version.LUCENE_30);
                IndexWriter writer = new IndexWriter(FSDirectory.Open(LuceneIndexDirectory), analyzer, false, IndexWriter.MaxFieldLength.UNLIMITED);
                Term term = new Term("Id", "AT-" + attachment.Id.ToString());
                writer.DeleteDocuments(term);
                writer.Optimize();
                writer.Commit();
                writer.Dispose();
            }
            catch (Exception ex)
            {
                Log.Error(ex);
                throw;
            }
        }

        public static void AddAttachmentToIndex(Attachment attachment)
        {
            try
            {
                StandardAnalyzer analyzer = new StandardAnalyzer(Lucene.Net.Util.Version.LUCENE_30);
                IndexWriter writer = new IndexWriter(FSDirectory.Open(LuceneIndexDirectory), analyzer, false, IndexWriter.MaxFieldLength.UNLIMITED);
                Document doc = new Document();
                string path = HttpContext.Current.Server.MapPath(attachment.Path);
                StreamReader reader = new StreamReader( new FileStream( Path.Combine( path , attachment.FileName), FileMode.Open) );                                
                doc.Add(new Field("Id", "AT-" + attachment.Id.ToString(), Field.Store.YES, Field.Index.NOT_ANALYZED));
                doc.Add(new Field("Title", attachment.FileName, Field.Store.YES, Field.Index.ANALYZED));
                doc.Add(new Field("Content", reader, Field.TermVector.WITH_POSITIONS));
                writer.AddDocument(doc);
                writer.Optimize();
                writer.Commit();
                writer.Dispose();
            }
            catch (Exception ex)
            {
                Log.Error(ex);
                throw;
            }
        }

       
    }
}