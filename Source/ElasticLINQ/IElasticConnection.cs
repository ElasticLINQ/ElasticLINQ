// Licensed under the Apache 2.0 License. See LICENSE.txt in the project root for more information.

using System;
using System.Threading;
using System.Threading.Tasks;
using ElasticLinq.Logging;
using ElasticLinq.Request;
using ElasticLinq.Response.Model;

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
        /// Issues search requests to elastic search
        /// </summary>
        /// <param name="body">The request body</param>
        /// <param name="searchRequest">The search request settings</param>
        /// <param name="token"></param>
        /// <param name="log">The logging mechanism for diagnostic information.</param>
        /// <returns>An elastic response</returns>
        Task<ElasticResponse> SearchAsync(
            string body,
            SearchRequest searchRequest,
            CancellationToken token,
            ILog log);

        /// <summary>
        /// Gets the uri of the search
        /// </summary>
        /// <param name="searchRequest">The search request settings</param>
        /// <returns>The uri of the search</returns>
        Uri GetSearchUri(SearchRequest searchRequest);
    }
}