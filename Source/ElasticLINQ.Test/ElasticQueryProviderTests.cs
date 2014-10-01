﻿// Licensed under the Apache 2.0 License. See LICENSE.txt in the project root for more information.

using ElasticLinq.Logging;
using ElasticLinq.Mapping;
using ElasticLinq.Retry;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using Xunit;
using ElasticLinq.Connection;

namespace ElasticLinq.Test
{
    public class ElasticQueryProviderTests
    {
        private static readonly IElasticConnection connection = new HttpElasticConnection(new Uri("http://localhost"));
        private static readonly IElasticMapping mapping = new TrivialElasticMapping();
        private static readonly ILog log = NullLog.Instance;
        private static readonly IRetryPolicy retryPolicy = NullRetryPolicy.Instance;
        private static readonly ElasticQueryProvider sharedProvider = new ElasticQueryProvider(connection, mapping, log, retryPolicy, "prefix");
        private static readonly Expression validExpression = Expression.Constant(new ElasticQuery<Sample>(sharedProvider));

        private class Sample { };

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

            Assert.Throws<ArgumentOutOfRangeException>(() => provider.CreateQuery<Sample>(Expression.Constant(new Sample())));
        }

        [Fact]
        [ExcludeFromCodeCoverage] // Expression isn't "executed"
        public void ExecuteThrowsArgumentNullExceptionIfNull()
        {
            Assert.Throws<ArgumentNullException>(() => sharedProvider.Execute(null));
        }

        [Fact]
        [ExcludeFromCodeCoverage] // Expression isn't "executed"
        public void ExecuteTThrowsArgumentNullExceptionIfNull()
        {
            Assert.Throws<ArgumentNullException>(() => sharedProvider.Execute<Sample>(null));
        }
    }
}