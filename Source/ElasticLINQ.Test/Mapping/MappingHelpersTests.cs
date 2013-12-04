// Licensed under the Apache 2.0 License. See LICENSE.txt in the project root for more information.

using System.Reflection;
using ElasticLinq.Mapping;
using System;
using System.Diagnostics.CodeAnalysis;
using Xunit;

namespace ElasticLinq.Test.Mapping
{
    public class MappingHelpersTests
    {
        [Fact]
        public void ToCamelCaseWithAllCapsLowersFirstCapitalOnly()
        {
            var actual = "ALLCAPS".ToCamelCase();

            Assert.Equal("aLLCAPS", actual);
        }

        [Fact]
        public void ToCamelCaseWithAllLowerCaseReturnsAllLowerCase()
        {
            var actual = "lowercase".ToCamelCase();

            Assert.Equal("lowercase", actual);
        }

        [Fact]
        public void ToCamelCaseWithMixedCaseOnlyChangesFirstLetter()
        {
            var actual = "MixedCaseExample".ToCamelCase();

            Assert.Equal("mixedCaseExample", actual);
        }

        [Fact]
        public void ToCamelCaseWithCamelCaseReturnsCamelCase()
        {
            var actual = "alreadyCamelCase".ToCamelCase();

            Assert.Equal("alreadyCamelCase", actual);
        }

        [Fact]
        public void ToCamelCaseWithSingleCharStringReturnsSingleCharLowered()
        {
            var actual = "S".ToCamelCase();

            Assert.Equal("s", actual);
        }

        [Fact]
        public void ToCamelCaseWithEmptyStringReturnsEmptyString()
        {
            var actual = "".ToCamelCase();

            Assert.Equal("", actual);
        }

        [Fact]
        [ExcludeFromCodeCoverage] // Expression isn't "executed"
        public void ToCamelCaseWithNullThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => ((string)null).ToCamelCase());
        }

        [Fact]
        public void ToPluralWithStringNotEndingInSAddsS()
        {
            var actual = "test".ToPlural();

            Assert.Equal("tests", actual);
        }

        [Fact]
        public void ToPluralWithStringEndingInSDoesNotAddS()
        {
            var actual = "tests".ToPlural();

            Assert.Equal("tests", actual);
        }

        [Fact]
        public void ToPluralWithEmptyStringReturnsEmptyString()
        {
            var actual = "".ToPlural();

            Assert.Equal("", actual);
        }

        [Fact]
        [ExcludeFromCodeCoverage] // Expression isn't "executed"
        public void ToPluralWithNullThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => ((string)null).ToPlural());
        }

        [Fact]
        public void GetSelectionPropertyReturnsReadWriteNonGenericValueProperty()
        {
            var selectionProperty = MappingHelpers.GetSelectionProperty(typeof(ClassWithOneValidSelectionProperty));

            Assert.IsAssignableFrom<PropertyInfo>(selectionProperty);
            Assert.Equal("Valid", selectionProperty.Name);
            Assert.Equal(typeof(ClassWithOneValidSelectionProperty), selectionProperty.DeclaringType);
            Assert.Equal(typeof(int), selectionProperty.PropertyType);
        }

        class ClassWithOneValidSelectionProperty
        {
            private int backing;

            public int? Nullable { get; set; }
            public int WriteOnly { set { backing = value; } }
            public int ReadOnly { get { return backing; } }
            public string NotValueType { get; set; }
            public int Field;

            public int Valid { get; set; }
        }
    }
}