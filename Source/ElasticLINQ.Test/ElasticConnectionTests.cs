// Licensed under the Apache 2.0 License. See LICENSE.txt in the project root for more information.

using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using ElasticLinq.Logging;
using ElasticLinq.Mapping;
using ElasticLinq.Request;
using ElasticLinq.Request.Criteria;
using ElasticLinq.Request.Formatters;
using ElasticLinq.Retry;
using ElasticLinq.Test.Utility;
using Xunit;

namespace ElasticLinq.Test
{
    public class ElasticConnectionTests
    {
        static readonly ElasticConnection defaultConnection = new ElasticConnection(new Uri("http://a.b.com:9000/"));
        static readonly ICriteria criteria = new ExistsCriteria("greenField");

        static readonly IElasticMapping mapping = new TrivialElasticMapping();
        static readonly ILog log = NullLog.Instance;
        static readonly CancellationToken token = new CancellationToken();

        readonly Uri endpoint = new Uri("http://localhost:1234/abc");
        const string Password = "thePassword";
        const string UserName = "theUser";

        [Fact]
        public static void GuardClauses_Constructor()
        {
            Assert.Throws<ArgumentNullException>(() => new ElasticConnection(null));
            Assert.Throws<ArgumentException>(() => new ElasticConnection(new Uri("http://localhost/"), index: ""));
        }

        [Fact]
        public void GuardClauses_Timeout()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => new ElasticConnection(endpoint, timeout: TimeSpan.FromDays(-1)));
        }

        [Fact]
        public void ConstructorWithOneArgSetsPropertyFromParameter()
        {
            var connection = new ElasticConnection(endpoint);

            Assert.Equal(endpoint, connection.Endpoint);
        }

        [Fact]
        public void ConstructorWithThreeArgsSetsPropertiesFromParameters()
        {
            var connection = new ElasticConnection(endpoint, UserName, Password);

            Assert.Equal(endpoint, connection.Endpoint);
        }

        [Fact]
        public void ConstructorWithAllArgsSetsPropertiesFromParameters()
        {
            var expectedEndpoint = new Uri("http://coruscant.gov");
            var expectedTimeout = TimeSpan.FromSeconds(1234);
            const string expectedIndex = "h2g2";
            var expectedOptions = new ElasticConnectionOptions { Pretty = true };

            var actual = new ElasticConnection(expectedEndpoint, UserName, Password, expectedTimeout, expectedIndex, expectedOptions);

            Assert.Equal(expectedEndpoint, actual.Endpoint);
            Assert.Equal(expectedTimeout, actual.Timeout);
            Assert.Equal(expectedIndex, actual.Index);
            Assert.Equal(expectedOptions, actual.Options);
        }

        [Fact]
        public void ConstructorCreatesHttpClientWithDefaultTimeout()
        {
            var defaultTimeout = TimeSpan.FromSeconds(10);
            var connection = new ElasticConnection(endpoint, UserName, Password);

            Assert.NotNull(connection.HttpClient);
            Assert.Equal(defaultTimeout, connection.Timeout);
        }

        [Fact]
        public void ConstructorCreatesHttpClientWithSpecifiedTimeout()
        {
            var timeout = TimeSpan.FromSeconds(3);
            var connection = new ElasticConnection(endpoint, UserName, Password, timeout);

            Assert.NotNull(connection.HttpClient);
            Assert.Equal(timeout, connection.Timeout);
        }

        [Fact]
        public void ConstructorCreatesDefaultOptions()
        {
            var actual = new ElasticConnection(endpoint);

            Assert.NotNull(actual.Options);
        }

        [Fact]
        public async Task DisposeKillsHttpClient()
        {
            var connection = new ElasticConnection(endpoint, UserName, Password);

            connection.Dispose();

            await Assert.ThrowsAsync<NullReferenceException>(() => connection.HttpClient.GetAsync(new Uri("http://something.com"))).ConfigureAwait(false);
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2202:Do not dispose objects multiple times")]
        [Fact]
        public void DoubleDisposeDoesNotThrow()
        {
            var connection = new ElasticConnection(endpoint, UserName, Password);

            connection.Dispose();
            connection.Dispose();
        }

        [Fact]
        public async Task SubclassDisposeKillsHttpClientAndCallsOwnDispose()
        {
            var connection = new MyConnection(endpoint, UserName, Password);

            Assert.Null(connection.Disposing);
            connection.Dispose();

            await Assert.ThrowsAsync<NullReferenceException>(() => connection.HttpClient.GetAsync(new Uri("http://something.com"))).ConfigureAwait(false);
            Assert.Equal(connection.Disposing, true);
        }

        [Fact]
        public static async Task NoAuthorizationWithEmptyUserName()
        {
            var messageHandler = new SpyMessageHandler();
            var localConnection = new ElasticConnection(messageHandler, new Uri("http://localhost"));
            var request = new SearchRequest { DocumentType = "docType" };
            var formatter = new SearchRequestFormatter(localConnection, mapping, request);

            await localConnection.SearchAsync(
                formatter.Body,
                request,
                token,
                log).ConfigureAwait(false);

            Assert.Null(messageHandler.Request.Headers.Authorization);
        }

        [Fact]
        public static async Task ForcesBasicAuthorizationWhenProvidedWithUsernameAndPassword()
        {
            var messageHandler = new SpyMessageHandler();
            var localConnection = new ElasticConnection(messageHandler, new Uri("http://localhost"), "myUser", "myPass");
            var request = new SearchRequest { DocumentType = "docType" };
            var formatter = new SearchRequestFormatter(localConnection, mapping, request);

            await localConnection.SearchAsync(
                formatter.Body,
                request,
                token,
                log).ConfigureAwait(false);

            var auth = messageHandler.Request.Headers.Authorization;
            Assert.NotNull(auth);
            Assert.Equal("Basic", auth.Scheme);
            Assert.Equal("myUser:myPass", Encoding.ASCII.GetString(Convert.FromBase64String(auth.Parameter)));
        }

        [Fact]
        public static async Task NonSuccessfulHttpRequestThrows()
        {
            var messageHandler = new SpyMessageHandler();
            messageHandler.Response.StatusCode = HttpStatusCode.NotFound;
            var localConnection = new ElasticConnection(messageHandler, new Uri("http://localhost"), "myUser", "myPass");
            var request = new SearchRequest { DocumentType = "docType" };
            var formatter = new SearchRequestFormatter(localConnection, mapping, request);

            var ex = await Record.ExceptionAsync(() => localConnection.SearchAsync(
                formatter.Body,
                request,
                token,
                log)).ConfigureAwait(false);

            Assert.IsType<HttpRequestException>(ex);
            Assert.Equal("Response status code does not indicate success: 404 (Not Found).", ex.Message);
        }

        [Fact]
        public static async Task LogsDebugMessagesDuringExecution()
        {
            var responseString = BuildResponseString(2, 1, 1, 0.3141, "testIndex", "testType", "testId");
            var messageHandler = new SpyMessageHandler();
            var spyLog = new SpyLog();
            messageHandler.Response.Content = new StringContent(responseString);
            var localConnection = new ElasticConnection(messageHandler, new Uri("http://localhost"), "myUser", "myPass", index: "SearchIndex");
            var request = new SearchRequest { DocumentType = "abc123", Size = 2112 };
            var formatter = new SearchRequestFormatter(localConnection, mapping, request);

            await localConnection.SearchAsync(
                formatter.Body,
                request,
                token,
                spyLog).ConfigureAwait(false);

            Assert.Equal(4, spyLog.Entries.Count);
            Assert.Equal(@"Request: POST http://localhost/SearchIndex/abc123/_search", spyLog.Entries[0].Message);
            Assert.Equal(@"Body:" + '\n' + @"{""size"":2112,""timeout"":""10s""}", spyLog.Entries[1].Message);
            Assert.True(new Regex(@"Response: 200 OK \(in \d+ms\)").Match(spyLog.Entries[2].Message).Success);
            Assert.True(new Regex(@"Deserialized \d+ bytes into 1 hits in \d+ms").Match(spyLog.Entries[3].Message).Success);
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
                var response = ElasticConnection.ParseResponse(stream, log);
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

        [Theory]
        [InlineData(null, null, "http://a.b.com:9000/_all/_search")]
        [InlineData("index1,index2", null, "http://a.b.com:9000/index1,index2/_search")]
        [InlineData(null, "docType1,docType2", "http://a.b.com:9000/_all/docType1,docType2/_search")]
        [InlineData("index1,index2", "docType1,docType2", "http://a.b.com:9000/index1,index2/docType1,docType2/_search")]
        public void UriFormatting(string index, string documentType, string expectedUri)
        {
            var connection = new ElasticConnection(new Uri("http://a.b.com:9000/"), index: index);

            Assert.Equal(expectedUri, connection.GetSearchUri(new SearchRequest { DocumentType = documentType }).ToString());
        }

        [Fact]
        public void PrettySetsUriQueryWhenNoOtherQueryUriParameters()
        {
            var connection = new ElasticConnection(new Uri("http://coruscant.gov/some"), options: new ElasticConnectionOptions { Pretty = true });
            var prettyUri = connection.GetSearchUri(new SearchRequest { DocumentType = "type1", Filter = criteria });

            Assert.Equal("pretty=true", prettyUri.GetComponents(UriComponents.Query, UriFormat.Unescaped));
        }

        [Fact]
        public void PrettyAppendsUriQueryParameterWhenOtherQueryUriParameters()
        {
            var connection = new ElasticConnection(new Uri("http://coruscant.gov/some?human=false"),
                options: new ElasticConnectionOptions { Pretty = true });
            var prettyUri = connection.GetSearchUri(new SearchRequest { DocumentType = "type1", Filter = criteria });

            var parameters = prettyUri.GetComponents(UriComponents.Query, UriFormat.Unescaped).Split('&');
            Assert.Equal(2, parameters.Length);
            Assert.Contains("human=false", parameters);
            Assert.Contains("pretty=true", parameters);
        }


        [Fact]
        public void PrettyChangesUriQueryParameterWhenDifferentValueAlreadyExists()
        {
            var connection = new ElasticConnection(new Uri("http://coruscant.gov/some?pretty=false&human=true"),
                options: new ElasticConnectionOptions { Pretty = true });
            var prettyUri = connection.GetSearchUri(new SearchRequest { DocumentType = "type1", Filter = criteria });

            var parameters = prettyUri.GetComponents(UriComponents.Query, UriFormat.Unescaped).Split('&');
            Assert.Equal(2, parameters.Length);
            Assert.Contains("human=true", parameters);
            Assert.Contains("pretty=true", parameters);
        }

        [Fact]
        public static async Task SearchAsyncThrowsTaskCancelledExceptionWithAlreadyCancelledCancellationToken()
        {
            var spyLog = new SpyLog();
            var localConnection = new ElasticConnection(new Uri("http://localhost"), index: "SearchIndex");
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

        public class MyConnection : ElasticConnection
        {
            internal bool? Disposing;

            public MyConnection(Uri endpoint, string userName = null, string password = null, TimeSpan? timeout = null, string index = null, ElasticConnectionOptions options = null)
                : base(endpoint, userName, password, timeout, index, options)
            {
            }

            protected override void Dispose(bool disposing)
            {
                Disposing = disposing;
                base.Dispose(disposing);
            }
        }
    }
}