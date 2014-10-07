// Licensed under the Apache 2.0 License. See LICENSE.txt in the project root for more information.

using ElasticLinq.Mapping;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Reflection;
using Xunit;

namespace ElasticLinq.Test.Mapping
{
    public class MappingHelpersTests
    {
        private readonly static CultureInfo USCulture = new CultureInfo(0x0409);

        [Fact]
        public static void ToCamelCaseWithAllCapsLowersFirstCapitalOnly()
        {
            var actual = "ALLCAPS".ToCamelCase(USCulture);

            Assert.Equal("aLLCAPS", actual);
        }

        [Fact]
        public static void ToCamelCaseWithAllLowerCaseReturnsAllLowerCase()
        {
            var actual = "lowercase".ToCamelCase(USCulture);

            Assert.Equal("lowercase", actual);
        }

        [Fact]
        public static void ToCamelCaseWithMixedCaseOnlyChangesFirstLetter()
        {
            var actual = "MixedCaseExample".ToCamelCase(USCulture);

            Assert.Equal("mixedCaseExample", actual);
        }

        [Fact]
        public static void ToCamelCaseWithCamelCaseReturnsCamelCase()
        {
            var actual = "alreadyCamelCase".ToCamelCase(USCulture);

            Assert.Equal("alreadyCamelCase", actual);
        }

        [Fact]
        public static void ToCamelCaseWithSingleCharStringReturnsSingleCharLowered()
        {
            var actual = "S".ToCamelCase(USCulture);

            Assert.Equal("s", actual);
        }

        [Fact]
        public static void ToCamelCaseWithEmptyStringReturnsEmptyString()
        {
            var actual = "".ToCamelCase(USCulture);

            Assert.Equal("", actual);
        }

        [Fact]
        [ExcludeFromCodeCoverage] // Expression isn't "executed"
        public static void ToCamelCaseWithNullThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => ((string)null).ToCamelCase(USCulture));
        }

        [Fact]
        public static void ToPluralWithStringNotEndingInSAddsS()
        {
            var actual = "test".ToPlural(USCulture);

            Assert.Equal("tests", actual);
        }

        [Fact]
        public static void ToPluralWithStringEndingInSDoesNotAddS()
        {
            var actual = "tests".ToPlural(USCulture);

            Assert.Equal("tests", actual);
        }

        [Fact]
        public static void ToPluralWithEmptyStringReturnsEmptyString()
        {
            var actual = "".ToPlural(USCulture);

            Assert.Equal("", actual);
        }

        [Fact]
        [ExcludeFromCodeCoverage] // Expression isn't "executed"
        public static void ToPluralWithNullThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => ((string)null).ToPlural(USCulture));
        }

        [Fact]
        public static void GetSelectionPropertyReturnsReadWriteNonGenericValueProperty()
        {
            var selectionProperty = MappingHelpers.GetSelectionProperty(typeof(ClassWithOneValidSelectionProperty));

            Assert.IsAssignableFrom<PropertyInfo>(selectionProperty);
            Assert.Equal("Valid", selectionProperty.Name);
            Assert.Equal(typeof(ClassWithOneValidSelectionProperty), selectionProperty.DeclaringType);
            Assert.Equal(typeof(int), selectionProperty.PropertyType);
        }

        [Fact]
        public static void GetSelectionPropertyWithNoSuitablePropertyThrows()
        {
            var ex = Record.Exception(() => MappingHelpers.GetSelectionProperty(typeof(ClassWithNoValidSelectionProperties)));

            Assert.IsType<InvalidOperationException>(ex);
            Assert.Equal("Could not find public read/write non-generic value type property to use for a default query against ElasticLinq.Test.Mapping.MappingHelpersTests+ClassWithNoValidSelectionProperties.", ex.Message);
        }

        class ClassWithOneValidSelectionProperty
        {
            public int Valid { get; set; }
        }

        [ExcludeFromCodeCoverage]
        class ClassWithNoValidSelectionProperties
        {
            private int backing;

            public int? Nullable { get; set; }
            public int WriteOnly { set { backing = value; } }
            public int ReadOnly { get { return backing; } }
            public string NotValueType { get; set; }
            private int NonPublic { get; set; }

            public int Field = 1;
        }
    }
}