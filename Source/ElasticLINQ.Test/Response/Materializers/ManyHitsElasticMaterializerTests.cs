// Licensed under the Apache 2.0 License. See LICENSE.txt in the project root for more information.

using System;
using ElasticLinq.Response.Materializers;
using System.Collections.Generic;
using ElasticLinq.Response.Model;
using Xunit;

namespace ElasticLinq.Test.Response.Materializers
{
    public class ManyHitsElasticMaterializerTests
    {
        [Fact]
        public void ManyOfTMaterializesObjects()
        {
            var hits = MaterializerTestHelper.CreateSampleHits(3);
            var materialized = ManyHitsElasticMaterializer.Many<SampleClass>(hits, MaterializerTestHelper.ItemCreator);

            Assert.Equal(hits.Count, materialized.Count);
            var index = 0;
            foreach (var hit in hits)
                Assert.Equal(hit.fields["someField"], materialized[index++].SampleField);
        }

        [Fact]
        public void ManyMaterializesObjects()
        {
            var response = MaterializerTestHelper.CreateSampleResponse(10);
            var expected = response.hits.hits;

            var materializer = new ManyHitsElasticMaterializer(MaterializerTestHelper.ItemCreator, typeof(SampleClass));
            var actual = materializer.Materialize(response);

            var actualList = Assert.IsType<List<SampleClass>>(actual);

            Assert.Equal(expected.Count, actualList.Count);
            var index = 0;
            foreach (var hit in expected)
                Assert.Equal(hit.fields["someField"], actualList[index++].SampleField);
        }

        [Fact]
        public void MaterializeThrowsArgumentNullExceptionWhenElasticResponseIsNull()
        {
            var materializer = new ManyHitsElasticMaterializer(MaterializerTestHelper.ItemCreator, typeof(SampleClass));

            Assert.Throws<ArgumentNullException>(() => materializer.Materialize(null));
        }

        [Fact]
        public void MaterializeReturnsEmptyListWhenHitsIsNull()
        {
            var materializer = new ManyHitsElasticMaterializer(MaterializerTestHelper.ItemCreator, typeof(SampleClass));
            var response = new ElasticResponse { hits = null };

            var materialized = materializer.Materialize(response);

            var materializedList = Assert.IsType<List<SampleClass>>(materialized);
            Assert.Empty(materializedList);
        }

        [Fact]
        public void MaterializeReturnsEmptyListWhenHitsHitsAreNull()
        {
            var materializer = new ManyHitsElasticMaterializer(MaterializerTestHelper.ItemCreator, typeof(SampleClass));
            var response = new ElasticResponse { hits = new Hits { hits = null } };

            var materialized = materializer.Materialize(response);

            var materializedList = Assert.IsType<List<SampleClass>>(materialized);
            Assert.Empty(materializedList);
        }

        [Fact]
        public void MaterializeReturnsEmptyListWhenHitsHitsAreEmpty()
        {
            var materializer = new ManyHitsElasticMaterializer(MaterializerTestHelper.ItemCreator, typeof(SampleClass));
            var response = new ElasticResponse { hits = null };

            var materialized = materializer.Materialize(response);

            var materializedList = Assert.IsType<List<SampleClass>>(materialized);
            Assert.Empty(materializedList);
        }
    }
}