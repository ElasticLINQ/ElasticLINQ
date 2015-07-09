// Licensed under the Apache 2.0 License. See LICENSE.txt in the project root for more information.

using System;
using System.Threading.Tasks;
using Xunit;

namespace ElasticLinq.Test
{
    public class ElasticConnectionTests
    {
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