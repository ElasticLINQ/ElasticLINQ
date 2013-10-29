// Copyright (c) Tier 3 Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 

using ElasticLinq.Request;
using ElasticLinq.Utility;
using System;
using Xunit;

namespace ElasticLinq.Test.Request
{
    public class ElasticRequestProcessorTests
    {
        [Fact]
        public void ConstructorThrowsArgumentNullExceptionWhenConnectionIsNull()
        {
            var textWriter = new NullTextWriter();
            Assert.Throws<ArgumentNullException>(() => new ElasticRequestProcessor(null, textWriter));
        }
    }
}