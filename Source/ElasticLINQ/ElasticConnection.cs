// Licensed under the Apache 2.0 License. See LICENSE.txt in the project root for more information.

using ElasticLinq.Utility;
using System;
using System.Diagnostics;
using System.Net;
using System.Net.Http;

namespace ElasticLinq
{
    /// <summary>
    /// Specifies connection parameters for ElasticSearch.
    /// </summary>
    [DebuggerDisplay("{Endpoint.ToString(),nq}{Index,nq}")]
    public class ElasticConnection : IDisposable
    {
        private static readonly TimeSpan defaultTimeout = TimeSpan.FromSeconds(10);

        private readonly Uri endpoint;
        private readonly string index;
        private readonly string password;
        private readonly TimeSpan timeout = defaultTimeout;
        private readonly string userName;
        private readonly HttpClient httpClient;

        public ElasticConnection(Uri endpoint, string userName = null, string password = null, TimeSpan? timeout = null, string index = null)
            : this(new HttpClientHandler(), endpoint, userName, password, index, timeout) { }

        internal ElasticConnection(HttpMessageHandler innerMessageHandler, Uri endpoint, string userName = null, string password = null, string index = null, TimeSpan? timeout = null)
        {
            Argument.EnsureNotNull("endpoint", endpoint);
            if (timeout.HasValue)
                Argument.EnsurePositive("value", timeout.Value);
            if (index != null)
                Argument.EnsureNotBlank("index", index);

            this.endpoint = endpoint;
            this.userName = userName;
            this.password = password;
            this.index = index;
            this.timeout = timeout ?? defaultTimeout;

            var httpClientHandler = innerMessageHandler as HttpClientHandler;
            if (httpClientHandler != null && httpClientHandler.SupportsAutomaticDecompression)
                httpClientHandler.AutomaticDecompression = DecompressionMethods.GZip;

            httpClient = new HttpClient(new ForcedAuthHandler(this.userName, this.password, innerMessageHandler), true);
        }

        public HttpClient HttpClient
        {
            get { return httpClient; }
        }

        public Uri Endpoint
        {
            get { return endpoint; }
        }

        public string Index
        {
            get { return index; }
        }

        public string Password
        {
            get { return password; }
        }

        public TimeSpan Timeout
        {
            get { return timeout; }
        }

        public string UserName
        {
            get { return userName; }
        }

        public void Dispose()
        {
            httpClient.Dispose();
        }
    }
}