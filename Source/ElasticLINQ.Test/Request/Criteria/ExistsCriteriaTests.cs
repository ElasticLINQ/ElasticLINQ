// Copyright (c) Tier 3 Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 

using ElasticLinq.Request.Criteria;
using System;
using System.Diagnostics.CodeAnalysis;
using Xunit;

namespace ElasticLinq.Test.Request.Criteria
{
    public class ExistsCriteriaTests
    {
        [Fact]
        public void NamePropertyIsExists()
        {
            var criteria = new ExistsCriteria("something");

            Assert.Equal("exists", criteria.Name);
        }

        [Fact]
        public void ConstructorSetsCriteria()
        {
            const string field = "myField";

            var criteria = new ExistsCriteria(field);

            Assert.Equal(field, criteria.Field);
        }

        [Fact]
        [ExcludeFromCodeCoverage] // Expression isn't "executed"
        public void ConstructorThrowsArgumentNullExceptionWhenFieldIsNull()
        {
            Assert.Throws<ArgumentNullException>(() => new ExistsCriteria(null));
        }

        [Fact]
        [ExcludeFromCodeCoverage] // Expression isn't "executed"
        public void ConstructorThrowsArgumentExceptionWhenFieldIsBlank()
        {
            Assert.Throws<ArgumentException>(() => new ExistsCriteria(" "));
        }
   }
}