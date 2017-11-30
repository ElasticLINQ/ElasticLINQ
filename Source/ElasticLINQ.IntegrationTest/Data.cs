using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ElasticLinq.IntegrationTest.Models;
using ElasticLinq.Mapping;
using ElasticLinq.Retry;

namespace ElasticLinq.IntegrationTest
{
    class Data
    {
        public static readonly Uri Endpoint = new Uri("http://52.183.26.158:9200/");

        const string Index = "integrationtest";
        static readonly ElasticConnectionOptions options = new ElasticConnectionOptions { SearchSizeDefault = 1000, Pretty = true };
        static readonly IElasticConnection connection = new BreakOnInvalidQueryConnection(Endpoint, index: Index, options: options);

        readonly ElasticContext elasticContext = new ElasticContext(connection, new TrivialElasticMapping(), retryPolicy: new NoRetryPolicy());
        readonly List<object> memory = new List<object>();

        public IQueryable<T> Elastic<T>()
        {
            return elasticContext.Query<T>();
        }

        public IQueryable<T> Memory<T>()
        {
            return memory.AsQueryable().OfType<T>();
        }

        public void LoadMemoryFromElastic()
        {
            memory.Clear();
            memory.AddRange(elasticContext.Query<WebUser>());
            memory.AddRange(elasticContext.Query<JobOpening>());

            const int expectedDataCount = 200;
            if (memory.Count != expectedDataCount)
                throw new InvalidOperationException(
                    $"Tests expect {expectedDataCount} entries but {memory.Count} loaded from Elasticsearch index '{elasticContext.Connection.Index}' at {((ElasticConnection) elasticContext.Connection).Endpoint}");
        }
    }

    public class NoRetryPolicy : IRetryPolicy
    {
        public async Task<TResult> ExecuteAsync<TResult>(Func<CancellationToken, Task<TResult>> operationFunc, Func<TResult, Exception, Boolean> shouldRetryFunc, Action<TResult, Dictionary<String, Object>> appendLogInfoFunc = null,
            CancellationToken cancellationToken = new CancellationToken())
        {
            return await operationFunc(cancellationToken).ConfigureAwait(false);
        }
    }
}