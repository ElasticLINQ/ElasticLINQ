// Licensed under the Apache 2.0 License. See LICENSE.txt in the project root for more information.

using ElasticLinq.Mapping;
using ElasticLinq.Test.TestSupport;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using Xunit;

namespace ElasticLinq.Test.Integration
{
    public class BasicConnectionTests
    {
        private static readonly IElasticMapping mapping = new TrivialElasticMapping();

        class Robot
        {
            string Id { get; set; }
            string Name { get; set; }
        }

        [Fact]
        public void QueryEvaluationCausesPostToConnectionEndpoint()
        {
            using (var httpStub = new HttpStub(ZeroHits))
            {
                var context = MakeElasticContext(httpStub.Uri);

                var dummy = context.Query<Robot>().FirstOrDefault();

                var request = httpStub.Requests.Single();
                Assert.Equal("POST", request.HttpMethod);
                Assert.Equal("/robots/_search", request.RawUrl);
            }
        }

        [Fact]
        public void QueryEvaluationWithNoNullResponseThrowsInvalidOperationException()
        {
            using (var httpStub = new HttpStub(c => { }))
            {
                var context = MakeElasticContext(httpStub.Uri);

                Assert.Throws<InvalidOperationException>(() => context.Query<Robot>().FirstOrDefault());

                var request = httpStub.Requests.Single();
                Assert.Equal("POST", request.HttpMethod);
                Assert.Equal("/robots/_search", request.RawUrl);
            }
        }

        [Fact]
        public void ProviderExecuteCausesPostToConnectionEndpoint()
        {
            using (var httpStub = new HttpStub(ZeroHits))
            {
                var provider = new ElasticQueryProvider(new ElasticConnection(httpStub.Uri), mapping);
                var query = new ElasticQuery<Robot>(provider);

                provider.Execute(query.Expression);

                var request = httpStub.Requests.Single();
                Assert.Equal("POST", request.HttpMethod);
                Assert.Equal("/robots/_search", request.RawUrl);
            }
        }

        [Fact]
        public void ProviderExecuteTCausesPostToConnectionEndpoint()
        {
            using (var httpStub = new HttpStub(ZeroHits))
            {
                var provider = new ElasticQueryProvider(new ElasticConnection(httpStub.Uri), mapping);
                var query = new ElasticQuery<Robot>(provider);

                provider.Execute<IEnumerable<Robot>>(query.Expression);

                var request = httpStub.Requests.Single();
                Assert.Equal("POST", request.HttpMethod);
                Assert.Equal("/robots/_search", request.RawUrl);
            }
        }

        private static ElasticContext MakeElasticContext(Uri uri)
        {
            return new ElasticContext(new ElasticConnection(uri), mapping);
        }

        private static void ZeroHits(HttpListenerContext context)
        {
            context.Response.Write("{\"took\":1,\"timed_out\":false,\"_shards\":{\"total\":5,\"successful\":5,\"failed\":0}," +
                       "\"hits\":{\"total\":0,\"max_score\":null,\"hits\":[]}}");
        }
    }
}