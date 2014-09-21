using System;
using System.Net;
using System.Threading;
using Abot.Crawler;
using Abot.Poco;
using ThrongBot.Common;
using ThrongBot.Common.Entities;
using log4net;

namespace ThrongBot
{
    public class CrawlDaddy : ICrawlDaddy
    {
        static ILog _logger = LogManager.GetLogger(typeof(CrawlDaddy).FullName);
        private IMyScheduler _scheduler = null;
        private IWebCrawler _crawler = null;
        private ILogicProvider _provider = null;
        private IRepository _repo = null;
        private CrawlConfiguration _config = null;
        private CancellationTokenSource _cancelToken = new CancellationTokenSource();

        public CrawlDaddy(ILogicProvider provider, IRepository repo)
        {
            _provider = provider;
            _repo = repo;
        }

        #region IDisposable methods

        public void Dispose()
        {
            var disposable = _provider as IDisposable;
            if (disposable != null)
                disposable.Dispose();
            _provider = null;

            disposable = _repo as IDisposable;
            if (disposable != null)
                disposable.Dispose();
            _repo = null;

            if (_crawler != null)
            {
                _crawler.PageCrawlStartingAsync -= crawler_ProcessPageCrawlStarting;
                _crawler.PageCrawlCompletedAsync -= crawler_ProcessPageCrawlCompleted;
                _crawler.PageCrawlDisallowedAsync -= crawler_PageCrawlDisallowed;
                _crawler.PageLinksCrawlDisallowedAsync -= crawler_PageLinksCrawlDisallowed;
                _crawler = null;
            }
        }        
        #endregion

        public CrawlerRun CrawlerDefinition { get; set; }
        public int SessionId
        {
            get { return CrawlerDefinition.SessionId; }
        }
        public int CrawlerId
        {
            get { return CrawlerDefinition.CrawlerId; }
        }
        public Uri Seed { get; private set; }
        public string BaseDomain
        {
            get { return CrawlerDefinition.BaseDomain; }
        }

        public event EventHandler<ExternalLinksFoundEventArgs> ExternalLinksFound;
        #region OnExternalLinksFound
        protected virtual void OnExternalLinksFound(int crawlerId, Uri pageUri)
        {
            try
            {
                EventHandler<ExternalLinksFoundEventArgs> threadSafeEvent = ExternalLinksFound;
                if (threadSafeEvent != null)
                    threadSafeEvent(this, new ExternalLinksFoundEventArgs(crawlerId, pageUri));
            }
            catch (Exception e)
            {
                _logger.Error("An unhandled exception was thrown by a subscriber of the ExternalLinksFound event for url:"
                               + pageUri.AbsoluteUri);
                _logger.Error(e);
            }
        } 
        #endregion

        public event EventHandler<DomainCrawlStartedEventArgs> DomainCrawlStarted;
        #region OnDomainCrawlStarted
        protected virtual void OnDomainCrawlStarted(CrawlerRun definition)
        {
            try
            {
                EventHandler<DomainCrawlStartedEventArgs> threadSafeEvent = DomainCrawlStarted;
                if (threadSafeEvent != null)
                    threadSafeEvent(this, 
                                    new DomainCrawlStartedEventArgs(definition.SessionId, 
                                                                    definition.CrawlerId, 
                                                                    definition.SeedUrl, 
                                                                    definition.StartTime, 
                                                                    definition.BaseDomain));
            }
            catch (Exception e)
            {
                _logger.Error("An unhandled exception was thrown by a subscriber of the DomainCrawlStarting event for seed:"
                               + Seed.AbsoluteUri);
                _logger.Error(e);
            }
        }
        #endregion

        public event EventHandler<DomainCrawlEndedEventArgs> DomainCrawlEnded;
        #region OnDomainCrawlEnded
        protected virtual void OnDomainCrawlEnded(CrawlerRun definition)
        {
            try
            {
                EventHandler<DomainCrawlEndedEventArgs> threadSafeEvent = DomainCrawlEnded;

                if (threadSafeEvent != null)
                    threadSafeEvent(this, 
                                    new DomainCrawlEndedEventArgs(definition.SessionId, 
                                                                  definition.CrawlerId, 
                                                                  definition.EndTime.Value, 
                                                                  definition.ErrorOccurred, 
                                                                  definition.BaseDomain));
            }
            catch (Exception e)
            {
                _logger.Error("An unhandled exception was thrown by a subscriber of the DomainCrawlEnded event for crawl:"
                               + definition.CrawlerId);
                _logger.Error(e);
            }
        }
        #endregion

        public event EventHandler<LinkCrawlCompletedArgs> LinkCrawlCompleted;
        #region OnLinkCrawlCompleted
        protected virtual void OnLinkCrawlCompleted(CrawlerRun definition, 
                                                    string sourceUrl, 
                                                    string targetUrl, 
                                                    HttpStatusCode status, 
                                                    bool errorOccurred,
                                                    bool externalLinksFound)
        {
            try
            {
                EventHandler<LinkCrawlCompletedArgs> threadSafeEvent = LinkCrawlCompleted;
                if (threadSafeEvent != null)
                    threadSafeEvent(this,
                                    new LinkCrawlCompletedArgs()
                                    {
                                        SourceUrl = sourceUrl,
                                        TargetUrl = targetUrl,
                                        Status = status,
                                        ErrorOccurred = errorOccurred,
                                        CrawlerId = definition.CrawlerId,
                                        SessionId = definition.SessionId
                                    });
            }
            catch (Exception e)
            {
                _logger.Error("An unhandled exception was thrown by a subscriber of the LinkCrawlCompleted event for crawl:"
                               + definition.CrawlerId);
                _logger.Error(e);
            }
        }
        #endregion

        /// <summary>
        /// Initializes the crawler from configuration and stores a definition of the instance
        /// </summary>
        /// <param name="seedUrl"></param>
        /// <param name="sessionId"></param>
        /// <param name="crawlerId"></param>
        public bool InitializeCrawler(string seedUrl, int sessionId, int crawlerId)
        {
            // load the crawl configuration
            _config = new CrawlConfiguration();
            _config.CrawlTimeoutSeconds = 100;
            _config.MaxConcurrentThreads = 1;
            _config.MaxPagesToCrawl = 20;
            _config.IsExternalPageCrawlingEnabled = false;
            _config.IsExternalPageLinksCrawlingEnabled = false;
            _config.MinCrawlDelayPerDomainMilliSeconds = 3000;
            _config.DownloadableContentTypes = "text/html, text/plain";
            _config.IsHttpRequestAutoRedirectsEnabled = true;
            _config.IsUriRecrawlingEnabled = false;
            _config.UserAgentString = "Mozilla/5.0 (Windows NT 6.1; WOW64; rv:31.0) Gecko/20100101 Firefox/31.0";

            #region Old _config stuff
            //public long CrawlTimeoutSeconds { get; set; }
            //public string DownloadableContentTypes { get; set; }
            //public int HttpRequestMaxAutoRedirects { get; set; }
            //public int HttpRequestTimeoutInSeconds { get; set; }
            //public int HttpServicePointConnectionLimit { get; set; }
            //public bool IsExternalPageCrawlingEnabled { get; set; }
            //public bool IsExternalPageLinksCrawlingEnabled { get; set; }
            //public bool IsForcedLinkParsingEnabled { get; set; }
            //public bool IsHttpRequestAutomaticDecompressionEnabled { get; set; }
            //public bool IsHttpRequestAutoRedirectsEnabled { get; set; }
            //public bool IsRespectAnchorRelNoFollowEnabled { get; set; }
            //public bool IsRespectMetaRobotsNoFollowEnabled { get; set; }
            //public bool IsRespectRobotsDotTextEnabled { get; set; }
            //public bool IsUriRecrawlingEnabled { get; set; }
            //public int MaxConcurrentThreads { get; set; }
            //public int MaxCrawlDepth { get; set; }
            //public int MaxMemoryUsageCacheTimeInSeconds { get; set; }
            //public int MaxMemoryUsageInMb { get; set; }
            //public long MaxPageSizeInBytes { get; set; }
            //public long MaxPagesToCrawl { get; set; }
            //public long MaxPagesToCrawlPerDomain { get; set; }
            //public int MaxRobotsDotTextCrawlDelayInSeconds { get; set; }
            //public int MinAvailableMemoryRequiredInMb { get; set; }
            //public long MinCrawlDelayPerDomainMilliSeconds { get; set; }
            //public string RobotsDotTextUserAgentString { get; set; }
            //public string UserAgentString { get; set; } 
            #endregion

            //check if a crawl is already defined
            var existingRun = _repo.GetCrawl(sessionId, crawlerId);
            if (existingRun != null)
            {
                var mssg = string.Format("CrawlerRun exists with sessionId: {0} and crawlerId: {1}; cancelling run ...", sessionId, crawlerId);
                _logger.Error(mssg);
                return false;
            }
            Seed = new Uri(seedUrl);
            CrawlerDefinition = new CrawlerRun()
            {
                SessionId = sessionId,
                SeedUrl = Seed.AbsoluteUri,
                CrawlerId = crawlerId,
                BaseDomain = Seed.GetBaseDomain()
            };
            _repo.AddCrawl(CrawlerDefinition);
            _scheduler = new MyScheduler(new LogicProvider(), CrawlerDefinition, _repo);

            _crawler = new PoliteWebCrawler(_config, null, null, _scheduler, null, null, null, null, null);
            _crawler.CrawlBag.SessionId = CrawlerDefinition.SessionId;
            _crawler.CrawlBag.CrawlerId = CrawlerDefinition.CrawlerId;
            _crawler.ShouldScheduleLink(ShouldScheduleLink);
            _crawler.ShouldCrawlPage(ShouldCrawlPage);

            _crawler.PageCrawlStartingAsync += crawler_ProcessPageCrawlStarting;
            _crawler.PageCrawlCompletedAsync += crawler_ProcessPageCrawlCompleted;
            _crawler.PageCrawlDisallowedAsync += crawler_PageCrawlDisallowed;
            _crawler.PageLinksCrawlDisallowedAsync += crawler_PageLinksCrawlDisallowed;

            return true;
        }

        public bool InitializeCrawler(string seedUrl, int sessionId, int crawlerId, CrawlConfiguration config)
        {
            _config = config;

            //check if a crawl is already defined
            var existingRun = _repo.GetCrawl(sessionId, crawlerId);
            if (existingRun != null)
            {
                var mssg = string.Format("CrawlerRun exists with sessionId: {0} and crawlerId: {1}; cancelling run ...", sessionId, crawlerId);
                _logger.Error(mssg);
                return false;
            }
            Seed = new Uri(seedUrl);
            CrawlerDefinition = new CrawlerRun()
            {
                SessionId = sessionId,
                SeedUrl = Seed.AbsoluteUri,
                CrawlerId = crawlerId,
                BaseDomain = Seed.GetBaseDomain()
            };
            _repo.AddCrawl(CrawlerDefinition);
            _scheduler = new MyScheduler(new LogicProvider(), CrawlerDefinition, _repo);

            _crawler = new PoliteWebCrawler(_config, null, null, _scheduler, null, null, null, null, null);
            _crawler.CrawlBag.SessionId = CrawlerDefinition.SessionId;
            _crawler.CrawlBag.CrawlerId = CrawlerDefinition.CrawlerId;
            _crawler.ShouldScheduleLink(ShouldScheduleLink);
            _crawler.ShouldCrawlPage(ShouldCrawlPage);

            _crawler.PageCrawlStartingAsync += crawler_ProcessPageCrawlStarting;
            _crawler.PageCrawlCompletedAsync += crawler_ProcessPageCrawlCompleted;
            _crawler.PageCrawlDisallowedAsync += crawler_PageCrawlDisallowed;
            _crawler.PageLinksCrawlDisallowedAsync += crawler_PageLinksCrawlDisallowed;

            return true;
        }

        public void StartCrawl()
        {
            DateTime timeStamp = DateTime.Now;
            CrawlerDefinition.StartTime = timeStamp;
            CrawlerDefinition.InProgress = true;
            _repo.UpdateCrawl(CrawlerDefinition);

            OnDomainCrawlStarted(CrawlerDefinition);

            CrawlResult result = _crawler.Crawl(Seed, _cancelToken);
            CrawlerDefinition.InProgress = false;
            CrawlerDefinition.EndTime = CrawlerDefinition.StartTime.Add(result.Elapsed);
            CrawlerDefinition.ErrorOccurred = result.ErrorOccurred;

            _repo.UpdateCrawl(CrawlerDefinition);
            OnDomainCrawlEnded(CrawlerDefinition);

            if (result.ErrorOccurred)
            {
                var mssg = string.Format("Crawl of {0} completed with error: {1}", result.RootUri.AbsoluteUri, result.ErrorException.Message);
                _logger.Error(mssg);
            }
            else
            {
                var mssg = string.Format("Crawl of {0} completed without error.", result.RootUri.AbsoluteUri);
                _logger.Debug(mssg);
            }
        }

        public void CancelCrawl()
        {
            _cancelToken.Cancel();
        }

        private void crawler_ProcessPageCrawlStarting(object sender, PageCrawlStartingArgs e)
        {
            CrawlContext context = e.CrawlContext;

            //Also id info to the page to crawl (will be passed to Crawled Page)
            e.PageToCrawl.PageBag.SessionId = SessionId;
            e.PageToCrawl.PageBag.CrawlerId = CrawlerId;

            PageToCrawl pageToCrawl = e.PageToCrawl;
            _logger.DebugFormat("Page Crawl Starting {0} which was found on page {1}", 
                                pageToCrawl.Uri.AbsoluteUri, 
                                pageToCrawl.ParentUri.AbsoluteUri);
        }
        private void crawler_ProcessPageCrawlCompleted(object sender, PageCrawlCompletedArgs e)
        {
            CrawledPage crawledPage = e.CrawledPage;
            bool externalLinksFound = false;
            _logger.DebugFormat("Page Crawl Completed {0}; Status {1}; Source URL: {2}; CrawlerId: {3}; SessionId: {4}",
                                crawledPage.Uri.AbsoluteUri,
                                crawledPage.HttpWebResponse.StatusCode,
                                crawledPage.ParentUri.AbsoluteUri,
                                crawledPage.PageBag.CrawlerId,
                                crawledPage.PageBag.SessionId);

            //----------------------------------------
            // create and store the crawled link
            var crawledLink = new CrawledLink();
            crawledLink.SessionId = crawledPage.PageBag.SessionId;
            crawledLink.CrawlerId = crawledPage.PageBag.CrawlerId;
            crawledLink.SourceUrl = crawledPage.ParentUri.AbsoluteUri;
            crawledLink.TargetUrl = crawledPage.Uri.AbsoluteUri; // what was crawled
            crawledLink.StatusCode = crawledPage.HttpWebResponse.StatusCode;
            crawledLink.IsRoot = crawledPage.IsRoot;
            crawledLink.CrawlDepth = crawledPage.CrawlDepth;

            //------------

            if (crawledPage.WebException != null)
            {
                // store error information if it occurred
                crawledLink.ErrorOccurred = true;
                crawledLink.Exception = crawledPage.WebException.Message; //TODO store more data of the exception

                _logger.Error(string.Format("A WebException occurred for Target Url: {0}; Source URL: {1}; CrawlerId: {2}; SessionId: {3}",
                                crawledLink.TargetUrl, crawledLink.SourceUrl, crawledLink.CrawlerId, crawledLink.SessionId),
                              crawledPage.WebException);
            }
            _scheduler.RecordCrawledLink(crawledLink);

            //----------------------------------------
            // Check if the page should be processed, if true process it 
            //  - extract the title, keywords, description, cookies, etc from the page
            //    and save processed data.
            if (crawledPage.WebException == null)
            {       
                if (IsPageToBeProcessed(crawledPage.Uri, crawledPage.HttpWebResponse.StatusCode))
                {
                    using (var processor = _provider.GetInstanceOf<ICrawledPageProcessor>())
                    {
                        var result = processor.ProcessPage(crawledPage);
                        _repo.AddProcessedPage(result);
                    }
                }

                externalLinksFound = _scheduler.ProcessParsedLinks(crawledPage);
                if (externalLinksFound)
                {
                    OnExternalLinksFound(CrawlerId, crawledPage.Uri);
                }
            }

            string mssg = null;
            if (crawledPage.WebException != null || crawledPage.HttpWebResponse.StatusCode != HttpStatusCode.OK)
            {
                mssg = string.Format("Crawl of page failed {0}; source: {1}", crawledPage.Uri.AbsoluteUri, crawledPage.ParentUri.AbsoluteUri);
                _logger.Error(mssg);
            }
            else
            {
                mssg = string.Format("Crawl of page succeeded {0}; source: {1}", crawledPage.Uri.AbsoluteUri, crawledPage.ParentUri.AbsoluteUri);
                _logger.Debug(mssg);
            }

            if (string.IsNullOrEmpty(crawledPage.Content.Text))
            {
                mssg = string.Format("Page had no content {0}", crawledPage.Uri.AbsoluteUri);
                _logger.Error(mssg);
            }

            //------------

            OnLinkCrawlCompleted(CrawlerDefinition, 
                                 crawledPage.ParentUri.AbsoluteUri, 
                                 crawledPage.Uri.AbsoluteUri, 
                                 crawledPage.HttpWebResponse.StatusCode,
                                 crawledPage.WebException != null,
                                 externalLinksFound);
        }
        private void crawler_PageLinksCrawlDisallowed(object sender, PageLinksCrawlDisallowedArgs e)
        {
            CrawledPage crawledPage = e.CrawledPage;
            var mssg = string.Format("Did not crawl the links on page {0} due to {1}", crawledPage.Uri.AbsoluteUri, e.DisallowedReason);
            _logger.Debug(mssg);
        }
        private void crawler_PageCrawlDisallowed(object sender, PageCrawlDisallowedArgs e)
        {
            PageToCrawl pageToCrawl = e.PageToCrawl;
            var mssg = string.Format("Did not crawl page {0} due to {1}", pageToCrawl.Uri.AbsoluteUri, e.DisallowedReason);
            _logger.Debug(mssg);
        }

        /// <summary>
        /// Returns true if the page at the url is to be processed.
        /// </summary>
        /// <returns>Bool</returns>
        public bool IsPageToBeProcessed(Uri uri, HttpStatusCode code)
        {
            bool processPage = false;

            processPage = code == System.Net.HttpStatusCode.OK;

            if (processPage)
            {
                processPage = !_repo.IsBlackListed(uri.GetBaseDomain());
                if (processPage)
                {
                    processPage = !_repo.IsPageProcessed(uri.AbsoluteUri);
                }
            }

            return processPage;
        }

        //Delegates for the Abot WebCrawler instance
        /// <summary>
        /// Delegate passed to WebCrawler to determine if any parsed links should be 
        /// scheduled or not.  Always returns false to override specific Abot.WebCrawler.cs
        /// behavior (see remarks) allowing total control of scheduled links external to Abot.
        /// </summary>
        /// <returns>false</returns>
        /// <remarks>This will always return false as it prevents the abot crawler from adding links
        /// and instead we add the links when each crawled page is processed in the 
        /// crawler_ProcessPageCrawlCompleted handler. see line ~795 in Abot.Crawler.WebCrawler.cs
        /// </remarks>
        public bool ShouldScheduleLink(Uri uri, CrawledPage page, CrawlContext context)
        {
            return false;
        }

        /// <summary>
        /// Delegate passed to WebCrawler which calls functionaltiy to look at 
        /// blacklisted urls when deciding whether a page should be crawled or not. 
        /// If the <paramref name="pageToCrawl"/> Domain is blacklisted, then the 
        /// CrawlDecision.Allow is set to false.  This delegate is called after the default
        /// CrawlDecisionMaker.ShouldCrawlPage() method is called.
        /// </summary>
        /// <returns>CrawlDecision</returns>
        /// <remarks>The default CrawlDecisionMaker.ShouldCrawlPage() method is called first, but then
        /// this method will be called.  No reason to override the default CrawlDecisionMaker, see
        /// about line ~721 in WebCrawler.cs</remarks>
        public CrawlDecision ShouldCrawlPage(PageToCrawl pageToCrawl, CrawlContext crawlContext)
        {
            CrawlDecision decision = null;

            var domain = pageToCrawl.Uri.GetBaseDomain();
            if (_repo.IsBlackListed(domain))
            {
                decision = new CrawlDecision
                {
                    Allow = false,
                    Reason = string.Format("The domain {0} is blacklisted", domain)
                };
            }
            else
            {
                decision = new CrawlDecision() { Allow = true };
            }

            return decision;
        }
    }
}
