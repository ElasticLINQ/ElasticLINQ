// Copyright (c) Tier 3 Inc. All rights reserved.
// This source code is made available under the terms of the Microsoft Public License (MS-PL)

using System;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace ElasticLinq.Request.Formatter
{
    internal class PostBodySearchRequestFormatter : SearchRequestFormatter
    {
        public PostBodySearchRequestFormatter(ElasticConnection connection, ElasticSearchRequest searchRequest)
            : base(connection, searchRequest)
        {
        }

        protected override void CompleteSearchUri(UriBuilder builder)
        {
        }

        public string Body
        {
            get { return CreateJsonPayload().ToString(); }
        }

        private JObject CreateJsonPayload()
        {
            var root = new JObject();

            if (SearchRequest.Skip > 0)
                root.Add("from", SearchRequest.Skip);

            if (SearchRequest.Take.HasValue)
                root.Add("size", SearchRequest.Take.Value);

            return root;
        }
    }
}