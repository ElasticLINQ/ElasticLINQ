// Licensed under the Apache 2.0 License. See LICENSE.txt in the project root for more information.

using ElasticLinq.Response.Materializers;
using ElasticLinq.Response.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace ElasticLinq.Test.Response.Materializers
{
    public class HighlightMaterializerTests
    {
        public class SampleClassWithHighlight : SampleClass
        {
            public List<string> SampleField_Highlight { get; set; }
        }

        public static readonly Func<Hit, SampleClassWithHighlight> DefaultBySourceItemCreator =
            h => h._source.ToObject<SampleClassWithHighlight>();

        [Fact]
        public void HighlightMaterializerTests_Throws_If_Not_Chained()
        {
            var response = MaterializerTestHelper.CreateSampleResponse(1);
            var materializer = new HighlightElasticMaterializer(null);

            Assert.Throws<ArgumentNullException>(()=>materializer.Materialize(response));
        }
        
        [Fact]
        public void HighlightMaterializerTests_Must_Recognize_Highlighted_Result()
        {
            var response = MaterializerTestHelper.CreateSampleResponseWithHighlight(1);
            var materializer = new HighlightElasticMaterializer(new ListHitsElasticMaterializer(DefaultBySourceItemCreator, typeof(SampleClassWithHighlight)));

            var result = materializer.Materialize(response);

            var actualList = Assert.IsAssignableFrom<IEnumerable<SampleClassWithHighlight>>(result);
            var highlighted = actualList.First().SampleField_Highlight;
            Assert.NotNull(highlighted);
        }

    }
}
