using ThrongBot.Common.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ThrongBot.Common.Entities
{
    public class SessionConfiguration
    {
        public SessionConfiguration()
        {

        }
        public SessionConfiguration(int id)
        {
            SessionId = id;
        }
        public virtual Guid Id { get; set; }
        public virtual int SessionId { get; set;}
        public virtual int MaxConcurrentCrawls { get; set; }
    }
}
