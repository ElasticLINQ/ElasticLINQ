// Licensed under the Apache 2.0 License. See LICENSE.txt in the project root for more information.

using System;
using Xunit;

namespace ElasticLinq.Test
{
    public class ElasticConnectionTests
    {
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
            Assert.Equal(UserName, connection.UserName);
            Assert.Equal(Password, connection.Password);
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
    }
}