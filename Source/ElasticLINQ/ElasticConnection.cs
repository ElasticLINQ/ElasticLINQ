// Licensed under the Apache 2.0 License. See LICENSE.txt in the project root for more information.

using System.Net.Http;
using ElasticLinq.Utility;
using System;
using System.Diagnostics;

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
        private string index;
        private readonly string password;
        private readonly TimeSpan timeout = defaultTimeout;
        private readonly string userName;
        private readonly HttpClient httpClient;

        public ElasticConnection(Uri endpoint, string userName = null, string password = null, TimeSpan? timeout = null)
            : this(new WebRequestHandler(), endpoint, userName, password, timeout) {}

        internal ElasticConnection(HttpMessageHandler innerMessageHandler, Uri endpoint, string userName = null, string password = null, TimeSpan? timeout = null)
        {
            Argument.EnsureNotNull("endpoint", endpoint);
            if (timeout.HasValue)
                Argument.EnsurePositive("value", timeout.Value);

            this.endpoint = endpoint;
            this.userName = userName;
            this.password = password;
            this.timeout = timeout ?? defaultTimeout;

            httpClient = new HttpClient(new ForcedAuthHandler(this.userName, this.password, innerMessageHandler));
        }

        public HttpClient HttpClient
        {
            get { return httpClient; }
        }

        public Uri Endpoint
        {
            get { return endpoint; }
        }

        // TODO: Move to ctor and make immutable
        public string Index
        {
            get { return index; }
            set
            {
                Argument.EnsureNotBlank("value", value);
                index = value;
            }
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