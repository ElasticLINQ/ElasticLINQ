// Licensed under the Apache 2.0 License. See LICENSE.txt in the project root for more information.

using ElasticLinq.Logging;
using ElasticLinq.Mapping;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using Xunit;
using ElasticLinq.Connection;

namespace ElasticLinq.Test
{
    public class ElasticQueryTests
    {
        private static readonly IElasticConnection connection = new HttpElasticConnection(new Uri("http://localhost"));
        private static readonly ElasticQueryProvider provider = new ElasticQueryProvider(connection, new TrivialElasticMapping(), NullLog.Instance, NullRetryPolicy.Instance, "prefix");
        private static readonly Expression validExpression = Expression.Constant(new ElasticQuery<Sample>(provider));

        private class Sample { };

        [Fact]
        [ExcludeFromCodeCoverage] // Expression isn't "executed"
        public void ConstructorsThrowsArgumentNullExceptionWhenProviderIsNull()
        {
            Assert.Throws<ArgumentNullException>(() => new ElasticQuery<Sample>(null));
            Assert.Throws<ArgumentNullException>(() => new ElasticQuery<Sample>(null, validExpression));
        }

        [Fact]
        [ExcludeFromCodeCoverage] // Expression isn't "executed"
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
            var query = new ElasticQuery<Sample>(provider, validExpression);

            Assert.Equal(validExpression, query.Expression);
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
    }
}