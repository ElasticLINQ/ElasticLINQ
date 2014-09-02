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
        public void CountMaterializerReturnsZeroCount()
        {
            const int expected = 0;
            var response = new ElasticResponse { hits = new Hits { hits = new List<Hit>(), total = expected } };
            var materializer = new CountElasticMaterializer();

            var actual = materializer.Materialize(response);

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void CountMaterializerReturnsIntCount()
        {
            const int expected = int.MaxValue;
            var response = new ElasticResponse { hits = new Hits { hits = new List<Hit>(), total = expected } };
            var materializer = new CountElasticMaterializer();

            var actual = materializer.Materialize(response);

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void CountMaterializerReturnsLongCount()
        {
            const long expected = ((long)int.MaxValue) + 1;
            var response = new ElasticResponse { hits = new Hits { hits = new List<Hit>(), total = expected } };
            var materializer = new CountElasticMaterializer();

            var actual = materializer.Materialize(response);

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void CountMaterializerThrowsForNegativeCount()
        {
            var response = new ElasticResponse { hits = new Hits { hits = new List<Hit>(), total = -1 } };
            var materializer = new CountElasticMaterializer();

            Assert.Throws<ArgumentOutOfRangeException>(() => materializer.Materialize(response));
        }
    }
}