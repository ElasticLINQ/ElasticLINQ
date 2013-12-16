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
        public void GuardClauses_Constructor()
        {
            Assert.Throws<ArgumentNullException>(() => new ElasticConnection(null));
            Assert.Throws<ArgumentNullException>(() => new ElasticConnection(null, UserName, Password));
            Assert.Throws<ArgumentNullException>(() => new ElasticConnection(endpoint, null, Password));
            Assert.Throws<ArgumentNullException>(() => new ElasticConnection(endpoint, UserName, null));

            Assert.Throws<ArgumentException>(() => new ElasticConnection(endpoint, "", Password));
            Assert.Throws<ArgumentException>(() => new ElasticConnection(endpoint, UserName, ""));
        }

        [Fact]
        public void GuardClauses_Index()
        {
            var connection = new ElasticConnection(endpoint);

            Assert.Throws<ArgumentNullException>(() => connection.Index = null);
            Assert.Throws<ArgumentException>(() => connection.Index = "");
        }

        [Fact]
        public void GuardClauses_Timeout()
        {
            var connection = new ElasticConnection(endpoint);

            Assert.Throws<ArgumentOutOfRangeException>(() => connection.Timeout = TimeSpan.FromDays(-1));
        }

        [Fact]
        public void TimeoutPropertyCanBeSet()
        {
            var expected = TimeSpan.FromSeconds(123);

            var connection = new ElasticConnection(endpoint) { Timeout = expected };

            Assert.Equal(expected, connection.Timeout);
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
    }
}