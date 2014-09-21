using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Abot.Core;
using Abot.Poco;
using ThrongBot.Common.Entities;

namespace ThrongBot.Common
{
    public interface IMyScheduler : IScheduler
    {
        int SessionId { get; }
        int CrawlerId { get;}
        string BaseDomain { get; }
        Guid RecordCrawledLink(CrawledLink link);
        PageToCrawl GetNextLinkToCrawl(int sessionId);
        void AddLinkToCrawl(LinkToCrawl link);
        void AddLinksToCrawl(List<LinkToCrawl> links);
        /// <summary>
        /// Processes the parsed links in the crawled page to schedule for
        /// crawling.  True is returned if any of the links are external, where
        /// external being any link with a root domain different than the crawled
        /// page's Uri property.
        /// </summary>
        /// <param name="page">The CrawledPage object returned from a successful page crawl.</param>
        /// <returns>True if any of the parsed links are external.</returns>
        bool ProcessParsedLinks(CrawledPage page);
    }
}
