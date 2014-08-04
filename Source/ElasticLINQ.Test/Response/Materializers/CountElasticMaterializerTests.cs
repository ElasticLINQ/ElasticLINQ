using ElasticLinq.Response.Materializers;
using Xunit;

namespace ElasticLinq.Test.Response.Materializers
{
    public class CountElasticMaterializerTests
    {
        [Fact]
        public void FirstOrDefaultReturnsDefaultGivenNoResults()
        {
            const int expected = 5;
            var response = MaterializerTestHelper.CreateSampleResponse(expected);
            var materializer = new CountElasticMaterializer();

            var actual = materializer.Materialize(response);

            Assert.Equal(expected, actual);
        }        
    }
}