// Copyright (c) Tier 3 Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 

using ElasticLinq.Mapping;
using ElasticLinq.Utility;
using System;
using System.IO;
using System.Text;
using Xunit;

namespace ElasticLinq.Test
{
    public class ElasticContextTests
    {
        private static readonly ElasticConnection connection = new ElasticConnection(new Uri("http://localhost"), TimeSpan.FromSeconds(10));
        private static readonly IElasticMapping mapping = new TrivialElasticMapping();

        private class Sample { };

        [Fact]
        public void ConstructorSetsPropertiesFromParameters()
        {
            var context = new ElasticContext(connection, mapping);

            Assert.Same(connection, context.Connection);
            Assert.Same(mapping, context.Mapping);
        }

        [Fact]
        public void ConstructorThrowsArgumentNullExceptionWhenConnectionIsNull()
        {
            Assert.Throws<ArgumentNullException>(() => new ElasticContext(null, mapping));
        }

        [Fact]
        public void ConstructorThrowsArgumentNullExceptionWhenMappingIsNull()
        {
            Assert.Throws<ArgumentNullException>(() => new ElasticContext(connection, null));
        }

        [Fact]
        public void ConstructorDoesNotSetLogProperty()
        {
            var context = new ElasticContext(connection, mapping);

            Assert.Null(context.Log);
        }

        [Fact]
        public void LogPropertyCanBeSet()
        {
            var stringBuilder = new StringBuilder();
            var expectedLog = new StringWriter(stringBuilder);

            var context = new ElasticContext(connection, mapping) { Log = expectedLog };

            Assert.Same(expectedLog, context.Log);
        }

        [Fact]
        public void LogPropertyCanBeUnset()
        {
            var context = new ElasticContext(connection, mapping) { Log = Console.Out };
            context.Log = null;

            Assert.Null(context.Log);
        }

        [Fact]
        public void QueryPropertyReturnsElasticQueryWithConnectionAndMapping()
        {
            var context = new ElasticContext(connection, mapping);

            var query = context.Query<Sample>();

            Assert.NotNull(query);
            Assert.IsType<ElasticQueryProvider>(query.Provider);
            var elasticProvider = (ElasticQueryProvider)query.Provider;
            Assert.Same(context.Connection, elasticProvider.Connection);
            Assert.Same(context.Mapping, elasticProvider.Mapping);
        }
    }
}