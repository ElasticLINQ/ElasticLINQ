// Licensed under the Apache 2.0 License. See LICENSE.txt in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;

namespace ElasticLinq.Utility
{
    internal static class UriBuilderExtensions
    {
        public static Dictionary<string, string> GetQueryParameters(this UriBuilder uri)
        {                 
            return (uri.Query + " ").Substring(1)
                .Trim()
                .Split(new [] { '&' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(p => p.Split('='))
                .ToDictionary(k => k[0], v => v.Length > 1 ? v[1] : "");
        }      
            
        public static void SetQueryParameters(this UriBuilder uri, Dictionary<string, string> parameters)
        {
            Argument.EnsureNotNull("parameters", parameters);
            uri.Query = String.Join("&", parameters
                .Select(p => p.Key + (String.IsNullOrEmpty(p.Value) ? "" : "=" + p.Value)));
        }
    }
}