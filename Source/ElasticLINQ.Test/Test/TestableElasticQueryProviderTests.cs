// Licensed under the Apache 2.0 License. See LICENSE.txt in the project root for more information.

using System;
using System.Linq.Expressions;
using Xunit;

namespace ElasticLinq.Test.Test
{
    public class TestableElasticQueryProviderTests
    {
        private class Sample { public string Text { get; set; } }

        [Fact]
        public void CreateQueryCreatesTestableElasticQueryOfObjectWithGivenExpression()
        {
            var expectedContext = new TestableElasticContext();
            var expectedExpression = Expression.Constant("weebl");
            var provider = new TestableElasticQueryProvider(expectedContext);

            var query = provider.CreateQuery(expectedExpression);

            Assert.IsType<TestableElasticQuery<object>>(query);
            Assert.Same(expectedExpression, query.Expression);
        }

        [Fact]
        public void CreateQueryOfTCreatesTestableElasticQueryOfTWithGivenExpression()
        {
            var expectedContext = new TestableElasticContext();
            var expectedExpression = Expression.Constant("weebl");
            var provider = new TestableElasticQueryProvider(expectedContext);

            var query = provider.CreateQuery<Sample>(expectedExpression);

            Assert.IsType<TestableElasticQuery<Sample>>(query);
            Assert.Same(expectedExpression, query.Expression);
        }

        [Fact]
        public void ExecuteCompilesAndDynamicallyInvokesExpression()
        {
            const string expectedResult = "IwasEvaluated";
            var expression = Expression.Constant(expectedResult);

            var expectedContext = new TestableElasticContext();
            var provider = new TestableElasticQueryProvider(expectedContext);

            var actual = provider.Execute(expression);

            Assert.Same(expectedResult, actual);
        }

        [Fact]
        public void ExecuteOfTCompilesAndDynamicallyInvokesExpression()
        {
            const string expectedResult = "IwasEvaluated";
            var expression = Expression.Constant(expectedResult);

            var expectedContext = new TestableElasticContext();
            var provider = new TestableElasticQueryProvider(expectedContext);

            var actual = provider.Execute<string>(expression);

            Assert.Same(expectedResult, actual);
        }
    }
}