// Licensed under the Apache 2.0 License. See LICENSE.txt in the project root for more information.

using ElasticLinq.Logging;
using ElasticLinq.Mapping;
using ElasticLinq.Request.Formatter;
using ElasticLinq.Response.Model;
using ElasticLinq.Retry;
using ElasticLinq.Utility;
using Newtonsoft.Json;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

namespace ElasticLinq.Request
{
    /// <summary>
    /// Sends ElasticSearch requests via HTTP and ensures the response is materialized.
    /// </summary>
    internal class ElasticRequestProcessor
    {
        private readonly ElasticConnection connection;
        private readonly ILog log;
        private readonly IElasticMapping mapping;
        private readonly IRetryPolicy retryPolicy;

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

        public Task<ElasticResponse> SearchAsync(ElasticSearchRequest searchRequest)
        {
            var formatter = new PostBodyRequestFormatter(connection, mapping, searchRequest);
            log.Debug(null, null, "Request: POST {0}", formatter.Uri);
            log.Debug(null, null, "Body: {0}", formatter.Body);

            return retryPolicy.ExecuteAsync(
                async () =>
                {
                    using (var requestMessage = new HttpRequestMessage(HttpMethod.Post, formatter.Uri) { Content = new StringContent(formatter.Body) })
                    using (var response = await SendRequestAsync(connection.HttpClient, requestMessage))
                    using (var responseStream = await response.Content.ReadAsStreamAsync())
                        return ParseResponse(responseStream, log);
                },
                (response, exception) => exception is TaskCanceledException,
                (response, additionalInfo) =>
                {
                    additionalInfo["index"] = connection.Index;
                    additionalInfo["query"] = formatter.Body;
                });
        }

        private async Task<HttpResponseMessage> SendRequestAsync(HttpClient httpClient, HttpRequestMessage requestMessage)
        {
            var stopwatch = Stopwatch.StartNew();
            var response = await httpClient.SendAsync(requestMessage);
            stopwatch.Stop();

            log.Debug(null, null, "Response: {0} {1} (in {2}ms)", (int)response.StatusCode, response.StatusCode, stopwatch.ElapsedMilliseconds);

            response.EnsureSuccessStatusCode();
            return response;
        }

        internal static ElasticResponse ParseResponse(Stream responseStream, ILog log)
        {
            var stopwatch = Stopwatch.StartNew();

            using (var streamReader = new StreamReader(responseStream))
            using (var textReader = new JsonTextReader(streamReader))
            {
                var results = new JsonSerializer().Deserialize<ElasticResponse>(textReader);
                var resultCount = results == null ? 0 : results.hits.hits.Count;
                stopwatch.Stop();

                log.Debug(null, null, "De-serialized {0} bytes into {1} hits in {2}ms", responseStream.Length, resultCount, stopwatch.ElapsedMilliseconds);

                return results;
            }
        }
    }
}
