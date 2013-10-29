// Copyright (c) Tier 3 Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 

using System;
using ElasticLinq.Request;
using Xunit;

namespace ElasticLinq.Test.Request
{
    public class SortOptionTests
    {
        [Fact]
        public void ConstructorSetsPropertiesWithAscendingTrue()
        {
            const string expectedName = "SomeField";
            const bool expectedAscending = true;

            var sortOption = new SortOption(expectedName, expectedAscending);

            Assert.Equal(expectedName, sortOption.Name);
            Assert.Equal(expectedAscending, sortOption.Ascending);
        }

        [Fact]
        public void ConstructorSetsPropertiesWithAscendingFalse()
        {
            const string expectedName = "SomeOtherField";
            const bool expectedAscending = false;

            var sortOption = new SortOption(expectedName, expectedAscending);

            Assert.Equal(expectedName, sortOption.Name);
            Assert.Equal(expectedAscending, sortOption.Ascending);
        }

        [Fact]
        public void ConstructorThrowsArgumentNullExceptionWhenNameIsNull()
        {
            Assert.Throws<ArgumentNullException>(() => new SortOption(null, true));
        }

        [Fact]
        public void ConstructorThrowsArgumentExceptionWhenNameIsEmptyNull()
        {
            Assert.Throws<ArgumentException>(() => new SortOption("", true));
        }
    }
}
