using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ThrongBot.Common
{
    public class DomainCrawlEndedEventArgs: EventArgs
    {
        public DomainCrawlEndedEventArgs()
        { }

        public DomainCrawlEndedEventArgs(int sessionId, int crawlerId, DateTime endTime, bool errorOccurred, string baseDomain)
        {
            SessionId = sessionId;
            CrawlerId = crawlerId;
            EndTime = endTime;
            ErrorOccurred = errorOccurred;
            BaseDomain = baseDomain;
        }
        
        public int SessionId { get; set;}
        public int CrawlerId { get; set;}
        public DateTime EndTime { get; set; }
        public bool ErrorOccurred { get; set; }
        public string BaseDomain { get; set; }
    }
}
