// Licensed under the Apache 2.0 License. See LICENSE.txt in the project root for more information.

using ElasticLinq.Request.Criteria;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ElasticLinq.Request.Formatter
{
    /// <summary>
    /// Formats an ElasticSearchRequest into a JSON POST body to be sent
    /// to ElasticSearch for querying.
    /// </summary>
    internal class PostBodyRequestFormatter : RequestFormatter
    {
        readonly Lazy<string> body;

        public PostBodyRequestFormatter(ElasticConnection connection, ElasticSearchRequest searchRequest)
            : base(connection, searchRequest)
        {
            body = new Lazy<string>(() => CreateJsonPayload().ToString(Formatting.None));
        }

        protected override void CompleteSearchUri(UriBuilder builder)
        {
        }

        public string Body
        {
            get { return body.Value; }
        }

        private JObject CreateJsonPayload()
        {
            var root = new JObject();

            if (SearchRequest.Fields.Any())
                root.Add("fields", new JArray(SearchRequest.Fields));

            if (SearchRequest.Query != null)
                root.Add("query", BuildCriteria(SearchRequest.Query));

            if (SearchRequest.Filter != null)
                root.Add("filter", BuildCriteria(SearchRequest.Filter));

            if (SearchRequest.SortOptions.Any())
                root.Add("sort", Build(SearchRequest.SortOptions));

            if (SearchRequest.From > 0)
                root.Add("from", SearchRequest.From);

            if (SearchRequest.Size.HasValue)
                root.Add("size", SearchRequest.Size.Value);

            if (Connection.Timeout != TimeSpan.Zero)
                root.Add("timeout", Format(Connection.Timeout));

            return root;
        }

        private static JArray Build(IEnumerable<SortOption> sortOptions)
        {
            return new JArray(sortOptions.Select(Build).ToArray());
        }

        private static object Build(SortOption sortOption)
        {
            if (!sortOption.IgnoreUnmapped)
                return sortOption.Ascending
                    ? (object)sortOption.Name
                    : new JObject(new JProperty(sortOption.Name, "desc"));

            var properties = new List<JProperty> { new JProperty("ignore_unmapped", true) };
            if (!sortOption.Ascending)
                properties.Add(new JProperty("order", "desc"));

            return new JObject(new JProperty(sortOption.Name, new JObject(properties)));
        }

        private static JObject BuildCriteria(ICriteria criteria)
        {
            if (criteria is RangeCriteria)
                return Build((RangeCriteria)criteria);

            if (criteria is RegexpCriteria)
                return Build((RegexpCriteria)criteria);

            if (criteria is PrefixCriteria)
                return Build((PrefixCriteria)criteria);

            if (criteria is TermCriteria)
                return Build((TermCriteria)criteria);

            if (criteria is NotCriteria)
                return Build((NotCriteria)criteria);

            if (criteria is QueryStringCriteria)
                return Build((QueryStringCriteria)criteria);

            // Base class formatters using name property

            if (criteria is SingleFieldCriteria)
                return Build((SingleFieldCriteria)criteria);

            if (criteria is CompoundCriteria)
                return Build((CompoundCriteria)criteria);

            throw new InvalidOperationException(String.Format("Unknown criteria type {0}", criteria.GetType()));
        }

        private static JObject Build(QueryStringCriteria criteria)
        {
            var unformattedValue = criteria.Value; // We do not reformat query_string
            return new JObject(new JProperty(criteria.Name, new JObject(new JProperty("query", unformattedValue))));
        }

        private static JObject Build(RangeCriteria criteria)
        {
            // Range filters can be combined by field
            return new JObject(
                new JProperty(criteria.Name,
                    new JObject(new JProperty(criteria.Field,
                        new JObject(criteria.Specifications.Select(s =>
                            new JProperty(s.Name, FormatTerm(s.Value))).ToList())))));
        }

        private static JObject Build(RegexpCriteria criteria)
        {
            return new JObject(new JProperty(criteria.Name, new JObject(new JProperty(criteria.Field, criteria.Regexp))));
        }

        private static JObject Build(PrefixCriteria criteria)
        {
            return new JObject(new JProperty(criteria.Name, new JObject(new JProperty(criteria.Field, criteria.Prefix))));
        }

        private static JObject Build(TermCriteria criteria)
        {
            // Terms filter with one item is a single term filter
            var formattedValue = criteria.Values.Count == 1
                ? FormatTerm(criteria.Values[0])
                : new JArray(criteria.Values.Select(FormatTerm).ToArray());

            return new JObject(new JProperty(criteria.Name, new JObject(new JProperty(criteria.Field, formattedValue))));
        }

        private static JObject Build(SingleFieldCriteria criteria)
        {
            return new JObject(new JProperty(criteria.Name, new JObject(new JProperty("field", criteria.Field))));
        }

        private static JObject Build(NotCriteria criteria)
        {
            return new JObject(new JProperty(criteria.Name, BuildCriteria(criteria.Criteria)));
        }

        private static JObject Build(CompoundCriteria criteria)
        {
            // A compound filter with one item can be collapsed
            return criteria.Criteria.Count == 1
                ? BuildCriteria(criteria.Criteria.First())
                : new JObject(new JProperty(criteria.Name, new JArray(criteria.Criteria.Select(BuildCriteria).ToList())));
        }
    }
}