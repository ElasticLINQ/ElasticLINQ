// Licensed under the Apache 2.0 License. See LICENSE.txt in the project root for more information.

using ElasticLinq.Logging;
using ElasticLinq.Mapping;
using ElasticLinq.Request.Formatters;
using ElasticLinq.Response.Model;
using ElasticLinq.Retry;
using ElasticLinq.Utility;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
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
                    log).ConfigureAwait(false),
                (response, exception) => !cancellationToken.IsCancellationRequested && exception != null,
                (response, additionalInfo) =>
                {
                    additionalInfo["index"] = connection.Index;
                    additionalInfo["uri"] = connection.GetSearchUri(searchRequest);
                    additionalInfo["query"] = formatter.Body;
                }, cancellationToken);
        }

        internal static ElasticResponse ParseResponse(Stream responseStream, ILog log)
        {
            var stopwatch = Stopwatch.StartNew();

            using (var textReader = new JsonTextReader(new StreamReader(responseStream)))
            {
                var results = new JsonSerializer().Deserialize<ElasticResponse>(textReader);
                stopwatch.Stop();

                var resultSummary = string.Join(", ", GetResultSummary(results));
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
                if (results.hits?.hits != null && results.hits.hits.Count > 0)
                    yield return results.hits.hits.Count + " hits";
            }
        }
    }
}
