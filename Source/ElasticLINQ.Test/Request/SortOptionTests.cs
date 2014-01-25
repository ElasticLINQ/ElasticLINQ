// Licensed under the Apache 2.0 License. See LICENSE.txt in the project root for more information.

using ElasticLinq.Request;
using System;
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
        public void ConstructorSetsOptionalIgnoreUnmappedPropertyFromParameter()
        {
            const string expectedName = "SomeOtherField";
            const bool expectedAscending = false;
            const bool expectedIgnoreUnmapped = true;

            var sortOption = new SortOption(expectedName, expectedAscending, expectedIgnoreUnmapped);

            Assert.Equal(expectedName, sortOption.Name);
            Assert.Equal(expectedAscending, sortOption.Ascending);
            Assert.Equal(expectedIgnoreUnmapped, sortOption.IgnoreUnmapped);
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
