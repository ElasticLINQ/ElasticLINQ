// Licensed under the Apache 2.0 License. See LICENSE.txt in the project root for more information.

namespace ElasticLinq.Connection
{
    using System;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Reflection;
    using System.Threading.Tasks;
    using ElasticLinq.Communication;
    using ElasticLinq.Communication.Attributes;
    using ElasticLinq.Logging;
    using ElasticLinq.Path;
    using ElasticLinq.Utility;
    using Newtonsoft.Json;

    /// <summary>
    /// Specifies connection parameters for Elasticsearch.
    /// </summary>
    [DebuggerDisplay("{Endpoint.ToString(),nq}{Index,nq}")]
    public class HttpElasticConnection : IElasticConnection, IDisposable
    {
        private static readonly TimeSpan defaultTimeout = TimeSpan.FromSeconds(10);

        private readonly Uri endpoint;
        private readonly TimeSpan timeout = defaultTimeout;
        private readonly HttpClient httpClient;
        private readonly ElasticConnectionOptions options = new ElasticConnectionOptions();

        private bool disposed;

        /// <summary>
        /// Create a new ElasticConnection with the given parameters defining its properties.
        /// </summary>
        /// <param name="endpoint">The URL endpoint of the Elasticsearch server.</param>
        /// <param name="userName">UserName to use to connect to the server (optional).</param>
        /// <param name="password">Password to use to connect to the server (optional).</param>
        /// <param name="timeout">TimeSpan to wait for network responses before failing (optional, defaults to 10 seconds).</param>
        /// <param name="index">Name of the index to use on the server (optional).</param>
        public HttpElasticConnection(Uri endpoint, string userName = null, string password = null, TimeSpan? timeout = null)
            : this(new HttpClientHandler(), endpoint, userName, password, timeout) { }


        /// <summary>
        /// Create a new ElasticConnection with the given parameters for internal testing.
        /// </summary>
        /// <param name="innerMessageHandler">The HttpMessageHandler used to intercept network requests for testing.</param>
        /// <param name="endpoint">The URL endpoint of the Elasticsearch server.</param>
        /// <param name="userName">UserName to use to connect to the server (optional).</param>
        /// <param name="password">Password to use to connect to the server (optional).</param>
        /// <param name="timeout">TimeSpan to wait for network responses before failing (optional, defaults to 10 seconds).</param>
        /// <param name="index">Name of the index to use on the server (optional).</param>
        internal HttpElasticConnection(HttpMessageHandler innerMessageHandler, Uri endpoint, string userName = null, string password = null, TimeSpan? timeout = null)
        {
            Argument.EnsureNotNull("endpoint", endpoint);
            if (timeout.HasValue)
                Argument.EnsurePositive("value", timeout.Value);

            this.endpoint = endpoint;
            this.timeout = timeout ?? defaultTimeout;

            var httpClientHandler = innerMessageHandler as HttpClientHandler;
            if (httpClientHandler != null && httpClientHandler.SupportsAutomaticDecompression)
                httpClientHandler.AutomaticDecompression = DecompressionMethods.GZip;

            httpClient = new HttpClient(new ForcedAuthHandler(userName, password, innerMessageHandler), true);
        }

        public ElasticConnectionOptions Options
        {
            get { return this.options; }
        }

        /// <summary>
        /// Dispose of this ElasticConnection and any associated resources.
        /// </summary>
        public void Dispose()
        {
            if (disposed)
                return;

            disposed = true;
            httpClient.Dispose();
        }

        public async Task<bool> Head<TRequest>(TRequest request, ILog log)
        {
            var uri = MakeUri(this.endpoint, request);

            using (var requestMessage = new HttpRequestMessage(HttpMethod.Head, uri))
            {
                using (var response = await SendRequestAsync(this.httpClient, requestMessage, null, log))
                {
                    log.Debug(null, null, "<== HTTP RESPONSE ({0}) {1}", response.StatusCode, response.ReasonPhrase);

                    return response.StatusCode == HttpStatusCode.OK;
                }
            }
        }

        public async Task<TResponse> Get<TResponse, TRequest>(TRequest request, ILog log)
        {
            var uri = MakeUri(this.endpoint, request);

            using (var requestMessage = new HttpRequestMessage(HttpMethod.Get, uri))
            {
                using (var response = await SendRequestAsync(this.httpClient, requestMessage, null, log))
                {
                    log.Debug(null, null, "<== HTTP RESPONSE ({0}) {1}", response.StatusCode, response.ReasonPhrase);

                    using (var responseStream = await response.Content.ReadAsStreamAsync())
                    {
                        return ParseResponse<TResponse>(responseStream, log);
                    }
                }
            }
        }

        public async Task<TResponse> Post<TResponse, TRequest>(TRequest request, string body, ILog log)
        {
            var uri = MakeUri(this.endpoint, request);

            using (var requestMessage = new HttpRequestMessage(HttpMethod.Post, uri))
            {
                using (var response = await SendRequestAsync(this.httpClient, requestMessage, body, log))
                {
                    log.Debug(null, null, "<== HTTP RESPONSE ({0}) {1}", response.StatusCode, response.ReasonPhrase);

                    using (var responseStream = await response.Content.ReadAsStreamAsync())
                    {
                        return ParseResponse<TResponse>(responseStream, log);
                    }
                }
            }
        }

        private static Uri MakeUri<TRequest>(Uri endpoint, TRequest request)
        {
            var builder = new UriBuilder(endpoint);

            var route = ElasticRouteHelper.GetRoute(request);

            if (string.IsNullOrEmpty(route) == false)
            {
                builder.Path += route;
            }

            return builder.Uri;
        }

        private static Uri MakeUri(Uri endpoint, ElasticPath path)
        {
            var builder = new UriBuilder(endpoint);

            if (path != null)
            {
                string pathSegment = "*";

                if (path.IndexPath != null)
                {
                    pathSegment = path.IndexPath.PathSegment;
                }

                builder.Path += pathSegment + "/";

                if (path.TypePath != null)
                {
                    builder.Path += path.TypePath.PathSegment;
                }
            }

            return builder.Uri;
        }

        private static Uri UpdateUri(Uri uri, ElasticConnectionOptions options)
        {
            var builder = new UriBuilder(uri);

            var parameters = builder.Query.Split('&')
                .Select(p => p.Split('='))
                .ToDictionary(k => k[0], v => v.Length > 1 ? v[1] : null);

            if (options.Pretty == true)
            {
                parameters["pretty"] = "true";
            }

            if (options.Human == false)
            {
                parameters["human"] = "false";
            }

            builder.Query = String.Join("&",
                parameters.Select(p => p.Value == null ? p.Key : p.Key + "=" + p.Value));

            return builder.Uri;
        }

        private async Task<HttpResponseMessage> SendRequestAsync(HttpClient httpClient, HttpRequestMessage requestMessage, string body, ILog log)
        {
            if (body != null)
            {
                requestMessage.Content = new StringContent(body);
            }

            requestMessage.RequestUri = UpdateUri(requestMessage.RequestUri, this.Options);

            log.Debug(null, null, "==> {0} - {1} - {2}", requestMessage.Method, requestMessage.RequestUri, body);

            var response = await httpClient.SendAsync(requestMessage);

            return response;
        }

        private static TResponse ParseResponse<TResponse>(Stream responseStream, ILog log)
        {
            using (var reader = new StreamReader(responseStream))
            {
                string response = reader.ReadToEnd();

                log.Debug(null, null, "<== {0}", response);

                return JsonConvert.DeserializeObject<TResponse>(response);
            }
        }
    }
}
