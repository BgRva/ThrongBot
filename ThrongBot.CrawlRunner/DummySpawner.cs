using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ThrongBot.Common;

namespace ThrongBot.CrawlRunner
{
    public class DummySpawner : ICrawlSpawner
    {
        public DummySpawner()
        {

        }
        public bool CanSpawn(int sessionId)
        {
            return false;;
        }

        public Common.Entities.CrawlerRun SpawnCrawl(int sessionId, Uri externalUri)
        {
           return null;
        }

        public void Dispose()
        {
        }
    }
}
