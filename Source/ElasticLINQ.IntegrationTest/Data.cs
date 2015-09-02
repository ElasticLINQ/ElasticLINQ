using System;
using System.Collections.Generic;
using System.Linq;
using ElasticLinq;
using ElasticLinq.Mapping;
using ElasticLINQ.IntegrationTest.Models;

namespace ElasticLINQ.IntegrationTest
{
    class Data
    {
        private const string Index = "integrationtest";
        private static readonly Uri elasticsearchEndpoint = new Uri("http://elasticlinq.cloudapp.net:9200");
        private static readonly ElasticConnectionOptions options = new ElasticConnectionOptions { SearchSizeDefault = 1000 };
        private static readonly ElasticConnection connection = new ElasticConnection(elasticsearchEndpoint, index:Index, options:options);

        private readonly ElasticContext elasticContext = new ElasticContext(connection, new TrivialElasticMapping());
        private readonly List<object> memory = new List<object>();

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
                    String.Format("Tests expect {0} entries but {1} loaded from Elasticsearch index '{2}' at {3}",
                        expectedDataCount, memory.Count,
						elasticContext.Connection.Index, ((ElasticConnection)elasticContext.Connection).Endpoint));
        }
    }
}