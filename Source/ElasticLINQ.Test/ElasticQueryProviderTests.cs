// Copyright (c) Tier 3 Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 

using ElasticLinq.Mapping;
using ElasticLinq.Utility;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using Xunit;

namespace ElasticLinq.Test
{
    public class ElasticQueryProviderTests
    {
        private static readonly ElasticConnection connection = new ElasticConnection(new Uri("http://localhost"));
        private static readonly IElasticMapping mapping = new TrivialElasticMapping();
        private static readonly ElasticQueryProvider sharedProvider = new ElasticQueryProvider(connection, mapping);
        private static readonly Expression validExpression = Expression.Constant(new ElasticQuery<Sample>(sharedProvider));

        private class Sample { };

        [Fact]
        public void LogPropertyCanBeSet()
        {
            var provider = new ElasticQueryProvider(connection, mapping);

            var log = Console.Out;
            provider.Log = log;

            Assert.Equal(log, provider.Log);
        }

        [Fact]
        public void CreateQueryTReturnsElasticQueryTWithProviderSet()
        {
            var provider = new ElasticQueryProvider(connection, mapping);

            var query = provider.CreateQuery<Sample>(validExpression);

            Assert.IsType<ElasticQuery<Sample>>(query);
            Assert.Same(provider, query.Provider);
        }

        [Fact]
        public void CreateQueryReturnsElasticQueryWithProviderSet()
        {
            var provider = new ElasticQueryProvider(connection, mapping);

            var query = provider.CreateQuery(validExpression);

            Assert.IsType<ElasticQuery<Sample>>(query);
            Assert.Same(provider, query.Provider);
        }

        [Fact]
        public void CreateQueryThrowsArgumentOutOfRangeIfExpressionTypeNotAssignableFromIQueryable()
        {
            var provider = new ElasticQueryProvider(connection, mapping);
            
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