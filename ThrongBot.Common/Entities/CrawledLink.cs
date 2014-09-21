using System;
using System.Net;

namespace ThrongBot.Common.Entities
{
    public class CrawledLink
    {
        public virtual Guid Id { get; set; }
        public virtual int SessionId { get; set; }
        public virtual int CrawlerId { get; set; }
        public virtual string SourceUrl { get; set; }
        public virtual string TargetUrl { get; set; }
        public virtual HttpStatusCode StatusCode { get; set; }
        public virtual bool ErrorOccurred { get; set; }
        public virtual string Exception { get; set; }
        public virtual bool IsRoot { get; set; }
        public virtual bool Bypassed { get; set; }
        public virtual int CrawlDepth { get; set; }
    }
}
