// Licensed under the Apache 2.0 License. See LICENSE.txt in the project root for more information.

using ElasticLinq.Logging;
using ElasticLinq.Mapping;
using ElasticLinq.Retry;
using NSubstitute;
using System;
using Xunit;

namespace ElasticLinq.Test
{
    public class ElasticContextTests
    {
        private static readonly ElasticConnection connection = new ElasticConnection(new Uri("http://localhost"));

        private class Sample { };

        [Fact]
        public void ConstructorSetsPropertiesFromParameters()
        {
            var mapping = Substitute.For<IElasticMapping>();
            var log = Substitute.For<ILog>();
            var retryPolicy = Substitute.For<IRetryPolicy>();

            var context = new ElasticContext(connection, mapping, log, retryPolicy);

            Assert.Same(connection, context.Connection);
            Assert.Same(mapping, context.Mapping);
            Assert.Same(log, context.Log);
            Assert.Same(retryPolicy, context.RetryPolicy);
        }

        [Fact]
        public void DefaultValuesFromConstructorAreProvided()
        {
            var context = new ElasticContext(connection);

            Assert.IsType<TrivialElasticMapping>(context.Mapping);
            Assert.Same(NullLog.Instance, context.Log);
            var policy = Assert.IsType<RetryPolicy>(context.RetryPolicy);
            Assert.Same(Delay.Instance, policy.Delay);
            Assert.Equal(100, policy.InitialRetryMilliseconds);
            Assert.Same(NullLog.Instance, policy.Log);
            Assert.Equal(10, policy.MaxAttempts);
        }

        [Fact]
        public void ConstructorThrowsArgumentNullExceptionWhenConnectionIsNull()
        {
            Assert.Throws<ArgumentNullException>(() => new ElasticContext(null));
        }

        [Fact]
        public void QueryPropertyReturnsElasticQueryWithConnectionAndMapping()
        {
            var context = new ElasticContext(connection);

            var query = context.Query<Sample>();

            Assert.NotNull(query);
            Assert.IsType<ElasticQueryProvider>(query.Provider);
            var elasticProvider = (ElasticQueryProvider)query.Provider;
            Assert.Same(context.Connection, elasticProvider.Connection);
            Assert.Same(context.Mapping, elasticProvider.Mapping);
        }
    }
}