using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DomainCrawler.Common.Entities;
using NHibernate.Mapping.ByCode;
using NHibernate.Mapping.ByCode.Conformist;

namespace DomainCrawler.Repository.SqlServer.Mappings
{
    public class CrawlerRunMapping : ClassMapping<CrawlerRun>
    {
        public CrawlerRunMapping()
        {
            Id(x => x.Id);
            Property(x => x.CrawlerId);
            Property(x => x.SessionId);
            Property(x => x.SeedUrl);
            Property(x => x.BaseDomain);
            Property(x => x.StartTime);
            Property(x => x.EndTime);
            Property(x => x.Depth);
            Property(x => x.CrawledCount);
            Property(x => x.InProgress);
            Property(x => x.ErrorOccurred);
        }
    }
}
