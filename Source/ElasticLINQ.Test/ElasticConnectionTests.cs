// Copyright (c) Tier 3 Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 

using System;
using Xunit;

namespace ElasticLinq.Test
{
    public class ElasticConnectionTests
    {
        private readonly Uri endpoint = new Uri("http://localhost:1234/abc");
        private readonly TimeSpan timeout = TimeSpan.FromSeconds(19.2);
        private const string Index = "myIndex";

        [Fact]
        public void ConstructorSetsPropertiesFromParameters()
        {
            var connection = new ElasticConnection(endpoint, timeout, Index);

            Assert.Equal(endpoint, connection.Endpoint);
            Assert.Equal(timeout, connection.Timeout);
            Assert.Equal(Index, connection.Index);
        }

        [Fact]
        public void ConstructorThrowsArgumentNullExceptionWhenEndpointIsNull()
        {
            Assert.Throws<ArgumentNullException>(() => new ElasticConnection(null, timeout));
        }

        [Fact]
        public void ConstructorThrowsArgumentOutOfRangeExceptionWhenTimespanLessThanZero()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => new ElasticConnection(endpoint, TimeSpan.FromDays(-1), Index));
        }


        [Fact]
        public void ConstructorThrowsArgumentNullExceptionWhenIndexSuppliedButNull()
        {
            Assert.Throws<ArgumentNullException>(() => new ElasticConnection(endpoint, timeout, null));
        }

        [Fact]
        public void ConstructorThrowsArgumentExceptionWhenIndexSuppliedButBlank()
        {
            Assert.Throws<ArgumentException>(() => new ElasticConnection(endpoint, timeout, ""));
        }
    }
}