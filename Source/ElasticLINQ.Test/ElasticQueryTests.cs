// Copyright (c) Tier 3 Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 

using ElasticLinq;
using ElasticLinq.Mapping;
using System;
using System.Linq.Expressions;
using Xunit;

namespace ElasticLINQ.Test
{
    public class ElasticQueryTests
    {
        private static readonly ElasticConnection connection = new ElasticConnection(new Uri("http://localhost"), TimeSpan.FromSeconds(10));
        private static readonly ElasticQueryProvider provider = new ElasticQueryProvider(connection, new TrivialElasticMapping());
        private static readonly Expression validExpression = Expression.Constant(new ElasticQuery<Sample>(provider));

        private class Sample { };

        [Fact]
        public void ConstructorsThrowsArgumentNullExceptionWhenProviderIsNull()
        {
            Assert.Throws<ArgumentNullException>(() => new ElasticQuery<Sample>(null));
            Assert.Throws<ArgumentNullException>(() => new ElasticQuery<Sample>(null, validExpression));
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
            var query = new ElasticQuery<Sample>(provider, validExpression);

            Assert.Equal(validExpression, query.Expression);
        }

        [Fact]
        public void ConstructorWithNoExpressionDefaultsExpressionPropertyToConstantOfQuery()
        {
            var query = new ElasticQuery<Sample>(provider);

            Assert.IsType<ConstantExpression>(query.Expression);
            Assert.Same(query, (((ConstantExpression)query.Expression)).Value);
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