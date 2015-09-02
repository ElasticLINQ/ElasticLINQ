// Licensed under the Apache 2.0 License. See LICENSE.txt in the project root for more information.

using System.Collections.Generic;
using System.Threading.Tasks;
using ElasticLinq.Logging;
using ElasticLinq.Mapping;
using ElasticLinq.Retry;
using System;
using System.Linq.Expressions;
using Xunit;

namespace ElasticLinq.Test
{
    public class ElasticQueryProviderTests
    {
        private static readonly ElasticConnection connection = new ElasticConnection(new Uri("http://localhost"));
        private static readonly IElasticMapping mapping = new TrivialElasticMapping();
        private static readonly ILog log = NullLog.Instance;
        private static readonly IRetryPolicy retryPolicy = NullRetryPolicy.Instance;

        private static readonly ElasticQueryProvider sharedProvider = new ElasticQueryProvider(connection, mapping, log,
			retryPolicy, "prefix");

        private static readonly Expression validExpression =
            Expression.Constant(new ElasticQuery<Sample>(sharedProvider));

        private class Sample { };
        private class Sample2 { public int ID { get; set; }}

        [Fact]
        public void CreateQueryTReturnsElasticQueryTWithProviderSet()
        {
			var provider = new ElasticQueryProvider(connection, mapping, log, retryPolicy, "prefix");

            var query = provider.CreateQuery<Sample>(validExpression);

            Assert.IsType<ElasticQuery<Sample>>(query);
            Assert.Same(provider, query.Provider);
        }

        [Fact]
        public void CreateQueryReturnsElasticQueryWithProviderSet()
        {
			var provider = new ElasticQueryProvider(connection, mapping, log, retryPolicy, "prefix");

            var query = provider.CreateQuery(validExpression);

            Assert.IsType<ElasticQuery<Sample>>(query);
            Assert.Same(provider, query.Provider);
        }

        [Fact]
        public void CreateQueryThrowsArgumentOutOfRangeIfExpressionTypeNotAssignableFromIQueryable()
        {
			var provider = new ElasticQueryProvider(connection, mapping, log, retryPolicy, "prefix");

            Assert.Throws<ArgumentOutOfRangeException>(
                () => provider.CreateQuery<Sample>(Expression.Constant(new Sample())));
        }

        [Fact]
        public void ExecuteThrowsArgumentNullExceptionIfNull()
        {
            Assert.Throws<ArgumentNullException>(() => sharedProvider.Execute(null));
        }

        [Fact]
        public void ExecuteTThrowsArgumentNullExceptionIfNull()
        {
            Assert.Throws<ArgumentNullException>(() => sharedProvider.Execute<Sample>(null));
        }

        [Fact]
        public void CreateQueryRethrowsTargetException()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => sharedProvider.CreateQuery(Expression.Constant(null)));
        }

        [Fact]
        public void ExecuteRethrowsAggregateException()
        {
            var context = new ElasticContext(connection, mapping, log, new ThrowsRetryPolicy());
            var query = context.Query<Sample2>();
            Assert.Throws<NotImplementedException>(() => query.Provider.Execute(query.Expression));
        }

        private class ThrowsRetryPolicy : IRetryPolicy
        {
            public Task<TResult> ExecuteAsync<TResult>(Func<Task<TResult>> operationFunc, Func<TResult, Exception, bool> shouldRetryFunc, Action<TResult, Dictionary<string, object>> appendLogInfoFunc = null)
            {
                throw new AggregateException(new NotImplementedException("Rethrowing exception for testing"));
            }
        }
    }
}