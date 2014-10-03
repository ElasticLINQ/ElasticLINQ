// Licensed under the Apache 2.0 License. See LICENSE.txt in the project root for more information.

using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using Xunit;

namespace ElasticLinq.Test
{
    public class ElasticConnectionTests
    {
        private readonly Uri endpoint = new Uri("http://localhost:1234/abc");
        private const string Password = "thePassword";
        private const string UserName = "theUser";

        [Fact]
        [ExcludeFromCodeCoverage]
        public static void GuardClauses_Constructor()
        {
            Assert.Throws<ArgumentNullException>(() => new ElasticConnection(null));
            Assert.Throws<ArgumentException>(() => new ElasticConnection(new Uri("http://localhost/"), index: ""));
        }

        [Fact]
        [ExcludeFromCodeCoverage]
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
        [ExcludeFromCodeCoverage]
        public async Task DisposeKillsHttpClient()
        {
            var connection = new ElasticConnection(endpoint, UserName, Password);

            connection.Dispose();

            await Assert.ThrowsAsync<ObjectDisposedException>(() => connection.HttpClient.GetAsync(new Uri("http://something.com")));
        }
    }
}