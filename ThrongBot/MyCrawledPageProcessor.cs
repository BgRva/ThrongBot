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
        public MyCrawledPageProcessor()
        {
        }

        #region IDisposable methods

        public void Dispose()
        {
        }        

        #endregion

        public ProcessedPage ProcessPage(Abot.Poco.CrawledPage page)
        {
            //TODO extract data
            var processed = new ProcessedPage();
            processed.SessionId = page.PageBag.SessionId;
            processed.CrawlerId = page.PageBag.CrawlerId;
            processed.PageUrl = page.Uri.AbsoluteUri;
            processed.StatusCode = page.HttpWebResponse.StatusCode;


            var cookies = page.HttpWebResponse.Cookies;
            //TODO store cookies
            return processed;
        }

    }
}
