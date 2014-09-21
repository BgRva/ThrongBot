using ThrongBot.Common.Entities;
using System;
namespace ThrongBot.Common
{
    public interface ICrawlDaddy : IDisposable
    {
        int SessionId { get; }
        int CrawlerId { get; }
        Uri Seed { get; }
        string BaseDomain { get; }
        bool InitializeCrawler(string seedUrl, int sessionId, int crawlerId);
        void StartCrawl();
        void CancelCrawl();
        event EventHandler<ThrongBot.Common.DomainCrawlEndedEventArgs> DomainCrawlEnded;
        event EventHandler<ThrongBot.Common.DomainCrawlStartedEventArgs> DomainCrawlStarted;
        event EventHandler<ThrongBot.Common.ExternalLinksFoundEventArgs> ExternalLinksFound;
        event EventHandler<ThrongBot.Common.LinkCrawlCompletedArgs> LinkCrawlCompleted;
    }
}
