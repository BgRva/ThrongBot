using System;

namespace ThrongBot.Common.Entities
{
    public class CrawlerRun
    {
        public CrawlerRun()
        {
            StartTime = new DateTime(1753, 1, 1);
        }
        public virtual Guid Id { get; set; }
        public virtual int SessionId { get; set; }
        public virtual int CrawlerId { get; set; }
        public virtual string SeedUrl { get; set; }
        public virtual string BaseDomain { get; set; }
        public virtual DateTime StartTime { get; set; }
        public virtual DateTime? EndTime { get; set; }
        public virtual int Depth { get; set; }
        public virtual int CrawledCount { get; set; }
        public virtual bool InProgress { get; set; }
        public virtual bool ErrorOccurred { get; set; }
    }
}
