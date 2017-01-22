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
        public static readonly Uri v1Endpoint = new Uri("http://integration.elasticlinq.net:9200");

        const string Index = "integrationtest";
        static readonly ElasticConnectionOptions options = new ElasticConnectionOptions { SearchSizeDefault = 1000 };
        static readonly ElasticConnection connection = new ElasticConnection(v1Endpoint, index: Index, options: options);

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
                    string.Format("Tests expect {0} entries but {1} loaded from Elasticsearch index '{2}' at {3}",
                        expectedDataCount, memory.Count,
                        elasticContext.Connection.Index, ((ElasticConnection)elasticContext.Connection).Endpoint));
        }
    }

    public class NoRetryPolicy : IRetryPolicy
    {
        public async Task<TResult> ExecuteAsync<TResult>(Func<CancellationToken, Task<TResult>> operationFunc, Func<TResult, Exception, Boolean> shouldRetryFunc, Action<TResult, Dictionary<String, Object>> appendLogInfoFunc = null,
            CancellationToken cancellationToken = new CancellationToken())
        {
            return await operationFunc(cancellationToken);
        }
    }
}