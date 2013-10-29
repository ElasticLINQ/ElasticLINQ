// Copyright (c) Tier 3 Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 

using System;
using System.Globalization;

namespace ElasticLinq.Request.Formatter
{
    internal abstract class RequestFormatter
    {
        internal static RequestFormatter Create(ElasticConnection connection, ElasticSearchRequest searchRequest)
        {
            return new PostBodyRequestFormatter(connection, searchRequest);
        }

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