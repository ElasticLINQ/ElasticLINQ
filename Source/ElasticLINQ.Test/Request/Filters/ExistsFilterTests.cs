// Copyright (c) Tier 3 Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 

using ElasticLinq.Request.Filters;
using System;
using System.Diagnostics.CodeAnalysis;
using Xunit;

namespace ElasticLinq.Test.Request.Filters
{
    public class ExistsFilterTests
    {
        [Fact]
        public void NamePropertyIsExists()
        {
            var filter = new ExistsFilter("something");

            Assert.Equal("exists", filter.Name);
        }

        [Fact]
        public void ConstructorSetsFilters()
        {
            const string field = "myField";

            var filter = new ExistsFilter(field);

            Assert.Equal(field, filter.Field);
        }

        [Fact]
        [ExcludeFromCodeCoverage] // Expression isn't "executed"
        public void ConstructorThrowsArgumentNullExceptionWhenFieldIsNull()
        {
            Assert.Throws<ArgumentNullException>(() => new ExistsFilter(null));
        }

        [Fact]
        [ExcludeFromCodeCoverage] // Expression isn't "executed"
        public void ConstructorThrowsArgumentExceptionWhenFieldIsBlank()
        {
            Assert.Throws<ArgumentException>(() => new ExistsFilter(" "));
        }
   }
}