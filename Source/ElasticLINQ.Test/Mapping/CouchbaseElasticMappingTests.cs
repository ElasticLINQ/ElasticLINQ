// Licensed under the Apache 2.0 License. See LICENSE.txt in the project root for more information.

using ElasticLinq.Mapping;
using ElasticLinq.Request.Criteria;
using ElasticLinq.Response.Model;
using Newtonsoft.Json.Linq;
using System;
using System.Diagnostics.CodeAnalysis;
using Xunit;

namespace ElasticLinq.Test.Mapping
{
    public class CouchbaseElasticMappingTests
    {
        private class Sample
        {
            public int PropertyOfSample { get; set; }
        };

        public string SampleProperty { get; set; }

        [Fact]
        public void GetTypeNameIsCouchbaseDocumentByDefault()
        {
            const string expected = "couchbaseDocument";

            var mapping = new CouchbaseElasticMapping();

            Assert.Equal(expected, mapping.GetTypeName(typeof(CouchbaseElasticMappingTests)));
        }

        [Fact]
        public void ConstructorThrowsArgumentNullExceptionIfTypeNameIsNull()
        {
            Assert.Throws<ArgumentNullException>(() => new CouchbaseElasticMapping(null, false));
        }

        [Fact]
        public void ConstructorThrowsArgumentExceptionIfTypeNameIsEmpty()
        {
            Assert.Throws<ArgumentException>(() => new CouchbaseElasticMapping("", false));
        }

        [Fact]
        public void ConstructorTypeNameOverridesGetTypeName()
        {
            const string expected = "myTypeName";

            var mapping = new CouchbaseElasticMapping(expected, false);

            Assert.Equal(expected, mapping.GetTypeName(typeof(CouchbaseElasticMappingTests)));
        }

        [Fact]
        public void GetFieldNameDoesNotIncludeTypeNameByDefault()
        {
            var propertyInfo = GetType().GetProperty("SampleProperty");

            var mapping = new CouchbaseElasticMapping();
            var actual = mapping.GetFieldName(propertyInfo);

            Assert.Equal("doc.couchbaseElasticMappingTests.sampleProperty", actual);
        }

        [Fact]
        public void GetFieldNameDoesNotIncludeTypeNameWhenRequestedNotTo()
        {
            var propertyInfo = GetType().GetProperty("SampleProperty");

            var mapping = new CouchbaseElasticMapping("shouldNotInclude", false);
            var actual = mapping.GetFieldName(propertyInfo);

            Assert.Equal("doc.couchbaseElasticMappingTests.sampleProperty", actual);
        }

        [Fact]
        public void GetFieldNameDoesIncludeTypeNameWhenRequestedTo()
        {
            var propertyInfo = GetType().GetProperty("SampleProperty");

            var mapping = new CouchbaseElasticMapping("myTypeName", true);
            var actual = mapping.GetFieldName(propertyInfo);

            Assert.Equal("myTypeName.doc.couchbaseElasticMappingTests.sampleProperty", actual);
        }

        [Fact]
        public void GetFieldNamePrefixesAndCamelCasesMemberName()
        {
            var propertyInfo = GetType().GetProperty("SampleProperty");

            var mapping = new CouchbaseElasticMapping();
            var actual = mapping.GetFieldName(propertyInfo);

            Assert.Equal("doc.couchbaseElasticMappingTests.sampleProperty", actual);
        }

        [Fact]
        [ExcludeFromCodeCoverage] // Expression isn't "executed"
        public void GetFieldNameThrowsArgumentNullExceptionWhenMemberInfoIsNull()
        {
            var mapping = new CouchbaseElasticMapping();

            Assert.Throws<ArgumentNullException>(() => mapping.GetFieldName(null));
        }

        [Fact]
        public void GetObjectSourceReturnsCorrectObjectFromHitSource()
        {
            var expected = new JObject(new JProperty("ThisIs", "MyDocument"));
            var hit = new Hit
            {
                _source = new JObject(
                    new JProperty("doc", new JObject(
                    new JProperty("sample", expected))))
            };

            var actual = new CouchbaseElasticMapping().GetObjectSource(typeof(Sample), hit);

            Assert.Same(expected, actual);
        }

        [Fact]
        public void GetObjectSourceThrowsArgumentNullExceptionWhenTypeIsNull()
        {
            var mapping = new CouchbaseElasticMapping();

            Assert.Throws<ArgumentNullException>(() => mapping.GetObjectSource(null, new Hit()));
        }

        [Fact]
        public void GetObjectSourceThrowsArgumentNullExceptionWhenHitIsNull()
        {
            var mapping = new CouchbaseElasticMapping();

            Assert.Throws<ArgumentNullException>(() => mapping.GetObjectSource(typeof(Hit), null));
        }

        [Fact]
        public void GetTypeSelectionCriteriaReturnsExistsWithFullyQualifiedField()
        {
            var mapping = new CouchbaseElasticMapping("myType", true);

            var criteria = mapping.GetTypeSelectionCriteria(typeof(Sample));

            Assert.IsType<ExistsCriteria>(criteria);
            var existsCriteria = (ExistsCriteria)criteria;

            Assert.Equal("myType.doc.sample.propertyOfSample", existsCriteria.Field);
        }

        [Fact]
        public void GetTypeSelectionCriteriaReturnsExistsWithPartiallyQualifiedField()
        {
            var mapping = new CouchbaseElasticMapping();

            var criteria = mapping.GetTypeSelectionCriteria(typeof(Sample));

            Assert.IsType<ExistsCriteria>(criteria);
            var existsCriteria = (ExistsCriteria)criteria;

            Assert.Equal("doc.sample.propertyOfSample", existsCriteria.Field);
        }

    }
}