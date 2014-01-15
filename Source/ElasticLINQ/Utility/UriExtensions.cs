// Licensed under the Apache 2.0 License. See LICENSE.txt in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;

namespace ElasticLinq.Utility
{
    internal static class UriExtensions
    {
        public static Dictionary<string, string> GetQueryParameters(this UriBuilder uri)
        {
            var query = uri.Query.StartsWith("?") ? uri.Query.Substring(1) : uri.Query;
            return query
                .Split('&').Select(p => p.Split('='))
                .ToDictionary(k => k[0], v => v.Length > 1 ? v[1] : "");   
        }

        public static void SetQueryParameters(this UriBuilder uri, Dictionary<string, string> parameters)
        {
            uri.Query = String.Join("&", parameters.Select(p => p.Key + "=" + p.Value));
        }
    }
}