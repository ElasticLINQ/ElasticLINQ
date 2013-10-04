// Copyright (c) Tier 3 Inc. All rights reserved.
// This source code is made available under the terms of the Microsoft Public License (MS-PL)

using Newtonsoft.Json.Linq;
using System;
using System.Linq;

namespace ElasticLinq.Request.Formatter
{
    internal class PostBodyRequestFormatter : RequestFormatter
    {
        public PostBodyRequestFormatter(ElasticConnection connection, ElasticSearchRequest searchRequest)
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

            AddFilters(SearchRequest.Filter, root);

            if (SearchRequest.SortOptions.Any())
                root.Add("sort", new JArray(
                    SearchRequest.SortOptions
                        .Select(o => o.Ascending ? (object) o.Name : new JObject(new JProperty(o.Name, "desc")))
                        .ToArray()));

            if (SearchRequest.From > 0)
                root.Add("from", SearchRequest.From);

            if (SearchRequest.Size.HasValue)
                root.Add("size", SearchRequest.Size.Value);

            root.Add("timeout", Format(Connection.Timeout));

            return root;
        }

        private void AddFilters(Filter topFilter, JObject root)
        {
            if (topFilter != null)
                root.Add("filter", BuildFilter(topFilter));
        }

        private JObject BuildFilter(Filter filter)
        {
            if (filter is CompoundFilter)
                return BuildCompoundFilter((CompoundFilter) filter);

            if (filter is TermFilter)
                return BuildTermsFilter((TermFilter)filter);

            throw new InvalidOperationException(String.Format("Unknown filter type {0}", filter.GetType()));
        }

        private JObject BuildTermsFilter(TermFilter filter)
        {
            return new JObject(new JProperty(filter.Name, new JObject(new JProperty(filter.Field, new JArray(filter.Values.ToArray())))));
        }

        private JObject BuildCompoundFilter(CompoundFilter filter)
        {
            return new JObject(new JProperty(filter.Name), filter.Filters.Select(BuildFilter).ToList());
        }
    }
}