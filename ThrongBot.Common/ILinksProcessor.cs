using System;
using System.Collections.Generic;
using Abot.Poco;
using ThrongBot.Common.Entities;

namespace ThrongBot.Common
{
    /// <summary>
    /// Defines behavior to process the parsed links from a crawled page and prepare them
    /// for the scheduler.
    /// </summary>
    public interface ILinksProcessor : ILogic, IDisposable
    {
        /// <summary>
        /// Processes the parsed links in the <paramref name="page"/>.  Following completion
        /// of this method, the property <see cref="LinksToCrawl"/> will have the list of
        /// all links that should be scheduled for crawling and the property 
        /// <see cref="LinksToByPass"/> will have a list of links that should be bypassed, 
        /// bypassed means add the links to the crawled list but with a 'bypassed' flag.
        /// </summary>
        /// <returns>List of LinkToCrawlModel</returns>
        void ProcessLinks(CrawledPage page);
        /// <summary>
        /// Gets the list of links that are POTENTIALLY to be scheduled for crawling, this property
        /// should be called after calling the ProcessLinks() method.  Default result
        /// is an empty list.
        /// </summary>
        List<LinkToCrawl> LinksToCrawl { get; }
        /// <summary>
        /// Gets the list of links that are to be to be bypassed but should still be maintained
        /// and therefor each link has its Bypassed flag set to true.  This property
        /// should be called after calling the ProcessLinks() method.  Default result
        /// is an empty list.
        /// </summary>
        List<CrawledLink> LinksToByPass { get; }
        /// <summary>
        /// Returns true if any parsed links in the crawled page were external links, this
        /// is updated each time ProcessLinks() is called.
        /// </summary>
        bool ExternalLinksFound { get; }
    }
}
