using System;
using System.Collections.Generic;
using ElasticLinq.Response.Materializers;
using ElasticLinq.Response.Model;
using Xunit;

namespace ElasticLinq.Test.Response.Materializers
{
    public class AnyElasticMaterializerTests
    {
        [Fact]
        public void ReturnsFalseWhenNoHits()
        {
            const int total = 0;
            var response = new ElasticResponse { hits = new Hits { hits = new List<Hit>(), total = total } };
            var materializer = new AnyElasticMaterializer();

            var actual = materializer.Materialize(response);

            Assert.Equal(false, actual);
        }

        [Theory]
        [InlineData(1)]
        [InlineData(2)]
        public void ReturnsTrueWhenOneOrMoreHits(int total)
        {
            var response = new ElasticResponse { hits = new Hits { hits = new List<Hit>(), total = total } };
            var materializer = new AnyElasticMaterializer();

            var actual = materializer.Materialize(response);

            Assert.Equal(true, actual);
        }

        [Fact]
        public void ThrowsWhenNegativeHits()
        {
            const int total = -1;
            var response = new ElasticResponse { hits = new Hits { hits = new List<Hit>(), total = total } };
            var materializer = new AnyElasticMaterializer();

            Assert.Throws<ArgumentOutOfRangeException>(() => materializer.Materialize(response));
        }
    }
}