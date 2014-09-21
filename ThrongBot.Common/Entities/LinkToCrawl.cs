
using System;
namespace ThrongBot.Common.Entities
{
    public class LinkToCrawl
    {
        private string _targetBaseDomain = null;

        public virtual Guid Id { get; set; }
        public virtual int SessionId { get; set; }
        public virtual bool InProgress { get; set; }
        /// <summary>
        /// Gets or sets the root domain of the <see cref="TargetUrl"/> where the
        /// root domain is formatted as x.com (always lower case)
        /// </summary>
        public virtual string TargetBaseDomain
        {
            get
            {
                return _targetBaseDomain;
            }
            set
            {
                if (value != null)
                    _targetBaseDomain = value.ToLower();
                else
                    _targetBaseDomain = null;
            }
        }
        public virtual string SourceUrl { get; set; }
        public virtual string TargetUrl { get; set; }
        public virtual int CrawlDepth { get; set; }
        // required for conversion to/from Abot poco
        public virtual bool IsRoot { get; set; }
        // required for conversion to/from Abot poco
        public virtual bool IsInternal { get; set; }
    }
}
