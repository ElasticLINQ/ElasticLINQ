// Copyright (c) Tier 3 Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 

using System.Collections;
using System.Collections.Generic;
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
            Func<IList, object> expectedFinalTransform = a => a;

            var result = new ElasticTranslateResult(expectedSearch, expectedProjector, expectedFinalTransform);

            Assert.Same(expectedSearch, result.SearchRequest);
            Assert.Same(expectedProjector, result.Projector);
            Assert.Same(expectedFinalTransform, result.FinalTransform);

            Assert.Null(expectedProjector.Invoke(new Hit())); // For code coverage only
            Assert.Null(expectedFinalTransform.Invoke(null)); // For code coverage only
        }

        [Fact]
        public void ConstructorDefaultsFinalTransformToNonTransforming()
        {
            var expectedSearch = new ElasticSearchRequest("someType");
            Func<Hit, object> expectedProjector = h => null;

            var result = new ElasticTranslateResult(expectedSearch, expectedProjector);

            Assert.Same(expectedSearch, result.SearchRequest);
            Assert.Same(expectedProjector, result.Projector);
            
            Assert.NotNull(result.FinalTransform);
            var transforming = new List<string>();
            var afterTransforming = result.FinalTransform.Invoke(transforming);
            Assert.Same(transforming, afterTransforming);

            Assert.Null(expectedProjector.Invoke(new Hit())); // For code coverage only
        }
    }
}