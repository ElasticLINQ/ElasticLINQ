// Licensed under the Apache 2.0 License. See LICENSE.txt in the project root for more information.

using ElasticLinq.Logging;
using ElasticLinq.Mapping;
using ElasticLinq.Request.Formatters;
using ElasticLinq.Response.Model;
using ElasticLinq.Retry;
using ElasticLinq.Utility;
using System.Threading;
using System.Threading.Tasks;

namespace ElasticLinq.Request
{
    /// <summary>
    /// Sends Elasticsearch requests via HTTP and ensures materialization of the response.
    /// </summary>
    class ElasticRequestProcessor
    {
        readonly IElasticConnection connection;
        readonly ILog log;
        readonly IElasticMapping mapping;
        readonly IRetryPolicy retryPolicy;

        public ElasticRequestProcessor(IElasticConnection connection, IElasticMapping mapping, ILog log, IRetryPolicy retryPolicy)
        {
            Argument.EnsureNotNull(nameof(connection), connection);
            Argument.EnsureNotNull(nameof(mapping), mapping);
            Argument.EnsureNotNull(nameof(log), log);
            Argument.EnsureNotNull(nameof(retryPolicy), retryPolicy);

            this.connection = connection;
            this.mapping = mapping;
            this.log = log;
            this.retryPolicy = retryPolicy;
        }

        public Task<ElasticResponse> SearchAsync(SearchRequest searchRequest, CancellationToken cancellationToken)
        {
            var formatter = new SearchRequestFormatter(connection, mapping, searchRequest);

            return retryPolicy.ExecuteAsync(
                async token => await connection.SearchAsync(
                    formatter.Body,
                    searchRequest,
                    token,
                    log),
                (response, exception) => !cancellationToken.IsCancellationRequested && exception != null,
                (response, additionalInfo) =>
                {
                    additionalInfo["index"] = connection.Index;
                    additionalInfo["uri"] = connection.GetSearchUri(searchRequest);
                    additionalInfo["query"] = formatter.Body;
                }, cancellationToken);
        }
    }
}
