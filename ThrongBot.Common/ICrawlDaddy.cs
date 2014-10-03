using ThrongBot.Common.Entities;
using System;
using Abot.Poco;
namespace ThrongBot.Common
{
    /// <summary>
    /// Defines common behavior for implementations that 'wrap' an instance of Abot
    /// </summary>
    public interface ICrawlDaddy : IDisposable
    {
        int SessionId { get; }
        int CrawlerId { get; }
        Uri Seed { get; }
        string BaseDomain { get; }
        bool IsAsync { get; set; }
        bool InitializeCrawler(string seedUrl, int sessionId, int crawlerId);
        void StartCrawl();
        void CancelCrawl();
        event EventHandler<ThrongBot.Common.DomainCrawlEndedEventArgs> DomainCrawlEnded;
        event EventHandler<ThrongBot.Common.DomainCrawlStartedEventArgs> DomainCrawlStarted;
        event EventHandler<ThrongBot.Common.ExternalLinksFoundEventArgs> ExternalLinksFound;
        event EventHandler<ThrongBot.Common.LinkCrawlCompletedArgs> LinkCrawlCompleted;
    }
}
