// Copyright (c) Tier 3 Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 

using ElasticLinq.Request;
using Xunit;

namespace ElasticLinq.Test.Request
{
    public class ElasticSearchRequestTests
    {
        [Fact]
        public void ConstructorSetsPropertiesForRequiredParameters()
        {
            const string expectedType = "myType";

            var request = new ElasticSearchRequest(expectedType);

            Assert.Equal(expectedType, request.Type);
        }

        [Fact]
        public void ConstructorHasSensibleDefaultValues()
        {
            var request = new ElasticSearchRequest("myType");

            Assert.Equal(0, request.From);
            Assert.Null(request.Size);
            Assert.Empty(request.Fields);
            Assert.Empty(request.SortOptions);
            Assert.Null(request.Filter);
        }

        [Fact]
        public void ConstructorSetsPropertiesForOptionalParameters()
        {
            const int expectedFrom = 101;
            var request = new ElasticSearchRequest("myType", expectedFrom);

            Assert.Equal(expectedFrom, request.From);
        }
    }
}