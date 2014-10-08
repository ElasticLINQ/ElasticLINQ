// Licensed under the Apache 2.0 License. See LICENSE.txt in the project root for more information.

using ElasticLinq.Logging;
using ElasticLinq.Mapping;
using ElasticLinq.Request;
using ElasticLinq.Retry;
using ElasticLinq.Test.Utility;
using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Xunit;

namespace ElasticLinq.Test.Request
{
    public class ElasticRequestProcessorTests
    {
        private static readonly ElasticConnection connection = new ElasticConnection(new Uri("http://localhost"), index: "SearchIndex");
        private static readonly IElasticMapping mapping = new TrivialElasticMapping();
        private static readonly ILog log = NullLog.Instance;
        private static readonly IRetryPolicy retryPolicy = NullRetryPolicy.Instance;

        [Fact]
        public static void Constructor_GuardClauses()
        {
            Assert.Throws<ArgumentNullException>(() => new ElasticRequestProcessor(null, mapping, log, retryPolicy));
            Assert.Throws<ArgumentNullException>(() => new ElasticRequestProcessor(connection, null, log, retryPolicy));
            Assert.Throws<ArgumentNullException>(() => new ElasticRequestProcessor(connection, mapping, null, retryPolicy));
            Assert.Throws<ArgumentNullException>(() => new ElasticRequestProcessor(connection, mapping, log, null));
        }

        [Fact]
        public static async Task NoAuthorizationWithEmptyUserName()
        {
            var messageHandler = new SpyMessageHandler();
            var localConnection = new ElasticConnection(messageHandler, new Uri("http://localhost"));
            var processor = new ElasticRequestProcessor(localConnection, mapping, log, retryPolicy);
            var request = new SearchRequest { DocumentType = "docType" };

            await processor.SearchAsync(request);

            Assert.Null(messageHandler.Request.Headers.Authorization);
        }

        [Fact]
        public static async Task ForcesBasicAuthorizationWhenProvidedWithUsernameAndPassword()
        {
            var messageHandler = new SpyMessageHandler();
            var localConnection = new ElasticConnection(messageHandler, new Uri("http://localhost"), "myUser", "myPass");
            var processor = new ElasticRequestProcessor(localConnection, mapping, log, retryPolicy);
            var request = new SearchRequest { DocumentType = "docType" };

            await processor.SearchAsync(request);

            var auth = messageHandler.Request.Headers.Authorization;
            Assert.NotNull(auth);
            Assert.Equal("Basic", auth.Scheme);
            Assert.Equal("myUser:myPass", Encoding.ASCII.GetString(Convert.FromBase64String(auth.Parameter)));
        }

        [Fact]
        public static void NonSuccessfulHttpRequestThrows()
        {
            var messageHandler = new SpyMessageHandler();
            messageHandler.Response.StatusCode = HttpStatusCode.NotFound;
            var localConnection = new ElasticConnection(messageHandler, new Uri("http://localhost"), "myUser", "myPass");
            var processor = new ElasticRequestProcessor(localConnection, mapping, log, retryPolicy);
            var request = new SearchRequest { DocumentType = "docType" };

            var ex = Record.Exception(() => processor.SearchAsync(request).GetAwaiter().GetResult());

            Assert.IsType<HttpRequestException>(ex);
            Assert.Equal("Response status code does not indicate success: 404 (Not Found).", ex.Message);
        }

        [Fact]
        public static void ParseResponseReturnsParsedResponseGivenValidStream()
        {
            const int took = 2;
            const int shards = 1;
            const int hits = 1;
            const double score = 0.3141;
            const string index = "testIndex";
            const string type = "testType";
            const string id = "testId";

            var responseString = BuildResponseString(took, shards, hits, score, index, type, id);

            using (var stream = new MemoryStream(Encoding.UTF8.GetBytes(responseString)))
            {
                var response = ElasticRequestProcessor.ParseResponse(stream, log);
                Assert.NotNull(response);
                Assert.Equal(took, response.took);
                Assert.Equal(hits, response.hits.total);
                Assert.Equal(score, response.hits.max_score);

                Assert.NotEmpty(response.hits.hits);
                Assert.Equal(score, response.hits.hits[0]._score);
                Assert.Equal(index, response.hits.hits[0]._index);
                Assert.Equal(type, response.hits.hits[0]._type);
                Assert.Equal(id, response.hits.hits[0]._id);
            }
        }

        [Fact]
        public static async Task LogsDebugMessagesDuringExecution()
        {
            var responseString = BuildResponseString(2, 1, 1, 0.3141, "testIndex", "testType", "testId");
            var messageHandler = new SpyMessageHandler();
            var log = new SpyLog();
            messageHandler.Response.Content = new StringContent(responseString);
            var localConnection = new ElasticConnection(messageHandler, new Uri("http://localhost"), "myUser", "myPass", index: "SearchIndex");
            var processor = new ElasticRequestProcessor(localConnection, mapping, log, retryPolicy);
            var request = new SearchRequest { DocumentType = "abc123", Size = 2112 };

            await processor.SearchAsync(request);

            Assert.Equal(4, log.Messages.Count);
            Assert.Equal(@"[VERBOSE] Request: POST http://localhost/SearchIndex/abc123/_search", log.Messages[0]);
            Assert.Equal(@"[VERBOSE] Body:" +'\n' + @"{""size"":2112,""timeout"":""10s""}", log.Messages[1]);
            Assert.True(new Regex(@"\[VERBOSE\] Response: 200 OK \(in \d+ms\)").Match(log.Messages[2]).Success);
            Assert.True(new Regex(@"\[VERBOSE\] Deserialized \d+ bytes into 1 hits in \d+ms").Match(log.Messages[3]).Success);
        }

        private static string BuildResponseString(int took, int shards, int hits, double score, string index, string type, string id)
        {
            return "{\"took\":" + took + "," +
                                 "\"timed_out\":false," +
                                 "\"_shards\":{\"total\":" + shards + "," +
                                 "\"successful\":" + shards + "" +
                                 ",\"failed\":0}," +
                                 "\"hits\":{\"total\":" + hits + "," +
                                 "\"max_score\":" + score + ",\"hits\":" +
                                 "[{\"_index\":\"" + index + "\"," +
                                 "\"_type\":\"" + type + "\"," +
                                 "\"_id\":\"" + id + "\"," +
                                 "\"_score\":" + score + "," +
                                 "\"fields\":{\"name\":\"testField\"}}]}}";
        }
    }
}