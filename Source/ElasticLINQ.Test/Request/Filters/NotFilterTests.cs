// Copyright (c) Tier 3 Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 

using ElasticLinq.Request.Filters;
using System;
using Xunit;

namespace ElasticLINQ.Test.Request.Filters
{
    public class NotFilterTests
    {
        private readonly IFilter sampleFilter = new TermFilter("field", "value");

        [Fact]
        public void NamePropertyIsNot()
        {
            var filter = NotFilter.Create(sampleFilter);

            Assert.Equal("not", filter.Name);
        }

        [Fact]
        public void CreateReturnsNotFilterWithChildFilterSet()
        {
            var filter = NotFilter.Create(sampleFilter);

            Assert.IsType<NotFilter>(filter);
            Assert.Equal(sampleFilter, ((NotFilter)filter).ChildFilter);
        }

        [Fact]
        public void CreateUnwrapsNestedNotFilters()
        {
            var filter = NotFilter.Create(NotFilter.Create(sampleFilter));

            Assert.IsType<TermFilter>(filter);
            Assert.Equal(sampleFilter, filter);
        }

        [Fact]
        public void ConstructorThrowsArgumentNullExceptionWhenFieldIsNull()
        {
            Assert.Throws<ArgumentNullException>(() => NotFilter.Create(null));
        }
   }
}