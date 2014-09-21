using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ThrongBot.Common
{
    public interface IBootstrapper
    {
        T Resolve<T>();
    }
}
