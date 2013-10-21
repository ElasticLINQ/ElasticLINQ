// Copyright (c) Tier 3 Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 

using ElasticLinq.Request.Filters;
using System;
using Xunit;

namespace ElasticLINQ.Test.Request.Filters
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
        public void ConstructorThrowsArgumentNullExceptionWhenFieldIsNull()
        {
            Assert.Throws<ArgumentNullException>(() => new ExistsFilter(null));
        }

        [Fact]
        public void ConstructorThrowsArgumentExceptionWhenFieldIsBlank()
        {
            Assert.Throws<ArgumentException>(() => new ExistsFilter(" "));
        }
   }
}