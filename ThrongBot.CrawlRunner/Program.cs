using ThrongBot.Common;
using ThrongBot.Common.Entities;
using log4net;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ThrongBot.Repository.SqlServer;
using System.Configuration;

namespace ThrongBot.CrawlRunner
{
    class Program
    {
        static ILog _logger = LogManager.GetLogger(typeof(Program).FullName);
        static bool _inProgress;
        static int _sessionId;
        static int _crawlerId;
        static string _seedUrl;     

        static void Main(string[] args)
        {
            #region parse args

            if (!ParseArgs(args))
            {
                Console.Read();
                return;
            } 

            #endregion

            #region Initializae

            Bootstrap.Bootstrapper.Initialize(new Bootstrap.ApplicationModule());

            #endregion
            Console.WriteLine("Session Id: {0}", _sessionId);
            Console.WriteLine("Crawler Id: {0}", _crawlerId);
            Console.WriteLine("Seed Url:   {0}", _seedUrl);
            Console.WriteLine("Press any key to start crawling ...");
            Console.ReadLine();

            try
            {
                if (!IsCrawlDefined(_sessionId, _crawlerId))
                {
                    _inProgress = true;
                    var crawler = CreateAndInitCrawler(_sessionId, _crawlerId, _seedUrl);
                    crawler.StartCrawl();
                }
            }
            catch (Exception e)
            {
                _logger.Error(string.Format("Exception caught in crawl sessionId: {0} and crawlerId: {1};", _sessionId, _crawlerId), 
                              e);
                Console.WriteLine("{0} caught and suppressed in main ...", e.Message);
            }
            //-----------------
            Console.WriteLine("Press any key to exit ...");
            Console.Read();
        }

        public static ICrawlDaddy CreateAndInitCrawler(int sessionId, int crawlerId, string seedUrl)
        {
            var daddy = Bootstrap.Bootstrapper.Resolve<ICrawlDaddy>("Live");
            //var daddy = new TestSupport.FakeCrawlDaddy();
            daddy.InitializeCrawler(seedUrl, sessionId, crawlerId);
            daddy.DomainCrawlStarted += daddy_DomainCrawlStarting;
            daddy.DomainCrawlEnded += daddy_DomainCrawlEnded;
            daddy.ExternalLinksFound += daddy_ExternalLinksFound;
            daddy.LinkCrawlCompleted += daddy_LinkCrawlCompleted;
            return daddy;
        }
        private static void daddy_LinkCrawlCompleted(object sender, LinkCrawlCompletedArgs e)
        {
            Console.WriteLine(string.Format("Link Crawl Completed {0}", e.TargetUrl));
            Console.WriteLine();
        }
        private static void daddy_ExternalLinksFound(object sender, ExternalLinksFoundEventArgs e)
        {
            Console.WriteLine(string.Format("External Links Found at {1}", e.CrawlerId, e.PageUri));
            Console.WriteLine();
        }
        private static void daddy_DomainCrawlEnded(object sender, DomainCrawlEndedEventArgs e)
        {
            Console.WriteLine(string.Format("Crawl End Time: {0}", e.EndTime));
            Console.WriteLine();
        }
        private static void daddy_DomainCrawlStarting(object sender, DomainCrawlStartedEventArgs e)
        {
            Console.WriteLine(string.Format("Crawl Start Time {0}", e.StartTime));
            Console.WriteLine();
        }

        public static IRepository GetRepo()
        {
            var connStr = System.Configuration.ConfigurationManager.ConnectionStrings["SqlServerRepository"].ConnectionString;
            var sessionFactory = NHibernateHelper.SessionFactory;
            var repo = new Repository.SqlServer.Repository(sessionFactory, connStr);
            return repo;
        }

        private static bool IsCrawlDefined(int sessionId, int crawlerId)
        {
            bool result = true;

            try
            {
                var repo = Bootstrap.Bootstrapper.Resolve<IRepository>();
                var crawl = repo.GetCrawl(sessionId, crawlerId);
                if (crawl != null)
                {
                    Console.WriteLine(string.Format("CrawlerRun exists with sessionId: {0} and crawlerId: {1};", _sessionId, _crawlerId));
                    if (crawl.EndTime.HasValue && crawl.EndTime.Value > crawl.StartTime)
                        Console.WriteLine(string.Format("    The crawl completed on {0}", crawl.EndTime.Value));
                    else
                        Console.WriteLine(string.Format("    InProgress = {0}", crawl.InProgress));
                }
                else
                    result = false;
            }
            catch (Exception e)
            {
                throw new Exception("Exception thrown initializing a repo and checking if a crawl exists.", e);
            }

            return result;
        }
        private static bool ParseArgs(string[] args)
        {
            bool result = true;

            if (args == null || args.Length != 3)
            {
                result = false;
                Console.WriteLine();
                Console.WriteLine("Usage: ThrongBot.CrawlRunner.exe sessionId crawlerId seedUrl");
                Console.WriteLine();
            }
            else
            {
                if (!int.TryParse(args[0], out _sessionId))
                {
                    result = false;
                    System.Console.WriteLine(string.Format("Invalid session id: {0}", args[0]));
                }
                if (!int.TryParse(args[1], out _crawlerId))
                {
                    result = false;
                    System.Console.WriteLine(string.Format("Invalid crawler id: {0}", args[1]));
                }
                if (string.IsNullOrWhiteSpace(args[2]))
                {
                    result = false;
                    System.Console.WriteLine(string.Format("Invalid seed url: {0}", args[2]));
                }
                else
                {
                    _seedUrl = args[2];
                }
            }

            return result;
        }
       

        //=======================

        public static void LogTest()
        {
            Console.WriteLine("_logger test:");
            string mssg = "test message ...";

            _logger.Fatal(mssg);
            _logger.Error(mssg);
            _logger.Warn(mssg);
            _logger.Info(mssg);
            _logger.Debug(mssg);

            try
            {
                throw new ArgumentException("Test outer exception", new NotImplementedException("Test inner exception"));
            }
            catch (Exception ex)
            {
                Console.WriteLine("_logger exception test:");
                _logger.Error("Exception test", ex);
            }
            Console.WriteLine();
        }

        public static bool CanExternalCrawlBeSpawned(IRepository repo, int sessionId)
        {
            bool canSpawn = false;
            try
            {
                var appSetting = ConfigurationManager.AppSettings["SpawnCrawlsWhenExternalLinksFound"];
                bool.TryParse(appSetting, out canSpawn);
            }
            catch (ConfigurationErrorsException)
            {
                //suppress
                return canSpawn = false;
            }

            int inProgress = repo.GetCountOfCrawlsInProgress(sessionId);
            if (inProgress < repo.GetSession(sessionId).MaxConcurrentCrawls)
            {
                canSpawn = true;
            }

            return canSpawn;
        }

        public static void SpawnExternalCrawl(IRepository repo, int sessionId, int crawlerId, Uri externalUri)
        {
            //if we are less than max concurrent crawls
            //and this crawl is not defined
            //spawn the process

            int inProgress = repo.GetCountOfCrawlsInProgress(sessionId);
            if (inProgress < repo.GetSession(sessionId).MaxConcurrentCrawls)
            {
                int nextCrawlerId = -1;
                bool alreadyDefined = false;
                do
                {
                    nextCrawlerId = repo.GetNextCrawlerId(sessionId);
                    alreadyDefined = repo.GetCrawl(sessionId, nextCrawlerId) != null;
                } while (alreadyDefined || nextCrawlerId <= crawlerId);

                //Spawn
                {
                    string args = string.Format("{0} {1} {2}", sessionId, nextCrawlerId, externalUri.AbsoluteUri);
                    ProcessStartInfo psi = new ProcessStartInfo("ThrongBot.CrawlRunner.exe", args);

                    Process p = Process.Start(psi);

                    Console.WriteLine("Process {0} spawned, crawlerId: {1}, seedUrl: {1}", p.Id, nextCrawlerId, externalUri.AbsoluteUri);
                }
            }
        }
    }
}
