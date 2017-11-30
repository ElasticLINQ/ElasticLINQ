// Licensed under the Apache 2.0 License. See LICENSE.txt in the project root for more information.

using ElasticLinq.Logging;
using ElasticLinq.Mapping;
using ElasticLinq.Retry;
using ElasticLinq.Test.TestSupport;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Xunit;

namespace ElasticLinq.Test.Integration
{
    public class BasicConnectionTests
    {
        static readonly IElasticMapping mapping = new TrivialElasticMapping();
        static readonly ILog log = NullLog.Instance;
        static readonly IRetryPolicy retryPolicy = NullRetryPolicy.Instance;

        class Robot
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public decimal Cost { get; set; }
        }

        [Fact]
        public async Task AcceptEncodingIsGzip()
        {
            using(var httpStub = new HttpStub(ZeroHits, 1))
            {
                var context = MakeElasticContext(httpStub.Uri);

                var dummy = context.Query<Robot>().FirstOrDefault();
                Assert.Null(dummy);

                await httpStub.Completion.ConfigureAwait(false);
                var request = httpStub.Requests.Single();
                Assert.Contains("Accept-Encoding", request.Headers.AllKeys);
                Assert.Equal("gzip", request.Headers["Accept-Encoding"]);
            }            
        }

        [Fact]
        public async Task QueryEvaluationCausesPostToConnectionEndpoint()
        {
            using (var httpStub = new HttpStub(ZeroHits, 1))
            {
                var context = MakeElasticContext(httpStub.Uri);

                var dummy = context.Query<Robot>().FirstOrDefault();
                Assert.Null(dummy);

                await httpStub.Completion.ConfigureAwait(false);
                var request = httpStub.Requests.Single();
                Assert.Equal("POST", request.HttpMethod);
                Assert.Equal("/_all/robots/_search", request.RawUrl);
            }
        }

        [Fact]
        public async Task QueryEvaluationWithNoNullResponseThrowsInvalidOperationException()
        {
            using (var httpStub = new HttpStub(c => { }, 1))
            {
                var context = MakeElasticContext(httpStub.Uri);

                Assert.Throws<InvalidOperationException>(() => context.Query<Robot>().FirstOrDefault());

                await httpStub.Completion.ConfigureAwait(false);
                var request = httpStub.Requests.Single();
                Assert.Equal("POST", request.HttpMethod);
                Assert.Equal("/_all/robots/_search", request.RawUrl);
            }
        }

        [Fact]
        public async Task ProviderExecuteCausesPostToConnectionEndpoint()
        {
            using (var httpStub = new HttpStub(ZeroHits, 1))
            {
                var provider = new ElasticQueryProvider(new ElasticConnection(httpStub.Uri), mapping, log, retryPolicy);
                var query = new ElasticQuery<Robot>(provider);

                provider.Execute(query.Expression);

                await httpStub.Completion.ConfigureAwait(false);
                var request = httpStub.Requests.Single();
                Assert.Equal("POST", request.HttpMethod);
                Assert.Equal("/_all/robots/_search", request.RawUrl);
            }
        }

        [Fact]
        public async Task ProviderExecuteTCausesPostToConnectionEndpoint()
        {
            using (var httpStub = new HttpStub(ZeroHits, 1))
            {
                var provider = new ElasticQueryProvider(new ElasticConnection(httpStub.Uri), mapping, log, retryPolicy);
                var query = new ElasticQuery<Robot>(provider);

                provider.Execute<IEnumerable<Robot>>(query.Expression);

                await httpStub.Completion.ConfigureAwait(false);
                var request = httpStub.Requests.Single();
                Assert.Equal("POST", request.HttpMethod);
                Assert.Equal("/_all/robots/_search", request.RawUrl);
            }
        }

        [Fact]
        public async Task QueryImplicitGetEnumeratorCausesConnection()
        {
            using (var httpStub = new HttpStub(ZeroHits, 1))
            {
                var provider = new ElasticQueryProvider(new ElasticConnection(httpStub.Uri), mapping, log, retryPolicy);
                var query = new ElasticQuery<Robot>(provider);

                var enumerator = query.GetEnumerator();

                await httpStub.Completion.ConfigureAwait(false);
                var request = httpStub.Requests.Single();
                Assert.Equal("POST", request.HttpMethod);
                Assert.Equal("/_all/robots/_search", request.RawUrl);
                Assert.NotNull(enumerator);
            }
        }

        [Fact]
        public async Task QueryExplicitIEnumerableGetEnumeratorCausesConnection()
        {
            using (var httpStub = new HttpStub(ZeroHits, 1))
            {
                var provider = new ElasticQueryProvider(new ElasticConnection(httpStub.Uri), mapping, log, retryPolicy);
                var query = new ElasticQuery<Robot>(provider);

                var enumerator = ((IEnumerable)query).GetEnumerator();

                await httpStub.Completion.ConfigureAwait(false);
                var request = httpStub.Requests.Single();
                Assert.Equal("POST", request.HttpMethod);
                Assert.Equal("/_all/robots/_search", request.RawUrl);
                Assert.NotNull(enumerator);
            }
        }

        [Fact]
        public void ConstantFalseDoesNotCauseConnection()
        {
            using (var httpStub = new HttpStub(ZeroHits, 1))
            {
                var provider = new ElasticQueryProvider(new ElasticConnection(httpStub.Uri), mapping, log, retryPolicy);
                var query = new ElasticQuery<Robot>(provider).Where(f => false);

                ((IEnumerable)query).GetEnumerator();

                Assert.Empty(httpStub.Requests);
            }
        }

        static ElasticContext MakeElasticContext(Uri uri)
        {
            return new ElasticContext(new ElasticConnection(uri), mapping, log, retryPolicy);
        }

        static void ZeroHits(HttpListenerContext context)
        {
            var response = new
            {
                took = 1,
                timed_out = false,
                _shards = new { total = 5, successful = 5, failed = 0 },
                hits = new { total = 0, max_score = (string)null, hits = new object[0] }
            };

            context.Response.Write(JObject.FromObject(response).ToString(Formatting.None));
        }
    }
}