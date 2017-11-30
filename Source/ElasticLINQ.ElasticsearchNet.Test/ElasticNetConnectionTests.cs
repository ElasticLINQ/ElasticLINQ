using System;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using ElasticLinq.ElasticsearchNet.Test.TestSupport;
using ElasticLinq.Logging;
using ElasticLinq.Mapping;
using ElasticLinq.Request;
using ElasticLinq.Request.Formatters;
using Elasticsearch.Net;
using Elasticsearch.Net.Connection;
using NSubstitute;
using Xunit;

namespace ElasticLinq.ElasticsearchNet.Test
{
    public class ElasticNetConnectionTests
    {
        static readonly IElasticMapping mapping = new TrivialElasticMapping();
        static readonly ILog log = NullLog.Instance;

        [Fact]
        public static void GuardClauses_Constructor()
        {
            Assert.Throws<ArgumentNullException>(() => new ElasticNetConnection(null));
        }

        [Fact]
        public void GuardClauses_Timeout()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => new ElasticNetConnection(Substitute.For<IElasticsearchClient>(), timeout: TimeSpan.FromDays(-1)));
        }

        [Fact]
        public void ConstructorWithAllArgsSetsPropertiesFromParameters()
        {
            var expectedTimeout = TimeSpan.FromSeconds(1234);
            const string expectedIndex = "h2g2";
            var expectedOptions = new ElasticConnectionOptions { Pretty = true };

            var actual = new ElasticNetConnection(Substitute.For<IElasticsearchClient>(), expectedIndex, expectedTimeout, expectedOptions);

            Assert.Equal(expectedTimeout, actual.Timeout);
            Assert.Equal(expectedIndex, actual.Index);
            Assert.Equal(expectedOptions, actual.Options);
        }

        [Fact]
        public void ConstructorCreatesDefaultOptions()
        {
            var actual = new ElasticNetConnection(Substitute.For<IElasticsearchClient>());

            Assert.NotNull(actual.Options);
        }

        [Fact]
        public static async Task NonSuccessfulHttpRequestThrows()
        {
            var client = Substitute.For<IElasticsearchClient>();

            client.SearchAsync<string>(
                    "_all",
                    "docType",
                    @"{""timeout"":""10s""}",
                    Arg.Any<Func<SearchRequestParameters, SearchRequestParameters>>())
                .Returns(Task.FromResult(ElasticsearchResponse<string>.Create(
                    new ConnectionConfiguration(),
                    404,
                    "_search",
                    "_all",
                    new byte[0])));

            var localConnection = new ElasticNetConnection(client);
            var request = new SearchRequest { DocumentType = "docType" };
            var formatter = new SearchRequestFormatter(localConnection, mapping, request);

            var ex = await Record.ExceptionAsync(() => localConnection.SearchAsync(
                formatter.Body,
                request,
                CancellationToken.None,
                log)).ConfigureAwait(false);

            Assert.IsType<HttpRequestException>(ex);
            Assert.Equal("Response status code does not indicate success: 404", ex.Message);
        }

        [Fact]
        public static async Task LogsDebugMessagesDuringExecution()
        {
            var client = Substitute.For<IElasticsearchClient>();

            var responseString = BuildResponseString(2, 1, 1, 0.3141, "testIndex", "testType", "testId");
            var spyLog = new SpyLog();

            client.SearchAsync<string>(
                    "SearchIndex",
                    "abc123",
                    @"{""size"":2112,""timeout"":""10s""}",
                    Arg.Any<Func<SearchRequestParameters, SearchRequestParameters>>())
                .Returns(Task.FromResult(ElasticsearchResponse<string>.Create(
                    new ConnectionConfiguration(),
                    200,
                    "_search",
                    "http://localhost/SearchIndex/abc123/_search",
                    new byte[0],
                    responseString)));

            var localConnection = new ElasticNetConnection(client, index: "SearchIndex");
            var request = new SearchRequest { DocumentType = "abc123", Size = 2112 };
            var formatter = new SearchRequestFormatter(localConnection, mapping, request);

            await localConnection.SearchAsync(
                formatter.Body,
                request,
                CancellationToken.None,
                spyLog).ConfigureAwait(false);

            Assert.Equal(4, spyLog.Entries.Count);
            Assert.Equal(@"Request: POST http://localhost/SearchIndex/abc123/_search", spyLog.Entries[0].Message);
            Assert.Equal(@"Body:" + '\n' + @"{""size"":2112,""timeout"":""10s""}", spyLog.Entries[1].Message);
            Assert.True(new Regex(@"Response: 200 OK \(in \d+ms\)").Match(spyLog.Entries[2].Message).Success);
            Assert.True(new Regex(@"Deserialized \d+ characters into 1 hits in \d+ms").Match(spyLog.Entries[3].Message).Success);
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

            var response = ElasticNetConnection.ParseResponse(responseString, log);
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

        [Fact]
        public static async Task SearchAsyncThrowsTaskCancelledExceptionWithAlreadyCancelledCancellationToken()
        {
            var client = Substitute.For<IElasticsearchClient>();

            var spyLog = new SpyLog();
            var localConnection = new ElasticNetConnection(client, "SearchIndex");
            var request = new SearchRequest { DocumentType = "docType" };
            var formatter = new SearchRequestFormatter(localConnection, mapping, request);

            var ex = await Record.ExceptionAsync(() => localConnection.SearchAsync(
                formatter.Body,
                request,
                new CancellationToken(true),
                spyLog)).ConfigureAwait(false);

            Assert.IsType<TaskCanceledException>(ex);
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