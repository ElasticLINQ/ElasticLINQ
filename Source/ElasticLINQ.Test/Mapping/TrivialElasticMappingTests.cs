// Copyright (c) Tier 3 Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 

using ElasticLinq.Mapping;
using ElasticLinq.Response.Model;
using Newtonsoft.Json.Linq;
using System;
using System.Reflection;
using Xunit;

namespace ElasticLINQ.Test.Mapping
{
    public class TrivialElasticMappingTests
    {
        private class SingularTypeName { }
        private class PluralTypeNames { }

        [Fact]
        public void GetFieldNameCamelCasesMemberName()
        {
            var memberInfo = MethodBase.GetCurrentMethod();
            var mapping = new TrivialElasticMapping();

            var actual = mapping.GetFieldName(memberInfo);

            Assert.Equal("getFieldNameCamelCasesMemberName", actual);
        }

        [Fact]
        public void GetFieldNameThrowsArgumentNullExceptionWhenMemberInfoIsNull()
        {
            var mapping = new TrivialElasticMapping();

            Assert.Throws<ArgumentNullException>(() => mapping.GetFieldName(null));
        }

        [Fact]
        public void GetTypeNameCamelCasesAndPluralizesSingularTypeName()
        {
            var type = typeof(SingularTypeName);
            var mapping = new TrivialElasticMapping();

            var actual = mapping.GetTypeName(type);

            Assert.Equal("singularTypeNames", actual);
        }

        [Fact]
        public void GetTypeNameCamelCasesPluralTypeName()
        {
            var type = typeof(PluralTypeNames);
            var mapping = new TrivialElasticMapping();

            var actual = mapping.GetTypeName(type);

            Assert.Equal("pluralTypeNames", actual);
        }

        [Fact]
        public void GetTypeNameThrowsArgumentNullExceptionWhenMemberInfoIsNull()
        {
            var mapping = new TrivialElasticMapping();

            Assert.Throws<ArgumentNullException>(() => mapping.GetTypeName(null));
        }

        [Fact]
        public void GetObjectSourceReturnsHitSourceField()
        {
            var expected = new JObject(new JProperty("Id", "ExpectedSource"));
            var hit = new Hit { _source = expected };
            var mapping = new TrivialElasticMapping();

            var actual = mapping.GetObjectSource(typeof(Hit), hit);

            Assert.Same(expected, actual);
        }

        [Fact]
        public void GetObjectSourceThrowsArgumentNullExceptionWhenHitIsNull()
        {
            var mapping = new TrivialElasticMapping();

            Assert.Throws<ArgumentNullException>(() => mapping.GetObjectSource(typeof(Hit), null));
        }
    }
}