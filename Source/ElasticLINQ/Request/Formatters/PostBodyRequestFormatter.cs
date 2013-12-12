// Licensed under the Apache 2.0 License. See LICENSE.txt in the project root for more information.

using ElasticLinq.Request.Criteria;
using ElasticLinq.Request.Facets;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ElasticLinq.Request.Formatters
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
            var root = new JObject { { "timeout", Format(Connection.Timeout) } };

            if (SearchRequest.Fields.Any())
                root.Add("fields", new JArray(SearchRequest.Fields));

            if (SearchRequest.Query != null)
                root.Add("query", Build(SearchRequest.Query));

            if (SearchRequest.Filter != null)
                root.Add("filter", Build(SearchRequest.Filter));

            if (SearchRequest.SortOptions.Any())
                root.Add("sort", Build(SearchRequest.SortOptions));

            if (SearchRequest.From > 0)
                root.Add("from", SearchRequest.From);

            if (SearchRequest.Size.HasValue)
                root.Add("size", SearchRequest.Size.Value);

            if (SearchRequest.Facets.Any())
                root.Add("facets", Build(SearchRequest.Facets));

            return root;
        }

        private static JToken Build(IEnumerable<IFacet> facets)
        {
            return new JObject(facets.Select(Build));
        }

        private static JProperty Build(IFacet facet)
        {
            JToken facetBody = null;

            if (facet is StatisticalFacet)
                facetBody = Build((StatisticalFacet)facet);

            if (facetBody != null)
                return new JProperty(facet.Name, new JObject(new JProperty(facet.Type, facetBody)));
                
            throw new InvalidOperationException("Unknown class of IFacet");
        }

        private static JToken Build(StatisticalFacet statisticalFacet)
        {
            return statisticalFacet.Fields.Count() == 1
                ? new JObject(new JProperty("field", statisticalFacet.Fields.First()))
                : new JObject(new JProperty("fields", new JArray(statisticalFacet.Fields)));
        }

        private static JArray Build(IEnumerable<SortOption> sortOptions)
        {
            return new JArray(sortOptions.Select(Build));
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

        private static JObject Build(ICriteria criteria)
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
            return new JObject(new JProperty(criteria.Name, Build(criteria.Criteria)));
        }

        private static JObject Build(CompoundCriteria criteria)
        {
            // A compound filter with one item can be collapsed
            return criteria.Criteria.Count == 1
                ? Build(criteria.Criteria.First())
                : new JObject(new JProperty(criteria.Name, new JArray(criteria.Criteria.Select(Build).ToList())));
        }
    }
}