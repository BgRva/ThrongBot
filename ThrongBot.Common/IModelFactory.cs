using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Abot.Poco;
using ThrongBot.Common.Entities;

namespace ThrongBot.Common
{
    public interface IModelFactory : ILogic, IDisposable
    {
        LinkToCrawl ConvertToLinkToCrawl(PageToCrawl page, int sessionId);
        LinkToCrawl CreateLinkToCrawl(CrawledPage page, Uri targetUri, int sessionId);
        PageToCrawl ConvertToPageToCrawl(LinkToCrawl link, int crawlerId);
        /// <summary>
        /// Creates and returns a CrawledLink instance where <paramref name="targetUri"/> 
        /// is the url that was crawled.
        /// </summary>
        CrawledLink CreateCrawledLink(Uri sourceUri, Uri targetUri, int sessionId, int crawlerId);
        CrawledLink CreateCrawledLink(string sourceUrl, string targetUrl, int sessionId, int crawlerId);
        /// <summary>
        /// Creates and returns a CrawledLink instance where page.<paramref name="Uri"/> 
        /// is the url that was crawled.
        /// </summary>
        CrawledLink CreateCrawledLink(CrawledPage page, int sessionId, int crawlerId);
    }
}
