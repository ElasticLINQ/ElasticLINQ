// Licensed under the Apache 2.0 License. See LICENSE.txt in the project root for more information.

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace ElasticLinq.Request.Formatter
{
    /// <summary>
    /// Formats various parts of a <see cref="ElasticSearchRequest"/>.
    /// </summary>
    internal abstract class RequestFormatter
    {
        protected readonly ElasticConnection Connection;
        protected readonly ElasticSearchRequest SearchRequest;

        protected RequestFormatter(ElasticConnection connection, ElasticSearchRequest searchRequest)
        {
            Connection = connection;
            SearchRequest = searchRequest;
        }

        protected abstract void CompleteSearchUri(UriBuilder uriBuilder);

        public Uri Uri
        {
            get { return BuildSearchUri(); }
        }

        private Uri BuildSearchUri()
        {
            var builder = new UriBuilder(Connection.Endpoint);

            builder.Path += String.Join("/",
                new[] { Connection.Index, SearchRequest.Type, "_search" }
               .Where(s => !String.IsNullOrEmpty(s)));

            if (!String.IsNullOrEmpty(SearchRequest.SearchType))
            {
                var parameters = QueryStringToDictionary(builder);
                parameters["search_type"] = SearchRequest.SearchType;
                builder.Query = DictionaryToQueryString(parameters);
            }

            CompleteSearchUri(builder);

            return builder.Uri;
        }

        protected static Dictionary<string, string> QueryStringToDictionary(UriBuilder builder)
        {
            return (builder.Query + " ").Substring(1)
                .Trim()
                .Split('&')
                .Select(p => p.Split('='))
                .ToDictionary(k => k[0], v => v.Length > 1 ? v[1] : "");
        }

        protected static string DictionaryToQueryString(Dictionary<string, string> dictionary)
        {
            return string.Join("&", dictionary.Select(q => q.Key + "=" + q.Value));
        }

        internal static string Format(TimeSpan timeSpan)
        {
            if (timeSpan.Milliseconds != 0)
                return timeSpan.TotalMilliseconds.ToString(CultureInfo.InvariantCulture);

            if (timeSpan.Seconds != 0)
                return timeSpan.TotalSeconds.ToString(CultureInfo.InvariantCulture) + "s";

            return timeSpan.TotalMinutes.ToString(CultureInfo.InvariantCulture) + "m";
        }

        internal static object FormatTerm(object value)
        {
            if (value is string)
                return value.ToString().ToLower(CultureInfo.CurrentCulture);

            return value;
        }
    }
}