using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ThrongBot.Watcher.Args
{
    public class CrawlViewExteranlLinksFoundEventArgs : EventArgs
    {
        public CrawlViewExteranlLinksFoundEventArgs()
        { }

        public CrawlViewExteranlLinksFoundEventArgs(int crawlerId, Uri link)
        {
            CrawlerId = crawlerId;
            Link = link;
        }

        public int CrawlerId { get; set; }
        public Uri Link { get; set; }
    }
}
