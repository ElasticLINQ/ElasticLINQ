// Copyright (c) Tier 3 Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 

using ElasticLinq.Request;
using ElasticLinq.Request.Visitors;
using ElasticLinq.Response.Model;
using System;
using Xunit;

namespace ElasticLinq.Test.Request.Visitors
{
    public class ElasticTranslateResultTests
    {
        [Fact]
        public void ConstructorSetsProperties()
        {
            var expectedSearch = new ElasticSearchRequest("someType");
            Func<Hit, object> expectedProjector = h => null;

            var result = new ElasticTranslateResult(expectedSearch, expectedProjector);

            Assert.Same(expectedSearch, result.SearchRequest);
            Assert.Same(expectedProjector, result.Projector);
        }
    }
}