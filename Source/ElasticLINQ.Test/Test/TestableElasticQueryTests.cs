using System.Linq.Expressions;
using Xunit;

namespace ElasticLinq.Test.Test
{
    public class TestableElasticQueryTests
    {
        [Fact]
        public void ConstructorWithAllArgSetsCorrectProperties()
        {
            var expectedContext = new TestableElasticContext();

            var query = new TestableElasticQuery<FakeClass>(expectedContext);

            Assert.Equal(expectedContext, query.Context);
            Assert.Equal(typeof(FakeClass), query.ElementType);
            Assert.Equal(expectedContext.Provider, query.Provider);
            Assert.IsType<ConstantExpression>(query.Expression);
        }

        class FakeClass
        {       
        }
    }
}