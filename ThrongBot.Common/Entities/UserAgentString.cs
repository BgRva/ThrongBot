using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ThrongBot.Common.Entities
{
    public class UserAgentString
    {
        public virtual Guid Id { get; set; }
        public virtual string UserAgent { get; set; }
    }
}
