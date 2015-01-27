// Licensed under the Apache 2.0 License. See LICENSE.txt in the project root for more information.

using ElasticLinq.Mapping;
using ElasticLinq.Request.Criteria;
using ElasticLinq.Test.TestSupport;
using ElasticLinq.Utility;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using System.Reflection;
using Xunit;

namespace ElasticLinq.Test.Mapping
{
    public class ElasticMappingTests
    {
        private enum Day { Sunday, Monday, Tuesday, Wednesday, Thursday, Friday, Saturday };

        private class FieldClass { public string AField; }

        private class FormatClass
        {
            public string Analyzed { get; set; }

            [NotAnalyzed]
            public string NotAnalyzed { get; set; }

            public object NonString { get; set; }

            public int IntegerValue { get; set; }

            public Identifier JsonConverterToString { get; set; }

            public Day DayProperty { get; set; }

            [JsonProperty("CustomPropertyName")]
            public string NotSoCustom { get; set; }
        }

        public static IEnumerable<object[]> FormatClassData
        {
            get
            {
                yield return new object[] { false, "Analyzed", "SomeValue", "\"SomeValue\"" };
                yield return new object[] { true, "Analyzed", "SomeValue", "\"somevalue\"" };
                yield return new object[] { true, "Analyzed", null, "null" };
                yield return new object[] { true, "NotAnalyzed", "SomeValue", "\"SomeValue\"" };
                yield return new object[] { true, "NonString", new { X = 42, y = "Hello" }, "{\"X\":42,\"y\":\"Hello\"}" };
                yield return new object[] { true, "JsonConverterToString", new Identifier("Hello World"), "\"hello world!!\"" };
                yield return new object[] { true, "JsonConverterToString", null, "null" };
            }
        }

        [Theory]
        [MemberData("FormatClassData")]
        public static void FormatValue(bool lowerCaseAnalyzedFieldValues, string propertyName, object inputValue, string expected)
        {
            var memberInfo = typeof(FormatClass).GetProperty(propertyName);
            var mapping = new ElasticMapping(lowerCaseAnalyzedFieldValues: lowerCaseAnalyzedFieldValues);

            var result = mapping.FormatValue(memberInfo, inputValue);

            Assert.Equal(expected, result.ToString(Formatting.None));
        }

        [Fact]
        [ExcludeFromCodeCoverage] // Expression isn't "executed"
        public static void FormatValue_GuardClause()
        {
            var mapping = new ElasticMapping();

            Assert.Throws<ArgumentNullException>(() => mapping.FormatValue(null, "value"));
        }

        [Theory]
        [InlineData(false, "a.B.c", "a.B.c.GetFieldName")]
        [InlineData(false, "", "GetFieldName")]
        [InlineData(false, null, "GetFieldName")]
        [InlineData(true, "a.B.c", "a.B.c.getFieldName")]
        [InlineData(true, "", "getFieldName")]
        [InlineData(true, null, "getFieldName")]
        public static void GetFieldName(bool camelCaseFieldNames, string prefix, string expected)
        {
            var memberInfo = MethodBase.GetCurrentMethod();
            var mapping = new ElasticMapping(camelCaseFieldNames: camelCaseFieldNames);

            var actual = mapping.GetFieldName(prefix, memberInfo);

            Assert.Equal(expected, actual);
        }

        [Fact]
        public static void GetFieldName_HonorsJsonPropertyName()
        {
            var memberInfo = TypeHelper.GetMemberInfo((FormatClass f) => f.NotSoCustom);
            var mapping = new ElasticMapping();

            var actual = mapping.GetFieldName("", memberInfo);

            Assert.Equal("CustomPropertyName", actual);
        }

        [Fact]
        public static void GetFieldName_GuardClause()
        {
            var mapping = new ElasticMapping();

            Assert.Throws<ArgumentNullException>(() => mapping.GetFieldName("", (MemberExpression)null));
            Assert.Throws<NotSupportedException>(() => mapping.GetFieldName("", Expression.Field(Expression.Constant(new FieldClass { AField = "test" }), "AField")));
            Assert.Throws<ArgumentNullException>(() => mapping.GetFieldName("", (MemberInfo)null));
        }

        private class SingularTypeName { }
        private class PluralTypeNames { }

        [Theory]
        [InlineData(false, false, typeof(SingularTypeName), "SingularTypeName")]
        [InlineData(false, true, typeof(SingularTypeName), "SingularTypeNames")]
        [InlineData(true, false, typeof(SingularTypeName), "singularTypeName")]
        [InlineData(true, true, typeof(SingularTypeName), "singularTypeNames")]
        [InlineData(true, true, typeof(PluralTypeNames), "pluralTypeNames")]
        public static void GetDocumentType(bool camelCaseTypeNames, bool pluralizeTypeNames, Type type, string expected)
        {
            var mapping = new ElasticMapping(camelCaseTypeNames: camelCaseTypeNames, pluralizeTypeNames: pluralizeTypeNames);

            var actual = mapping.GetDocumentType(type);

            Assert.Equal(expected, actual);
        }

        [Fact]
        public static void GetDocumentType_GuardClause()
        {
            var mapping = new ElasticMapping();

            Assert.Throws<ArgumentNullException>(() => mapping.GetDocumentType(null));
        }

        [Fact]
        public static void GetTypeSelectionCriteria()
        {
            var mapping = new ElasticMapping();

            var criteria = mapping.GetTypeSelectionCriteria(typeof(FormatClass));

            Assert.Null(criteria);
        }

        [Fact]
        public static void GetTypeSelectionCriteria_GuardClause()
        {
            var mapping = new ElasticMapping();

            Assert.Throws<ArgumentNullException>(() => mapping.GetTypeSelectionCriteria(null));
        }

        [Fact]
        public static void Format_WithEnumPropertyProducesStringWhenMappingSpecifiesStringFormat()
        {
            var mapping = new ElasticMapping(enumFormat: EnumFormat.String);
            var memberInfo = TypeHelper.GetMemberInfo((FormatClass f) => f.DayProperty);

            var actual = mapping.FormatValue(memberInfo, (int)Day.Saturday);

            Assert.Equal("saturday", actual);
        }

        [Fact]
        public static void Format_WithEnumPropertyThrowsArgumentOutOfRangeWithUndefinedEnum()
        {
            var mapping = new ElasticMapping(enumFormat: EnumFormat.String);
            var memberInfo = TypeHelper.GetMemberInfo((FormatClass f) => f.DayProperty);

            Assert.Throws<ArgumentOutOfRangeException>(() => mapping.FormatValue(memberInfo, 127));
        }

        [Fact]
        public static void Format_WithEnumPropertyProducesIntWhenMappingSpecifiesIntFormat()
        {
            var mapping = new ElasticMapping(enumFormat: EnumFormat.Integer);
            var memberInfo = TypeHelper.GetMemberInfo((FormatClass f) => f.DayProperty);

            var actual = mapping.FormatValue(memberInfo, (int)Day.Saturday);

            Assert.Equal((int)Day.Saturday, actual);
        }
    }
}