// Copyright (c) Tier 3 Inc. All rights reserved.
// This source code is made available under the terms of the Microsoft Public License (MS-PL)

using ElasticLinq.Utility;
using System.Collections.Generic;
using Xunit;

namespace ElasticLINQ.Test.Utility
{
    public class StaticConstructorsTests
    {
        [Fact]
        public void KeyValuePairCreateProducesCorrectType()
        {
            var actual = KeyValuePair.Create("", 1);
            Assert.IsType<KeyValuePair<string, int>>(actual);
        }

        [Fact]
        public void KeyValuePairCreateSetsCorrectValues()
        {
            const decimal expectedDecimal = 42.0m; 
            const string expectedString = "Deep Thought";

            var actual = KeyValuePair.Create(expectedDecimal, expectedString);

            Assert.Equal(expectedDecimal, actual.Key);
            Assert.Equal(expectedString, actual.Value);
        }
    }
}