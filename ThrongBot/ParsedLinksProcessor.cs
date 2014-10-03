using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Abot.Poco;
using ThrongBot.Common;
using ThrongBot.Common.Entities;
using log4net;

namespace ThrongBot
{
    public class ParsedLinksProcessor : ILinksProcessor
    {
        static ILog _logger = LogManager.GetLogger(typeof(ParsedLinksProcessor).FullName);
        private ILogicProvider _provider = null;

        public ParsedLinksProcessor(ILogicProvider provider)
        {
            _provider = provider;
        }

        #region IDisposable methods

        public void Dispose()
        {
            var disposable = _provider as IDisposable;
            if (disposable != null)
                disposable.Dispose();
            _provider = null;
        }
        #endregion

        public void ProcessLinks(Abot.Poco.CrawledPage page)
        {
            if (page.ParsedLinks == null || page.ParsedLinks.Count() == 0)
            {
                _logger.DebugFormat("CrawledPage contained 0 parsed links");
                LinksToCrawl = new List<LinkToCrawl>();
                LinksToByPass = new List<CrawledLink>();
                return;
            }

            LinksToByPass = new List<CrawledLink>();
            MapOfLinksToCrawl = new Dictionary<string, LinkToCrawl>();

            using (var factory = _provider.GetInstanceOf<IModelFactory>())
            {
                var sessionId = page.PageBag.SessionId;
                var crawlerId = page.PageBag.CrawlerId;
                LinkToCrawl link = null;
                CrawledLink bypassedLink = null;
                foreach (var targetUri in page.ParsedLinks)
                {
                    ProcessLink(page, factory, targetUri, sessionId, crawlerId);
                }

                LinksToCrawl = MapOfLinksToCrawl.Values.ToList();
                MapOfLinksToCrawl.Clear();
                MapOfLinksToCrawl = null;
                if (_logger.IsDebugEnabled)
                {
                    _logger.DebugFormat("TargetUrls of new LinksToCrawl: {0}",
                                        String.Join("; ", LinksToCrawl.Select(o => o.TargetUrl)));
                    _logger.DebugFormat("TargetUrls of new LinksToByPass: {0}",
                                        String.Join("; ", LinksToByPass.Select(o => o.TargetUrl)));
                }
            }
        }
        public List<LinkToCrawl> LinksToCrawl { get; internal set; }
        public List<CrawledLink> LinksToByPass { get; internal set; }
        public bool ExternalLinksFound { get; private set; }

        //Helpers

        /// <summary>
        /// Dictionary of links that are to be crawled, this is used in processing
        /// the links but is not intended for caller consumption.  Each call either adds
        /// an entry to LinksToByPass or MapOfLinksToCrawl.
        /// </summary>
        internal Dictionary<string, LinkToCrawl> MapOfLinksToCrawl { get; set; }
        /// <summary>
        /// Processes the Uri specified by <paramref name="targetUri"/> as a potential link to be crawled,
        /// bypassed, or ignored.
        /// </summary>
        /// <param name="page">The CrawledPage from which the targetUri was parsed.</param>
        /// <param name="factory">An instance of IModelFactory</param>
        /// <param name="targetUri">The target Uri being processed</param>
        internal void ProcessLink(Abot.Poco.CrawledPage page, IModelFactory factory, Uri targetUri, int sessionId, int crawlerId)
        {
            CrawledLink bypassedLink = null;

            if (targetUri.Scheme == Uri.UriSchemeMailto)
            {
                // Mailto schema: bypass
                bypassedLink = factory.CreateCrawledLink(page.Uri, targetUri, sessionId, crawlerId);
                bypassedLink.IsRoot = false;
                bypassedLink.CrawlDepth = page.CrawlDepth + 1;
                bypassedLink.StatusCode = HttpStatusCode.OK;
                bypassedLink.Bypassed = true;
                LinksToByPass.Add(bypassedLink);
            }
            else if (string.Compare(page.Uri.AbsoluteUri, targetUri.AbsoluteUri) == 0)
            {
                // Exact self loops: bypass
                bypassedLink = factory.CreateCrawledLink(page.Uri, targetUri, sessionId, crawlerId);
                bypassedLink.IsRoot = false;
                bypassedLink.CrawlDepth = page.CrawlDepth + 1;
                bypassedLink.StatusCode = HttpStatusCode.OK;
                bypassedLink.Bypassed = true;
                LinksToByPass.Add(bypassedLink);
            }
            else if (MapOfLinksToCrawl.ContainsKey(targetUri.AbsoluteUri))
            {
                // Duplicates: bypass
                bypassedLink = factory.CreateCrawledLink(page.Uri, targetUri, sessionId, crawlerId);
                bypassedLink.IsRoot = false;
                bypassedLink.CrawlDepth = page.CrawlDepth + 1;
                bypassedLink.StatusCode = HttpStatusCode.OK;
                bypassedLink.Bypassed = true;
                LinksToByPass.Add(bypassedLink);
            }
            else
            {
                // process link to be crawled that was parsed from a crawled page, so
                // it will not be a root.
                var link = factory.CreateLinkToCrawl(page, targetUri, sessionId);
                MapOfLinksToCrawl.Add(targetUri.AbsoluteUri, link);

                if (string.Compare(page.Uri.GetBaseDomain(), targetUri.GetBaseDomain(), true) != 0)
                    ExternalLinksFound |= true;
            }
        }
    }
}
