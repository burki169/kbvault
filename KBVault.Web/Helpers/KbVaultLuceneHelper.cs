using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using KBVault.Dal.Entities;
using KBVault.Web.Models.Public;
using Lucene.Net.Analysis.Standard;
using Lucene.Net.Documents;
using Lucene.Net.Index;
using Lucene.Net.QueryParsers;
using Lucene.Net.Search;
using Lucene.Net.Store;
using NLog;

namespace KBVault.Web.Helpers
{
    public class KbVaultLuceneHelper
    {
        private static string LuceneIndexDirectory
        {
            get
            {
                return HttpContext.Current.Server.MapPath("~/Lucene");
            }
        }

        public static List<KbSearchResultItemViewModel> DoSearch(string text, int page = 1, int resultCount = 20)
        {
            try
            {
                if (page < 1)
                {
                    throw new ArgumentException("Page");
                }

                if (string.IsNullOrEmpty(text))
                {
                    throw new ArgumentNullException("Search Text");
                }

                var results = new List<KbSearchResultItemViewModel>();
                var analyzer = new StandardAnalyzer(Lucene.Net.Util.Version.LUCENE_30);
                var searcher = new IndexSearcher(FSDirectory.Open(LuceneIndexDirectory));
                var fields = new string[] { "Title", "Content" };
                var parser = new MultiFieldQueryParser(Lucene.Net.Util.Version.LUCENE_30, fields, analyzer);
                var q = ParseQuery(text, parser);
                var hits = searcher.Search(q, page * resultCount);
                if (hits.ScoreDocs.Any())
                {
                    for (int i = (page - 1) * resultCount; i < hits.ScoreDocs.Count(); i++)
                    {
                        var doc = searcher.Doc(hits.ScoreDocs[i].Doc);
                        var item = new KbSearchResultItemViewModel
                        {
                            ArticleId = Convert.ToInt32(doc.Get("Id").ToString().Replace("KB-", string.Empty).Replace("AT-", string.Empty)),
                            IsArticle = doc.Get("Id").StartsWith("KB-"),
                            IsAttachment = doc.Get("Id").StartsWith("AT-"),
                            ArticleTitle = doc.Get("Title")
                        };
                        results.Add(item);
                    }
                }

                searcher.Dispose();
                return results;
            }
            catch (Exception ex)
            {
                LogManager.GetCurrentClassLogger().Error(ex);
                throw;
            }
        }

        public static void RemoveArticleFromIndex(Article article)
        {
            try
            {
                var analyzer = new StandardAnalyzer(Lucene.Net.Util.Version.LUCENE_30);
                IndexWriter writer;

                try
                {
                    writer = new IndexWriter(FSDirectory.Open(LuceneIndexDirectory), analyzer, false, IndexWriter.MaxFieldLength.UNLIMITED);
                }
                catch (System.IO.FileNotFoundException)
                {
                    writer = new IndexWriter(FSDirectory.Open(LuceneIndexDirectory), analyzer, true, IndexWriter.MaxFieldLength.UNLIMITED);
                }

                using (var searcher = new IndexSearcher(FSDirectory.Open(LuceneIndexDirectory)))
                {
                    var term = new Term("Id", "KB-" + article.Id.ToString());
                    var q = new TermQuery(term);
                    var docs = searcher.Search(q, 10);

                    writer.DeleteDocuments(term);
                    writer.Optimize();
                    writer.Commit();
                    writer.Dispose();
                }
            }
            catch (Exception ex)
            {
                LogManager.GetCurrentClassLogger().Error(ex);
                throw;
            }
            finally
            {
                IndexWriter.Unlock(FSDirectory.Open(LuceneIndexDirectory));
            }
        }

        public static void AddArticleToIndex(Article article)
        {
            try
            {
                RemoveArticleFromIndex(article);
                var analyzer = new StandardAnalyzer(Lucene.Net.Util.Version.LUCENE_30);
                var writer = new IndexWriter(FSDirectory.Open(LuceneIndexDirectory), analyzer, false, IndexWriter.MaxFieldLength.UNLIMITED);
                var doc = new Document();
                var decodedHtml = HttpUtility.HtmlDecode(article.Content);
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
                LogManager.GetCurrentClassLogger().Error(ex);
                throw;
            }
            finally
            {
                IndexWriter.Unlock(FSDirectory.Open(LuceneIndexDirectory));
            }
        }

        public static void RemoveAttachmentFromIndex(Attachment attachment)
        {
            try
            {
                var analyzer = new StandardAnalyzer(Lucene.Net.Util.Version.LUCENE_30);
                var writer = new IndexWriter(FSDirectory.Open(LuceneIndexDirectory), analyzer, false, IndexWriter.MaxFieldLength.UNLIMITED);
                var term = new Term("Id", "AT-" + attachment.Id.ToString());
                writer.DeleteDocuments(term);
                writer.Optimize();
                writer.Commit();
                writer.Dispose();
            }
            catch (Exception ex)
            {
                LogManager.GetCurrentClassLogger().Error(ex);
                throw;
            }
            finally
            {
                IndexWriter.Unlock(FSDirectory.Open(LuceneIndexDirectory));
            }
        }

        public static void AddAttachmentToIndex(Attachment attachment)
        {
            try
            {
                var analyzer = new StandardAnalyzer(Lucene.Net.Util.Version.LUCENE_30);
                var writer = new IndexWriter(FSDirectory.Open(LuceneIndexDirectory), analyzer, false, IndexWriter.MaxFieldLength.UNLIMITED);
                var doc = new Document();
                var path = HttpContext.Current.Server.MapPath(attachment.Path);
                var localFilePath = Path.Combine(path, attachment.FileName);
                if (File.Exists(localFilePath))
                {
                    var reader = new StreamReader(new FileStream(Path.Combine(path, attachment.FileName), FileMode.Open));
                    doc.Add(new Field("Id", "AT-" + attachment.ArticleId.ToString(), Field.Store.YES, Field.Index.NOT_ANALYZED));
                    doc.Add(new Field("Title", attachment.FileName, Field.Store.YES, Field.Index.ANALYZED));
                    doc.Add(new Field("Content", reader, Field.TermVector.WITH_POSITIONS));
                    writer.AddDocument(doc);
                }

                writer.Optimize();
                writer.Commit();
                writer.Dispose();
            }
            catch (Exception ex)
            {
                LogManager.GetCurrentClassLogger().Error(ex);
                throw;
            }
            finally
            {
                IndexWriter.Unlock(FSDirectory.Open(LuceneIndexDirectory));
            }
        }

        private static Query ParseQuery(string searchQuery, QueryParser parser)
        {
            Query q;
            try
            {
                q = parser.Parse(searchQuery.Trim() + "*");
            }
            catch (ParseException e)
            {
                LogManager.GetCurrentClassLogger().Error("Query parser exception", e);
                q = null;
            }

            if (string.IsNullOrEmpty(q?.ToString()))
            {
                var cooked = Regex.Replace(searchQuery, @"[^\w\.@-]", " ");
                q = parser.Parse(cooked);
            }

            return q;
        }

        // Taken from
        // http://www.dotnetperls.com/remove-html-tags
        private static string StripTagsCharArray(string source)
        {
            var array = new char[source.Length];
            var arrayIndex = 0;
            var inside = false;

            for (int i = 0; i < source.Length; i++)
            {
                var let = source[i];
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
    }
}