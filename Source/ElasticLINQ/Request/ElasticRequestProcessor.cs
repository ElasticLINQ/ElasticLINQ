// Licensed under the Apache 2.0 License. See LICENSE.txt in the project root for more information.

using ElasticLinq.Logging;
using ElasticLinq.Mapping;
using ElasticLinq.Request.Formatters;
using ElasticLinq.Response.Model;
using ElasticLinq.Retry;
using ElasticLinq.Utility;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

namespace ElasticLinq.Request
{
    /// <summary>
    /// Sends Elasticsearch requests via HTTP and ensures materialization of the response.
    /// </summary>
    internal class ElasticRequestProcessor
    {
        private readonly IElasticConnection connection;
        private readonly ILog log;
        private readonly IElasticMapping mapping;
        private readonly IRetryPolicy retryPolicy;

        public ElasticRequestProcessor(IElasticConnection connection, IElasticMapping mapping, ILog log, IRetryPolicy retryPolicy)
        {
            Argument.EnsureNotNull("connection", connection);
            Argument.EnsureNotNull("mapping", mapping);
            Argument.EnsureNotNull("log", log);
            Argument.EnsureNotNull("retryPolicy", retryPolicy);

            this.connection = connection;
            this.mapping = mapping;
            this.log = log;
            this.retryPolicy = retryPolicy;
        }

        public Task<ElasticResponse> SearchAsync(SearchRequest searchRequest)
        {
            var formatter = new SearchRequestFormatter(connection, mapping, searchRequest);

            return retryPolicy.ExecuteAsync(
                async () =>
                {
                    return await connection.Post<ElasticResponse>(formatter.Uri, formatter.Body, this.log);
                },
                (response, exception) => exception is TaskCanceledException,
                (response, additionalInfo) =>
                {
                    additionalInfo["index"] = connection.Index;
                    additionalInfo["query"] = formatter.Body;
                });
        }
    }
}
