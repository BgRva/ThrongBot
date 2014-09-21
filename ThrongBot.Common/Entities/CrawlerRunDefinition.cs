using System;

namespace ThrongBot.Common.Entities
{
    public class CrawlerRunDefinition
    {
        public virtual int CrawlerId { get; set; }
        public virtual string SeedUrl { get; set; }
    }
}
