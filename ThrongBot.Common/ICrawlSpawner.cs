using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ThrongBot.Common.Entities;

namespace ThrongBot.Common
{
    public interface ICrawlSpawner: IDisposable
    {
        /// <summary>
        /// Returns true if a new crawl can be spawned for the specified sessionId.
        /// </summary>
        bool CanSpawn(int sessionId);
        CrawlerRun SpawnCrawl(int sessionId, Uri externalUri);
    }
}
