// Copyright (c) Tier 3 Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 

using ElasticLinq.Request.Criteria;
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

        private static void AddFilters(ICriteria topFilter, JObject root)
        {
            if (topFilter != null)
                root.Add("filter", BuildFilter(topFilter));
        }

        private static JObject BuildFilter(ICriteria criteria)
        {
            if (criteria is RangeCriteria)
                return Create((RangeCriteria)criteria);

            if (criteria is TermCriteria)
                return Create((TermCriteria)criteria);

            if (criteria is ExistsCriteria)
                return Create((ExistsCriteria)criteria);

            if (criteria is NotCriteria)
                return Create((NotCriteria)criteria);

            if (criteria is CompoundCriteria)
                return Create((CompoundCriteria)criteria);

            throw new InvalidOperationException(String.Format("Unknown filter type {0}", criteria.GetType()));
        }

        private static JObject Create(RangeCriteria criteria)
        {
            // Range filters can be combined by field
            return new JObject(new JProperty(criteria.Name, new JObject(new JProperty(criteria.Field,
                   new JObject(criteria.Specifications.Select(s => new JProperty(s.Name, s.Value)).ToList())))));
        }

        private static JObject Create(TermCriteria criteria)
        {
            // Terms filter with one item is a single term filter
            var value = criteria.Values.Count == 1 ? criteria.Values[0] : new JArray(criteria.Values.ToArray());
            return new JObject(new JProperty(criteria.Name, new JObject(new JProperty(criteria.Field, value))));
        }

        private static JObject Create(ExistsCriteria criteria)
        {
            return new JObject(new JProperty(criteria.Name, new JObject(new JProperty("field", criteria.Field))));
        }

        private static JObject Create(NotCriteria criteria)
        {
            return new JObject(new JProperty(criteria.Name, BuildFilter(criteria.Criteria)));
        }

        private static JObject Create(CompoundCriteria criteria)
        {
            return criteria.Criteria.Count == 1    // A compound filter with one item can be collapsed
                ? BuildFilter(criteria.Criteria.First())
                : new JObject(new JProperty(criteria.Name, new JArray(criteria.Criteria.Select(BuildFilter).ToList())));
        }
    }
}