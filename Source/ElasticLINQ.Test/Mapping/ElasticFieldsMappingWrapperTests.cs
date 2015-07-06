// Licensed under the Apache 2.0 License. See LICENSE.txt in the project root for more information.

using System.Linq.Expressions;
using ElasticLinq.Mapping;
using NSubstitute;
using Xunit;

namespace ElasticLinq.Test.Mapping
{
    public class ElasticFieldsMappingWrapperTests
    {
        [Fact]
        public static void FormatValue_PassesThrough()
        {
            var innerMapping = Substitute.For<IElasticMapping>();
            var mapping = new ElasticFieldsMappingWrapper(innerMapping);
            var memberInfo = typeof(string).GetProperty("Length");

            mapping.FormatValue(memberInfo, "abc");

            innerMapping.Received(1).FormatValue(memberInfo, "abc");
        }

        [Fact]
        public static void GetDocumentType_PassesThrough()
        {
            var innerMapping = Substitute.For<IElasticMapping>();
            var mapping = new ElasticFieldsMappingWrapper(innerMapping);
            var type = typeof(ElasticFieldsMappingWrapperTests);

            mapping.GetDocumentType(type);

            innerMapping.Received(1).GetDocumentType(type);
        }

        private class Sample { }

        [Theory]
        [InlineData("Id", "_id")]
        [InlineData("Score", "_score")]
        public static void GetFieldName_WithElasticFieldsMember_ReturnsRootedName(string propertyName, string expectedValue)
        {
            var innerMapping = Substitute.For<IElasticMapping>();
            var mapping = new ElasticFieldsMappingWrapper(innerMapping);
            var member = typeof(ElasticFields).GetProperty(propertyName);
            var memberExpression = Expression.MakeMemberAccess(null, member);

            var result = mapping.GetFieldName(typeof(Sample), memberExpression);

            Assert.Equal(expectedValue, result);
            innerMapping.Received(0).GetFieldName(typeof(Sample), memberExpression);
        }

        [Fact]
        public static void GetFieldName_WithNonElasticFieldsMember_PassesThrough()
        {
            var innerMapping = Substitute.For<IElasticMapping>();
            var mapping = new ElasticFieldsMappingWrapper(innerMapping);
            var member = typeof(string).GetProperty("Length");
            var constantExpression = Expression.Constant("string value");
            var memberExpression = Expression.MakeMemberAccess(constantExpression, member);

            mapping.GetFieldName(typeof(Sample), memberExpression);

            innerMapping.Received(1).GetFieldName(typeof(Sample), memberExpression);
        }

        [Fact]
        public static void GetTypeExistsCriteria_PassesThrough()
        {
            var innerMapping = Substitute.For<IElasticMapping>();
            var mapping = new ElasticFieldsMappingWrapper(innerMapping);
            var type = typeof(ElasticFieldsMappingWrapperTests);

            mapping.GetTypeSelectionCriteria(type);

            innerMapping.Received(1).GetTypeSelectionCriteria(type);
        }
    }
}