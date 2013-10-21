// Copyright (c) Tier 3 Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 

using ElasticLinq.Mapping;
using System.Reflection;
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
    }
}