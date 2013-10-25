// Copyright (c) Tier 3 Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 

using ElasticLinq;
using System;
using Xunit;

namespace ElasticLinq.Test
{
    public class ElasticConnectionTests
    {
        [Fact]
        public void ConstructorSetsProperties()
        {
            var expectedEndpoint = new Uri("http://localhost:1234/abc");
            var expectedTimeout = TimeSpan.FromSeconds(19.2);
            const string expectedIndex = "myIndex";
            const bool expectedPreferGetRequests = true;

            var connection = new ElasticConnection(expectedEndpoint, expectedTimeout, expectedIndex, expectedPreferGetRequests);

            Assert.Equal(expectedEndpoint, connection.Endpoint);
            Assert.Equal(expectedTimeout, connection.Timeout);
            Assert.Equal(expectedIndex, connection.Index);
            Assert.Equal(expectedPreferGetRequests, connection.PreferGetRequests);
        }
    }
}