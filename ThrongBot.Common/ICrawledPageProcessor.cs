using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Abot.Poco;
using ThrongBot.Common.Entities;

namespace ThrongBot.Common
{
    /// <summary>
    /// Defines behavior to process and extract data from a crawled page.
    /// </summary>
    public interface ICrawledPageProcessor : ILogic, IDisposable
    {
        /// <summary>
        /// Processes <paramref name="page"/> and extracts relavant data for storage 
        /// and returns a ProcessedPageModel object that can be persisted.
        /// </summary>
        /// <param name="page">The CrawledPage to be processed</param>
        /// <returns>A ProcessedPage to be stored</returns>
        /// <remarks>processing can include saving errors and responses to track
        /// sites that refused crawling or errored out</remarks>
        ProcessedPage ProcessPage( CrawledPage page);
    }
}
