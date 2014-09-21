using ThrongBot.Common;
using ThrongBot.Common.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ThrongBot.TestSupport
{
    public static class TestData
    {
        public static CrawlerRun GetCrawlerRun(string seed, string baseDomain)
        {
            var run = new CrawlerRun();
            run.SessionId = 7;
            run.CrawlerId = 34;
            run.BaseDomain = baseDomain;
            run.CrawledCount = 33;
            run.Depth = 3;
            run.StartTime = new DateTime(2013, 3, 3);
            run.EndTime = run.StartTime.Add(new TimeSpan(1, 1, 1));
            run.ErrorOccurred = false;
            run.InProgress = true;
            run.SeedUrl = seed;

            return run;
        }

        public static ProcessedPage GetProcessedPage(string url)
        {
            var page = new ProcessedPage();
            page.SessionId = 77;
            page.CrawlerId = 34;
            page.PageUrl = url;
            page.Title = "Blah 1";
            page.Description = "Blah blah and blah";
            page.KeyWords = "blue, red";
            page.StatusCode = System.Net.HttpStatusCode.Ambiguous;
            return page;
        }

        public static CrawledLink GetCrawledLink(string srcUrl, string targetUrl)
        {
            var link = new CrawledLink();
            link.SessionId = 54;
            link.CrawlerId = 64;
            link.SourceUrl = srcUrl;
            link.TargetUrl = targetUrl;
            link.StatusCode = System.Net.HttpStatusCode.Conflict;
            link.ErrorOccurred = true;
            link.Exception = new Exception("BLAH").ToString();
            link.IsRoot = true;
            link.Bypassed = true;
            link.CrawlDepth = 3;

            return link;
        }

        public static LinkToCrawl GetLinkToCrawl(string srcUrl, string targetUrl)
        {
            var link = new LinkToCrawl();
            link.SessionId = 54;
            link.InProgress = true;
            link.SourceUrl = srcUrl;
            link.TargetUrl = targetUrl;
            link.TargetBaseDomain = "LL.Com";
            link.CrawlDepth = 3;
            link.IsRoot = true;
            link.IsInternal = true;

            return link;
        }
    }
}
