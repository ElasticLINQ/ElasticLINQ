// Copyright (c) Tier 3 Inc. All rights reserved.
// This source code is made available under the terms of the Microsoft Public License (MS-PL)

using System.Linq;
using Newtonsoft.Json.Linq;
using System;

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

            if (SearchRequest.Fields.Any())
                root.Add("fields", new JArray(SearchRequest.Fields));

            if (SearchRequest.TermCriteria.Any())
            {
                root.Add("filter", new JObject(
                    new JProperty("terms", new JObject(
                        SearchRequest.TermCriteria
                            .Select(c => new JProperty(c.Key, new JArray(c.Value.ToArray())
                            ))))));
            }

            if (SearchRequest.SortOptions.Any())
                root.Add("sort", new JArray(
                    SearchRequest.SortOptions
                        .Select(o => o.Ascending ? (object) o.Name : new JObject(new JProperty(o.Name, "desc")))
                        .ToArray()));

            if (SearchRequest.Skip > 0)
                root.Add("from", SearchRequest.Skip);

            if (SearchRequest.Take.HasValue)
                root.Add("size", SearchRequest.Take.Value);

            root.Add("timeout", Format(Connection.Timeout));

            return root;
        }
    }
}