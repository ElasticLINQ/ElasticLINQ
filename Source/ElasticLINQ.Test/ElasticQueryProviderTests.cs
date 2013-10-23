// Copyright (c) Tier 3 Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 

using ElasticLinq;
using ElasticLinq.Mapping;
using ElasticLinq.Utility;
using System;
using System.Linq.Expressions;
using Xunit;

namespace ElasticLINQ.Test
{
    public class ElasticQueryProviderTests
    {
        private static readonly ElasticConnection connection = new ElasticConnection(new Uri("http://localhost"), TimeSpan.FromSeconds(10));
        private static readonly ElasticQueryProvider sharedProvider = new ElasticQueryProvider(connection, new TrivialElasticMapping());
        private static readonly IElasticMapping mapping = new TrivialElasticMapping();
        private static readonly Expression validExpression = Expression.Constant(new ElasticQuery<Sample>(sharedProvider));

        private class Sample { };

        [Fact]
        public void LogPropertyCanBeSet()
        {
            var provider = new ElasticQueryProvider(connection, mapping);

            var log = new NullTextWriter();
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
    }
}