// Copyright (c) Tier 3 Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 

using ElasticLinq.Request;
using ElasticLinq.Utility;
using System;
using System.Diagnostics.CodeAnalysis;
using Xunit;

namespace ElasticLinq.Test.Request
{
    public class ElasticRequestProcessorTests
    {
        private static readonly ElasticConnection connection = new ElasticConnection(new Uri("http://localhost"), TimeSpan.FromSeconds(10));

        [Fact]
        [ExcludeFromCodeCoverage] // Expression isn't "executed"
        public void ConstructorThrowsArgumentNullExceptionWhenConnectionIsNull()
        {
            var textWriter = new NullTextWriter();
            Assert.Throws<ArgumentNullException>(() => new ElasticRequestProcessor(null, textWriter));
        }

        [Fact]
        [ExcludeFromCodeCoverage] // Expression isn't "executed"
        public void ConstructorDoesntThrowWithValidParameters()
        {
            var textWriter = new NullTextWriter();
            Assert.DoesNotThrow(() => new ElasticRequestProcessor(connection, textWriter));
        }

    }
}