// Licensed under the Apache 2.0 License. See LICENSE.txt in the project root for more information.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using ElasticLinq.Logging;
using ElasticLinq.Request;
using ElasticLinq.Response.Model;
using Elasticsearch.Net;
using Newtonsoft.Json;
using TraceEventType = ElasticLinq.Logging.TraceEventType;

namespace ElasticLinq.ElasticsearchNet
{
    /// <summary>
    /// Specifies connection parameters for Elasticsearch.
    /// </summary>
    public class ElasticNetConnection : BaseElasticConnection
    {
        private readonly IElasticsearchClient client;

        /// <summary>
        /// Create a new ElasticNetConnection with the given parameters defining its properties.
        /// </summary>
        /// <param name="client">The ElasticsearchClient to use in order to contact elasticsearch.</param>
        /// <param name="timeout">TimeSpan to wait for network responses before failing (optional, defaults to 10 seconds).</param>
        /// <param name="index">Name of the index to use on the server (optional).</param>
        /// <param name="options">Additional options that specify how this connection should behave.</param>
        public ElasticNetConnection(
            IElasticsearchClient client,
            string index = null,
            TimeSpan? timeout = null,
            ElasticConnectionOptions options = null)
            : base(index, timeout, options)
        {
            if (client == null) throw new ArgumentNullException(nameof(client));

            this.client = client;
        }

        /// <inheritdoc/>
        public override async Task<ElasticResponse> SearchAsync(
            string body,
            SearchRequest searchRequest,
            CancellationToken token,
            ILog log)
        {
            var stopwatch = Stopwatch.StartNew();

            var response = await Task.Run(() => client.SearchAsync<string>(
                                Index ?? "_all",
                                searchRequest.DocumentType,
                                body,
                                searchParams => searchParams),
                            token);

            stopwatch.Stop();

            log.Log(TraceEventType.Verbose, null, null, "Request: POST {0}", response.RequestUrl);
            log.Log(TraceEventType.Verbose, null, null, "Body:\n{0}", body);
            log.Log(TraceEventType.Verbose, null, null, "Response: {0} {1} (in {2}ms)", response.HttpStatusCode, response.HttpStatusCode.HasValue ? ((HttpStatusCode)response.HttpStatusCode).ToString() : "", stopwatch.ElapsedMilliseconds);

            if (!response.Success)
                throw new HttpRequestException($"Response status code does not indicate success: {response.HttpStatusCode}");

            return ParseResponse(response.Response, log);
        }

        /// <inheritdoc/>
        public override Uri GetSearchUri(SearchRequest searchRequest)
        {
            return new Uri("");
        }

        internal static ElasticResponse ParseResponse(string response, ILog log)
        {
            var stopwatch = Stopwatch.StartNew();

            using (var textReader = new JsonTextReader(new StringReader(response)))
            {
                var results = new JsonSerializer().Deserialize<ElasticResponse>(textReader);
                stopwatch.Stop();

                var resultSummary = string.Join(", ", GetResultSummary(results));

                log.Log(
                    TraceEventType.Verbose,
                    null,
                    null,
                    "Deserialized {0} characters into {1} in {2}ms",
                    response.Length,
                    resultSummary,
                    stopwatch.ElapsedMilliseconds);

                return results;
            }
        }

        static IEnumerable<string> GetResultSummary(ElasticResponse results)
        {
            if (results == null)
            {
                yield return "nothing";
            }
            else
            {
                if (results.hits?.hits != null && results.hits.hits.Count > 0)
                    yield return results.hits.hits.Count + " hits";
            }
        }
    }
}