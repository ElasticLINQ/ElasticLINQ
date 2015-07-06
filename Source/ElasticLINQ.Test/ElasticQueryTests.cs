// Licensed under the Apache 2.0 License. See LICENSE.txt in the project root for more information.

using System.Linq;
using ElasticLinq.Logging;
using ElasticLinq.Mapping;
using System;
using System.Linq.Expressions;
using ElasticLinq.Request;
using Xunit;

namespace ElasticLinq.Test
{
    public class ElasticQueryTests
    {
        private static readonly ElasticConnection connection = new ElasticConnection(new Uri("http://localhost"));
        private static readonly ElasticQueryProvider provider = new ElasticQueryProvider(connection, new TrivialElasticMapping(), NullLog.Instance, NullRetryPolicy.Instance);
        private static readonly Expression validConstantExpression = Expression.Constant(new ElasticQuery<Sample>(provider));

        private class Sample
        {
            public string Text { get; set; }
        };

        [Fact]
        public void ConstructorsThrowsArgumentNullExceptionWhenProviderIsNull()
        {
            Assert.Throws<ArgumentNullException>(() => new ElasticQuery<Sample>(null));
            Assert.Throws<ArgumentNullException>(() => new ElasticQuery<Sample>(null, validConstantExpression));
        }

        [Fact]
        public void ConstructorsThrowsArgumentOutOfRangeExceptionWhenExpressionIsNotAssignableFromIQueryableT()
        {
            var unassignableExpression = Expression.Constant(1);
            Assert.Throws<ArgumentOutOfRangeException>(() => new ElasticQuery<Sample>(provider, unassignableExpression));
        }

        [Fact]
        public void ConstructorsSetProviderProperty()
        {
            var firstConstructor = new ElasticQuery<Sample>(provider);
            var secondConstructor = new ElasticQuery<Sample>(provider);

            Assert.Same(provider, firstConstructor.Provider);
            Assert.Same(provider, secondConstructor.Provider);
        }

        [Fact]
        public void ConstructorSetsExpressionProperty()
        {
            var query = new ElasticQuery<Sample>(provider, validConstantExpression);

            Assert.Equal(validConstantExpression, query.Expression);
        }

        [Fact]
        public void ConstructorWithNoExpressionDefaultsExpressionPropertyToConstantOfQuery()
        {
            var query = new ElasticQuery<Sample>(provider);

            var constantExpression = Assert.IsType<ConstantExpression>(query.Expression);
            Assert.Same(query, constantExpression.Value);
        }

        [Fact]
        public void ElementTypePropertyReturnsGenericArgument()
        {
            var query = new ElasticQuery<Sample>(provider);

            var elementType = query.ElementType;

            Assert.Equal(typeof(Sample), elementType);
        }

        [Fact]
        public void LinqMethodReturnsElasticQueryType()
        {
            var query = new ElasticQuery<Sample>(provider).Where(s => s.Text == "a");

            Assert.IsType<ElasticQuery<Sample>>(query);
        }

        [Fact]
        public void ToQueryInfoReturnsValidQueryInfo()
        {
            var elasticQuery = Assert.IsType<ElasticQuery<Sample>>(new ElasticQuery<Sample>(provider).Where(s => s.Text == "something1"));

            var queryInfo = elasticQuery.ToQueryInfo();

            Assert.IsType<QueryInfo>(queryInfo);
            Assert.NotNull(queryInfo);

            Assert.Contains("something1", queryInfo.Query);
            Assert.StartsWith(connection.Endpoint.OriginalString, queryInfo.Uri.OriginalString);
        }
    }
}