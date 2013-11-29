// Licensed under the Apache 2.0 License. See LICENSE.txt in the project root for more information.

using System;
using System.Globalization;

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

        protected abstract void CompleteSearchUri(UriBuilder builder);

        public Uri Uri
        {
            get { return BuildSearchUri(); }
        }

        private Uri BuildSearchUri()
        {
            var builder = new UriBuilder(Connection.Endpoint);

            if (!String.IsNullOrEmpty(Connection.Index))
                builder.Path += Connection.Index + "/";

            if (!String.IsNullOrEmpty(SearchRequest.Type))
                builder.Path += SearchRequest.Type + "/";

            builder.Path += "_search";

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
    }
}