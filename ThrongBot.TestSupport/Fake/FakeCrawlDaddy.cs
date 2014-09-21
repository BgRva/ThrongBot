using ThrongBot.Common;
using ThrongBot.Common.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ThrongBot.TestSupport
{
    public class FakeCrawlDaddy : ICrawlDaddy
    {
        public int SessionId { get; private set; }
        public int CrawlerId { get; private set; }
        public Uri Seed { get; private set; }
        public string BaseDomain { get; private set; }

        public bool InitializeCrawler(string seedUrl, int sessionId, int crawlerId)
        {
            SessionId = sessionId;
            CrawlerId = crawlerId;
            Seed = new Uri(seedUrl);
            BaseDomain = Seed.GetBaseDomain();
            return true;
        }

        public void StartCrawl()
        {
            OnDomainCrawlStarted(new CrawlerRun() {CrawlerId = CrawlerId});

            for (int i = 0; i < CrawlerId; i++)
            {
                Thread.Sleep(1000);

                if (i > 1 && i % 7 == 0)
                {
                    OnExternalLinksFound(CrawlerId, new Uri(string.Format("http://www.X-{0}.com", i)));
                }
                else
                {
                    OnLinkCrawlCompleted(new CrawlerRun() {CrawlerId = CrawlerId} , "X", string.Format("http://www.X-{0}.com", i), HttpStatusCode.Accepted, false, false);
                }
            }
            OnDomainCrawlEnded(new CrawlerRun() {CrawlerId = CrawlerId});
        }

        public void CancelCrawl()
        {
            throw new NotImplementedException();
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
                throw e;
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
                throw e;
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
                throw e;
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
                throw e;
            }
        }
        #endregion

        public void Dispose()
        {
        }
    }
}
