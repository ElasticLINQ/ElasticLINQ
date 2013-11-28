// Licensed under the Apache 2.0 License. See LICENSE.txt in the project root for more information.

using System;
using Xunit;

namespace ElasticLinq.Test
{
    public class ElasticConnectionTests
    {
        private readonly Uri endpoint = new Uri("http://localhost:1234/abc");
        private readonly string password = "thePassword";
        private readonly string userName = "theUser";

        [Fact]
        public void GuardClauses_Constructor()
        {
            Assert.Throws<ArgumentNullException>(() => new ElasticConnection(null));
            Assert.Throws<ArgumentNullException>(() => new ElasticConnection(null, userName, password));
            Assert.Throws<ArgumentNullException>(() => new ElasticConnection(endpoint, null, password));
            Assert.Throws<ArgumentNullException>(() => new ElasticConnection(endpoint, userName, null));

            Assert.Throws<ArgumentException>(() => new ElasticConnection(endpoint, "", password));
            Assert.Throws<ArgumentException>(() => new ElasticConnection(endpoint, userName, ""));
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
        public void ConstructorWithOneArgSetsPropertyFromParameter()
        {
            var connection = new ElasticConnection(endpoint);

            Assert.Equal(endpoint, connection.Endpoint);
        }

        [Fact]
        public void ConstructorWithThreeArgsSetsPropertiesFromParameters()
        {
            var connection = new ElasticConnection(endpoint, userName, password);

            Assert.Equal(endpoint, connection.Endpoint);
            Assert.Equal(userName, connection.UserName);
            Assert.Equal(password, connection.Password);
        }
    }
}