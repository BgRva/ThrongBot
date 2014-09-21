using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ThrongBot.Common
{
    //http://weblog.west-wind.com/posts/2012/Apr/24/Getting-a-base-Domain-from-a-Domain
    public static class UriExtensions
    {
        /// <summary>
        /// Returns the base domain from a domain name (lower case)
        /// Example: http://www.west-wind.com returns west-wind.com
        /// </summary>
        /// <param name="uri"></param>
        /// <returns></returns>
        public static string GetBaseDomain(this Uri uri)
        {
            if (uri.HostNameType == UriHostNameType.Dns)
                return NetworkUtils.GetBaseDomain(uri.DnsSafeHost);

            return uri.Host;
        }
    }

    public static class NetworkUtils
    {
        /// <summary>
        /// Returns a base domain name from a full domain name (all lower case).
        /// For example: www.west-wind.com produces west-wind.com
        /// </summary>
        /// <param name="domainName">Dns Domain name as a string, with a format like www.x.com</param>
        /// <returns>base domain (all lower case), i.e: x.com for www.x.com</returns>
        public static string GetBaseDomain(string domainName)
        {
            var tokens = domainName.Split('.');

            // only split 3 segments like www.west-wind.com
            if (tokens == null || tokens.Length != 3)
                return domainName;

            var tok = new List<string>(tokens);
            var remove = tokens.Length - 2;
            tok.RemoveRange(0, remove);

            return (tok[0] + "." + tok[1]).ToLower();
        }
    }
}
