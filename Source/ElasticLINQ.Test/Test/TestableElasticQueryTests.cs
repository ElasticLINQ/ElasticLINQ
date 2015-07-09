// Licensed under the Apache 2.0 License. See LICENSE.txt in the project root for more information.

using System.Collections;
using System.Linq;
using System.Linq.Expressions;
using Xunit;

namespace ElasticLinq.Test.Test
{
    public class TestableElasticQueryTests
    {
        [Fact]
        public static void ConstructorWithAllArgSetsCorrectProperties()
        {
            var expectedExpression = Expression.Constant("oh hai");
            var context = new TestableElasticContext();

            var query = new TestableElasticQuery<FakeClass>(context, expectedExpression);

            Assert.Equal(context, query.Context);
            Assert.Equal(typeof(FakeClass), query.ElementType);
            Assert.Equal(context.Provider, query.Provider);
            Assert.Equal(expectedExpression, query.Expression);
        }

        [Fact]
        public static void ConstructorDefaultsToConstantExpressionIfNotSpecified()
        {
            var context = new TestableElasticContext();

            var query = new TestableElasticQuery<FakeClass>(context);

            Assert.IsType<ConstantExpression>(query.Expression);
        }

        [Fact]
        public static void ImplicitGetEnumeratorCapturesQueryOnContext()
        {
            const string expectedBody = "requestwascaptured";
            var context = new TestableElasticContext();

            var dummy = new TestableElasticQuery<FakeClass>(context).Where(f => f.Name == expectedBody).ToArray();

            Assert.Equal(1, context.Requests.Count);
            Assert.Contains(expectedBody, context.Requests[0].Query);
        }

        [Fact]
        public static void ExplicitGetEnumeratorCapturesQueryOnContext()
        {
            var context = new TestableElasticContext();

            var dummy = ((IEnumerable) new TestableElasticQuery<FakeClass>(context).Where(f => f.Name == "a")).GetEnumerator();

            Assert.Equal(1, context.Requests.Count);
        }

        class FakeClass
        {
            public string Name { get; set; }
        }
    }
}