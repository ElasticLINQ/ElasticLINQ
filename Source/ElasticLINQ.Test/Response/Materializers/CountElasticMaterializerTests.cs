// Licensed under the Apache 2.0 License. See LICENSE.txt in the project root for more information.

using System;
using System.Collections.Generic;
using ElasticLinq.Response.Materializers;
using ElasticLinq.Response.Model;
using Xunit;

namespace ElasticLinq.Test.Response.Materializers
{
    public class CountElasticMaterializerTests
    {
        [Fact]
        public void CountMaterializerReturnsIntCount()
        {
            const int expected = int.MaxValue;
            var response = new ElasticResponse { hits = new Hits { hits = new List<Hit>(), total = expected } };
            var materializer = new CountElasticMaterializer(typeof(int));

            var actual = materializer.Materialize(response);

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void CountMaterializerReturnsLongCount()
        {
            const long expected = (long)int.MaxValue + 1;
            var response = new ElasticResponse { hits = new Hits { hits = new List<Hit>(), total = expected } };
            var materializer = new CountElasticMaterializer(typeof(long));

            var actual = materializer.Materialize(response);

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void CountMaterializerThrowsForNegativeCount()
        {
            var response = new ElasticResponse { hits = new Hits { hits = new List<Hit>(), total = -1 } };
            var materializer = new CountElasticMaterializer(typeof(int));

            Assert.Throws<ArgumentOutOfRangeException>(() => materializer.Materialize(response));
        }
    }
}