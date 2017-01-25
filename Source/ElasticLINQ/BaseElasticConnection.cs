// Licensed under the Apache 2.0 License. See LICENSE.txt in the project root for more information.

using System;
using System.Threading;
using System.Threading.Tasks;
using ElasticLinq.Logging;
using ElasticLinq.Request;
using ElasticLinq.Response.Model;
using ElasticLinq.Utility;

namespace ElasticLinq
{
    /// <summary>
    /// Specifies connection parameters for Elasticsearch.
    /// </summary>
    public abstract class BaseElasticConnection : IElasticConnection
    {
        static readonly TimeSpan defaultTimeout = TimeSpan.FromSeconds(10);

        /// <summary>
        /// Create a new BaseElasticConnection with the given parameters for internal testing.
        /// </summary>
        /// <param name="timeout">TimeSpan to wait for network responses before failing (optional, defaults to 10 seconds).</param>
        /// <param name="index">Name of the index to use on the server (optional).</param>
        /// <param name="options">Additional options that specify how this connection should behave.</param>
        protected BaseElasticConnection(string index = null, TimeSpan? timeout = null, ElasticConnectionOptions options = null)
        {
            if (timeout.HasValue)
                Argument.EnsurePositive(nameof(timeout), timeout.Value);
            if (index != null)
                Argument.EnsureNotBlank(nameof(index), index);

            Index = index;
            Options = options ?? new ElasticConnectionOptions();
            Timeout = timeout ?? defaultTimeout;
        }

        /// <summary>
        /// The name of the index on the Elasticsearch server.
        /// </summary>
        /// <example>northwind</example>
        public string Index { get; }

        /// <summary>
        /// How long to wait for a response to a network request before
        /// giving up.
        /// </summary>
        public TimeSpan Timeout { get; }

        /// <summary>
        /// Additional options that specify how this connection should behave.
        /// </summary>
        public ElasticConnectionOptions Options { get; }

        /// <summary>
        /// Issues search requests to elastic search
        /// </summary>
        /// <param name="body">The request body</param>
        /// <param name="searchRequest">The search request settings</param>
        /// <param name="token">The cancellation token to allow aborting the operation</param>
        /// <param name="log">The logging mechanism for diagnostic information.</param>
        /// <returns>An elastic response</returns>
        public abstract Task<ElasticResponse> SearchAsync(string body, SearchRequest searchRequest, CancellationToken token, ILog log);

        /// <summary>
        /// Gets the uri of the search
        /// </summary>
        /// <param name="searchRequest">The search request settings</param>
        /// <returns>The uri of the search</returns>
        public abstract Uri GetSearchUri(SearchRequest searchRequest);
    }
}
