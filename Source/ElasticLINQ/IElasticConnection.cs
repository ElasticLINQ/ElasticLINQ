// Licensed under the Apache 2.0 License. See LICENSE.txt in the project root for more information.

using ElasticLinq.Logging;
using ElasticLinq.Request;
using ElasticLinq.Response.Model;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace ElasticLinq
{
    /// <summary>
    /// The interface all clients which make requests to elastic search must implement
    /// </summary>
    public interface IElasticConnection
    {
        /// <summary>
        /// The name of the index on the Elasticsearch server.
        /// </summary>
        string Index { get; }

        /// <summary>
        /// Additional options that specify how this connection should behave.
        /// </summary>
        ElasticConnectionOptions Options { get; }

        /// <summary>
        /// How long to wait for a response to a network request before
        /// giving up.
        /// </summary>
        TimeSpan Timeout { get; }

        /// <summary>
        /// Issuessearch requests to Elasticsearch.
        /// </summary>
        /// <param name="body">The request body.</param>
        /// <param name="searchRequest">The search request settings.</param>
        /// <param name="token">The cancellation token to allow this request to be cancelled.</param>
        /// <param name="log">The logging mechanism for diagnostic information.</param>
        /// <returns>An ElasticResponse object containing the desired search results.</returns>
        Task<ElasticResponse> SearchAsync(
            string body,
            SearchRequest searchRequest,
            CancellationToken token,
            ILog log);

        /// <summary>
        /// Get the uri to be used to search Elasticsearch.
        /// </summary>
        /// <param name="searchRequest">The search request settings.</param>
        /// <returns>The uri that will be used to search Elasticsearch.</returns>
        Uri GetSearchUri(SearchRequest searchRequest);
    }
}
