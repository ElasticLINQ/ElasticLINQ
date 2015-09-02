// Licensed under the Apache 2.0 License. See LICENSE.txt in the project root for more information.

using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using ElasticLinq.Logging;
using ElasticLinq.Mapping;
using ElasticLinq.Request;
using ElasticLinq.Request.Formatters;
using ElasticLinq.Test.Utility;
using Xunit;

namespace ElasticLinq.Test
{
    public class ElasticConnectionTests
    {
		private static readonly IElasticMapping mapping = new TrivialElasticMapping();
		private static readonly ILog log = NullLog.Instance;
        private readonly Uri endpoint = new Uri("http://localhost:1234/abc");
        private const string Password = "thePassword";
        private const string UserName = "theUser";

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

            await Assert.ThrowsAsync<NullReferenceException>(() => connection.HttpClient.GetAsync(new Uri("http://something.com")));
        }

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

            await Assert.ThrowsAsync<NullReferenceException>(() => connection.HttpClient.GetAsync(new Uri("http://something.com")));
            Assert.Equal(connection.Disposing, true);
        }

		[Fact]
		public static async Task NoAuthorizationWithEmptyUserName()
		{
			var messageHandler = new SpyMessageHandler();
			var localConnection = new ElasticConnection(messageHandler, new Uri("http://localhost"));
			var request = new SearchRequest { DocumentType = "docType" };
			var formatter = new SearchRequestFormatter(localConnection, mapping, request);

			await localConnection.Search(
				localConnection.Index ?? "_all",
				request.DocumentType,
				formatter.Body,
				request,
				log);

			Assert.Null(messageHandler.Request.Headers.Authorization);
		}

		[Fact]
		public static async Task ForcesBasicAuthorizationWhenProvidedWithUsernameAndPassword()
		{
			var messageHandler = new SpyMessageHandler();
			var localConnection = new ElasticConnection(messageHandler, new Uri("http://localhost"), "myUser", "myPass");
			var request = new SearchRequest { DocumentType = "docType" };
			var formatter = new SearchRequestFormatter(localConnection, mapping, request);

			await localConnection.Search(
				localConnection.Index ?? "_all",
				request.DocumentType,
				formatter.Body,
				request,
				log);

			var auth = messageHandler.Request.Headers.Authorization;
			Assert.NotNull(auth);
			Assert.Equal("Basic", auth.Scheme);
			Assert.Equal("myUser:myPass", Encoding.ASCII.GetString(Convert.FromBase64String(auth.Parameter)));
		}

		[Fact]
		public static async void NonSuccessfulHttpRequestThrows()
		{
			var messageHandler = new SpyMessageHandler();
			messageHandler.Response.StatusCode = HttpStatusCode.NotFound;
			var localConnection = new ElasticConnection(messageHandler, new Uri("http://localhost"), "myUser", "myPass");
			var request = new SearchRequest { DocumentType = "docType" };
			var formatter = new SearchRequestFormatter(localConnection, mapping, request);

			var ex = await Record.ExceptionAsync(() => localConnection.Search(
				localConnection.Index ?? "_all",
				request.DocumentType,
				formatter.Body,
				request,
				log));

			Assert.IsType<HttpRequestException>(ex);
			Assert.Equal("Response status code does not indicate success: 404 (Not Found).", ex.Message);
		}

		[Fact]
		public static async Task LogsDebugMessagesDuringExecution()
		{
			var responseString = BuildResponseString(2, 1, 1, 0.3141, "testIndex", "testType", "testId");
			var messageHandler = new SpyMessageHandler();
			var log = new SpyLog();
			messageHandler.Response.Content = new StringContent(responseString);
			var localConnection = new ElasticConnection(messageHandler, new Uri("http://localhost"), "myUser", "myPass", index: "SearchIndex");
			var request = new SearchRequest { DocumentType = "abc123", Size = 2112 };
			var formatter = new SearchRequestFormatter(localConnection, mapping, request);

			await localConnection.Search(
				localConnection.Index ?? "_all",
				request.DocumentType,
				formatter.Body,
				request,
				log);

			Assert.Equal(4, log.Messages.Count);
			Assert.Equal(@"[VERBOSE] Request: POST http://localhost/SearchIndex/abc123/_search", log.Messages[0]);
			Assert.Equal(@"[VERBOSE] Body:" + '\n' + @"{""size"":2112,""timeout"":""10s""}", log.Messages[1]);
			Assert.True(new Regex(@"\[VERBOSE\] Response: 200 OK \(in \d+ms\)").Match(log.Messages[2]).Success);
			Assert.True(new Regex(@"\[VERBOSE\] Deserialized \d+ bytes into 1 hits in \d+ms").Match(log.Messages[3]).Success);
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