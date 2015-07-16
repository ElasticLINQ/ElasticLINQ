// Licensed under the Apache 2.0 License. See LICENSE.txt in the project root for more information.

using ElasticLinq.Logging;
using ElasticLinq.Mapping;
using ElasticLinq.Request.Formatters;
using ElasticLinq.Response.Model;
using ElasticLinq.Retry;
using ElasticLinq.Utility;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace ElasticLinq.Request
{
    /// <summary>
    /// Sends Elasticsearch requests via HTTP and ensures materialization of the response.
    /// </summary>
    class ElasticRequestProcessor
    {
        readonly ElasticConnection connection;
        readonly ILog log;
        readonly IElasticMapping mapping;
        readonly IRetryPolicy retryPolicy;

        public ElasticRequestProcessor(ElasticConnection connection, IElasticMapping mapping, ILog log, IRetryPolicy retryPolicy)
        {
            Argument.EnsureNotNull("connection", connection);
            Argument.EnsureNotNull("mapping", mapping);
            Argument.EnsureNotNull("log", log);
            Argument.EnsureNotNull("retryPolicy", retryPolicy);

            this.connection = connection;
            this.mapping = mapping;
            this.log = log;
            this.retryPolicy = retryPolicy;
        }

        public Task<ElasticResponse> SearchAsync(SearchRequest searchRequest, CancellationToken cancellationToken)
        {
            var formatter = new SearchRequestFormatter(connection, mapping, searchRequest);
            log.Debug(null, null, "Request: POST {0}", formatter.Uri);
            log.Debug(null, null, "Body:\n{0}", formatter.Body);

            return retryPolicy.ExecuteAsync(
                async token =>
                {
                    using (var requestMessage = new HttpRequestMessage(HttpMethod.Post, formatter.Uri) { Content = new StringContent(formatter.Body) })
                    using (var response = await SendRequestAsync(connection.HttpClient, requestMessage, token))
                    using (var responseStream = await response.Content.ReadAsStreamAsync())
                        return ParseResponse(responseStream, log);
                },
                (response, exception) => !cancellationToken.IsCancellationRequested && exception != null,
                (response, additionalInfo) =>
                {
                    additionalInfo["index"] = connection.Index;
                    additionalInfo["uri"] = formatter.Uri;
                    additionalInfo["query"] = formatter.Body;
                }, cancellationToken);
        }

        async Task<HttpResponseMessage> SendRequestAsync(HttpClient httpClient, HttpRequestMessage requestMessage, CancellationToken cancellationToken)
        {
            var stopwatch = Stopwatch.StartNew();
            var response = await httpClient.SendAsync(requestMessage, cancellationToken);
            stopwatch.Stop();

            log.Debug(null, null, "Response: {0} {1} (in {2}ms)", (int)response.StatusCode, response.StatusCode, stopwatch.ElapsedMilliseconds);

            response.EnsureSuccessStatusCode();
            return response;
        }

        internal static ElasticResponse ParseResponse(Stream responseStream, ILog log)
        {
            var stopwatch = Stopwatch.StartNew();

            using (var textReader = new JsonTextReader(new StreamReader(responseStream)))
            {
                var results = new JsonSerializer().Deserialize<ElasticResponse>(textReader);
                stopwatch.Stop();

                var resultSummary = string.Join(", ", GetResultSummary(results));
                log.Debug(null, null, "Deserialized {0} bytes into {1} in {2}ms", responseStream.Length, resultSummary, stopwatch.ElapsedMilliseconds);

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
                if (results.hits != null && results.hits.hits != null && results.hits.hits.Count > 0)
                    yield return results.hits.hits.Count + " hits";

                if (results.facets != null && results.facets.Count > 0)
                    yield return results.facets.Count + " facets";
            }
        }
    }
}
