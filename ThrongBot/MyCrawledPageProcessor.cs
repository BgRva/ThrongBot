using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using ThrongBot.Common;
using ThrongBot.Common.Entities;

namespace ThrongBot
{
    public class MyCrawledPageProcessor : ICrawledPageProcessor
    {
        #region IDisposable methods

        public void Dispose()
        {
        }        

        #endregion

        /// <summary>
        /// Processes a crawled page and returns a ProcessedPage object
        /// which can be stored.
        /// </summary>
        /// <param name="page">The results of a crawled url</param>
        /// <returns>ProcessedPage or null</returns>
        public ProcessedPage ProcessPage(Abot.Poco.CrawledPage page)
        {
            //TODO extract data
            var processed = new ProcessedPage();
            processed.SessionId = page.PageBag.SessionId;
            processed.CrawlerId = page.PageBag.CrawlerId;
            processed.PageUrl = page.Uri.AbsoluteUri;
            processed.StatusCode = page.HttpWebResponse.StatusCode;

            //TODO store cookies
            var cookies = page.HttpWebResponse.Cookies;

            return processed;
        }

    }
}
