// Licensed under the Apache 2.0 License. See LICENSE.txt in the project root for more information.

using ElasticLinq.Response.Materializers;
using System.Collections.Generic;
using Xunit;

namespace ElasticLinq.Test.Response.Materializers
{
    public class ManyHitsElasticMaterializerTests
    {
        [Fact]
        public void ManyOfTMaterializesObjects()
        {
            var hits = MaterializerTestHelper.CreateSampleHits(3);
            var materialized = ElasticManyHitsMaterializer.Many<SampleClass>(hits, MaterializerTestHelper.ItemCreator);

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

            var materializer = new ElasticManyHitsMaterializer(MaterializerTestHelper.ItemCreator, typeof(SampleClass));
            var actual = materializer.Materialize(response);

            var actualList = Assert.IsType<List<SampleClass>>(actual);

            Assert.Equal(expected.Count, actualList.Count);
            var index = 0;
            foreach (var hit in expected)
                Assert.Equal(hit.fields["someField"], actualList[index++].SampleField);
        }
    }
}