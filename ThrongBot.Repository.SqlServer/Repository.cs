using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ThrongBot.Common;
using ThrongBot.Common.Entities;
using NHibernate;
using NHibernate.Linq;
using System.Data.SqlClient;
using System.Data;
using log4net;
using System.Threading;

namespace ThrongBot.Repository.SqlServer
{
    public class Repository : IRepository
    {
        static ILog _logger = LogManager.GetLogger(typeof(Repository).FullName);
        private ISessionFactory _sessionFactory = null;
        private string _connStr = null;

        public Repository(ISessionFactory sessionFactory, string connStr)
        {
            _sessionFactory = sessionFactory;
            _connStr = connStr;
        }
        public void Dispose()
        {
            _sessionFactory = null;
        }

        public void AddSession(SessionConfiguration session)
        {
            using (var sn = _sessionFactory.OpenSession())
            {
                using (var transaction = sn.BeginTransaction())
                {
                    sn.Save(session);
                    transaction.Commit();
                }
            }
        }
        public void UpdateSession(SessionConfiguration session)
        {
            using (var sn = _sessionFactory.OpenSession())
            {
                using (var transaction = sn.BeginTransaction())
                {
                    sn.Update(session);
                    transaction.Commit();
                }
            }
        }
        public SessionConfiguration GetSession(int sessionId)
        {
            SessionConfiguration result = null;
            using (var sn = _sessionFactory.OpenSession())
            {
                using (var transaction = sn.BeginTransaction())
                {
                    var q = sn.Query<SessionConfiguration>()
                              .Where(x => x.SessionId == sessionId);
                    result = q.FirstOrDefault();
                    transaction.Commit();
                }
            }
            return result;
        }
        public void DeleteSession(int sessionId)
        {
            var query = string.Format("DELETE FROM [SessionConfiguration] WHERE [SessionId] = '{0}';",
                               sessionId);

            using (var connection = new SqlConnection(_connStr))
            {
                var command = new SqlCommand(query, connection) { CommandType = CommandType.Text };
                command.Connection.Open();
                command.ExecuteNonQuery();
            }
        }

        public void AddCrawl(CrawlerRun crawl)
        {
            using (var session = _sessionFactory.OpenSession())
            {
                using (var transaction = session.BeginTransaction())
                {
                    session.Save(crawl);
                    transaction.Commit();
                }
            }
        }
        public CrawlerRun GetCrawl(Guid id)
        {
            using (ISession session = NHibernateHelper.OpenSession())
                return session.Get<CrawlerRun>(id);
        }
        public CrawlerRun GetCrawl(int sessionId, int crawlerId)
        {
            CrawlerRun result = null;
            using (var session = _sessionFactory.OpenSession())
            {
                using (var transaction = session.BeginTransaction())
                {
                    var q = session.Query<CrawlerRun>()
                                   .Where(x => x.SessionId == sessionId)
                                   .Where(x => x.CrawlerId == crawlerId);
                    result = q.FirstOrDefault();
                    transaction.Commit();
                }
            }
            return result;
        }
        public void UpdateCrawl(CrawlerRun crawl)
        {
            using (var session = _sessionFactory.OpenSession())
            {
                using (var transaction = session.BeginTransaction())
                {
                    session.Update(crawl);
                    transaction.Commit();
                }
            }
        }
        public void DeleteCrawl(Guid id)
        {
            var query = string.Format("DELETE FROM [Crawler].[dbo].[CrawlerRun] WHERE [Id] = '{0}';",
                               id);

            using (var connection = new SqlConnection(_connStr))
            {
                var command = new SqlCommand(query, connection) { CommandType = CommandType.Text };
                command.Connection.Open();
                command.ExecuteNonQuery();
            }
        }
        public int GetCountOfCrawlsInProgress(int sessionId)
        {
            var query = string.Format("SELECT COUNT(*) FROM [CrawlerRun] WHERE [SessionId] = {0} AND [InProgress] = 1;",
                                       sessionId);

            int count = -1;
            using (var connection = new SqlConnection(_connStr))
            {
                var command = new SqlCommand(query, connection) { CommandType = CommandType.Text };
                command.Connection.Open();
                count = (int)command.ExecuteScalar();
            }

            return count;
        }
        public CrawlerRun GetCrawl(int sessionId, string baseDomain)
        {
            var baseDomainLower = baseDomain.Trim().ToLower();
            CrawlerRun result = null;
            using (var session = _sessionFactory.OpenSession())
            {
                using (var transaction = session.BeginTransaction())
                {
                    var q = session.Query<CrawlerRun>()
                                   .Where(x => x.SessionId == sessionId)
                                   .Where(x => x.BaseDomain == baseDomainLower);
                    result = q.FirstOrDefault();
                    transaction.Commit();
                }
            }
            return result;
        }
        public int GetNextCrawlerId(int sessionId)
        {
            var query = string.Format("SELECT MAX([CrawlerId]) FROM [CrawlerRun] WHERE [SessionId] = {0};",
                                       sessionId);

            int max = -1;
            using (var connection = new SqlConnection(_connStr))
            {
                var command = new SqlCommand(query, connection) { CommandType = CommandType.Text };
                command.Connection.Open();
                max =  Convert.ToInt32(command.ExecuteScalar());
            }

            return max + 1;
        }
        public void AddProcessedPage(ProcessedPage result)
        {
            using (var session = _sessionFactory.OpenSession())
            {
                using (var transaction = session.BeginTransaction())
                {
                    session.Save(result);
                    transaction.Commit();
                }
            }
        }
        public ProcessedPage GetProcessedPage(Guid id)
        {
            using (ISession session = NHibernateHelper.OpenSession())
                return session.Get<ProcessedPage>(id);
        }
        public void UpdateProcessedPage(ProcessedPage result)
        {
            using (var session = _sessionFactory.OpenSession())
            {
                using (var transaction = session.BeginTransaction())
                {
                    session.Update(result);
                    transaction.Commit();
                }
            }
        }
        public void DeleteProcessedPage(Guid id)
        {
            var query = string.Format("DELETE FROM [Crawler].[dbo].[ProcessedPage] WHERE [Id] = '{0}';",
                               id);

            using (var connection = new SqlConnection(_connStr))
            {
                var command = new SqlCommand(query, connection) { CommandType = CommandType.Text };
                command.Connection.Open();
                command.ExecuteNonQuery();
            }
        }
        public bool IsPageProcessed(string url)
        {
            bool result = false;
            using (var session = _sessionFactory.OpenSession())
            {
                using (var transaction = session.BeginTransaction())
                {
                    var q = session.Query<ProcessedPage>()
                                   .Where(x => x.PageUrl == url);
                    result = q.Any();
                    transaction.Commit();
                }
            }
            return result;
        }

        public void AddCrawledLink(CrawledLink link, bool removeCorrespondingLinkToCrawl)
        {
            Thread.Sleep(100);
            using (var session = _sessionFactory.OpenSession())
            {
                if (removeCorrespondingLinkToCrawl)
                    DeleteLinkToCrawl(link.SessionId, link.SourceUrl, link.TargetUrl);
                using (var transaction = session.BeginTransaction())
                {
                    session.Save(link);
                    transaction.Commit();
                }
            }
        }
        public CrawledLink GetCrawledLink(int sessionId, int crawlerId, string srcUrl, string targetUrl)
        {
            CrawledLink result = null;
            using (var session = _sessionFactory.OpenSession())
            {
                using (var transaction = session.BeginTransaction())
                {
                    var q = session.Query<CrawledLink>()
                                   .Where(x => x.SessionId == sessionId)
                                   .Where(x => x.CrawlerId == crawlerId)
                                   .Where(x => x.SourceUrl == srcUrl)
                                   .Where(x => x.TargetUrl == targetUrl);
                    result = q.FirstOrDefault();
                    transaction.Commit();
                }
            }
            return result;
        }
        public CrawledLink GetCrawledLink(Guid id)
        {
            using (ISession session = NHibernateHelper.OpenSession())
                return session.Get<CrawledLink>(id);
        }
        public void DeleteCrawledLink(Guid id)
        {
            var query = string.Format("DELETE FROM [Crawler].[dbo].[CrawledLink] WHERE [Id] = '{0}';",
                       id);

            using (var connection = new SqlConnection(_connStr))
            {
                var command = new SqlCommand(query, connection) { CommandType = CommandType.Text };
                command.Connection.Open();
                command.ExecuteNonQuery();
            }
        }
        public bool IsCrawled(int sessionId, string targetUrl)
        {
            bool result = false;
            using (var session = _sessionFactory.OpenSession())
            {
                using (var transaction = session.BeginTransaction())
                {
                    var q = session.Query<CrawledLink>()
                                   .Where(x => x.SessionId == sessionId)
                                   .Where(x => x.TargetUrl == targetUrl);
                    result = q.Any();
                    transaction.Commit();
                }
            }
            return result;
        }
        public int GetCountOfCrawledLinks(int sessionId, int crawlerId)
        {
            int result = 0;
            using (var session = _sessionFactory.OpenSession())
            {
                using (var transaction = session.BeginTransaction())
                {
                    var q = session.Query<CrawledLink>()
                                   .Where(x => x.SessionId == sessionId)
                                   .Where(x => x.CrawlerId == crawlerId);
                    result = q.Count();
                    transaction.Commit();
                }
            }
            return result;
        }
        public void ClearCrawledLinks(int sessionId, int crawlerId)
        {
            var query = string.Format("DELETE FROM [CrawledLink] WHERE [SessionId] = {0} AND [CrawlerId] = {1}",
                                      sessionId, crawlerId);

            using (var connection = new SqlConnection(_connStr))
            {
                var command = new SqlCommand(query, connection) { CommandType = CommandType.Text };
                command.Connection.Open();
                command.ExecuteNonQuery();
            }
        }

        public void AddLinkToCrawl(LinkToCrawl link)
        {
            using (var session = _sessionFactory.OpenSession())
            {
                using (var transaction = session.BeginTransaction())
                {
                    session.Save(link);
                    transaction.Commit();
                }
            }
        }
        public LinkToCrawl GetLinkToCrawl(Guid id)
        {
            using (ISession session = NHibernateHelper.OpenSession())
                return session.Get<LinkToCrawl>(id);
        }
        public LinkToCrawl GetLinkToCrawl(int sessionId, string srcUrl, string targetUrl)
        {
            LinkToCrawl result = null;
            using (var session = _sessionFactory.OpenSession())
            {
                using (var transaction = session.BeginTransaction())
                {
                    var q = session.Query<LinkToCrawl>()
                                   .Where(x => x.SessionId == sessionId)
                                   .Where(x => x.SourceUrl == srcUrl)
                                   .Where(x => x.TargetUrl == targetUrl);
                    result = q.FirstOrDefault();
                    transaction.Commit();
                }
            }
            return result;
        }
        public bool IsToBeCrawled(int sessionId, string targetUrl)
        {
            bool result = false;
            using (var session = _sessionFactory.OpenSession())
            {
                using (var transaction = session.BeginTransaction())
                {
                    var q = session.Query<LinkToCrawl>()
                                   .Where(x => x.SessionId == sessionId)
                                   .Where(x => x.TargetUrl == targetUrl);
                    result = q.Any();
                    transaction.Commit();
                }
            }
            return result;
        }
        public void DeleteLinkToCrawl(Guid id)
        {
            var query = string.Format("DELETE FROM [Crawler].[dbo].[LinkToCrawl] WHERE [Id] = '{0}';",
               id);

            using (var connection = new SqlConnection(_connStr))
            {
                var command = new SqlCommand(query, connection) { CommandType = CommandType.Text };
                command.Connection.Open();
                command.ExecuteNonQuery();
            }
        }
        public void DeleteLinkToCrawl(int sessionId, string srcUrl, string targetUrl)
        {
            var query = string.Format("DELETE FROM [Crawler].[dbo].[LinkToCrawl] WHERE [SessionId] = {0} AND [SourceUrl] = '{1}' AND [TargetUrl] = '{2}';",
                                      sessionId, srcUrl.Trim(), targetUrl.Trim());

            using (var connection = new SqlConnection(_connStr))
            {
                var command = new SqlCommand(query, connection) { CommandType = CommandType.Text };
                command.Connection.Open();
                command.ExecuteNonQuery();
            }
        }
        public int GetCountOfLinksToCrawl(int sessionId, string baseDomain)
        {
            var query = string.Format("SELECT COUNT(*) FROM [LinkToCrawl] WHERE [SessionId] = {0} AND [TargetBaseDomain] = '{1}' AND [InProgress] = 0;",
                                       sessionId, baseDomain);

            int count = -1;
            using (var connection = new SqlConnection(_connStr))
            {
                var command = new SqlCommand(query, connection) { CommandType = CommandType.Text };
                command.Connection.Open();
                count = (int)command.ExecuteScalar();
            }

            return count;
        }
        public void ClearLinksToCrawl(int sessionId, string baseDomain)
        {
            var query = string.Format("DELETE FROM [LinkToCrawl] WHERE [SessionId] = {0} AND [TargetBaseDomain] = '{1}'",
                                      sessionId, baseDomain);

            using (var connection = new SqlConnection(_connStr))
            {
                var command = new SqlCommand(query, connection) { CommandType = CommandType.Text };
                command.Connection.Open();
                command.ExecuteNonQuery();
            }
        }
        public IEnumerable<LinkToCrawl> GetNextLinksToCrawl(int sessionId, string baseDomain)
        {
            IEnumerable<LinkToCrawl> result = null;
            using (var session = _sessionFactory.OpenSession())
            {
                using (var transaction = session.BeginTransaction())
                {
                    var q = session.QueryOver<LinkToCrawl>()
                                   .Where(x => x.SessionId == sessionId)
                                   .Where(x => x.TargetBaseDomain == baseDomain)
                                   .Where(x => x.InProgress == false)
                                   .List<LinkToCrawl>();
                    result = q.ToList();
                    transaction.Commit();
                }
            }
            return result;
        }
        public LinkToCrawl GetNextLinkToCrawl(int sessionId, string baseDomain, bool markAsInProgress)
        {
            LinkToCrawl result = null;
            using (var session = _sessionFactory.OpenSession())
            {
                using (var transaction = session.BeginTransaction())
                {
                    var q = session.Query<LinkToCrawl>()
                                   .Where(x => x.SessionId == sessionId)
                                   .Where(x => x.TargetBaseDomain == baseDomain)
                                   .Where(x => x.InProgress == false);
                    result = q.FirstOrDefault();
                    transaction.Commit();
                }
            }

            if (result != null && markAsInProgress)
            {
                var query = string.Format("UPDATE [Crawler].[dbo].[LinkToCrawl] SET [InProgress] = 1 WHERE [Id] = '{0}';",
                                           result.Id);

                using (var connection = new SqlConnection(_connStr))
                {
                    var command = new SqlCommand(query, connection) { CommandType = CommandType.Text };
                    command.Connection.Open();
                    command.ExecuteNonQuery();
                }
            }

            return result;
        }

        public UserAgentString GetUserAgent(Guid id)
        {
            using (ISession session = NHibernateHelper.OpenSession())
                return session.Get<UserAgentString>(id);
        }
        public IEnumerable<UserAgentString> GetUserAgents()
        {
            List<UserAgentString> agents = new List<UserAgentString>();
            var query = string.Format("SELECT [Id], [UserAgent] FROM [UserAgentString]");

            using (var connection = new SqlConnection(_connStr))
            {
                SqlCommand command = new SqlCommand(query, connection);
                connection.Open();

                using (var reader = command.ExecuteReader())
                {
                    UserAgentString agent = null;
                    while (reader.Read())
                    {
                        agent = new UserAgentString();
                        agent.Id = reader.GetGuid(0);
                        agent.UserAgent = reader.GetString(1);
                        agents.Add(agent);
                    }
                }
            }

            return agents;
        }


        public void AddBlacklisted(string url)
        {
            var query = string.Format("INSERT INTO [Crawler].[dbo].[BlackList] ([Url]) VALUES ('{0}')", url.Trim());

            using (var connection = new SqlConnection(_connStr))
            {
                var command = new SqlCommand(query, connection) { CommandType = CommandType.Text };
                command.Connection.Open();
                command.ExecuteNonQuery();
            }
        }
        public bool IsBlackListed(string url)
        {
            var query = string.Format("SELECT COUNT(*) FROM [BlackList] WHERE [Url] = '{0}'", url.Trim());

            int count = -1;
            using (var connection = new SqlConnection(_connStr))
            {
                var command = new SqlCommand(query, connection) { CommandType = CommandType.Text };
                command.Connection.Open();
                count = (int)command.ExecuteScalar();
            }

            return count > 0;
        }
        public IEnumerable<string> GetBlackList()
        {
            List<string> urls = new List<string>();
            var query = string.Format("SELECT [Url] FROM [BlackList] ORDER BY [Url]");

            using (var connection = new SqlConnection(_connStr))
            {
                SqlCommand command = new SqlCommand(query, connection);
                connection.Open();

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        urls.Add(reader.GetString(0));
                    }
                }
            }

            return urls;
        }

        public void AddKeyWords(List<string> keyWords)
        {
            throw new NotImplementedException();
        }
        public IEnumerable<string> GetKeyWords()
        {
            throw new NotImplementedException();
        }

        public void AddCookies(IEnumerable<System.Net.Cookie> cookies, int sessionId, int crawlerId, string sourceUrl, string targetUrl)
        {
            throw new NotImplementedException();
        }
        public void DeleteCookies(int sessionId, int crawlerId, string sourceUrl, string targetUrl)
        {
            throw new NotImplementedException();
        }
        public IEnumerable<System.Net.Cookie> GetCookies(int sessionId, int crawlerId, string sourceUrl, string targetUrl)
        {
            throw new NotImplementedException();
        }
        public void ClearCookies(int sessionId, int crawlerId)
        {
            throw new NotImplementedException();
        }
    }
}
