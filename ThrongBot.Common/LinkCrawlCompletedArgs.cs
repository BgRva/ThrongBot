using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ThrongBot.Common
{
    public class LinkCrawlCompletedArgs: EventArgs
    {
        public LinkCrawlCompletedArgs()
        { }

        public string SourceUrl {get;set;}
        public string TargetUrl {get;set;}
        public System.Net.HttpStatusCode Status { get; set; }
        public int SessionId {get; set;}
        public int CrawlerId { get; set; }
        public bool ErrorOccurred { get; set; }
        public bool ExternalLinksFound { get; set; }
    }
}
