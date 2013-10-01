// Copyright (c) Tier 3 Inc. All rights reserved.
// This source code is made available under the terms of the Microsoft Public License (MS-PL)

using System;
using System.Globalization;

namespace ElasticLinq.Request.Formatter
{
    internal abstract class SearchRequestFormatter
    {
        internal static SearchRequestFormatter Create(ElasticConnection connection,
            ElasticSearchRequest searchRequest)
        {
            var requiresPostBody = searchRequest.TermCriteria.Count > 1;
            var useGet = connection.PreferGetRequests && !requiresPostBody;

            return useGet
                ? (SearchRequestFormatter) new GetQuerySearchRequestFormatter(connection, searchRequest)
                : new PostBodySearchRequestFormatter(connection, searchRequest);
        }

        protected readonly ElasticConnection Connection;
        protected readonly ElasticSearchRequest SearchRequest;

        protected SearchRequestFormatter(ElasticConnection connection, ElasticSearchRequest searchRequest)
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

        internal static string FormatValue(object value)
        {
            // TODO: Look at date, time, decimal formatting etc.
            return value.ToString();
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