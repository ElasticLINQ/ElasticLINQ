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
    internal class SearchRequestFormatter
    {
        private readonly string[] parameterSeparator = { "&" };

        private readonly Lazy<string> body;
        private readonly ElasticConnection connection;
        private readonly IElasticMapping mapping;
        private readonly SearchRequest searchRequest;
        private readonly Uri uri;

        /// <summary>
        /// Create a new SearchRequestFormatter for the given connection, mapping and search request.
        /// </summary>
        /// <param name="connection">The ElasticConnection to prepare the SearchRequest for.</param>
        /// <param name="mapping">The IElasticMapping used to format the SearchRequest.</param>
        /// <param name="searchRequest">The SearchRequest to be formatted.</param>
        public SearchRequestFormatter(ElasticConnection connection, IElasticMapping mapping, SearchRequest searchRequest)
        {
            this.connection = connection;
            this.mapping = mapping;
            this.searchRequest = searchRequest;

            body = new Lazy<string>(() => CreateBody().ToString(connection.Options.Pretty ? Formatting.Indented : Formatting.None));
            uri = CreateUri();
        }

        /// <summary>
        /// The JSON formatted POST body for the request to be sent to Elasticsearch.
        /// </summary>
        public string Body
        {
            get { return body.Value; }
        }

        /// <summary>
        /// The Uri that the body should be posted to in order to execute the SearchRequest.
        /// </summary>
        public Uri Uri
        {
            get { return uri; }
        }

        private Uri CreateUri()
        {
            var builder = new UriBuilder(connection.Endpoint);
            builder.Path += (connection.Index ?? "_all") + "/";

            if (!String.IsNullOrEmpty(searchRequest.DocumentType))
                builder.Path += searchRequest.DocumentType + "/";

            builder.Path += "_search";

            var parameters = builder.Uri.GetComponents(UriComponents.Query, UriFormat.Unescaped)
                .Split(parameterSeparator, StringSplitOptions.RemoveEmptyEntries)
                .Select(p => p.Split('='))
                .ToDictionary(k => k[0], v => v.Length > 1 ? v[1] : null);

            if (!String.IsNullOrEmpty(searchRequest.SearchType))
                parameters["search_type"] = searchRequest.SearchType;

            if (connection.Options.Pretty)
                parameters["pretty"] = "true";

            builder.Query = String.Join("&", parameters.Select(p => p.Value == null ? p.Key : p.Key + "=" + p.Value));

            return builder.Uri;
        }

        private JObject CreateBody()
        {
            var root = new JObject();

            if (searchRequest.Fields.Any())
                root.Add("fields", new JArray(searchRequest.Fields));

            if (searchRequest.Query != null)
                root.Add("query", Build(searchRequest.Query));

            // Filters are pushed down to the facets for aggregate queries
            if (searchRequest.Filter != null && !searchRequest.Facets.Any())
                root.Add("filter", Build(searchRequest.Filter));

            if (searchRequest.SortOptions.Any())
                root.Add("sort", Build(searchRequest.SortOptions));

            if (searchRequest.From > 0)
                root.Add("from", searchRequest.From);

            if (searchRequest.Size.HasValue)
                root.Add("size", searchRequest.Size.Value);

            if (searchRequest.Facets.Any())
                root.Add("facets", Build(searchRequest.Facets, searchRequest.Filter));

            if (connection.Timeout != TimeSpan.Zero)
                root.Add("timeout", Format(connection.Timeout));

            return root;
        }

        private JToken Build(IEnumerable<IFacet> facets, ICriteria primaryFilter)
        {
            return new JObject(facets.Select(facet => Build(facet, primaryFilter)));
        }

        private JProperty Build(IFacet facet, ICriteria primaryFilter)
        {
            Argument.EnsureNotNull("facet", facet);

            var specificBody = Build(facet);
            var orderableFacet = facet as IOrderableFacet;
            if (orderableFacet != null && orderableFacet.Size.HasValue)
                specificBody["size"] = orderableFacet.Size.Value.ToString(CultureInfo.InvariantCulture);

            var namedBody = new JObject(new JProperty(facet.Type, specificBody));

            var combinedFilter = AndCriteria.Combine(primaryFilter, facet.Filter);
            if (combinedFilter != null)
                namedBody[facet is FilterFacet ? "filter" : "facet_filter"] = Build(combinedFilter);

            return new JProperty(facet.Name, namedBody);
        }

        private static JToken Build(IFacet facet)
        {
            if (facet is StatisticalFacet)
                return Build((StatisticalFacet)facet);

            if (facet is TermsStatsFacet)
                return Build((TermsStatsFacet)facet);

            if (facet is TermsFacet)
                return Build((TermsFacet)facet);

            if (facet is FilterFacet)
                return new JObject();

            throw new InvalidOperationException(string.Format("Unknown implementation of IFacet {0} can not be formatted", facet.GetType().Name));
        }

        private static JToken Build(StatisticalFacet statisticalFacet)
        {
            return new JObject(
                BuildFieldProperty(statisticalFacet.Fields)
            );
        }

        private static JToken Build(TermsStatsFacet termStatsFacet)
        {
            return new JObject(
                new JProperty("key_field", termStatsFacet.Key),
                new JProperty("value_field", termStatsFacet.Value)
            );
        }

        private static JToken Build(TermsFacet termsFacet)
        {
            return new JObject(BuildFieldProperty(termsFacet.Fields));
        }

        private static JToken BuildFieldProperty(ReadOnlyCollection<string> fields)
        {
            return fields.Count == 1
                ? new JProperty("field", fields.First())
                : new JProperty("fields", new JArray(fields));
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

        private JObject Build(ICriteria criteria)
        {
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

            var queryStringCriteria = new JObject(new JProperty("query", unformattedValue));

            if (criteria.Fields.Any())
                queryStringCriteria.Add(new JProperty("fields", new JArray(criteria.Fields)));

            return new JObject(new JProperty(criteria.Name, queryStringCriteria));
        }

        private JObject Build(RangeCriteria criteria)
        {
            // Range filters can be combined by field
            return new JObject(
                new JProperty(criteria.Name,
                    new JObject(new JProperty(criteria.Field,
                        new JObject(criteria.Specifications.Select(s =>
                            new JProperty(s.Name, mapping.FormatValue(criteria.Member, s.Value))).ToList())))));
        }

        private static JObject Build(RegexpCriteria criteria)
        {
            return new JObject(new JProperty(criteria.Name, new JObject(new JProperty(criteria.Field, criteria.Regexp))));
        }

        private static JObject Build(PrefixCriteria criteria)
        {
            return new JObject(new JProperty(criteria.Name, new JObject(new JProperty(criteria.Field, criteria.Prefix))));
        }

        private JObject Build(TermCriteria criteria)
        {
            return new JObject(
                new JProperty(criteria.Name, new JObject(
                    new JProperty(criteria.Field, mapping.FormatValue(criteria.Member, criteria.Value)))));
        }

        private JObject Build(TermsCriteria criteria)
        {
            var termsCriteria = new JObject(
                new JProperty(criteria.Field,
                    new JArray(criteria.Values.Select(x => mapping.FormatValue(criteria.Member, x)).Cast<Object>().ToArray())));

            if (criteria.ExecutionMode.HasValue)
                termsCriteria.Add(new JProperty("execution", criteria.ExecutionMode.GetValueOrDefault().ToString()));

            return new JObject(new JProperty(criteria.Name, termsCriteria));
        }

        private static JObject Build(SingleFieldCriteria criteria)
        {
            return new JObject(new JProperty(criteria.Name, new JObject(new JProperty("field", criteria.Field))));
        }

        private JObject Build(NotCriteria criteria)
        {
            return new JObject(new JProperty(criteria.Name, Build(criteria.Criteria)));
        }

        private static JObject Build(MatchAllCriteria criteria)
        {
            return new JObject(new JProperty(criteria.Name));
        }

        private JObject Build(CompoundCriteria criteria)
        {
            // A compound filter with one item can be collapsed
            return criteria.Criteria.Count == 1
                ? Build(criteria.Criteria.First())
                : new JObject(new JProperty(criteria.Name, new JArray(criteria.Criteria.Select(Build).ToList())));
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
