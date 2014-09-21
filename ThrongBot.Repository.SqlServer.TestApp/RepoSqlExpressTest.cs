using ThrongBot.Common;
using ThrongBot.Common.Entities;
using ThrongBot.TestSupport;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ThrongBot.Repository.SqlServer.TestApp
{
    public class RepoSqlExpressTest
    {
        private string _connStr = null;
        private Guid _crawlerRunId;
        private int _crawlerRunSessionId;
        private int _crawlerRunCrawlerId;
        private string _crawlerRunBaseDomain;

        private Guid _processedPageId;
        private Guid _crawledLinkId;
        private int _crawledSessionId;
        private int _crawledCrawlerId;
        private string _crawledSourceUrl;
        private string _crawledTargetUrl;
        private Guid _toCrawlId;
        private int _toCrawlSessionId;
        private string _toCrawlSourceUrl;
        private string _toCrawlBaseDomain;
        private string _toCrawlTargetUrl;
        private string _blackListedUrl = "www.google.de";

        public RepoSqlExpressTest(string connStr)
        {
            _connStr = connStr;
        }

        public void ClearDb()
        {
            var query = string.Format("DELETE FROM [CrawledLink]; DELETE FROM [CrawlerRun]; DELETE FROM [LinkToCrawl]; DELETE FROM [ProcessedPage];");

            using (var connection = new SqlConnection(_connStr))
            {
                var command = new SqlCommand(query, connection) { CommandType = CommandType.Text };
                command.Connection.Open();
                command.ExecuteNonQuery();
            }

            Console.WriteLine("All Tables Cleared");
            Console.WriteLine();
        }
        public void ClearTable(string tableName)
        {
            var query = string.Format("DELETE FROM [{0}];", tableName);

            using (var connection = new SqlConnection(_connStr))
            {
                var command = new SqlCommand(query, connection) { CommandType = CommandType.Text };
                command.Connection.Open();
                command.ExecuteNonQuery();
            }
        }

        //SessionConfiguration
        private int _sessionCfgId;
        private bool AddSessionTest()
        {
            Guid id = Guid.Empty;
            bool result = false;
            try
            {
                _sessionCfgId = 3337;
                var repo = GetRepo();
                var session = new SessionConfiguration(_sessionCfgId);
                session.MaxConcurrentCrawls = 32;

                session.Id = Guid.Empty;
                repo.AddSession(session);
                id = session.Id;

                result = true;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Fail: Ex thrown adding CrawlerRun: {0}", ex.Message);
                result = false;
            }

            if (result)
                Console.WriteLine("Pass: Session added, given id: {0}", id);
            else
                Console.WriteLine("Fail: Session not added");

            return result;
        }
        private bool GetSessionTest()
        {
            bool result = false;
            try
            {
                var repo = GetRepo();

                var session = repo.GetSession(_sessionCfgId);
                if (session == null)
                {
                    result = false;
                    Console.WriteLine("Fail: SessionConfig {0} was null", _sessionCfgId);
                }
                else
                {
                    result = true;
                    Console.WriteLine("Pass: SessionConfig {0} returned", session.Id);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Fail: Ex thrown getting SessionConfig: {0}", ex.Message);
                result = false;
            }
            return result;
        }
        private bool DeleteSessionTest()
        {
            bool result = false;
            try
            {
                var repo = GetRepo();

                repo.DeleteSession(_sessionCfgId);

                var check = repo.GetSession(_sessionCfgId);
                if (check == null)
                {
                    result = true;
                    Console.WriteLine("Pass: SessionConfig {0} deleted", _sessionCfgId);
                }
                else
                {
                    result = false;
                    Console.WriteLine("Fail: SessionConfig {0} not deleted", _sessionCfgId);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Fail: Ex thrown deleting CrawlerRun: {0}", ex.Message);
                result = false;
            }
            return result;
        }

        //CrawlerRun
        private bool AddCrawlerRunTest()
        {
            bool result = false;
            try
            {
                var repo = GetRepo();
                _crawlerRunBaseDomain = string.Format("c{0}.com", 33);
                var run = TestData.GetCrawlerRun(string.Format("http://www.c{0}.com/A", 33), _crawlerRunBaseDomain);
                run.Id = Guid.Empty;
                repo.AddCrawl(run);
                _crawlerRunId = run.Id;
                _crawlerRunSessionId = run.SessionId;
                _crawlerRunCrawlerId = run.CrawlerId;

                result = true;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Fail: Ex thrown adding CrawlerRun: {0}", ex.Message);
                result = false;
            }

            if (result)
                Console.WriteLine("Pass: CrawlerRun added, given id: {0}", _crawlerRunId);
            else
                Console.WriteLine("Fail: CrawlerRun not added");

            return result;
        }
        private bool GetCrawlerRunTest()
        {
            bool result = false;
            try
            {
                var repo = GetRepo();

                var run = repo.GetCrawl(_crawlerRunId);
                if (run == null)
                {
                    result = false;
                    Console.WriteLine("Fail: CrawlerRun {0} was null", _crawlerRunId);
                }
                else
                {
                    result = true;
                    Console.WriteLine("Pass: CrawlerRun {0} returned", _crawlerRunId);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Fail: Ex thrown getting CrawlerRun: {0}", ex.Message);
                result = false;
            }
            return result;
        }
        private bool GetCrawlerRunTest_By_Params()
        {
            bool result = false;
            try
            {
                var repo = GetRepo();

                var run = repo.GetCrawl(_crawlerRunSessionId, _crawlerRunCrawlerId);
                if (run == null)
                {
                    result = false;
                    Console.WriteLine("Fail: CrawlerRun {0} was null", _crawlerRunId);
                }
                else if (run.Id != _crawlerRunId)
                {
                    result = true;
                    Console.WriteLine("Fail: CrawlerRun with unexpeced id: {0} returned", run.Id);
                }
                else 
                {
                    result = true;
                    Console.WriteLine("Pass: CrawlerRun {0} returned", _crawlerRunId);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Fail: Ex thrown getting CrawlerRun: {0}", ex.Message);
                result = false;
            }
            return result;
        }
        private bool GetCrawlerRunTest_By_BaseDomain_Params()
        {
            bool result = false;
            try
            {
                var repo = GetRepo();

                var run = repo.GetCrawl(_crawlerRunSessionId, _crawlerRunBaseDomain);
                if (run == null)
                {
                    result = false;
                    Console.WriteLine("Fail: CrawlerRun {0} was null", _crawlerRunId);
                }
                else if (run.Id != _crawlerRunId)
                {
                    result = true;
                    Console.WriteLine("Fail: CrawlerRun with unexpected id: {0} returned", run.Id);
                }
                else
                {
                    result = true;
                    Console.WriteLine("Pass: CrawlerRun {0} returned", _crawlerRunId);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Fail: Ex thrown getting CrawlerRun: {0}", ex.Message);
                result = false;
            }
            return result;
        }
        private bool GetCountOfCrawlsInProgressTest()
        {
            bool result = false;
            try
            {
                var repo = GetRepo();

                var run = TestData.GetCrawlerRun(string.Format("http://www.ZZZ{0}.com/A", 339999), string.Format("c{0}.com", 339999));
                repo.AddCrawl(run);
                var count = repo.GetCountOfCrawlsInProgress(_crawlerRunSessionId);
                if (count != 2)
                {
                    result = false;
                    Console.WriteLine("Fail: Count should be {0}", 2);
                }
                else
                {
                    result = true;
                    Console.WriteLine("Pass: Count is {0}", count);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Fail: Ex thrown getting CrawlerRun: {0}", ex.Message);
                result = false;
            }
            return result;
        }
        private bool GetNextCrawlerIdTest()
        {
            bool result = false;
            try
            {
                var repo = GetRepo();

                var run = TestData.GetCrawlerRun(string.Format("http://www.ZZZ{0}.com/A", 339999), string.Format("c{0}.com", 339999));
                run.SessionId = _crawlerRunSessionId;
                run.CrawlerId = 9999993;

                repo.AddCrawl(run);
                var next = repo.GetNextCrawlerId(_crawlerRunSessionId);
                if (next != 9999994)
                {
                    result = false;
                    Console.WriteLine("Fail: Next crawler id should be {0}, not {1}", 9999994, next);
                }
                else
                {
                    result = true;
                    Console.WriteLine("Pass: Next crawler id is {0}", next);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Fail: Ex thrown getting CrawlerRun: {0}", ex.Message);
                result = false;
            }
            return result;
        }
        private bool UpdateCrawlerRunTest()
        {
            bool result = false;
            try
            {
                var repo = GetRepo();

                var run = TestData.GetCrawlerRun(string.Format("http://www.X{0}.com/A", 33), string.Format("X{0}.com", 33));
                run.Id = _crawlerRunId;

                repo.UpdateCrawl(run);

                var check = repo.GetCrawl(_crawlerRunId);
                if (check.SeedUrl == run.SeedUrl)
                    Console.WriteLine("Pass: CrawlerRun {0} updated", _crawlerRunId);
                else 
                    Console.WriteLine("Fail: CrawlerRun {0} not updated", _crawlerRunId);

                result = true;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Fail: Ex thrown updating CrawlerRun: {0}", ex.Message);
                result = false;
            }
            return result;
        }
        private bool DeleteCrawlerRunTest()
        {
            bool result = false;
            try
            {
                var repo = GetRepo();

                repo.DeleteCrawl(_crawlerRunId);

                var check = repo.GetCrawl(_crawlerRunId);
                if (check == null)
                {
                    result = true;
                    Console.WriteLine("Pass: CrawlerRun {0} deleted", _crawlerRunId);
                }
                else
                {
                    result = false;
                    Console.WriteLine("Fail: CrawlerRun {0} not deleted", _crawlerRunId);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Fail: Ex thrown deleting CrawlerRun: {0}", ex.Message);
                result = false;
            }
            return result;
        }

        //ProcessedPage
        private bool AddProcessedPageTest()
        {
            bool result = false;
            try
            {
                var repo = GetRepo();

                var page = TestData.GetProcessedPage(string.Format("http://www.c{0}.com/A", 33));
                page.Id = Guid.Empty;

                repo.AddProcessedPage(page);
                _processedPageId = page.Id;

                result = true;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Ex thrown adding ProcessedPage: {0}", ex.Message);
                result = false;
            }
            if (result)
                Console.WriteLine("Pass: ProcessedPage added, given id: {0}", _processedPageId);
            else
                Console.WriteLine("Fail: ProcessedPage not added");

            return result;
        }
        private bool GetProcessedPageTest()
        {
            bool result = false;
            try
            {
                var repo = GetRepo();

                var page = repo.GetProcessedPage(_processedPageId);
                if (page == null)
                {
                    result = false;
                    Console.WriteLine("Fail: ProcessedPage {0} was null", _processedPageId);
                }
                else
                {
                    result = true;
                    Console.WriteLine("Pass: ProcessedPage {0} returned", _processedPageId);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Fail: Ex thrown getting ProcessedPage: {0}", ex.Message);
                result = false;
            }
            return result;
        }
        private bool UpdateProcessedPageTest()
        {
            bool result = false;
            try
            {
                var repo = GetRepo();

                var page = TestData.GetProcessedPage(string.Format("http://www.X{0}.com/A", 33));
                page.Id = _processedPageId;

                repo.UpdateProcessedPage(page);


                var check = repo.GetProcessedPage(_processedPageId);
                if (check.PageUrl == page.PageUrl)
                    Console.WriteLine("Pass: ProcessedPage {0} updated", _processedPageId);
                else
                    Console.WriteLine("Fail: ProcessedPage {0} not updated", _processedPageId);

                result = true;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Fail: Ex thrown updating ProcessedPage: {0}", ex.Message);
                result = false;
            }
            return result;
        }
        private bool IsPageProcessedTest()
        {
            bool result = false;
            try
            {
                var repo = GetRepo();

                var check = repo.GetProcessedPage(_processedPageId);
                var url = check.PageUrl;

                var processed = repo.IsPageProcessed(url);

                if (!processed)
                {
                    Console.WriteLine("Fail: IsPageProcessed should be true for {0}", url);
                    result = false;
                }
                else
                {
                    Console.WriteLine("Pass: IsPageProcessed is true for {0}", url);  
                    result = true;
                }

                url = "XXXXX";
                processed = repo.IsPageProcessed(url);

                if (processed)
                {
                    Console.WriteLine("Fail: IsPageProcessed should be false for {0}", url);
                    result = false;
                }
                else
                {
                    Console.WriteLine("Pass: IsPageProcessed is false for {0}", url);
                    result = true;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Ex thrown updating IsPageProcessedTest: {0}", ex.Message);
                result = false;
            }
            return result;
        }
        private bool DeleteProcessedPageTest()
        {
            bool result = false;
            try
            {
                var repo = GetRepo();

                repo.DeleteProcessedPage(_processedPageId);

                var check = repo.GetProcessedPage(_processedPageId);
                if (check == null)
                {
                    result = true;
                    Console.WriteLine("Pass: ProcessedPage {0} deleted", _processedPageId);
                }
                else
                {
                    result = false;
                    Console.WriteLine("Fail: ProcessedPage {0} not deleted", _processedPageId);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Pass: Ex thrown deleting ProcessedPage: {0}", ex.Message);
                result = false;
            }
            return result;
        }

        //CrawledLink
        private bool AddCrawledLinkTest()
        {
            bool result = false;
            try
            {
                var repo = GetRepo();

                _crawledSessionId = 76;
                _crawledCrawlerId = 98;
                _crawledSourceUrl = string.Format("http://www.c{0}.com/A", 33);
                _crawledTargetUrl = string.Format("http://www.XX{0}.com/XX", 55);
                var link = TestData.GetCrawledLink(_crawledSourceUrl, _crawledTargetUrl);
                link.SessionId = _crawledSessionId;
                link.CrawlerId = _crawledCrawlerId;
                link.Id = Guid.Empty;

                repo.AddCrawledLink(link, true);
                _crawledLinkId = link.Id;

                result = true;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Ex thrown adding CrawledLink: {0}", ex.Message);
                result = false;
            }
            if (result)
                Console.WriteLine("Pass: CrawledLink added, given id: {0}", _crawledLinkId);
            else
                Console.WriteLine("Fail: CrawledLink not added");

            return result;
        }
        private bool GetCrawledLinkTest()
        {
            bool result = false;
            try
            {
                var repo = GetRepo();

                var link = repo.GetCrawledLink(_crawledLinkId);
                if (link == null)
                {
                    result = false;
                    Console.WriteLine("Fail: CrawledLink {0} was null", _crawledLinkId);
                }
                else
                {
                    result = true;
                    Console.WriteLine("Pass: CrawledLink {0} returned by id", _crawledLinkId);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Fail: Ex thrown getting CrawledLink: {0}", ex.Message);
                result = false;
            }
            return result;
        }
        private bool GetCrawledLinkTest_By_Params()
        {
            bool result = false;
            try
            {
                var repo = GetRepo();

                var link = repo.GetCrawledLink(_crawledSessionId, _crawledCrawlerId, _crawledSourceUrl, _crawledTargetUrl);
                if (link == null)
                {
                    result = false;
                    Console.WriteLine("Fail: CrawledLink by params {0} was null", _crawledLinkId);
                }
                else
                {
                    result = true;
                    Console.WriteLine("Pass: CrawledLink {0} returned by params", _crawledLinkId);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Fail: Ex thrown getting CrawledLink: {0}", ex.Message);
                result = false;
            }
            return result;
        }
        private bool IsCrawledTest()
        {
            bool result = false;
            try
            {
                var repo = GetRepo();

                var crawled = repo.IsCrawled(_crawledSessionId, _crawledTargetUrl);

                if (crawled)
                    Console.WriteLine("Pass: IsCrawled = true for {0}", _crawledLinkId);
                else
                    Console.WriteLine("Fail: IsCrawled should be true for {0}", _crawledLinkId);

                result = true;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Fail: Ex thrown checking IsCrawled: {0}", ex.Message);
                result = false;
            }
            return result;
        }
        private bool IsCrawledTest_Expecting_False()
        {
            bool result = false;
            try
            {
                var repo = GetRepo();

                var crawled = repo.IsCrawled(_crawledSessionId, "BLAH");

                if (!crawled)
                    Console.WriteLine("Pass: IsCrawled = false ");
                else
                    Console.WriteLine("Fail: IsCrawled should be false");

                result = true;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Fail: Ex thrown checking IsCrawled: {0}", ex.Message);
                result = false;
            }
            return result;
        }
        private bool GetCrawledLinkCountTest()
        {
            bool result = false;
            try
            {
                var repo = GetRepo();

                var count = repo.GetCountOfCrawledLinks(_crawledSessionId, _crawledCrawlerId);

                if (count != 1)
                {
                    Console.WriteLine("Fail: GetCountOfCrawledLinks should not be {0}", count);
                    result = false;
                }
                else
                {
                    Console.WriteLine("Pass: GetCountOfCrawledLinks = {0}", count);
                    result = true;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Ex thrown updating GetCountOfCrawledLinks: {0}", ex.Message);
                result = false;
            }
            return result;
        }
        private bool DeleteCrawledLinkTest()
        {
            bool result = false;
            try
            {
                var repo = GetRepo();

                repo.DeleteCrawledLink(_crawledLinkId);

                var check = repo.GetCrawledLink(_crawledLinkId);
                if (check == null)
                {
                    result = true;
                    Console.WriteLine("Pass: CrawledLink {0} deleted", _crawledLinkId);
                }
                else
                {
                    result = false;
                    Console.WriteLine("Fail: CrawledLink {0} not deleted", _crawledLinkId);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Pass: Ex thrown deleting CrawledLink: {0}", ex.Message);
                result = false;
            }
            return result;
        }
        private bool ClearCrawledLinksTest()
        {
            MethodBase method = MethodBase.GetCurrentMethod();

            bool result = false;
            try
            {
                var repo = GetRepo();

                var link = TestData.GetCrawledLink("X", "Y");
                link.Id = Guid.NewGuid();
                int sessionId = link.SessionId;
                int crawlerId = link.CrawlerId;
                repo.AddCrawledLink(link, false);

                link = TestData.GetCrawledLink("X1", "Y1");
                link.Id = Guid.NewGuid();
                link.SessionId = sessionId;
                link.CrawlerId = crawlerId;
                repo.AddCrawledLink(link, false);

                link = TestData.GetCrawledLink("X2", "Y2");
                link.Id = Guid.NewGuid();
                link.SessionId = sessionId;
                link.CrawlerId = crawlerId;
                repo.AddCrawledLink(link, false);

                link = TestData.GetCrawledLink("A", "B");
                link.Id = Guid.NewGuid();
                link.SessionId = 444;
                link.CrawlerId = 9999;
                repo.AddCrawledLink(link, false);

                var count = repo.GetCountOfCrawledLinks(sessionId, crawlerId);

                if (count == 3)
                {
                    result = true;

                    repo.ClearCrawledLinks(sessionId, crawlerId);
                    count = repo.GetCountOfCrawledLinks(sessionId, crawlerId);
                    if (count == 0)
                    {
                        result = true;

                        repo.ClearCrawledLinks(sessionId, crawlerId);
                        count = repo.GetCountOfCrawledLinks(sessionId, crawlerId);
                        Write.Pass(method.Name, string.Format("expected count = 0"));
                    }
                    else
                    {
                        Write.Fail(method.Name, string.Format("expected count = 0"));
                        result = false;
                    }
                }
                else
                {
                    Write.Fail(method.Name, string.Format("expected start count <> {0}", count));
                    result = false;
                }
            }
            catch (Exception ex)
            {
                Write.Ex(method.Name, ex.Message);
                result = false;
            }
            return result;
        }

        //LinkToCrawl
        private bool AddLinkToCrawlTest()
        {
            MethodBase method = MethodBase.GetCurrentMethod();

            bool result = false;
            try
            {
                var repo = GetRepo();

                _toCrawlSessionId = 76;
                _toCrawlSourceUrl = string.Format("http://www.c{0}.com/A", 33);
                _toCrawlBaseDomain = string.Format("c{0}.com", 33);
                _toCrawlTargetUrl = string.Format("http://www.XX{0}.com/XX", 55);
                var link = TestData.GetLinkToCrawl(_toCrawlSourceUrl, _toCrawlTargetUrl);
                link.SessionId = _toCrawlSessionId;
                link.TargetBaseDomain = _toCrawlBaseDomain;
                link.Id = Guid.Empty;

                repo.AddLinkToCrawl(link);
                _toCrawlId = link.Id;

                result = true;
            }
            catch (Exception ex)
            {
                Write.Ex(method.Name, ex.Message);
                result = false;
            }
            if (result)
                Write.Pass(method.Name, string.Format("added, given id: {0}", _toCrawlId));
            else
                Write.Fail(method.Name, string.Format("LinkToCrawl not added"));

            return result;
        }
        private bool GetLinkToCrawlTest()
        {
            MethodBase method = MethodBase.GetCurrentMethod();

            bool result = false;
            try
            {
                var repo = GetRepo();

                var link = repo.GetLinkToCrawl(_toCrawlId);
                if (link == null)
                {
                    result = false;
                    Write.Fail(method.Name, string.Format("LinkToCrawl {0} was null", _toCrawlId));
                }
                else
                {
                    result = true;
                    Write.Pass(method.Name, string.Format("LinkToCrawl {0} returned by id", _toCrawlId));
                }
            }
            catch (Exception ex)
            {
                Write.Ex(method.Name, ex.Message);
                result = false;
            }
            return result;
        }
        private bool GetLinkToCrawlTest_By_Params()
        {
            MethodBase method = MethodBase.GetCurrentMethod();

            bool result = false;
            try
            {
                var repo = GetRepo();

                var link = repo.GetLinkToCrawl(_toCrawlSessionId, _toCrawlSourceUrl, _toCrawlTargetUrl);
                if (link == null)
                {
                    result = false;
                    Write.Fail(method.Name, string.Format("LinkToCrawl by params {0} was null", _toCrawlId));
                }
                else
                {
                    result = true;
                    Write.Pass(method.Name, string.Format("LinkToCrawl {0} returned by params", _toCrawlId));
                }
            }
            catch (Exception ex)
            {
                Write.Ex(method.Name, ex.Message);
                result = false;
            }
            return result;
        }
        private bool IsToBeCrawledTest()
        {
            MethodBase method = MethodBase.GetCurrentMethod();

            bool result = false;
            try
            {
                var repo = GetRepo();

                var crawled = repo.IsToBeCrawled(_toCrawlSessionId, _toCrawlTargetUrl);

                if (crawled)
                    Write.Pass(method.Name, string.Format("true for {0}", _toCrawlId));
                else
                    Write.Fail(method.Name, string.Format("should be true for {0}", _toCrawlId));

                result = true;
            }
            catch (Exception ex)
            {
                Write.Ex(method.Name, ex.Message);
                result = false;
            }
            return result;
        }
        private bool IsToBeCrawledTest_Expecting_False()
        {
            MethodBase method = MethodBase.GetCurrentMethod();

            bool result = false;
            try
            {
                var repo = GetRepo();

                var crawled = repo.IsCrawled(_toCrawlSessionId, "BLAH");

                if (!crawled)
                    Write.Pass(method.Name, string.Format("false for {0}", _toCrawlId));
                else
                    Write.Fail(method.Name, string.Format("should be false for {0}", _toCrawlId));

                result = true;
            }
            catch (Exception ex)
            {
                Write.Ex(method.Name, ex.Message);
                result = false;
            }
            return result;
        }
        private bool DeleteLinkToCrawlTest()
        {
            MethodBase method = MethodBase.GetCurrentMethod();

            bool result = false;
            try
            {
                var repo = GetRepo();

                repo.DeleteLinkToCrawl(_toCrawlId);

                var check = repo.GetLinkToCrawl(_toCrawlId);
                if (check == null)
                {
                    result = true;
                    Write.Pass(method.Name, string.Format("{0} deleted", _toCrawlId));
                }
                else
                {
                    result = false;
                    Write.Fail(method.Name, string.Format("{0} not deleted", _toCrawlId));
                }
            }
            catch (Exception ex)
            {
                Write.Ex(method.Name, ex.Message);
                result = false;
            }
            return result;
        }
        private bool DeleteLinkToCrawlTest_By_Params()
        {
            MethodBase method = MethodBase.GetCurrentMethod();

            bool result = false;
            try
            {
                var repo = GetRepo();
                                
                _toCrawlSessionId = 76;
                _toCrawlBaseDomain = string.Format("c{0}.com", 33);
                var link = TestData.GetLinkToCrawl(_toCrawlSourceUrl, _toCrawlTargetUrl);
                link.SessionId = _toCrawlSessionId;
                link.TargetBaseDomain = _toCrawlBaseDomain;

                repo.AddLinkToCrawl(link);

                var id = link.Id;

                var check = repo.GetLinkToCrawl(id);
                if (check != null)
                {
                    repo.DeleteLinkToCrawl(_toCrawlSessionId, _toCrawlSourceUrl, _toCrawlTargetUrl);
                    check = repo.GetLinkToCrawl(_toCrawlId);
                    if (check == null)
                    {
                        result = true;
                        Write.Pass(method.Name, string.Format("{0} deleted", _toCrawlId));
                    }
                    else
                    {
                        result = false;
                        Write.Fail(method.Name, string.Format("{0} should be null", _toCrawlId));
                    }
                }
                else
                {
                    result = false;
                    Write.Fail(method.Name, string.Format("{0} should not be null", _toCrawlId));
                }
            }
            catch (Exception ex)
            {
                Write.Ex(method.Name, ex.Message);
                result = false;
            }
            return result;
        }
        private bool GetCountOfLinksToCrawlTest()
        {
            ClearTable("LinkToCrawl");

            MethodBase method = MethodBase.GetCurrentMethod();

            bool result = false;
            try
            {
                var repo = GetRepo();

                var link = TestData.GetLinkToCrawl("X", "Y");
                link.Id = Guid.NewGuid();
                link.TargetBaseDomain = _toCrawlBaseDomain;
                link.SessionId = _toCrawlSessionId;

                repo.AddLinkToCrawl(link);

                link = TestData.GetLinkToCrawl("X1", "Y1");
                link.Id = Guid.NewGuid();
                link.TargetBaseDomain = "Blah.ZZZ";
                link.SessionId = _toCrawlSessionId;
                repo.AddLinkToCrawl(link);

                link = TestData.GetLinkToCrawl("X2", "Y2");
                link.Id = Guid.NewGuid();
                link.TargetBaseDomain = _toCrawlBaseDomain.ToLower();
                link.SessionId = _toCrawlSessionId;
                repo.AddLinkToCrawl(link);

                link = TestData.GetLinkToCrawl("A", "B");
                link.Id = Guid.NewGuid();
                link.TargetBaseDomain = "X.COm";
                link.SessionId = 444;
                repo.AddLinkToCrawl(link);

                var count = repo.GetCountOfLinksToCrawl(_toCrawlSessionId, _toCrawlBaseDomain);

                if (count == 2)
                {
                    result = true;
                    Write.Pass(method.Name, string.Format("count = {0}", count));
                }
                else
                {
                    Write.Fail(method.Name, string.Format("count should not be {0}", count));
                    result = false;
                }
            }
            catch (Exception ex)
            {
                Write.Ex(method.Name, ex.Message);
                result = false;
            }
            return result;
        }
        private bool ClearLinkToCrawlsTest()
        {
            MethodBase method = MethodBase.GetCurrentMethod();

            bool result = false;
            try
            {
                var repo = GetRepo();

                var count = repo.GetCountOfLinksToCrawl(_toCrawlSessionId, _toCrawlBaseDomain);
                if (count > 0)
                {
                    result = true;

                    repo.ClearLinksToCrawl(_toCrawlSessionId, _toCrawlBaseDomain);
                    count = repo.GetCountOfLinksToCrawl(_toCrawlSessionId, _toCrawlBaseDomain);
                    if (count == 0)
                    {
                        result = true;

                        Write.Pass(method.Name, string.Format("count = 0"));
                    }
                    else
                    {
                        Write.Fail(method.Name, string.Format("expected count = 0"));
                        result = false;
                    }
                }
                else
                {
                    Write.Fail(method.Name, string.Format("expected start count <> {0}", count));
                    result = false;
                }
            }
            catch (Exception ex)
            {
                Write.Ex(method.Name, ex.Message);
                result = false;
            }
            return result;
        }
        private bool GetNextLinksToCrawlTest()
        {
            ClearTable("LinkToCrawl");

            MethodBase method = MethodBase.GetCurrentMethod();

            bool result = false;
            try
            {
                var repo = GetRepo();

                var link = TestData.GetLinkToCrawl("XAAA", "Y");
                link.Id = Guid.NewGuid();
                link.TargetBaseDomain = _toCrawlBaseDomain;
                link.SessionId = _toCrawlSessionId;

                repo.AddLinkToCrawl(link);

                link = TestData.GetLinkToCrawl("XAAA1", "Y1");
                link.Id = Guid.NewGuid();
                link.TargetBaseDomain = "Blah.ZZZ";
                link.SessionId = _toCrawlSessionId;
                repo.AddLinkToCrawl(link);

                link = TestData.GetLinkToCrawl("XAAA2", "Y2");
                link.Id = Guid.NewGuid();
                link.TargetBaseDomain = _toCrawlBaseDomain.ToLower();
                link.SessionId = _toCrawlSessionId;
                repo.AddLinkToCrawl(link);

                link = TestData.GetLinkToCrawl("AVVVVV", "B");
                link.Id = Guid.NewGuid();
                link.TargetBaseDomain = "X.COm";
                link.SessionId = 444;
                repo.AddLinkToCrawl(link);

                var results = repo.GetNextLinksToCrawl(_toCrawlSessionId, _toCrawlBaseDomain);

                if (results == null || results.Count() != 2)
                {
                    result = false;
                    Write.Fail(method.Name, string.Format("count should be {0}", 2));
                }
                else
                {
                    result = true;
                    Write.Pass(method.Name, string.Format("Results count = {0}", results.Count()));
                }
            }
            catch (Exception ex)
            {
                Write.Ex(method.Name, ex.Message);
                result = false;
            }
            return result;
        }
        private bool GetNextLinkToCrawlTest()
        {
            MethodBase method = MethodBase.GetCurrentMethod();

            bool result = false;
            try
            {
                var repo = GetRepo();

                var link = repo.GetNextLinkToCrawl(_toCrawlSessionId, _toCrawlBaseDomain, true);

                if (link != null)
                {
                    result = true;
                    Write.Pass(method.Name, string.Format("return link with id: {0}", link.Id));
                }
                else
                {
                    Write.Fail(method.Name, string.Format("returned null"));
                    result = false;
                }
            }
            catch (Exception ex)
            {
                Write.Ex(method.Name, ex.Message);
                result = false;
            }
            return result;
        }

        //UserAgents
        private bool GetUserAgentTest()
        {
            MethodBase method = MethodBase.GetCurrentMethod();

            bool result = false;
            try
            {
                var repo = GetRepo();
                Guid id = new Guid("8F15729F-D0E1-498F-80A6-91E32BA49E69");
                var agent = repo.GetUserAgent(id);
                if (agent == null)
                {
                    result = false;
                    Write.Fail(method.Name, string.Format("UserAgent {0} was null", _toCrawlId));
                }
                else
                {
                    result = true;
                    Write.Pass(method.Name, string.Format("UserAgent {0} returned by id", _toCrawlId));
                }
            }
            catch (Exception ex)
            {
                Write.Ex(method.Name, ex.Message);
                result = false;
            }
            return result;
        }
        private bool GetUserAgentsTest()
        {
            MethodBase method = MethodBase.GetCurrentMethod();

            bool result = false;
            try
            {
                var repo = GetRepo();
                var agents = repo.GetUserAgents();
                if (agents == null || agents.Count() == 0)
                {
                    result = false;
                    Write.Fail(method.Name, string.Format("Null/Empty was returned"));
                }
                else
                {
                    result = true;
                    Write.Pass(method.Name, string.Format("Agents count = {0}", agents.Count()));
                }
            }
            catch (Exception ex)
            {
                Write.Ex(method.Name, ex.Message);
                result = false;
            }
            return result;
        }

        //BlackList
        private bool AddBlacklistedTest()
        {
            MethodBase method = MethodBase.GetCurrentMethod();

            bool result = false;
            try
            {
                var repo = GetRepo();

                repo.AddBlacklisted(_blackListedUrl);

                result = true;
            }
            catch (Exception ex)
            {
                Write.Ex(method.Name, ex.Message);
                result = false;
            }
            if (result)
                Write.Pass(method.Name, string.Format("blacklisted url added"));
            else
                Write.Fail(method.Name, string.Format("blacklisted url not added"));

            return result;
        }
        private bool IsBlacklistedTest()
        {
            MethodBase method = MethodBase.GetCurrentMethod();

            bool result = false;
            try
            {
                var repo = GetRepo();

                result = repo.IsBlackListed(_blackListedUrl);

                if (result)
                    Write.Pass(method.Name, string.Format("true expected"));
                else
                    Write.Fail(method.Name, string.Format("true expected"));
            }
            catch (Exception ex)
            {
                Write.Ex(method.Name, ex.Message);
                result = false;
            }

            return result;
        }
        private bool IsBlacklistedTest_Expecting_False()
        {
            MethodBase method = MethodBase.GetCurrentMethod();

            bool result = false;
            try
            {
                var repo = GetRepo();

                result = repo.IsBlackListed(_blackListedUrl + "BLAH");

                if (!result)
                    Write.Pass(method.Name, string.Format("false expected"));
                else
                    Write.Fail(method.Name, string.Format("false expected"));
            }
            catch (Exception ex)
            {
                Write.Ex(method.Name, ex.Message);
                result = false;
            }

            return result;
        }
        private bool GetBlackListTest()
        {
            MethodBase method = MethodBase.GetCurrentMethod();

            bool result = false;
            try
            {
                var repo = GetRepo();

                var list = repo.GetBlackList();

                if (list == null || list.Count() == 0)
                {
                    result = false;
                    Write.Fail(method.Name, string.Format("Null/Empty was returned"));
                }
                else
                {
                    result = true;
                    Write.Pass(method.Name, string.Format("BlackList count = {0}", list.Count()));
                }
            }
            catch (Exception ex)
            {
                Write.Ex(method.Name, ex.Message);
                result = false;
            }

            return result;
        }

        public void RunTests()
        {
            ClearDb();


            Console.WriteLine("SessionConfig CRUD Tests (3)-------------------");
            #region CrawlerRun
            bool result = AddSessionTest();
            if (result)
                result = GetSessionTest();
            if (result)
                result = DeleteSessionTest();
            #endregion

            Console.WriteLine();

            Console.WriteLine("CrawlerRun CRUD Tests (7)-------------------");
            #region CrawlerRun
            result = AddCrawlerRunTest();
            if (result)
                result = GetCrawlerRunTest();
            if (result)
                result = GetCrawlerRunTest_By_Params();
            if (result)
                result = GetCrawlerRunTest_By_BaseDomain_Params();
            if (result)
                result = GetCountOfCrawlsInProgressTest();
            if (result)
                result = GetNextCrawlerIdTest();
            if (result)
                result = UpdateCrawlerRunTest();
            if (result)
                result = DeleteCrawlerRunTest(); 
            #endregion

            Console.WriteLine();

            Console.WriteLine("ProcessedPage CRUD Tests (6)-------------------");
            #region ProcessedPage
            result = AddProcessedPageTest();
            if (result)
                result = GetProcessedPageTest();
            if (result)
                result = UpdateProcessedPageTest();
            if (result)
                result = UpdateProcessedPageTest();
            if (result)
                result = IsPageProcessedTest();
            if (result)
                result = DeleteProcessedPageTest(); 
            #endregion

            Console.WriteLine();

            Console.WriteLine("CrawledLink CRUD Tests (8)-------------------");
            #region CrawledLink
            result = AddCrawledLinkTest();
            if (result)
                result = GetCrawledLinkTest();
            if (result)
                result = GetCrawledLinkTest_By_Params();
            if (result)
                result = IsCrawledTest();
            if (result)
                result = IsCrawledTest_Expecting_False();
            if (result)
                result = GetCrawledLinkCountTest();
            if (result)
                result = DeleteCrawledLinkTest();
            if (result)
                result = ClearCrawledLinksTest(); 
            #endregion

            Console.WriteLine();

            Console.WriteLine("LinkToCrawl CRUD Tests (11)-------------------");
            #region LinkToCrawl
            result = AddLinkToCrawlTest();
            if (result)
                result = GetLinkToCrawlTest();
            if (result)
                result = GetLinkToCrawlTest_By_Params();
            if (result)
                result = IsToBeCrawledTest();
            if (result)
                result = IsToBeCrawledTest_Expecting_False();
            if (result)
                result = DeleteLinkToCrawlTest();
            if (result)
                result = DeleteLinkToCrawlTest_By_Params();
            if (result)
                result = GetCountOfLinksToCrawlTest();
            if (result)
                result = ClearLinkToCrawlsTest();
            if (result)
                result = GetNextLinksToCrawlTest();
            if (result)
                result = GetNextLinkToCrawlTest(); 
            #endregion

            Console.WriteLine();

            Console.WriteLine("UserAgentString CRUD Tests (2)-------------------");
            result = GetUserAgentTest();
            if (result)
                result = GetUserAgentsTest();

            Console.WriteLine();

            Console.WriteLine("BlackList CRUD Tests (4)-------------------");
            result = AddBlacklistedTest();
            if (result)
                result = IsBlacklistedTest();
            if (result)
                result = IsBlacklistedTest_Expecting_False();
            if (result)
                result = GetBlackListTest();

            Console.WriteLine();
            Console.WriteLine("Done");
        }

        private IRepository GetRepo()
        {
            var sessionFactory = NHibernateHelper.SessionFactory;
            var repo = new Repository(sessionFactory, _connStr);
            return repo;
        }
        private void WritePass(string methodName, string body)
        {
            Console.WriteLine(string.Format("Pass: {0}: {1}", methodName, body));
        }
    }
}
