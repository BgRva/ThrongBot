using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ThrongBot.Common;
using log4net;

[assembly: log4net.Config.XmlConfigurator(Watch = true)]

namespace ThrongBot.FileBased.TestApp
{
    class Program
    {
        static ILog _logger = LogManager.GetLogger(typeof(Program).FullName);
        static void Main(string[] args)
        {
            Console.WriteLine("Press any key to start crawling ...");
            Console.ReadLine();
            int sessionId = 33;
            int crawlerId = 44;

            var repo = GetRepo();
            var existingRun = repo.GetCrawl(sessionId, crawlerId);
            if (existingRun != null)
            {
                var mssg = string.Format("CrawlerRun exists with sessionId: {0} and crawlerId: {1}; cancelling run ...", sessionId, crawlerId);
                Console.WriteLine(mssg);
            }
            else
            {
                var crawler = CreateAndInitCrawler(sessionId, crawlerId, "http://www.bluespiders.net", repo);
                crawler.StartCrawl();
            }

            //-----------------
            Console.WriteLine("Press any key to exit ...");
            Console.Read();

            //Close repo files
            if (repo is ThrongBot.Repository.File.FileBasedRepository)
            {
                var fRepo = (ThrongBot.Repository.File.FileBasedRepository)repo;
                fRepo.WriteDataFiles();
            }
            Console.WriteLine("Repository closed");
        }

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

        public static ICrawlDaddy CreateAndInitCrawler(int sessionId, int crawlerId, string seedUrl, IRepository repo)
        {
            var daddy = new CrawlDaddy(new LogicProvider(), repo);
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
            Console.WriteLine(string.Format("Crawl End Time: {0}",  e.EndTime));
            Console.WriteLine();
        }

        private static void daddy_DomainCrawlStarting(object sender, DomainCrawlStartedEventArgs e)
        {
            Console.WriteLine(string.Format("Crawl Start Time {0}", e.StartTime));
            Console.WriteLine();
        }

        public static IRepository GetRepo()
        {
            var repo = ThrongBot.Repository.File.FileBasedRepository.GetInstance();
            repo.LoadDataFiles();
            return repo;
        }
    }
}
