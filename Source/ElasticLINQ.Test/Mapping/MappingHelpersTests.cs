// Licensed under the Apache 2.0 License. See LICENSE.txt in the project root for more information.

using ElasticLinq.Mapping;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using ElasticLinq.Utility;
using Xunit;

namespace ElasticLinq.Test.Mapping
{
    public class MappingHelpersTests
    {
        private readonly static CultureInfo usCulture = new CultureInfo(0x0409);

        [Fact]
        public static void ToCamelCaseWithAllCapsLowersAllText()
        {
            var actual = "ALLCAPS".ToCamelCase(usCulture);

            Assert.Equal("allcaps", actual);
        }

        [Fact]
        public static void ToCamelCaseWithAllLowerCaseReturnsAllLowerCase()
        {
            var actual = "lowercase".ToCamelCase(usCulture);

            Assert.Equal("lowercase", actual);
        }

        [Fact]
        public static void ToCamelCaseWithAcronymLowersAcronym()
        {
            var actual = "ACRONYMThenLowerCase".ToCamelCase(usCulture);

            Assert.Equal("acronymThenLowerCase", actual);
        }

        [Fact]
        public static void ToCamelCaseEndingWithAcronym()
        {
            var actual = "EndsWithACRONYM".ToCamelCase(usCulture);

            Assert.Equal("endsWithACRONYM", actual);
        }

        [Fact]
        public static void ToCamelCaseWithMixedCaseOnlyChangesFirstLetter()
        {
            var actual = "MixedCaseExample".ToCamelCase(usCulture);

            Assert.Equal("mixedCaseExample", actual);
        }

        [Fact]
        public static void ToCamelCaseWithCamelCaseReturnsCamelCase()
        {
            var actual = "alreadyCamelCase".ToCamelCase(usCulture);

            Assert.Equal("alreadyCamelCase", actual);
        }

        [Fact]
        public static void ToCamelCaseWithSingleCharStringReturnsSingleCharLowered()
        {
            var actual = "S".ToCamelCase(usCulture);

            Assert.Equal("s", actual);
        }

        [Fact]
        public static void ToCamelCaseWithEmptyStringReturnsEmptyString()
        {
            var actual = "".ToCamelCase(usCulture);

            Assert.Equal("", actual);
        }

        [Fact]
        public static void ToCamelCaseWithNullThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => ((string)null).ToCamelCase(usCulture));
        }

        [Fact]
        public static void ToPluralWithStringNotEndingInSAddsS()
        {
            var actual = "test".ToPlural(usCulture);

            Assert.Equal("tests", actual);
        }

        [Fact]
        public static void ToPluralWithStringEndingInSDoesNotAddS()
        {
            var actual = "tests".ToPlural(usCulture);

            Assert.Equal("tests", actual);
        }

        [Fact]
        public static void ToPluralWithEmptyStringReturnsEmptyString()
        {
            var actual = "".ToPlural(usCulture);

            Assert.Equal("", actual);
        }

        [Fact]
        public static void ToPluralWithNullThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => ((string)null).ToPlural(usCulture));
        }

        [Fact]
        public static void GetSelectionPropertyReturnsReadWriteNonGenericValueProperty()
        {
            var expected = TypeHelper.GetMemberInfo((ClassWithOneValidSelectionProperty c) => c.Valid);

            var actual = MappingHelpers.GetTypeSelectionProperty(typeof(ClassWithOneValidSelectionProperty));

            Assert.Equal(expected, actual);
        }

        [Fact]
        public static void GetSelectionPropertyWithNoSuitablePropertyReturnsNull()
        {
            var actual = MappingHelpers.GetTypeSelectionProperty(typeof(ClassWithNoValidSelectionProperties));

            Assert.Null(actual);
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