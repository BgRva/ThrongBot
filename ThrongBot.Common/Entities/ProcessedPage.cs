using System;
using System.Net;

namespace ThrongBot.Common.Entities
{
    public class ProcessedPage
    {
        public virtual Guid Id { get; set; }
        public virtual int SessionId { get; set; }
        public virtual int CrawlerId { get; set; }
        public virtual string PageUrl { get; set; }
        public virtual string Title { get; set; }
        public virtual string Description { get; set; }
        public virtual string KeyWords { get; set; }
        public virtual HttpStatusCode StatusCode { get; set; }
    }
}
