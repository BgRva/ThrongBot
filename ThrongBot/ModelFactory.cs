using System;
using System.Net;
using Abot.Poco;
using ThrongBot.Common;
using ThrongBot.Common.Entities;

namespace ThrongBot
{
    public class ModelFactory : IModelFactory
    {
        public void Dispose() { }

        public virtual LinkToCrawl ConvertToLinkToCrawl(PageToCrawl page, int sessionId)
        {
            var link = new LinkToCrawl();
            link.SessionId = sessionId;
            link.SourceUrl = page.ParentUri.AbsoluteUri;
            link.TargetUrl = page.Uri.AbsoluteUri;
            link.TargetBaseDomain = page.Uri.GetBaseDomain();
            link.CrawlDepth = page.CrawlDepth;
            link.IsRoot = page.IsRoot;
            link.IsInternal = page.IsInternal;
            return link;
        }
        public virtual LinkToCrawl CreateLinkToCrawl(CrawledPage page, Uri targetUri, int sessionId)
        {
            var link = new LinkToCrawl();
            link.SessionId = sessionId;
            // this was the link that was just crawled to produce the CrawledPage
            link.SourceUrl = page.Uri.AbsoluteUri;
            // this is the link parsed that must be scheduled
            link.TargetUrl = targetUri.AbsoluteUri;
            link.TargetBaseDomain = targetUri.GetBaseDomain();
            // creating a link from a crawled page, so it will not be the root
            link.IsRoot = false;
            link.IsInternal = string.Compare(page.Uri.GetBaseDomain(), targetUri.GetBaseDomain(), true) == 0;
            // increasing depth is also done in the default scheduler
            link.CrawlDepth = page.CrawlDepth + 1;
            return link;
        }
        public virtual PageToCrawl ConvertToPageToCrawl(LinkToCrawl link, int crawlerId)
        {
            var page = new PageToCrawl(new Uri(link.TargetUrl));
            page.PageBag.SessionId = link.SessionId;
            page.PageBag.CrawlerId = crawlerId;
            page.ParentUri = new Uri(link.SourceUrl);
            page.CrawlDepth = link.CrawlDepth;
            page.IsInternal = link.IsInternal;
            page.IsRoot = link.IsRoot;
            return page;
        }
        public virtual CrawledLink CreateCrawledLink(Uri sourceUri, Uri targetUri, int sessionId, int crawlerId)
        {
            return CreateCrawledLink(sourceUri.AbsoluteUri, targetUri.AbsoluteUri, sessionId, crawlerId);
        }
        public virtual CrawledLink CreateCrawledLink(string sourceUrl, string targetUrl, int sessionId, int crawlerId)
        {
            var link = new CrawledLink();
            link.SessionId = sessionId;
            link.CrawlerId = crawlerId;
            link.SourceUrl = sourceUrl;
            link.TargetUrl = targetUrl;
            return link;
        }
        public virtual CrawledLink CreateCrawledLink(CrawledPage page, int sessionId, int crawlerId)
        {
            var link = new CrawledLink();
            link.SessionId = page.PageBag.SessionId;
            link.CrawlerId = page.PageBag.CrawlerId;
            link.SourceUrl = page.ParentUri.AbsoluteUri;
            link.TargetUrl = page.Uri.AbsoluteUri; // what was crawled
            link.StatusCode = page.HttpWebResponse.StatusCode;
            link.IsRoot = page.IsRoot;
            link.CrawlDepth = page.CrawlDepth;
            return link;
        }
    }
}
