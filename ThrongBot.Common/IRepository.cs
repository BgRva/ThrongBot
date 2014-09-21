using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Abot.Poco;
using ThrongBot.Common.Entities;

namespace ThrongBot.Common
{
    public interface IRepository : IDisposable
    {
        UserAgentString GetUserAgent(Guid id);
        IEnumerable<UserAgentString> GetUserAgents();

        void AddSession(SessionConfiguration session);
        SessionConfiguration GetSession(int sessionId);
        void DeleteSession(int sessionId);

        void AddCrawl(CrawlerRun crawl);
        void UpdateCrawl(CrawlerRun crawl);
        CrawlerRun GetCrawl(Guid id);
        CrawlerRun GetCrawl(int sessionId, int crawlerId);
        int GetNextCrawlerId(int sessionId);
        CrawlerRun GetCrawl(int sessionId, string baseDomain);
        void DeleteCrawl(Guid id);
        int GetCountOfCrawlsInProgress(int sessionId);       

        /// <summary>
        /// Returns the number of urls crawled for the specified sessionId and crawlerId
        /// </summary>
        int GetCountOfCrawledLinks(int sessionId, int crawlerId);

        /// <summary>
        /// Returns true if a page with the specified <paramref name="url"/>
        /// has already been processed
        /// </summary>
        /// <param name="url">The absolute url to check for</param>
        /// <returns>bool</returns>
        bool IsPageProcessed(string url);
        void AddProcessedPage(ProcessedPage result);
        void UpdateProcessedPage(ProcessedPage result);
        ProcessedPage GetProcessedPage(Guid id);
        void DeleteProcessedPage(Guid id);

        /// <summary>
        /// Returns true if <paramref name="targetUrl"/> is in the set of crawled
        /// links for the specified sessionId
        /// </summary>
        bool IsCrawled(int sessionId, string targetUrl);
        /// <summary>
        /// Adds <paramref name="link"/> as a crawled link.  If <paramref name="removeCorrespondingLinkToCrawl"/>
        /// then if there is a link to crawl with a matching source and target url, it is removed from the
        /// table of links to be crawled.
        /// </summary>
        /// <param name="link">The crawled link</param>
        /// <param name="removeCorrespondingLinkToCrawl">True to remove the corresponding link to be crawled.</param>
        void AddCrawledLink(CrawledLink link, bool removeCorrespondingLinkToCrawl);
        void DeleteCrawledLink(Guid id);
        CrawledLink GetCrawledLink(int sessionId, int crawlerId, string srcUrl, string targetUrl);
        CrawledLink GetCrawledLink(Guid id);
        void ClearCrawledLinks(int sessionId, int crawlerId);

        /// <summary>
        /// Returns true if <paramref name="targetUrl"/> is in the set of 
        /// links to becrawled or the specified sessionId
        /// </summary>
        bool IsToBeCrawled(int sessionId, string targetUrl);
        /// <summary>
        /// Adds the <paramref name="link"/> as a link to be crawled as a unique link with
        /// respect to sessionId, crawlerId, sourceUrl, and targetUrl.  If a link with
        /// the same sessionId, crawlerId, sourceUrl, and targetUrl already exists 
        /// then the link is not added.
        /// </summary>
        /// <param name="link">The LinkToCrawlModel model to be added for future crawling</param>
        /// <returns>The id of the newly added link object</returns>
        void AddLinkToCrawl(LinkToCrawl link);
        void DeleteLinkToCrawl(Guid id);
        void DeleteLinkToCrawl(int sessionId, string srcUrl, string targetUrl);
        LinkToCrawl GetLinkToCrawl(Guid id);
        LinkToCrawl GetLinkToCrawl(int sessionId, string srcUrl, string targetUrl);
        int GetCountOfLinksToCrawl(int sessionId, string baseDomain);
        /// <summary>
        /// Removes all entries with the specified <paramref name="sessionId"/> and 
        /// <paramref name="crawlerId"/>.
        /// </summary>
        void ClearLinksToCrawl(int sessionId, string baseDomain);

        /// <summary>
        /// Gets the list of all links that have not been crawled for the specified 
        /// session and base domain
        /// </summary>
        /// <param name="sessionId">The SessionId of the crawler</param>
        /// <param name="baseDomain">The base domain currently being crawled by the crawler</param>
        /// <returns>List of strings or empty list as default</returns>
        IEnumerable<LinkToCrawl> GetNextLinksToCrawl(int sessionId, string baseDomain);
        /// <summary>
        /// Gets the first entry of the next link to crawl for the specified session and base domain.  If
        /// it is marked as in progress, then this is marked in the data store as well so it will not be 
        /// retrieved in any future calls of next link(s) to crawl
        /// </summary>
        /// <param name="sessionId">The SessionId of the crawler</param>
        /// <param name="baseDomain">The base domain currently being crawled by the crawler</param>
        /// <param name="markAsInProgress">If true, then the next link to crawl is marked as in progress and this is persisted</param>
        /// <returns>LinkToCrawlModel or null</returns>
        LinkToCrawl GetNextLinkToCrawl(int sessionId, string baseDomain, bool markAsInProgress);


        /// <summary>
        /// Adds the url as a lower case domain
        /// </summary>
        /// <param name="url"></param>
        void AddBlacklisted(string url);
        /// <summary>
        /// Compares as lower case
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        bool IsBlackListed(string url);
        IEnumerable<string> GetBlackList();


        // TBD
        void AddKeyWords(List<string> keyWords);
        IEnumerable<string> GetKeyWords();

        void AddCookies(IEnumerable<Cookie> cookies, int sessionId, int crawlerId, string sourceUrl, string targetUrl);
        void DeleteCookies(int sessionId, int crawlerId, string sourceUrl, string targetUrl);
        IEnumerable<Cookie> GetCookies(int sessionId, int crawlerId, string sourceUrl, string targetUrl);
        void ClearCookies(int sessionId, int crawlerId);
    }
}
