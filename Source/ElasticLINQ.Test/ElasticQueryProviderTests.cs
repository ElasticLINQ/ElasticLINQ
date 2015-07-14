// Licensed under the Apache 2.0 License. See LICENSE.txt in the project root for more information.

using System.Collections.Generic;
using System.Threading.Tasks;
using ElasticLinq.Logging;
using ElasticLinq.Mapping;
using ElasticLinq.Retry;
using System;
using System.Linq.Expressions;
using System.Threading;
using Xunit;

namespace ElasticLinq.Test
{
    public class ElasticQueryProviderTests
    {
        static readonly ElasticConnection connection = new ElasticConnection(new Uri("http://localhost"));
        static readonly IElasticMapping mapping = new TrivialElasticMapping();
        static readonly ILog log = NullLog.Instance;
        static readonly IRetryPolicy retryPolicy = NullRetryPolicy.Instance;
        static readonly ElasticQueryProvider sharedProvider = new ElasticQueryProvider(connection, mapping, log, retryPolicy);

        static readonly Expression validExpression = Expression.Constant(new ElasticQuery<Sample>(sharedProvider));

        class Sample { };
        class Sample2 { public int ID { get; set; }}

        [Fact]
        public void CreateQueryTReturnsElasticQueryTWithProviderSet()
        {
            var provider = new ElasticQueryProvider(connection, mapping, log, retryPolicy);

            var query = provider.CreateQuery<Sample>(validExpression);

            Assert.IsType<ElasticQuery<Sample>>(query);
            Assert.Same(provider, query.Provider);
        }

        [Fact]
        public void CreateQueryReturnsElasticQueryWithProviderSet()
        {
            var provider = new ElasticQueryProvider(connection, mapping, log, retryPolicy);

            var query = provider.CreateQuery(validExpression);

            Assert.IsType<ElasticQuery<Sample>>(query);
            Assert.Same(provider, query.Provider);
        }

        [Fact]
        public void CreateQueryThrowsArgumentOutOfRangeIfExpressionTypeNotAssignableFromIQueryable()
        {
            var provider = new ElasticQueryProvider(connection, mapping, log, retryPolicy);

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
        public void ExecuteAsyncThrowsArgumentNullExceptionIfNull()
        {
            Assert.ThrowsAsync<ArgumentNullException>(() => sharedProvider.ExecuteAsync(null));
        }

        [Fact]
        public void ExecuteAsyncTThrowsArgumentNullExceptionIfNull()
        {
            Assert.ThrowsAsync<ArgumentNullException>(() => sharedProvider.ExecuteAsync<Sample>(null));
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

        class ThrowsRetryPolicy : IRetryPolicy
        {
            public Task<TResult> ExecuteAsync<TResult>(
                Func<CancellationToken, Task<TResult>> operationFunc, 
                Func<TResult, Exception, bool> shouldRetryFunc, 
                Action<TResult, Dictionary<string, object>> appendLogInfoFunc = null,
                CancellationToken cancellationToken = new CancellationToken())
            {
                throw new AggregateException(new NotImplementedException("Rethrowing exception for testing"));
            }
        }
    }
}