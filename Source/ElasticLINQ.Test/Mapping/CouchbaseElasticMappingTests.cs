// Copyright (c) Tier 3 Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 

using System;
using ElasticLinq.Mapping;
using System.Reflection;
using ElasticLinq.Response.Model;
using Newtonsoft.Json.Linq;
using Xunit;

namespace ElasticLINQ.Test.Mapping
{
    public class CouchbaseElasticMappingTests
    {
        private const string DefaultPrefix = "doc.couchbaseElasticMappingTests.";

        [Fact]
        public void GetTypeNameIsCouchbaseDocumentByDefault()
        {
            const string expected = "couchbaseDocument";

            var mapping = new CouchbaseElasticMapping();

            Assert.Equal(expected, mapping.GetTypeName(typeof(CouchbaseElasticMappingTests)));
        }

        [Fact]
        public void ConstructorTypeNameOverridesGetTypeName()
        {
            const string expected = "myTypeName";

            var mapping = new CouchbaseElasticMapping(expected);

            Assert.Equal(expected, mapping.GetTypeName(typeof(CouchbaseElasticMappingTests)));
        }

        [Fact]
        public void GetFieldNamePrefixesAndCamelCasesMemberName()
        {
            var memberInfo = MethodBase.GetCurrentMethod();

            var mapping = new CouchbaseElasticMapping();
            var actual = mapping.GetFieldName(memberInfo);

            Assert.Equal(DefaultPrefix + "getFieldNamePrefixesAndCamelCasesMemberName", actual);
        }

        [Fact]
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

        private class Sample { };

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
    }
}