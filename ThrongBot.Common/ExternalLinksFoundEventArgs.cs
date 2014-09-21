using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ThrongBot.Common
{
    public class ExternalLinksFoundEventArgs: EventArgs
    {
        public ExternalLinksFoundEventArgs()
        {

        }
        public ExternalLinksFoundEventArgs(int crawlerId, Uri pageUri)
        {
            CrawlerId = crawlerId;
            PageUri = pageUri;
        }

        /// <summary>
        /// The id of the crawler that crawled a page with
        /// external links
        /// </summary>
        public int CrawlerId { get; set;}
        /// <summary>
        /// Gets or sets the Uri of the page on which the external
        /// links were found
        /// </summary>
        public Uri PageUri { get; set; }
    }
}
