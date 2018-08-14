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
        public void ConstructorSetsOptionalUnmappedTypePropertyFromParameter()
        {
            const string expectedName = "SomeOtherField";
            const bool expectedAscending = false;
            const string expectedUnmappedType = "short";

            var sortOption = new SortOption(expectedName, expectedAscending, expectedUnmappedType);

            Assert.Equal(expectedName, sortOption.Name);
            Assert.Equal(expectedAscending, sortOption.Ascending);
            Assert.Equal(expectedUnmappedType, sortOption.UnmappedType);
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
