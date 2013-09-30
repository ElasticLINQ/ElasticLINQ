// Copyright (c) Tier 3 Inc. All rights reserved.
// This source code is made available under the terms of the Microsoft Public License (MS-PL)

using ElasticLinq.Response.Model;
using ElasticLinq.Utility;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace ElasticLinq.Request
{
    /// <summary>
    /// Sends ElasticSearch requests and ensures the response is materialized.
    /// </summary>
    internal class ElasticRequestProcessor
    {
        private readonly ElasticConnection connection;
        private readonly TextWriter log;

        public ElasticRequestProcessor(ElasticConnection connection, TextWriter log)
        {
            this.connection = connection;
            this.log = log;
        }

        public async Task<ElasticResponse> Search(ElasticSearchRequest searchRequest)
        {
            var response = await SendRequest(BuildSearchUri(searchRequest, connection));

            using (var responseStream = await response.Content.ReadAsStreamAsync())
                return ParseResponse(responseStream);
        }

        private async Task<HttpResponseMessage> SendRequest(Uri uri)
        {
            using (var httpClient = new HttpClient { Timeout = connection.Timeout })
            {
                log.WriteLine("Request " + uri);
                var stopwatch = new Stopwatch();
                stopwatch.Start();
                var response = await httpClient.GetAsync(uri);
                stopwatch.Stop();
                log.WriteLine("{0} response in {1}ms", response.StatusCode, stopwatch.ElapsedMilliseconds);
                return response;
            }
        }

        private ElasticResponse ParseResponse(Stream responseStream)
        {
            using (var streamReader = new StreamReader(responseStream))
            {
                var stopwatch = new Stopwatch();
                stopwatch.Start();
                var results = new JsonSerializer().Deserialize<ElasticResponse>(new JsonTextReader(streamReader));
                stopwatch.Stop();
                log.WriteLine("Deserialized {0} bytes into {1} objects in {2}ms",
                    responseStream.Length, results.hits.hits.Count, stopwatch.ElapsedMilliseconds);
                return results;
            }
        }

        internal static Uri BuildSearchUri(ElasticSearchRequest searchRequest, ElasticConnection connection)
        {
            var builder = new UriBuilder(connection.Endpoint);

            if (!String.IsNullOrEmpty(connection.Index))
                builder.Path += connection.Index + "/";

            if (!String.IsNullOrEmpty(searchRequest.Type))
                builder.Path += searchRequest.Type + "/";

            builder.Path += "_search";
            builder.Query = MakeQueryString(GetSearchParameters(searchRequest));

            return builder.Uri;
        }

        private static IEnumerable<KeyValuePair<string, string>> GetSearchParameters(ElasticSearchRequest searchRequest)
        {
            if (searchRequest.Fields.Any())
                yield return KeyValuePair.Create("fields", string.Join(",", searchRequest.Fields));

            foreach (var sortOption in searchRequest.SortOptions)
                yield return KeyValuePair.Create("sort", sortOption.Name + (sortOption.Ascending ? "" : ":desc"));

            if (searchRequest.Skip > 0)
                yield return KeyValuePair.Create("from", searchRequest.Skip.ToString(CultureInfo.InvariantCulture));

            if (searchRequest.Take.HasValue)
                yield return KeyValuePair.Create("size", searchRequest.Take.Value.ToString(CultureInfo.InvariantCulture));
        }

        private static string MakeQueryString(IEnumerable<KeyValuePair<string, string>> queryParameters)
        {
            return string.Join("&", queryParameters.Select(p => p.Key + (p.Value == null ? "" : "=" + p.Value)));
        }
    }
}