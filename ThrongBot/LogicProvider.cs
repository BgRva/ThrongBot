using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ThrongBot.Common;

namespace ThrongBot
{
    /// <summary>
    /// A factory class that can be injected into instances of CrawlDaddy which
    /// calls it as needed.
    /// </summary>
    public class LogicProvider : ILogicProvider
    {
        public T GetInstanceOf<T>() where T : ILogic
        {
            if (typeof(T) == typeof(ICrawledPageProcessor))
            {
                return (T)(new MyCrawledPageProcessor() as ILogic);
            }
            if (typeof(T) == typeof(ILinksProcessor))
            {
                return (T)(new ParsedLinksProcessor(new LogicProvider()) as ILogic);
            }
            if (typeof(T) == typeof(IModelFactory))
            {
                return (T)(new ModelFactory() as ILogic);
            }
            return default(T);
        }

        public void Dispose()
        {
        }
    }
}
