// Licensed under the Apache 2.0 License. See LICENSE.txt in the project root for more information.

using ElasticLinq.Logging;
using ElasticLinq.Mapping;
using ElasticLinq.Request;
using ElasticLinq.Request.Formatters;
using ElasticLinq.Retry;
using System;
using System.Threading;
using System.Threading.Tasks;
using NSubstitute;
using Xunit;

namespace ElasticLinq.Test.Request
{
    public class ElasticRequestProcessorTests
    {
        static readonly ElasticConnection connection = new ElasticConnection(new Uri("http://localhost:9912"), index: "SearchIndex");
        static readonly IElasticMapping mapping = new TrivialElasticMapping();
        static readonly ILog log = NullLog.Instance;
        static readonly IRetryPolicy retryPolicy = NullRetryPolicy.Instance;

        [Fact]
        public static void Constructor_GuardClauses()
        {
            Assert.Throws<ArgumentNullException>(() => new ElasticRequestProcessor(null, mapping, log, retryPolicy));
            Assert.Throws<ArgumentNullException>(() => new ElasticRequestProcessor(connection, null, log, retryPolicy));
            Assert.Throws<ArgumentNullException>(() => new ElasticRequestProcessor(connection, mapping, null, retryPolicy));
            Assert.Throws<ArgumentNullException>(() => new ElasticRequestProcessor(connection, mapping, log, null));
        }

        [Fact(Skip = "NSubstitute returns null on recieved async until 1.8.3 (unreleased)")]
        public static async Task ShouldCallElasticSearchClient()
        {
            var spyLog = new SpyLog();

            var mockConnection = Substitute.For<IElasticConnection>();

            mockConnection.Index.Returns("SearchIndex");
            mockConnection.Options.Returns(new ElasticConnectionOptions());
            mockConnection.Timeout.Returns(TimeSpan.FromSeconds(10));

            var request = new SearchRequest { DocumentType = "abc123", Size = 2112 };
            var token = new CancellationToken();

            var processor = new ElasticRequestProcessor(mockConnection, mapping, spyLog, retryPolicy);

            await processor.SearchAsync(request, token);

            await mockConnection.Received(1).SearchAsync(
               @"{""size"":2112,""timeout"":""10s""}",
               request,
               token,
               spyLog
               );
        }

        [Fact]
        public static async void SearchAsyncThrowsTaskCancelledExceptionWithSubsequentlyCancelledCancellationToken()
        {
            var request = new SearchRequest { DocumentType = "docType" };
            var processor = new ElasticRequestProcessor(connection, mapping, log, retryPolicy);

            var ex = await Record.ExceptionAsync(() => processor.SearchAsync(request, new CancellationTokenSource(500).Token));

            Assert.IsType<TaskCanceledException>(ex);
        }

        [Fact]
        public static async void SearchAsyncCapturesRequestInfoOnFailure()
        {
            var spyLog = new SpyLog();
            var brokenConnection = new ElasticConnection(new Uri("http://localhost:12"), index: "MyIndex");
            var processor = new ElasticRequestProcessor(brokenConnection, mapping, spyLog, new RetryPolicy(spyLog, 100, 1));
            var searchRequest = new SearchRequest { DocumentType = "docType" };
            var formatter = new SearchRequestFormatter(brokenConnection, mapping, searchRequest);

            var ex = await Record.ExceptionAsync(() => processor.SearchAsync(searchRequest, CancellationToken.None));

            Assert.IsType<RetryFailedException>(ex);
            var retryLogEntry = Assert.Single(spyLog.Entries, s => s.AdditionalInfo.ContainsKey("category") && s.AdditionalInfo["category"].Equals("retry"));
            Assert.Equal("MyIndex", retryLogEntry.AdditionalInfo["index"]);
            Assert.Equal(brokenConnection.GetSearchUri(searchRequest), retryLogEntry.AdditionalInfo["uri"]);
            Assert.Equal(formatter.Body, retryLogEntry.AdditionalInfo["query"]);
        }
    }
}