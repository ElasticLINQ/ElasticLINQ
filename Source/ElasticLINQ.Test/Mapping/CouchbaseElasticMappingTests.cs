// Licensed under the Apache 2.0 License. See LICENSE.txt in the project root for more information.

using ElasticLinq.Mapping;
using ElasticLinq.Request.Criteria;
using System;
using System.Linq.Expressions;
using Xunit;

namespace ElasticLinq.Test.Mapping
{
    class ClassWithValueType
    {
        public string Useless { get; set; }
        public Guid Candidate { get; set; }
    }

    class ClassWithNoRequiredProperties
    {
        public Guid? Id { get; set; }
        public string Useless { get; set; }
        public int? Something { get; set; }
    }

    public class CouchbaseElasticMappingTests
    {
        readonly static CouchbaseElasticMapping mapping = new CouchbaseElasticMapping();

        [Fact]
        public static void GetDocumentMappingPrefixReturnsDoc()
        {
            var result = mapping.GetDocumentMappingPrefix(null);

            Assert.Equal("doc", result);
        }

        [Fact]
        public static void GetDocumentTypeIsCouchbaseDocument()
        {
            var result = mapping.GetDocumentType(null);

            Assert.Equal("couchbaseDocument", result);
        }

        [Fact]
        public static void GetTypeExistsCriteriaReturnsValueType()
        {
            var result = mapping.GetTypeSelectionCriteria(typeof(ClassWithValueType));

            var existsCriteria = Assert.IsType<ExistsCriteria>(result);
            Assert.Equal("doc.candidate", existsCriteria.Field);
        }

        [Fact]
        public static void GetTypeExistsCriteriaThrowsInvalidOperationIfClassHasNoRequiredProperties()
        {
            Assert.Throws<InvalidOperationException>(() => mapping.GetTypeSelectionCriteria(typeof(ClassWithNoRequiredProperties)));
        }

        [Theory]
        [InlineData(false, "doc.Candidate")]
        [InlineData(true, "doc.candidate")]
        public static void GetFieldName_Correctly_Cases_And_Prefixes_Property_Name(bool camelCaseFieldNames, string expected)
        {
            Expression<Func<ClassWithValueType, Guid>> stringAccess = (ClassWithValueType s) => s.Candidate;

            var mapping = new CouchbaseElasticMapping(camelCaseFieldNames: camelCaseFieldNames);
            var actual = mapping.GetFieldName(typeof(ClassWithValueType), (MemberExpression)stringAccess.Body);

            Assert.Equal(expected, actual);
        }
    }
}