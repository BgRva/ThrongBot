using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Abot.Core;
using Abot.Poco;
using ThrongBot.Common;
using ThrongBot.Common.Entities;
using log4net;

namespace ThrongBot
{
    public class MyScheduler : IMyScheduler, IDisposable
    {
        static ILog _logger = LogManager.GetLogger(typeof(MyScheduler).FullName);
        private ILogicProvider _provider = null;
        private IRepository _repo = null;
        private bool _allowUriRecrawling;

        public MyScheduler(ILogicProvider provider, CrawlerRun definition, IRepository repo)
        {
            _provider = provider;
            _repo = repo;
            SessionId = definition.SessionId;
            CrawlerId = definition.CrawlerId;
            BaseDomain = definition.BaseDomain;
        }

        #region IDisposable methods

        public void Dispose()
        {
            var disposable = _provider as IDisposable;
            if (disposable != null)
                disposable.Dispose();
            _provider = null;
            //Do not dispose of the repo if it is passed in by the crawldaddy
            _repo = null;
        }   

        #endregion

        #region IScheduler methods

        public int Count
        {
            get
            {
                return _repo.GetCountOfLinksToCrawl(SessionId, BaseDomain);
            }
        }

        /// <summary>
        /// If this method is called, then it assumes some pre-logic for links to avoid has already
        /// been applied and that the <paramref name="page"/> should be stored for future crawling.
        /// </summary>
        /// <param name="page"></param>
        public void Add(PageToCrawl page)
        {
            if (page == null)
                throw new ArgumentNullException("page");
            _logger.DebugFormat("Add(page): Target: {0}, Source: {1}, Root: {2}",
                page.Uri.AbsoluteUri,
                page.ParentUri.AbsoluteUri,
                page.IsRoot);
            page.PageBag.SessionId = SessionId;
            page.PageBag.CrawlerId = CrawlerId;
            using (var factory = _provider.GetInstanceOf<IModelFactory>())
            {
                var link = factory.ConvertToLinkToCrawl(page, SessionId);
                AddLinkToCrawl(link);
            }
        }
        public void Add(IEnumerable<PageToCrawl> pages)
        {
            if (pages == null)
                throw new ArgumentNullException("pages");

            using (var factory = _provider.GetInstanceOf<IModelFactory>())
            {
                List<LinkToCrawl> links = new List<LinkToCrawl>();
                LinkToCrawl link = null;
                foreach(var page in pages)
                {
                    _logger.DebugFormat("Add(pages): Target: {0}, Source: {1}, Root: {2}",
                        page.Uri.AbsoluteUri,
                        page.ParentUri.AbsoluteUri,
                        page.IsRoot);
                    link = factory.ConvertToLinkToCrawl(page, SessionId);
                    links.Add(link);
                }
                AddLinksToCrawl(links);
            }
        }
        public PageToCrawl GetNext()
        {
            return GetNextLinkToCrawl(SessionId);
        }
        public void Clear()
        {
            _repo.ClearLinksToCrawl(SessionId, BaseDomain);
        } 

        #endregion

        #region IMyScheduler methods

        public int SessionId { get; private set; }
        public int CrawlerId { get; private set; }
        public string BaseDomain { get; private set; }

        public PageToCrawl GetNextLinkToCrawl(int sessionId)
        {
            PageToCrawl page = null;

            // the next link to crawl is a link in the current domain
            // that has not been crawled, see RecordCrawledUri() as well
            // for more info
            var linkToCrawl = _repo.GetNextLinkToCrawl(sessionId, BaseDomain, true);
            
            _logger.DebugFormat("_repo.GetNextLinkToCrawl(): Target: {0}, Source: {1}, Root: {2}", 
                                linkToCrawl.TargetUrl, linkToCrawl.SourceUrl, linkToCrawl.IsRoot);

            if (linkToCrawl != null)
            {
                using (var factory = _provider.GetInstanceOf<IModelFactory>())
                {
                    page = factory.ConvertToPageToCrawl(linkToCrawl, CrawlerId);
                }

                _logger.DebugFormat("GetNextLinkToCrawl(): Target: {0}, Source: {1}, Root: {2}",
                    page.Uri.AbsoluteUri,
                    page.ParentUri.AbsoluteUri,
                    page.IsRoot);
            }
            
            return page;
        }
        public Guid RecordCrawledLink(CrawledLink crawledLink)
        {
            _logger.DebugFormat("RecordCrawledLink(): TargetUrl: {0}, SourceUrl: {1}, Root: {2}",
                                crawledLink.TargetUrl,
                                crawledLink.SourceUrl,
                                crawledLink.IsRoot);
            // record the crawled link
            _repo.AddCrawledLink(crawledLink, true);

            return crawledLink.Id;
        }
        /// <summary>
        /// If this method is called, then it assumes some pre-logic for links to avoid has already
        /// been applied and that the <paramref name="page"/> should be stored for future crawling.
        /// </summary>
        public void AddLinkToCrawl(LinkToCrawl link)
        {
            if (link == null)
                throw new ArgumentNullException("link");

            _logger.DebugFormat("AddLinkToCrawl(): Target: {0}, Source: {1}, Root: {2}",
                                link.TargetUrl,
                                link.SourceUrl,
                                link.IsRoot);
            AddLinkToCrawlUnique(_repo, link);
        }

        public void AddLinksToCrawl(List<LinkToCrawl> links)
        {
            if (links == null)
                throw new ArgumentNullException("links");

            foreach (var link in links)
            {
                _logger.DebugFormat("AddLinksToCrawl(): Target: {0}, Source: {1}, Root: {2}",
                                    link.TargetUrl,
                                    link.SourceUrl,
                                    link.IsRoot);
                AddLinkToCrawlUnique(_repo, link);
            }
        }

        public bool ProcessParsedLinks(CrawledPage page)
        {
            _logger.DebugFormat("links parsed from crawled page: {0}", ConcatUris(page.ParsedLinks));
            // Before saving any links to crawl, process them to remove duplicates
            // and avoid black listed sites
            using (var linksProcessor = _provider.GetInstanceOf<ILinksProcessor>())
            {
                linksProcessor.ProcessLinks(page);
                foreach (var link in linksProcessor.LinksToCrawl)
                    AddLinkToCrawlUnique(_repo, link);
                foreach (var bypassed in linksProcessor.LinksToByPass)
                    _repo.AddCrawledLink(bypassed, true);
                return linksProcessor.ExternalLinksFound;
            }
        }

        #endregion

        /// <summary>
        /// Adds link as a link to crawl only after checking current links to
        /// crawl and links already crawled.  If link has already been crawled or
        /// is scheduled to be crawled, then it is added as a bypassed link.
        /// </summary>
        private void AddLinkToCrawlUnique(IRepository repo, LinkToCrawl link)
        {
            //If the link has already been crawled or it is scheduled to be crawled, then bypass
            if (repo.IsCrawled(link.SessionId, link.TargetUrl) || repo.IsToBeCrawled(link.SessionId, link.TargetUrl))
            {
                using (var factory = _provider.GetInstanceOf<IModelFactory>())
                {
                    var crawled = factory.CreateCrawledLink(link.SourceUrl, link.TargetUrl, SessionId, CrawlerId);
                    crawled.IsRoot = link.IsRoot;
                    crawled.CrawlDepth = link.CrawlDepth;
                    crawled.StatusCode = HttpStatusCode.OK;
                    crawled.Bypassed = true;
                    repo.AddCrawledLink(crawled, true);
                }
            }
            else
            {
                repo.AddLinkToCrawl(link);

                _logger.DebugFormat("AddLinkToCrawlUnique(): TargetUrl: {0}, SourceUrl: {1}, Root: {2}",
                    link.TargetUrl,
                    link.SourceUrl,
                    link.IsRoot);
            }
        }

        //FOR LOGGING
        private string ConcatUris(IEnumerable<Uri> uris)
        {
            var sb = new StringBuilder();
            foreach(var uri in uris)
            {
                sb.Append(uri.AbsoluteUri);
                sb.Append(Environment.NewLine);
            }
            return sb.ToString();
        }
    }
}
