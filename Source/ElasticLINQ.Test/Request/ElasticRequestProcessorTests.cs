// Licensed under the Apache 2.0 License. See LICENSE.txt in the project root for more information.

using ElasticLinq.Request;
using NSubstitute;
using Xunit;
using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace ElasticLinq.Test.Request
{
    public class ElasticRequestProcessorTests
    {
        private static readonly ElasticConnection connection = new ElasticConnection(new Uri("http://localhost"));

        [Fact]
        [ExcludeFromCodeCoverage] // Expression isn't "executed"
        public void ConstructorThrowsArgumentNullExceptionWhenConnectionIsNull()
        {
            Assert.Throws<ArgumentNullException>(() => new ElasticRequestProcessor(null, StreamWriter.Null));
        }

        [Fact]
        [ExcludeFromCodeCoverage] // Expression isn't "executed"
        public void ConstructorDoesntThrowWithValidParameters()
        {
            Assert.DoesNotThrow(() => new ElasticRequestProcessor(connection, StreamWriter.Null));
        }

        [Fact]
        public static async Task NoAuthorizationWithEmptyUserName()
        {
            var log = Substitute.For<TextWriter>();
            var messageHandler = new SpyMessageHandler();
            var processor = new ElasticRequestProcessor(connection, log, messageHandler);
            var request = new ElasticSearchRequest("docType");

            await processor.SearchAsync(request);

            Assert.Null(messageHandler.Request.Headers.Authorization);
        }

        [Fact]
        public static async Task ForcesBasicAuthorizationWhenProvidedWithUsernameAndPassword()
        {
            var log = Substitute.For<TextWriter>();
            var connection = new ElasticConnection(new Uri("http://localhost"), "myUser", "myPass");
            var messageHandler = new SpyMessageHandler();
            var processor = new ElasticRequestProcessor(connection, log, messageHandler);
            var request = new ElasticSearchRequest("docType");

            await processor.SearchAsync(request);

            var auth = messageHandler.Request.Headers.Authorization;
            Assert.NotNull(auth);
            Assert.Equal("Basic", auth.Scheme);
            Assert.Equal("myUser:myPass", Encoding.ASCII.GetString(Convert.FromBase64String(auth.Parameter)));
        }

        [Fact]
        public void ParseResponseReturnsParsedResponseGivenValidStream()
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
                var response = ElasticRequestProcessor.ParseResponse(stream, StreamWriter.Null);
                Assert.NotNull(response);
                Assert.Equal(took, response.took);
                Assert.Equal(shards, response._shards.successful);
                Assert.Equal(shards, response._shards.total);
                Assert.Equal(shards, response._shards.successful);
                Assert.Equal(0, response._shards.failed);
                Assert.Equal(hits, response.hits.total);
                Assert.Equal(score, response.hits.max_score);

                Assert.NotEmpty(response.hits.hits);
                Assert.Equal(score, response.hits.hits[0]._score);
                Assert.Equal(index, response.hits.hits[0]._index);
                Assert.Equal(type, response.hits.hits[0]._type);
                Assert.Equal(id, response.hits.hits[0]._id);
            }
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