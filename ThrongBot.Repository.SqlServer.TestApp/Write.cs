using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ThrongBot.Repository.SqlServer.TestApp
{
    public static class Write
    {
        public static void Pass(string methodName, string body)
        {
            Console.WriteLine(string.Format("Pass: {0}: {1}", methodName, body));
        }

        public static void Fail(string methodName, string body)
        {
            Console.WriteLine(string.Format("Fail: {0}: {1}", methodName, body));
        }

        public static void Ex(string methodName, string body)
        {
            Console.WriteLine(string.Format("Ex: {0}: {1}", methodName, body));
        }
    }
}
