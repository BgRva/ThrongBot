using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ThrongBot.Common
{
    public class DomainCrawlStartedEventArgs: EventArgs
    {
        public DomainCrawlStartedEventArgs()
        { }

        public DomainCrawlStartedEventArgs(int sessionId, int crawlerId, string seedUrl, DateTime startTime, string baseDomain)
        {
            SessionId = sessionId;
            CrawlerId = crawlerId;
            SeedUrl = seedUrl;
            StartTime = startTime;
            BaseDomain = baseDomain;
        }
        
        public int SessionId { get; set;}
        public int CrawlerId { get; set;}
        public string SeedUrl { get; set;}
        public string BaseDomain { get; set; }
        public DateTime StartTime { get; set; }
    }
}
