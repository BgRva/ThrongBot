using ThrongBot.Watcher.Args;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ThrongBot.Watcher
{
    public interface ICrawlView
    {
        event EventHandler<CrawlViewExteranlLinksFoundEventArgs> ExternalLinksFound;
        event EventHandler<CrawlViewEventArgs> ShowSomething;
        string[] GetDetails();
    }
}
