using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ThrongBot.Common;
using ThrongBot.Common.Entities;
using log4net;
using Newtonsoft.Json;

namespace ThrongBot.Repository.File
{
    public class FileBasedRepository : IRepository
    {
        static ILog _logger = LogManager.GetLogger(typeof(FileBasedRepository).FullName);
        private string _path = null;
        private const string _fileBlackListedUrls = @".\Files\BlackListedUrls.txt";
        private const string _fileCrawlerDefinitions = @".\Files\CrawlerDefinitions.txt";
        private const string _fileProcessedPages = @".\Files\ProcessedPages.txt";
        private const string _fileLinksToCrawl = @".\Files\LinksToCrawl.txt";
        private const string _fileCrawledLinks = @".\Files\CrawledLinks.txt";
        private int _ctr = 0;       
        private static FileBasedRepository _instance = new FileBasedRepository();

        private FileBasedRepository()
        {
            CrawlerDefinitions = new Dictionary<Guid, CrawlerRun>();
            ProcessedPages = new Dictionary<Guid, ProcessedPage>();
            LinksToCrawl = new Dictionary<Guid, LinkToCrawl>();
            CrawledLinks = new Dictionary<Guid, CrawledLink>();
            BlackListedUrls = new List<string>();
            UserAgents = new Dictionary<Guid,UserAgentString>();
            Guid id = new Guid("{AA8A2113-2941-402B-9FBE-7BCDB7FA3AC2}");
            UserAgents.Add(id, new UserAgentString() { Id = id, UserAgent = "Mozilla/5.0 (Windows NT 6.1; WOW64; rv:31.0) Gecko/20100101 Firefox/31.0" });
        }
        public static FileBasedRepository GetInstance()
        {
            return _instance;
        }

        public Dictionary<int, SessionConfiguration> Sessions { get; set; }
        public Dictionary<Guid, CrawlerRun> CrawlerDefinitions { get; set; }
        public Dictionary<Guid, ProcessedPage> ProcessedPages { get; set; }
        public Dictionary<Guid, LinkToCrawl> LinksToCrawl { get; set; }
        public Dictionary<Guid, CrawledLink> CrawledLinks { get; set; }
        public Dictionary<Guid, UserAgentString> UserAgents { get; set; }
        public List<string> BlackListedUrls { get; set; }

        public void AddSession(SessionConfiguration session)
        {
            Sessions.Add(session.SessionId, session);
        }
        public void UpdateSession(SessionConfiguration session)
        {
            if (Sessions.ContainsKey(session.SessionId))
            {
                Sessions[session.SessionId] = session;
            }
        }
        public SessionConfiguration GetSession(int sessionId)
        {
            if (Sessions.ContainsKey(sessionId))
            {
                return Sessions[sessionId];
            }
            return null;
        }
        public void DeleteSession(int sessionId)
        {
            if (Sessions.ContainsKey(sessionId))
            {
                Sessions.Remove(sessionId);
            }
        }

        public UserAgentString GetUserAgent(Guid id)
        {
            if (UserAgents.ContainsKey(id))
                return UserAgents[id];
            return null;
        }
        public IEnumerable<UserAgentString> GetUserAgents()
        {
            return UserAgents.Values.ToList();
        }
        public void AddCrawl(CrawlerRun crawl)
        {
            crawl.Id = NextId;
            CrawlerDefinitions.Add(crawl.Id, crawl);
        }
        public void UpdateCrawl(CrawlerRun crawl)
        {
            if (CrawlerDefinitions.ContainsKey(crawl.Id))
                CrawlerDefinitions[crawl.Id] = crawl;
        }
        public CrawlerRun GetCrawl(Guid id)
        {
            if (CrawlerDefinitions.ContainsKey(id))
                return CrawlerDefinitions[id];
            return null;
        }
        public CrawlerRun GetCrawl(int sessionId, int crawlerId)
        {
            var q = from c in CrawlerDefinitions.Values
                    where c.SessionId == sessionId &&
                          c.CrawlerId == crawlerId
                    select c;
            return q.FirstOrDefault();
        }
        public void DeleteCrawl(Guid id)
        {
            if (CrawlerDefinitions.ContainsKey(id))
                CrawlerDefinitions.Remove(id);
        }
        public CrawlerRun GetCrawl(int sessionId, string baseDomain)
        {
            var baseDomainLower = baseDomain.Trim().ToLower();
            var q = from c in CrawlerDefinitions.Values
                    where c.SessionId == sessionId &&
                          c.BaseDomain == baseDomainLower
                    select c;
            return q.FirstOrDefault();
        }
        public int GetNextCrawlerId(int sessionId)
        {
            var q = from c in CrawlerDefinitions.Values
                    where c.SessionId == sessionId 
                    select c.CrawlerId;
            return q.Max() + 1;
        }

        public int GetCountOfCrawlsInProgress(int sessionId)
        {
            var q = from c in CrawlerDefinitions.Values
                    where c.SessionId == sessionId && c.InProgress
                    select c;
            return q.Count();
        }

        public int GetCountOfCrawledLinks(int sessionId, int crawlerId)
        {
            return CrawledLinks.Count();
        }

        public bool IsPageProcessed(string url)
        {
            var q = from p in ProcessedPages.Values
                    where string.Compare(p.PageUrl, url) == 0
                    select p;
            return q.Any();
        }
        public void AddProcessedPage(ProcessedPage result)
        {
            result.Id = NextId;
            ProcessedPages.Add(result.Id, result);
        }
        public void UpdateProcessedPage(ProcessedPage result)
        {
            if (ProcessedPages.ContainsKey(result.Id))
                ProcessedPages[result.Id] = result;
        }
        public ProcessedPage GetProcessedPage(Guid id)
        {
            if (ProcessedPages.ContainsKey(id))
                return ProcessedPages[id];
            return null;
        }
        public void DeleteProcessedPage(Guid id)
        {
            if (ProcessedPages.ContainsKey(id))
                ProcessedPages.Remove(id);
        }

        public void AddKeyWords(List<string> keyWords)
        {
            throw new NotImplementedException();
        }
        public IEnumerable<string> GetKeyWords()
        {
            throw new NotImplementedException();
        }

        public void AddCrawledLink(CrawledLink link, bool removeCorrespondingLinkToCrawl)
        {
            // delete from LinksToCrawl first
            Thread.Sleep(100);
            if (removeCorrespondingLinkToCrawl)
                DeleteLinkToCrawl(link.SessionId, link.SourceUrl, link.TargetUrl);

            link.Id = NextId;
            CrawledLinks.Add(link.Id, link);
        }
        public bool IsCrawled(int sessionId, string targetUrl)
        {
            var q = from l in CrawledLinks.Values
                    where string.Compare(l.TargetUrl, targetUrl) == 0 &&
                          l.SessionId == sessionId 
                    select l;
            return q.Any();
        }
        public void DeleteCrawledLink(Guid id)
        {
            if (CrawledLinks.ContainsKey(id))
                CrawledLinks.Remove(id);
        }
        public CrawledLink GetCrawledLink(int sessionId, int crawlerId, string srcUrl, string targetUrl)
        {
            var q = from l in CrawledLinks.Values
                    where l.SessionId == sessionId &&
                          l.CrawlerId == crawlerId &&
                          l.SourceUrl == srcUrl &&
                          l.TargetUrl == targetUrl
                    select l;
            return q.FirstOrDefault();
        }
        public CrawledLink GetCrawledLink(Guid id)
        {
            var q = from l in CrawledLinks.Values
                    where l.Id == id
                    select l;
            return q.FirstOrDefault();
        }
        public void ClearCrawledLinks(int sessionId, int crawlerId)
        {
            List<Guid> toRemove = new List<Guid>();
            foreach(var link in CrawledLinks.Values)
            {
                if (link.SessionId == sessionId && link.CrawlerId == crawlerId)
                    toRemove.Add(link.Id);
            }
            foreach(var id in toRemove)
                CrawledLinks.Remove(id);
        }

        public void AddLinkToCrawl(LinkToCrawl link)
        {
            // if the link to add is NOT in the list of links to crawl or crawled links, then add it
            Thread.Sleep(100);
            var q = from l in LinksToCrawl.Values
                    where l.SessionId == link.SessionId &&
                    string.Compare(l.SourceUrl, link.SourceUrl, true) == 0 &&
                    string.Compare(l.TargetUrl, link.TargetUrl, true) == 0
                    select l;

            if (!q.Any())
            {
                var q2 = from l in CrawledLinks.Values
                         where l.SessionId == link.SessionId &&
                         string.Compare(l.SourceUrl, link.SourceUrl, true) == 0 &&
                         string.Compare(l.TargetUrl, link.TargetUrl, true) == 0
                         select l;

                if (!q2.Any())
                {
                    link.Id = NextId;
                    LinksToCrawl.Add(link.Id, link);
                }
            }
        }

        public bool IsToBeCrawled(int sessionId, string targetUrl)
        {
            var q = from l in LinksToCrawl.Values
                    where l.SessionId == sessionId &&
                    string.Compare(l.TargetUrl, targetUrl, true) == 0
                    select l;
            return q.Any();
        }
        public void DeleteLinkToCrawl(int sessionId, string srcUrl, string targetUrl)
        {
            Thread.Sleep(100);
            var q = from l in LinksToCrawl.Values
                    where l.SessionId == sessionId &&
                          l.SourceUrl == srcUrl &&
                          l.TargetUrl == targetUrl
                    select l;
            var link = q.FirstOrDefault();
            if (link != null && LinksToCrawl.ContainsKey(link.Id))
                LinksToCrawl.Remove(link.Id);
        }
        public void DeleteLinkToCrawl(Guid id)
        {
            Thread.Sleep(100);
            var q = from l in LinksToCrawl.Values
                    where l.Id == id 
                    select l;
            var link = q.FirstOrDefault();
            if (link != null && LinksToCrawl.ContainsKey(link.Id))
                LinksToCrawl.Remove(link.Id);
        }
        public LinkToCrawl GetLinkToCrawl(Guid id)
        {
            var q = from l in LinksToCrawl.Values
                    where l.Id == id 
                    select l;
            return q.FirstOrDefault();
        }
        public LinkToCrawl GetLinkToCrawl(int sessionId, string srcUrl, string targetUrl)
        {
            var q = from l in LinksToCrawl.Values
                    where l.SessionId == sessionId &&
                          l.SourceUrl == srcUrl &&
                          l.TargetUrl == targetUrl
                    select l;
            return q.FirstOrDefault();
        }
        public int GetCountOfLinksToCrawl(int sessionId, string baseDomain)
        {
            Thread.Sleep(100);
            var q = from l in LinksToCrawl.Values
                    where l.SessionId == sessionId &&
                          string.Compare(l.TargetBaseDomain, baseDomain, false) == 0 &&
                          l.InProgress == false
                    select l;
            return q.Count();
        }
        public void ClearLinksToCrawl(int sessionId, string baseDomain)
        {
            var q = from l in LinksToCrawl.Values
                    where l.SessionId == sessionId &&
                          string.Compare(l.TargetBaseDomain, baseDomain, false) == 0
                    select l.Id;
            foreach (var id in q.ToList())
                LinksToCrawl.Remove(id);
        }
        public IEnumerable<LinkToCrawl> GetNextLinksToCrawl(int sessionId, string baseDomain)
        {
            var q = from l in LinksToCrawl.Values
                    where l.SessionId == sessionId && 
                          string.Compare(l.TargetBaseDomain, baseDomain, false) == 0 &&
                          l.InProgress == false
                    select l;
            return q.ToList();
        }
        public LinkToCrawl GetNextLinkToCrawl(int sessionId, string baseDomain, bool markAsInProgress)
        {
            var q = from l in LinksToCrawl.Values
                    where l.SessionId == sessionId &&
                          string.Compare(l.TargetBaseDomain, baseDomain, false) == 0 &&
                          l.InProgress == false
                    select l;
            var link = q.FirstOrDefault();
            // Flag as in progress
            if (link != null && markAsInProgress)
                link.InProgress = true;
            return link;
        }

        public void AddBlacklisted(string url)
        {
            if (!BlackListedUrls.Contains(url.ToLower()))
                BlackListedUrls.Add(url.ToLower());
        }
        public bool IsBlackListed(string url)
        {
            return BlackListedUrls.Contains(url.ToLower());
        }
        public IEnumerable<string> GetBlackList()
        {
            return BlackListedUrls;
        }

        public void AddCookies(IEnumerable<System.Net.Cookie> cookies, int sessionId, int crawlerId, string sourceUrl, string targetUrl)
        {
            throw new NotImplementedException();
        }
        public void DeleteCookies(int sessionId, int crawlerId, string sourceUrl, string targetUrl)
        {
            throw new NotImplementedException();
        }
        public IEnumerable<System.Net.Cookie> GetCookies(int sessionId, int crawlerId, string sourceUrl, string targetUrl)
        {
            throw new NotImplementedException();
        }
        public void ClearCookies(int sessionId, int crawlerId)
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {}
        
        //Helpers

        private Guid NextId
        {
            get
            {
                return Guid.NewGuid();
            }
        }

        public void LoadDataFiles()
        {
            // Read blacklisted urls
            string[] lines = System.IO.File.ReadAllLines(_fileBlackListedUrls);
            BlackListedUrls = new List<string>(lines);
        }
        public void WriteDataFiles()
        {
            string json = null;
            if (CrawledLinks != null && CrawledLinks.Count > 0)
            {
                using (var sw = new StreamWriter(_fileCrawledLinks, false))
                {
                    foreach (var link in CrawledLinks.Values)
                    {
                        json = JsonConvert.SerializeObject(link);
                        sw.WriteLine(json);
                    }
                }
            }
            if (LinksToCrawl != null && LinksToCrawl.Count > 0)
            {
                using (var sw = new StreamWriter(_fileLinksToCrawl, false))
                {
                    foreach (var link in LinksToCrawl.Values)
                    {
                        json = JsonConvert.SerializeObject(link);
                        sw.WriteLine(json);
                    }
                }
            }
            if (CrawlerDefinitions != null && CrawlerDefinitions.Count > 0)
            {
                using (var sw = new StreamWriter(_fileCrawlerDefinitions, false))
                {
                    foreach (var crawler in CrawlerDefinitions.Values)
                    {
                        json = JsonConvert.SerializeObject(crawler);
                        sw.WriteLine(json);
                    }
                }
            }
            if (ProcessedPages != null && ProcessedPages.Count > 0)
            {
                using (var sw = new StreamWriter(_fileProcessedPages, false))
                {
                    foreach (var pages in ProcessedPages.Values)
                    {
                        json = JsonConvert.SerializeObject(pages);
                        sw.WriteLine(json);
                    }
                }
            }
        }
    }
}
