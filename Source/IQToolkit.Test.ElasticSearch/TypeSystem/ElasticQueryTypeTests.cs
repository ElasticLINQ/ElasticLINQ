// Copyright (c) Tier 3 Inc. All rights reserved.
// This source code is made available under the terms of the Microsoft Public License (MS-PL)

using IQToolkit.Data.ElasticSearch.TypeSystem;
using Xunit;

namespace IQToolkit.Test.ElasticSearch.TypeSystem
{
    public class ElasticQueryTypeTests
    {
        [Fact]
        public void ConstructorSetsProperties()
        {
            const bool notNull = true;
            const int length = 1234;
            const short precision = 18; const short scale = 9;

            var actual = new ElasticQueryType(notNull, length, precision, scale);

            Assert.Equal(notNull, actual.NotNull);
            Assert.Equal(length, actual.Length);
            Assert.Equal(precision, actual.Precision); 
            Assert.Equal(scale, actual.Scale);
        }
    }
}
