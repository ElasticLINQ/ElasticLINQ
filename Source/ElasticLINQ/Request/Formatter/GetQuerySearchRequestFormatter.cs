// Copyright (c) Tier 3 Inc. All rights reserved.
// This source code is made available under the terms of the Microsoft Public License (MS-PL)

using System.IO;
using ElasticLinq.Utility;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace ElasticLinq.Request.Formatter
{
    internal class GetQuerySearchRequestFormatter : SearchRequestFormatter
    {
        public GetQuerySearchRequestFormatter(ElasticConnection connection, ElasticSearchRequest searchRequest)
            : base(connection, searchRequest)
        {
        }

        protected override void CompleteSearchUri(UriBuilder builder)
        {
            builder.Query = MakeQueryString(GetSearchParameters(SearchRequest, Connection));
        }

        private static IEnumerable<KeyValuePair<string, string>> GetSearchParameters(ElasticSearchRequest searchRequest, ElasticConnection connection)
        {
            if (searchRequest.Fields.Any())
                yield return KeyValuePair.Create("fields", string.Join(",", searchRequest.Fields));

            foreach (var queryCriteria in searchRequest.QueryCriteria)
                yield return KeyValuePair.Create("q", queryCriteria.Key + ":" + queryCriteria.Value);

            foreach (var sortOption in searchRequest.SortOptions)
                yield return KeyValuePair.Create("sort", sortOption.Name + (sortOption.Ascending ? "" : ":desc"));

            if (searchRequest.Skip > 0)
                yield return KeyValuePair.Create("from", searchRequest.Skip.ToString(CultureInfo.InvariantCulture));

            if (searchRequest.Take.HasValue)
                yield return KeyValuePair.Create("size", searchRequest.Take.Value.ToString(CultureInfo.InvariantCulture));

            yield return KeyValuePair.Create("timeout", Format(connection.Timeout));
        }

        private static string MakeQueryString(IEnumerable<KeyValuePair<string, string>> queryParameters)
        {
            return string.Join("&", queryParameters.Select(p => p.Key + (p.Value == null ? "" : "=" + p.Value)));
        }
    }
}