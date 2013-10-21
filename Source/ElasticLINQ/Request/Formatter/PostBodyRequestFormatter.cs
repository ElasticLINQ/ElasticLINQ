// Copyright (c) Tier 3 Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 

using ElasticLinq.Request.Filters;
using Newtonsoft.Json.Linq;
using System;
using System.Linq;

namespace ElasticLinq.Request.Formatter
{
    /// <summary>
    /// Formats an ElasticSearchRequest into a JSON POST body to be sent
    /// to ElasticSearch for querying.
    /// </summary>
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
                        .Select(o => o.Ascending ? (object)o.Name : new JObject(new JProperty(o.Name, "desc")))
                        .ToArray()));

            if (SearchRequest.From > 0)
                root.Add("from", SearchRequest.From);

            if (SearchRequest.Size.HasValue)
                root.Add("size", SearchRequest.Size.Value);

            root.Add("timeout", Format(Connection.Timeout));

            return root;
        }

        private static void AddFilters(IFilter topFilter, JObject root)
        {
            if (topFilter != null)
                root.Add("filter", BuildFilter(topFilter));
        }

        private static JObject BuildFilter(IFilter filter)
        {
            if (filter is RangeFilter)
                return BuildRangeFilter((RangeFilter)filter);

            if (filter is TermFilter)
                return BuildTermsFilter((TermFilter)filter);

            if (filter is ExistsFilter)
                return BuildExistsFilter((ExistsFilter)filter);

            if (filter is NotFilter)
                return BuildNotFilter((NotFilter)filter);

            if (filter is CompoundFilter)
                return BuildCompoundFilter((CompoundFilter)filter);

            throw new InvalidOperationException(String.Format("Unknown filter type {0}", filter.GetType()));
        }

        private static JObject BuildExistsFilter(ExistsFilter filter)
        {
            return new JObject(new JProperty(filter.Name, new JObject(new JProperty("field", filter.Field))));
        }

        private static JObject BuildNotFilter(NotFilter filter)
        {
            return new JObject(new JProperty(filter.Name, BuildFilter(filter.ChildFilter)));
        }

        private static JObject BuildRangeFilter(RangeFilter filter)
        {
            // Range filters can be combined by field
            return new JObject(new JProperty(filter.Name, new JObject(new JProperty(filter.Field,
                   new JObject(filter.Specifications.Select(s => new JProperty(s.Name, s.Value)).ToList())))));
        }

        private static JObject BuildTermsFilter(TermFilter filter)
        {
            // Terms filter with one item is a single term filter
            var value = filter.Values.Count == 1 ? filter.Values[0] : new JArray(filter.Values.ToArray());
            return new JObject(new JProperty(filter.Name, new JObject(new JProperty(filter.Field, value))));
        }

        private static JObject BuildCompoundFilter(CompoundFilter filter)
        {
            return filter.Filters.Count == 1    // A compound filter with one item can be collapsed
                ? BuildFilter(filter.Filters.First())
                : new JObject(new JProperty(filter.Name, new JArray(filter.Filters.Select(BuildFilter).ToList())));
        }
    }
}