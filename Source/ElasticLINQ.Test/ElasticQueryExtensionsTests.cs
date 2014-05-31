// Licensed under the Apache 2.0 License. See LICENSE.txt in the project root for more information.

using ElasticLinq.Test.TestSupport;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Linq.Expressions;
using Xunit;

namespace ElasticLinq.Test
{
    public class ElasticQueryExtensionsTests
    {
        private class Sample
        {
            public string Property { get; set; }
        }

        [Fact]
        public void QueryIsAddedToExpressionTree()
        {
            var source = new FakeQueryProvider().CreateQuery<Sample>();
            var queried = source.Query(s => s.Property == "a");

            Assert.IsAssignableFrom<MethodCallExpression>(queried.Expression);
            var callExpression = (MethodCallExpression)queried.Expression;

            Assert.Equal(2, callExpression.Arguments.Count);
            Assert.Equal(ExpressionType.Quote, callExpression.Arguments[1].NodeType);
        }

        [Fact]
        public void QueryStringThrowsArgumentNullWhenArgumentIsNull()
        {
            var source = new FakeQueryProvider().CreateQuery<Sample>();
            Assert.Throws<ArgumentNullException>(() => source.QueryString(null));
        }

        [Fact]
        public void QueryStringIsAddedToExpressionTree()
        {
            const string expectedQueryString = "abcdef";
            var source = new FakeQueryProvider().CreateQuery<Sample>();
            var applied = source.QueryString(expectedQueryString);

            Assert.IsAssignableFrom<MethodCallExpression>(applied.Expression);
            var callExpression = (MethodCallExpression)applied.Expression;

            Assert.Equal(2, callExpression.Arguments.Count);
            var constantExpression = Assert.IsType<ConstantExpression>(callExpression.Arguments[1]);
            Assert.Equal(constantExpression.Value, expectedQueryString);
        }

        [Fact]
        public void OrderByScoreIsAddedToExpressionTree()
        {
            var source = new FakeQueryProvider().CreateQuery<Sample>();
            AssertIsAddedToExpressionTree<IQueryable<Sample>, Sample>(source, ElasticQueryExtensions.OrderByScore, "OrderByScore");
        }

        [Fact]
        public void OrderByScoreThrowArgumentNullExceptionIfSourceIsNull()
        {
            Assert.Throws<ArgumentNullException>(() => ElasticQueryExtensions.OrderByScore<Sample>(null));
        }

        [Fact]
        public void OrderByScoreDescendingIsAddedToExpressionTree()
        {
            var source = new FakeQueryProvider().CreateQuery<Sample>();
            AssertIsAddedToExpressionTree<IQueryable<Sample>, Sample>(source, ElasticQueryExtensions.OrderByScoreDescending, "OrderByScoreDescending");
        }

        [Fact]
        [ExcludeFromCodeCoverage] // Expression isn't "executed"
        public void OrderByScoreDescendingThrowArgumentNullExceptionIfSourceIsNull()
        {
            Assert.Throws<ArgumentNullException>(() => ElasticQueryExtensions.OrderByScoreDescending<Sample>(null));
        }

        [Fact]
        public void ThenByScoreIsAddedToExpressionTree()
        {
            var source = new FakeQueryProvider().CreateQuery<Sample>().OrderByScore();
            AssertIsAddedToExpressionTree<IOrderedQueryable<Sample>, Sample>(source, ElasticQueryExtensions.ThenByScore, "ThenByScore");
        }

        [Fact]
        [ExcludeFromCodeCoverage] // Expression isn't "executed"
        public void ThenByScoreThrowArgumentNullExceptionIfSourceIsNull()
        {
            Assert.Throws<ArgumentNullException>(() => ElasticQueryExtensions.ThenByScore<Sample>(null));
        }

        [Fact]
        public void ThenByScoreDescendingIsAddedToExpressionTree()
        {
            var source = new FakeQueryProvider().CreateQuery<Sample>().OrderByScore();
            AssertIsAddedToExpressionTree<IOrderedQueryable<Sample>, Sample>(source, ElasticQueryExtensions.ThenByScoreDescending, "ThenByScoreDescending");
        }

        [Fact]
        [ExcludeFromCodeCoverage] // Expression isn't "executed"
        public void ThenByScoreDescendingThrowArgumentNullExceptionIfSourceIsNull()
        {
            Assert.Throws<ArgumentNullException>(() => ElasticQueryExtensions.ThenByScoreDescending<Sample>(null));
        }

        [Fact]
        public static void WithElasticQuery_ToElasticSearchQuery_ReturnsJSON()
        {
            var query = new TestableElasticContext().Query<Sample>();

            var result = query.Where(x => x.Property == "2112").ToElasticSearchQuery();

            Assert.Equal(@"{""filter"":{""term"":{""property"":""2112""}}}", result);
        }

        [Fact]
        public static void WithNonElasticQuery_ToElasticSearchQuery_Throws()
        {
            var query = new[] { 42, 2112 }.AsQueryable();

            var ex = Assert.Throws<ArgumentException>(() => query.ToElasticSearchQuery());
            Assert.True(ex.Message.StartsWith("Query must be of type IElasticQuery<> to call ToElasticSearchQuery()"));
            Assert.Equal("source", ex.ParamName);
        }

        private static void AssertIsAddedToExpressionTree<TSequence, TElement>(TSequence source, Func<TSequence, TSequence> method, string methodName)
            where TSequence : IQueryable<TElement>
        {
            var afterMethod = method(source);
            var final = afterMethod.ToList();

            Assert.NotSame(source, afterMethod);
            Assert.NotNull(final);

            var finalExpression = Assert.IsType<FakeQueryProvider>(source.Provider).FinalExpression;
            Assert.Equal(ExpressionType.Call, finalExpression.NodeType);
            Assert.Equal(methodName, ((MethodCallExpression)finalExpression).Method.Name);
        }
    }
}