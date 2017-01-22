// Licensed under the Apache 2.0 License. See LICENSE.txt in the project root for more information.

using ElasticLinq.Mapping;
using ElasticLinq.Request.Criteria;
using ElasticLinq.Request.Facets;
using ElasticLinq.Utility;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;

namespace ElasticLinq.Request.Formatters
{
    /// <summary>
    /// Formats a SearchRequest into a JSON POST to be sent to Elasticsearch.
    /// </summary>
    class SearchRequestFormatter
    {
        static readonly CultureInfo transportCulture = CultureInfo.InvariantCulture;

        readonly Lazy<string> body;
        readonly IElasticConnection connection;
        readonly IElasticMapping mapping;
        readonly SearchRequest searchRequest;

        /// <summary>
        /// Create a new SearchRequestFormatter for the given connection, mapping and search request.
        /// </summary>
        /// <param name="connection">The ElasticConnection to prepare the SearchRequest for.</param>
        /// <param name="mapping">The IElasticMapping used to format the SearchRequest.</param>
        /// <param name="searchRequest">The SearchRequest to be formatted.</param>
        public SearchRequestFormatter(IElasticConnection connection, IElasticMapping mapping, SearchRequest searchRequest)
        {
            this.connection = connection;
            this.mapping = mapping;
            this.searchRequest = searchRequest;

            body = new Lazy<string>(() => CreateBody().ToString(connection.Options.Pretty ? Formatting.Indented : Formatting.None));
        }

        /// <summary>
        /// The JSON formatted POST body for the request to be sent to Elasticsearch.
        /// </summary>
        public string Body => body.Value;

        /// <summary>
        /// Create the Json HTTP request body for this request given the search query and connection.
        /// </summary>
        /// <returns>Json to be used to execute this query by Elasticsearch.</returns>
        JObject CreateBody()
        {
            var root = new JObject();

            if (searchRequest.Fields.Any())
                root.Add("fields", new JArray(searchRequest.Fields));

            if (searchRequest.MinScore.HasValue)
                root.Add("min_score", searchRequest.MinScore.Value);

            var queryRoot = root;

            // Filters cause a filtered query to be created
            if (searchRequest.Filter != null)
            {
                queryRoot = new JObject(new JProperty("filter", Build(searchRequest.Filter)));
                root.Add("query", new JObject(new JProperty("filtered", queryRoot)));
            }

            if (searchRequest.Query != null)
                queryRoot.Add("query", Build(searchRequest.Query));

            if (searchRequest.SortOptions.Any())
                root.Add("sort", Build(searchRequest.SortOptions));

            if (searchRequest.From > 0)
                root.Add("from", searchRequest.From);

            if (searchRequest.Highlight != null)
                root.Add("highlight", Build(searchRequest.Highlight));

            long? size = searchRequest.Size ?? connection.Options.SearchSizeDefault;
            if (size.HasValue && !searchRequest.Facets.Any())
                root.Add("size", size.Value);

            if (searchRequest.Facets.Any())
                root.Add("facets", Build(searchRequest.Facets, size));

            if (connection.Timeout != TimeSpan.Zero)
                root.Add("timeout", Format(connection.Timeout));

            return root;
        }

        JToken Build(IEnumerable<IFacet> facets, long? defaultSize)
        {
            return new JObject(facets.Select(facet => Build(facet, defaultSize)));
        }

        JProperty Build(IFacet facet, long? defaultSize)
        {
            Argument.EnsureNotNull(nameof(facet), facet);

            var specificBody = Build(facet);
            if (facet is IOrderableFacet)
            {
                var facetSize = ((IOrderableFacet)facet).Size ?? defaultSize;
                if (facetSize.HasValue)
                    specificBody["size"] = facetSize.Value.ToString(transportCulture);
            }

            var namedBody = new JObject(new JProperty(facet.Type, specificBody));

            if (facet.Filter != null)
                namedBody["filter"] = Build(facet.Filter);

            return new JProperty(facet.Name, namedBody);
        }

        static JToken Build(IFacet facet)
        {
            if (facet is StatisticalFacet)
                return Build((StatisticalFacet)facet);

            if (facet is TermsStatsFacet)
                return Build((TermsStatsFacet)facet);

            if (facet is TermsFacet)
                return Build((TermsFacet)facet);

            if (facet is FilterFacet)
                return new JObject();

            throw new InvalidOperationException(string.Format("Unknown implementation of IFacet '{0}' can not be formatted", facet.GetType().Name));
        }

        static JToken Build(StatisticalFacet statisticalFacet)
        {
            return new JObject(
                BuildFieldProperty(statisticalFacet.Fields)
            );
        }

        static JToken Build(TermsStatsFacet termStatsFacet)
        {
            return new JObject(
                new JProperty("key_field", termStatsFacet.Key),
                new JProperty("value_field", termStatsFacet.Value)
            );
        }

        static JToken Build(TermsFacet termsFacet)
        {
            return new JObject(BuildFieldProperty(termsFacet.Fields));
        }

        static JToken BuildFieldProperty(ReadOnlyCollection<string> fields)
        {
            return fields.Count == 1
                ? new JProperty("field", fields.First())
                : new JProperty("fields", new JArray(fields));
        }

        static JArray Build(IEnumerable<SortOption> sortOptions)
        {
            return new JArray(sortOptions.Select(Build));
        }

        static object Build(SortOption sortOption)
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

        JObject Build(ICriteria criteria)
        {
            if (criteria == null)
                return null;

            if (criteria is RangeCriteria)
                return Build((RangeCriteria)criteria);

            if (criteria is RegexpCriteria)
                return Build((RegexpCriteria)criteria);

            if (criteria is PrefixCriteria)
                return Build((PrefixCriteria)criteria);

            if (criteria is TermCriteria)
                return Build((TermCriteria)criteria);

            if (criteria is TermsCriteria)
                return Build((TermsCriteria)criteria);

            if (criteria is NotCriteria)
                return Build((NotCriteria)criteria);

            if (criteria is QueryStringCriteria)
                return Build((QueryStringCriteria)criteria);

            if (criteria is MatchAllCriteria)
                return Build((MatchAllCriteria)criteria);

            if (criteria is BoolCriteria)
                return Build((BoolCriteria)criteria);

            // Base class formatters using name property

            if (criteria is SingleFieldCriteria)
                return Build((SingleFieldCriteria)criteria);

            if (criteria is CompoundCriteria)
                return Build((CompoundCriteria)criteria);

            throw new InvalidOperationException(string.Format("Unknown criteria type '{0}'", criteria.GetType()));
        }

        static JObject Build(Highlight highlight)
        {
            var fields = new JObject();

            foreach (var field in highlight.Fields)
                fields.Add(new JProperty(field, new JObject()));

            var queryStringCriteria = new JObject(new JProperty("fields", fields));

            if (!string.IsNullOrWhiteSpace(highlight.PostTag))
                queryStringCriteria.Add(new JProperty("post_tags", new JArray(highlight.PostTag)));
            if (!string.IsNullOrWhiteSpace(highlight.PreTag))
                queryStringCriteria.Add(new JProperty("pre_tags", new JArray(highlight.PreTag)));

            return queryStringCriteria;
        }

        static JObject Build(QueryStringCriteria criteria)
        {
            var unformattedValue = criteria.Value; // We do not reformat query_string

            var queryStringCriteria = new JObject(new JProperty("query", unformattedValue));

            if (criteria.Fields.Any())
                queryStringCriteria.Add(new JProperty("fields", new JArray(criteria.Fields)));

            return new JObject(new JProperty(criteria.Name, queryStringCriteria));
        }

        JObject Build(RangeCriteria criteria)
        {
            // Range filters can be combined by field
            return new JObject(
                new JProperty(criteria.Name,
                    new JObject(new JProperty(criteria.Field,
                        new JObject(criteria.Specifications.Select(s =>
                            new JProperty(s.Name, mapping.FormatValue(criteria.Member, s.Value))).ToList())))));
        }

        static JObject Build(RegexpCriteria criteria)
        {
            return new JObject(new JProperty(criteria.Name, new JObject(new JProperty(criteria.Field, criteria.Regexp))));
        }

        static JObject Build(PrefixCriteria criteria)
        {
            return new JObject(new JProperty(criteria.Name, new JObject(new JProperty(criteria.Field, criteria.Prefix))));
        }

        JObject Build(TermCriteria criteria)
        {
            return new JObject(
                new JProperty(criteria.Name, new JObject(
                    new JProperty(criteria.Field, mapping.FormatValue(criteria.Member, criteria.Value)))));
        }

        JObject Build(TermsCriteria criteria)
        {
            var termsCriteria = new JObject(
                new JProperty(criteria.Field,
                    new JArray(criteria.Values.Select(x => mapping.FormatValue(criteria.Member, x)).Cast<object>().ToArray())));

            if (criteria.ExecutionMode.HasValue)
                termsCriteria.Add(new JProperty("execution", criteria.ExecutionMode.GetValueOrDefault().ToString()));

            return new JObject(new JProperty(criteria.Name, termsCriteria));
        }

        static JObject Build(SingleFieldCriteria criteria)
        {
            return new JObject(new JProperty(criteria.Name, new JObject(new JProperty("field", criteria.Field))));
        }

        JObject Build(NotCriteria criteria)
        {
            return new JObject(new JProperty(criteria.Name, Build(criteria.Criteria)));
        }

        static JObject Build(MatchAllCriteria criteria)
        {
            return new JObject(new JProperty(criteria.Name));
        }

        JObject Build(CompoundCriteria criteria)
        {
            // A compound filter with one item can be collapsed
            return criteria.Criteria.Count == 1
                ? Build(criteria.Criteria.First())
                : new JObject(new JProperty(criteria.Name, new JArray(criteria.Criteria.Select(Build).ToList())));
        }

        JObject Build(BoolCriteria criteria)
        {
            return new JObject(new JProperty(criteria.Name, new JObject(BuildProperties(criteria))));
        }

        IEnumerable<JProperty> BuildProperties(BoolCriteria criteria)
        {
            if (criteria.Must.Any())
                yield return new JProperty("must", new JArray(criteria.Must.Select(Build)));

            if (criteria.MustNot.Any())
                yield return new JProperty("must_not", new JArray(criteria.MustNot.Select(Build)));

            if (criteria.Should.Any())
            {
                yield return new JProperty("should", new JArray(criteria.Should.Select(Build)));
                yield return new JProperty("minimum_should_match", 1);
            }
        }

        internal static string Format(TimeSpan timeSpan)
        {
            if (timeSpan.Milliseconds != 0)
                return timeSpan.TotalMilliseconds.ToString(transportCulture);

            if (timeSpan.Seconds != 0)
                return timeSpan.TotalSeconds.ToString(transportCulture) + "s";

            return timeSpan.TotalMinutes.ToString(transportCulture) + "m";
        }
    }
}
