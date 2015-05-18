using ElasticLinq.Response.Materializers;
using ElasticLinq.Response.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
            var respomse = MaterializerTestHelper.CreateSampleResponse(1);
            var materializer = new HighlightElasticMaterializer(null);

            Assert.Throws<ArgumentNullException>(()=>materializer.Materialize(respomse));
        }

        

        [Fact]
        public void HighlightMaterializerTests_Must_recognize_Highlighted_Result()
        {
            var respomse = MaterializerTestHelper.CreateSampleResponseWithHighlight(1);
            var materializer = new HighlightElasticMaterializer(new ListHitsElasticMaterializer(DefaultBySourceItemCreator, typeof(SampleClassWithHighlight)));

            var result = materializer.Materialize(respomse);

            var actualList = Assert.IsType<List<SampleClassWithHighlight>>(result);
            var highlighted = actualList.First().SampleField_Highlight;
            Assert.NotNull(highlighted);
        }

    }
}
