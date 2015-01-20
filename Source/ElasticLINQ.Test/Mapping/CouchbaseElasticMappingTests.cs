// Licensed under the Apache 2.0 License. See LICENSE.txt in the project root for more information.

using ElasticLinq.Mapping;
using ElasticLinq.Request.Criteria;
using System;
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
        [Fact]
        public static void GetDocumentMappingPrefixReturnsDoc()
        {
            var mapping = new CouchbaseElasticMapping();

            var result = mapping.GetDocumentMappingPrefix(null);

            Assert.Equal("doc", result);
        }

        [Fact]
        public static void GetDocumentTypeIsCouchbaseDocument()
        {
            var mapping = new CouchbaseElasticMapping();

            var result = mapping.GetDocumentType(null);

            Assert.Equal("couchbaseDocument", result);
        }

        [Fact]
        public static void GetTypeExistsCriteriaReturnsValueType()
        {
            var mapping = new CouchbaseElasticMapping();

            var result = mapping.GetTypeExistsCriteria(typeof(ClassWithValueType));

            var existsCriteria = Assert.IsType<ExistsCriteria>(result);
            Assert.Equal("doc.candidate", existsCriteria.Field);
        }

        [Fact]
        public static void GetTypeExistsCriteriaThrowsInvalidOperationIfClassHasNoRequiredProperties()
        {
            var mapping = new CouchbaseElasticMapping();

            Assert.Throws<InvalidOperationException>(() => mapping.GetTypeExistsCriteria(typeof(ClassWithNoRequiredProperties)));
        }
    }
}