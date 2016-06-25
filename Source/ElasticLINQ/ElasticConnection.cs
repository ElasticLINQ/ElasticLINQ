// Licensed under the Apache 2.0 License. See LICENSE.txt in the project root for more information.

using ElasticLinq.Logging;
using ElasticLinq.Request;
using ElasticLinq.Response.Model;
using ElasticLinq.Utility;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace ElasticLinq
{
    /// <summary>
    /// Specifies connection parameters for Elasticsearch.
    /// </summary>
    [DebuggerDisplay("{Endpoint.ToString(),nq}{Index,nq}")]
    public class ElasticConnection : BaseElasticConnection, IDisposable
    {
        readonly string[] parameterSeparator = { "&" };
        readonly Uri endpoint;

        HttpClient httpClient;

        /// <summary>
        /// Create a new ElasticConnection with the given parameters defining its properties.
        /// </summary>
        /// <param name="endpoint">The URL endpoint of the Elasticsearch server.</param>
        /// <param name="userName">UserName to use to connect to the server (optional).</param>
        /// <param name="password">Password to use to connect to the server (optional).</param>
        /// <param name="timeout">TimeSpan to wait for network responses before failing (optional, defaults to 10 seconds).</param>
        /// <param name="index">Name of the index to use on the server (optional).</param>
        /// <param name="options">Additional options that specify how this connection should behave.</param>
        public ElasticConnection(Uri endpoint, string userName = null, string password = null, TimeSpan? timeout = null, string index = null, ElasticConnectionOptions options = null)
            : this(new HttpClientHandler(), endpoint, userName, password, index, timeout, options) { }


        /// <summary>
        /// Create a new ElasticConnection with the given parameters for internal testing.
        /// </summary>
        /// <param name="innerMessageHandler">The HttpMessageHandler used to intercept network requests for testing.</param>
        /// <param name="endpoint">The URL endpoint of the Elasticsearch server.</param>
        /// <param name="userName">UserName to use to connect to the server (optional).</param>
        /// <param name="password">Password to use to connect to the server (optional).</param>
        /// <param name="timeout">TimeSpan to wait for network responses before failing (optional, defaults to 10 seconds).</param>
        /// <param name="index">Name of the index to use on the server (optional).</param>
        /// <param name="options">Additional options that specify how this connection should behave.</param>
        internal ElasticConnection(HttpMessageHandler innerMessageHandler, Uri endpoint, string userName = null, string password = null, string index = null, TimeSpan? timeout = null, ElasticConnectionOptions options = null)
            : base(index, timeout, options)
        {
            Argument.EnsureNotNull(nameof(endpoint), endpoint);

            this.endpoint = endpoint;

            var httpClientHandler = innerMessageHandler as HttpClientHandler;
            if (httpClientHandler != null && httpClientHandler.SupportsAutomaticDecompression)
                httpClientHandler.AutomaticDecompression = DecompressionMethods.GZip;

            httpClient = new HttpClient(new ForcedAuthHandler(userName, password, innerMessageHandler), true);
        }

        /// <summary>
        /// The HttpClient used for issuing HTTP network requests.
        /// </summary>
        internal HttpClient HttpClient
        {
            get { return httpClient; }
        }

        /// <summary>
        /// The Uri that specifies the public endpoint for the server.
        /// </summary>
        /// <example>http://myserver.example.com:9200</example>
        public Uri Endpoint
        {
            get { return endpoint; }
        }

        /// <summary>
        /// Dispose of this ElasticConnection and any associated resources.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <inheritdoc/>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (httpClient != null)
                {
                    httpClient.Dispose();
                    httpClient = null;
                }
            }
        }

        /// <inheritdoc/>
        public override async Task<ElasticResponse> SearchAsync(
            string body,
            SearchRequest searchRequest,
            CancellationToken token,
            ILog log)
        {
            var uri = GetSearchUri(searchRequest);

            log.Debug(null, null, "Request: POST {0}", uri);
            log.Debug(null, null, "Body:\n{0}", body);

            using (var requestMessage = new HttpRequestMessage(HttpMethod.Post, uri) { Content = new StringContent(body) })
            using (var response = await SendRequestAsync(requestMessage, token, log))
            using (var responseStream = await response.Content.ReadAsStreamAsync())
                return ParseResponse(responseStream, log);
        }

        /// <inheritdoc/>
        public override Uri GetSearchUri(SearchRequest searchRequest)
        {
            var builder = new UriBuilder(endpoint);
            builder.Path += (Index ?? "_all") + "/";

            if (!String.IsNullOrEmpty(searchRequest.DocumentType))
                builder.Path += searchRequest.DocumentType + "/";

            builder.Path += "_search";

            var parameters = builder.Uri.GetComponents(UriComponents.Query, UriFormat.Unescaped)
                .Split(parameterSeparator, StringSplitOptions.RemoveEmptyEntries)
                .Select(p => p.Split('='))
                .ToDictionary(k => k[0], v => v.Length > 1 ? v[1] : null);

            if (!String.IsNullOrEmpty(searchRequest.SearchType))
                parameters["search_type"] = searchRequest.SearchType;

            if (Options.Pretty)
                parameters["pretty"] = "true";

            builder.Query = String.Join("&", parameters.Select(p => p.Value == null ? p.Key : p.Key + "=" + p.Value));

            return builder.Uri;
        }

        async Task<HttpResponseMessage> SendRequestAsync(HttpRequestMessage requestMessage, CancellationToken token, ILog log)
        {
            var stopwatch = Stopwatch.StartNew();
            var response = await httpClient.SendAsync(requestMessage, token);
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

                var resultSummary = String.Join(", ", GetResultSummary(results));
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