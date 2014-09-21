using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ThrongBot.Watcher.Args
{
    public class CrawlViewEventArgs : EventArgs
    {
        public CrawlViewEventArgs()
        { }

        public CrawlViewEventArgs(string something)
        {
            Something = something;
        }

        public string Something { get; set; }
    }
}
