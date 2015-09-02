// Licensed under the Apache 2.0 License. See LICENSE.txt in the project root for more information.

using ElasticLinq.Logging;
using ElasticLinq.Mapping;
using ElasticLinq.Request;
using ElasticLinq.Retry;
using System;
using System.Threading.Tasks;
using NSubstitute;
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
	    public static async Task ShouldCallElasticSearchClient()
	    {
			var log = new SpyLog();

		    var connection = Substitute.For<IElasticConnection>();

			connection.Index.Returns("SearchIndex");
			connection.Options.Returns(new ElasticConnectionOptions());
			connection.Timeout.Returns(TimeSpan.FromSeconds(10));

			var request = new SearchRequest { DocumentType = "abc123", Size = 2112 };

			var processor = new ElasticRequestProcessor(connection, mapping, log, retryPolicy);
			
			await processor.SearchAsync(request);

			connection.Received(1).Search(
			   @"{""size"":2112,""timeout"":""10s""}",
			   request,
			   log
			   );
	    }
    }
}