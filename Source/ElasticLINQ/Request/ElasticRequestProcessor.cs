// Licensed under the Apache 2.0 License. See LICENSE.txt in the project root for more information.

using ElasticLinq.Request.Formatters;
using ElasticLinq.Response.Model;
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
        private readonly HttpMessageHandler innerMessageHandler;
        private readonly TextWriter log;

        public ElasticRequestProcessor(ElasticConnection connection, TextWriter log)
            : this(connection, log, new WebRequestHandler()) { }

        internal ElasticRequestProcessor(ElasticConnection connection, TextWriter log, HttpMessageHandler innerMessageHandler)
        {
            Argument.EnsureNotNull("connection", connection);

            this.connection = connection;
            this.log = log;
            this.innerMessageHandler = innerMessageHandler;
        }

        public async Task<ElasticResponse> SearchAsync(ElasticSearchRequest searchRequest)
        {
            using (var requestMessage = CreateRequestMessage(searchRequest))
            using (var response = await SendRequestAsync(requestMessage))
            using (var responseStream = await response.Content.ReadAsStreamAsync())
                return ParseResponse(responseStream, log);
        }

        private HttpRequestMessage CreateRequestMessage(ElasticSearchRequest searchRequest)
        {
            var formatter = new PostBodyRequestFormatter(connection, searchRequest);
            var message = new HttpRequestMessage(HttpMethod.Post, formatter.Uri);

            if (log != null)
                log.WriteLine("Request {0} {1}", message.Method, message.RequestUri);

            var body = formatter.Body;
            message.Content = new StringContent(body);

            if (log != null)
                log.WriteLine("Body\n{0}", body);

            return message;
        }

        private async Task<HttpResponseMessage> SendRequestAsync(HttpRequestMessage requestMessage)
        {
            using (var httpClient = new HttpClient(new ForcedAuthHandler(connection.UserName, connection.Password, innerMessageHandler)) { Timeout = connection.Timeout })
            {
                var stopwatch = Stopwatch.StartNew();
                var response = await httpClient.SendAsync(requestMessage);
                stopwatch.Stop();

                if (log != null)
                    log.WriteLine("{0} response in {1}ms", response.StatusCode, stopwatch.ElapsedMilliseconds);

                return response;
            }
        }

        internal static ElasticResponse ParseResponse(Stream responseStream, TextWriter log)
        {
            using (var streamReader = new StreamReader(responseStream))
            {
                var stopwatch = Stopwatch.StartNew();
                var results = new JsonSerializer().Deserialize<ElasticResponse>(new JsonTextReader(streamReader));
                var resultCount = results == null ? 0 : results.hits.hits.Count;
                stopwatch.Stop();

                if (log != null)
                    log.WriteLine("Deserialized {0} bytes into {1} objects in {2}ms",
                        responseStream.Length, resultCount, stopwatch.ElapsedMilliseconds);

                return results;
            }
        }
    }
}