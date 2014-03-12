// Licensed under the Apache 2.0 License. See LICENSE.txt in the project root for more information.

using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using ElasticLinq.Utility;

namespace ElasticLinq.Request.Formatters
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
                var parameters = builder.GetQueryParameters();
                parameters["search_type"] = SearchRequest.SearchType;
                builder.SetQueryParameters(parameters);
            }

            CompleteSearchUri(builder);

            return builder.Uri;
        }

        internal static string Format(TimeSpan timeSpan)
        {
            if (timeSpan.Milliseconds != 0)
                return timeSpan.TotalMilliseconds.ToString(CultureInfo.InvariantCulture);

            if (timeSpan.Seconds != 0)
                return timeSpan.TotalSeconds.ToString(CultureInfo.InvariantCulture) + "s";

            return timeSpan.TotalMinutes.ToString(CultureInfo.InvariantCulture) + "m";
        }

        internal static JToken FormatTerm(object value)
        {
            if (value is string)
                return new JValue(value.ToString().ToLower(CultureInfo.CurrentCulture));

            return JToken.FromObject(value);
        }
    }
}