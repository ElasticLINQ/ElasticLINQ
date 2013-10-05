// Copyright (c) Tier 3 Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 

using ElasticLinq.Mapping;
using System.Reflection;
using Xunit;

namespace ElasticLINQ.Test.Mapping
{
    public class TrivialElasticMappingTests
    {
        [Fact]
        public void GetFieldNameLowersMemberName()
        {
            var memberInfo = MethodBase.GetCurrentMethod();

            var mapping = new TrivialElasticMapping();
            var actual = mapping.GetFieldName(memberInfo);

            Assert.Equal(memberInfo.Name.ToLowerInvariant(), actual);
        }

        [Fact]
        public void GetTypeNameLowersAndPluralizesSingularTypeName()
        {
            var type = typeof(SingularTypeName);

            var mapping = new TrivialElasticMapping();
            var actual = mapping.GetTypeName(type);

            Assert.Equal(type.Name.ToLowerInvariant() + "s", actual);
        }

        private class SingularTypeName { }

        [Fact]
        public void GetTypeNameLowersPluralTypeName()
        {
            var type = typeof(SingularTypeNames);

            var mapping = new TrivialElasticMapping();
            var actual = mapping.GetTypeName(type);

            Assert.Equal(type.Name.ToLowerInvariant(), actual);
        }

        private class SingularTypeNames { }
    }
}