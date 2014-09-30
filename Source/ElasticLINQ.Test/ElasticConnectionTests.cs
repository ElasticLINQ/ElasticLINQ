// Licensed under the Apache 2.0 License. See LICENSE.txt in the project root for more information.

using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using ElasticLinq.Connection;
using ElasticLinq.Logging;
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
            Assert.Throws<ArgumentNullException>(() => new HttpElasticConnection(null));
            Assert.Throws<ArgumentException>(() => new HttpElasticConnection(new Uri("http://localhost/"), index: ""));
        }

        [Fact]
        [ExcludeFromCodeCoverage]
        public void GuardClauses_Timeout()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => new HttpElasticConnection(endpoint, timeout: TimeSpan.FromDays(-1)));
        }

        [Fact]
        public void ConstructorWithOneArgSetsPropertyFromParameter()
        {
            var connection = new HttpElasticConnection(endpoint);

            Assert.Equal(endpoint, connection.Endpoint);
        }

        [Fact]
        public void ConstructorWithThreeArgsSetsPropertiesFromParameters()
        {
            var connection = new HttpElasticConnection(endpoint, UserName, Password);

            Assert.Equal(endpoint, connection.Endpoint);
        }

        [Fact]
        public void ConstructorCreatesHttpClientWithDefaultTimeout()
        {
            var defaultTimeout = TimeSpan.FromSeconds(10);
            var connection = new HttpElasticConnection(endpoint, UserName, Password);

            Assert.Equal(defaultTimeout, connection.Timeout);
        }

        [Fact]
        public void ConstructorCreatesHttpClientWithSpecifiedTimeout()
        {
            var timeout = TimeSpan.FromSeconds(3);
            var connection = new HttpElasticConnection(endpoint, UserName, Password, timeout);

            Assert.Equal(timeout, connection.Timeout);
        }

        [Fact]
        [ExcludeFromCodeCoverage]
        public async Task DisposeKillsHttpClient()
        {
            var connection = new HttpElasticConnection(endpoint, UserName, Password);
            
            connection.Dispose();

            await Assert.ThrowsAsync<ObjectDisposedException>(() => connection.Get<object>(new Uri("http://something.com"), NullLog.Instance));
        }
    }
}