// Licensed under the Apache 2.0 License. See LICENSE.txt in the project root for more information.

using ElasticLinq.Test.TestSupport;
using System;
using System.Linq;
using System.Linq.Expressions;
using ElasticLinq.Request;
using Xunit;

namespace ElasticLinq.Test
{
    public class ElasticQueryExtensionsTests
    {
        static readonly IQueryable<Sample> testableSample = new TestableElasticContext().Query<Sample>();
        static readonly IQueryable<Sample> fakeSample = new FakeQueryProvider().CreateQuery<Sample>();

        class Sample
        {
            public string Property { get; set; }
        }

        [Fact]
        public static void QueryIsAddedToExpressionTree()
        {
            var queried = fakeSample.Where(s => s.Property == "a");

            Assert.IsAssignableFrom<MethodCallExpression>(queried.Expression);
            var callExpression = (MethodCallExpression)queried.Expression;

            Assert.Equal(2, callExpression.Arguments.Count);
            Assert.Equal(ExpressionType.Quote, callExpression.Arguments[1].NodeType);
        }

        [Fact]
        public static void QueryString_GuardClauses()
        {
            Assert.Throws<ArgumentNullException>(() => ElasticQueryExtensions.QueryString<Sample>(null, ""));
            Assert.Throws<ArgumentNullException>(() => fakeSample.QueryString(null));
        }

        [Fact]
        public static void QueryStringIsAddedToExpressionTree()
        {
            const string expectedQueryString = "abcdef";
            var applied = fakeSample.QueryString(expectedQueryString);

            Assert.IsAssignableFrom<MethodCallExpression>(applied.Expression);
            var callExpression = (MethodCallExpression)applied.Expression;

            Assert.Equal(2, callExpression.Arguments.Count);
            var queryStringConstantExpression = Assert.IsType<ConstantExpression>(callExpression.Arguments[1]);
            Assert.Equal(queryStringConstantExpression.Value, expectedQueryString);
        }

        [Fact]
        public static void QueryStringWithFields_GuardClauses()
        {
            Assert.Throws<ArgumentNullException>(() => ElasticQueryExtensions.QueryString<Sample>(null, "", new[] { "one", "two" }));
            Assert.Throws<ArgumentNullException>(() => fakeSample.QueryString(null, new[] { "one", "two" }));
            Assert.Throws<ArgumentOutOfRangeException>(() => fakeSample.QueryString("hi", null));
            Assert.Throws<ArgumentOutOfRangeException>(() => fakeSample.QueryString("hi", new string[] { }));
        }

        [Fact]
        public static void QueryStringWithFieldsIsAddedToExpressionTree()
        {
            const string expectedQueryString = "abcdef";
            var expectedFields = new[] { "three", "four" };

            var applied = fakeSample.QueryString(expectedQueryString, expectedFields);

            Assert.IsAssignableFrom<MethodCallExpression>(applied.Expression);
            var callExpression = (MethodCallExpression)applied.Expression;

            Assert.Equal(3, callExpression.Arguments.Count);
            var queryStringConstantExpression = Assert.IsType<ConstantExpression>(callExpression.Arguments[1]);
            Assert.Equal(queryStringConstantExpression.Value, expectedQueryString);
            var fieldsConstantExpression = Assert.IsType<ConstantExpression>(callExpression.Arguments[2]);
            Assert.Equal(fieldsConstantExpression.Value, expectedFields);
        }

        [Fact]
        public void OrderByScoreIsAddedToExpressionTree()
        {
            AssertIsAddedToExpressionTree<IQueryable<Sample>, Sample>(fakeSample, ElasticQueryExtensions.OrderByScore, "OrderByScore");
        }

        [Fact]
        public void OrderByScore_GuardClauses()
        {
            Assert.Throws<ArgumentNullException>(() => ElasticQueryExtensions.OrderByScore<Sample>(null));
        }

        [Fact]
        public void OrderByScoreDescendingIsAddedToExpressionTree()
        {
            AssertIsAddedToExpressionTree<IQueryable<Sample>, Sample>(fakeSample, ElasticQueryExtensions.OrderByScoreDescending, "OrderByScoreDescending");
        }

        [Fact]
        public void OrderByScoreDescending_GuardClauses()
        {
            Assert.Throws<ArgumentNullException>(() => ElasticQueryExtensions.OrderByScoreDescending<Sample>(null));
        }

        [Fact]
        public void ThenByScoreIsAddedToExpressionTree()
        {
            AssertIsAddedToExpressionTree<IOrderedQueryable<Sample>, Sample>(fakeSample.OrderByScore(), ElasticQueryExtensions.ThenByScore, "ThenByScore");
        }

        [Fact]
        public static void ThenByScore_GuardClauses()
        {
            Assert.Throws<ArgumentNullException>(() => ElasticQueryExtensions.ThenByScore<Sample>(null));
        }

        [Fact]
        public void ThenByScoreDescendingIsAddedToExpressionTree()
        {
            AssertIsAddedToExpressionTree<IOrderedQueryable<Sample>, Sample>(fakeSample.OrderByScore(), ElasticQueryExtensions.ThenByScoreDescending, "ThenByScoreDescending");
        }

        [Fact]
        public static void ThenByScoreDescending_GuardClauses()
        {
            Assert.Throws<ArgumentNullException>(() => ElasticQueryExtensions.ThenByScoreDescending<Sample>(null));
        }

        [Fact]
        public static void WithElasticQuery_ToElasticSearchQuery_ReturnsJSON()
        {
            var queryInfo = testableSample.Where(x => x.Property == "2112").ToQueryInfo();

            Assert.Equal(@"{""query"":{""term"":{""property"":""2112""}}}", queryInfo.Query);
        }

        [Fact]
        public static void WithNonElasticQuery_ToElasticSearchQuery_Throws()
        {
            var query = new[] { 42, 2112 }.AsQueryable();

            var ex = Assert.Throws<ArgumentException>(() => query.ToQueryInfo());
            Assert.Contains("Query must be of type IElasticQuery<> to call ToQueryInfo()", ex.Message);
            Assert.Equal("source", ex.ParamName);
        }

        [Fact]
        public static void HighlightQuery_Throws()
        {
            Assert.Throws<ArgumentNullException>(() => ElasticQueryExtensions.Highlight<Sample, string>(null, null));
            Assert.Throws<ArgumentNullException>(() => ElasticQueryExtensions.Highlight<Sample, string>(null, e => e.Property));
            Assert.Throws<NotSupportedException>(() => testableSample.Highlight(e => e.Property.Equals("").ToString()).ToList());
        }

        [Fact]
        public static void HighlightQuery_SpecifiesField()
        {
            var body = testableSample.Highlight(e => e.Property).ToQueryInfo().Query;

            Assert.Contains("{\"highlight\":{\"fields\":{\"property\":{}}}}", body);
        }

        [Fact]
        public static void HighlightQuery_ReturnsJSON_WithConfig_Tags()
        {
            var body = testableSample
                .Highlight(e => e.Property, new Highlight { PostTag = "<a>", PreTag = "<b>" })
                .ToQueryInfo().Query;

            Assert.Contains("\"post_tags\":[\"<a>\"]", body);
            Assert.Contains("\"pre_tags\":[\"<b>\"]", body);
        }

        public class MultiPropertySample
        {
            public string Property1 { get; set; }
            public string Property2 { get; set; }
        }

        [Fact]
        public static void HighlightQuery_ReturnsJSON_CallChain()
        {
            var body = new TestableElasticContext().Query<MultiPropertySample>()
                .Highlight(e => e.Property1)
                .Highlight(e => e.Property2)
                .ToQueryInfo().Query;

            Assert.Contains("\"fields\":{\"property2\":{},\"property1\":{}}", body);
        }

        [Fact]
        public static void HighlightQuery_ReturnsJSON_CallChain_ConfigInLast()
        {
            var body = new TestableElasticContext().Query<MultiPropertySample>()
                .Highlight(e => e.Property1, new Highlight { PreTag = "<a>" })
                .Highlight(e => e.Property2, new Highlight { PreTag = "<b>", PostTag = "<c>" })
                .ToQueryInfo().Query;

            Assert.Contains("\"pre_tags\":[\"<b>\"]", body);
            Assert.Contains("\"post_tags\":[\"<c>\"]", body);
            Assert.DoesNotContain("\"pre_tags\":[\"<a>\"]", body);
        }

        static void AssertIsAddedToExpressionTree<TSequence, TElement>(TSequence source, Func<TSequence, TSequence> method, string methodName)
            where TSequence : IQueryable<TElement>
        {
            var afterMethod = method(source);
            var final = afterMethod.ToList();

            Assert.NotEqual(source, afterMethod);
            Assert.NotNull(final);

            var finalExpression = Assert.IsType<FakeQueryProvider>(source.Provider).FinalExpression;
            Assert.Equal(ExpressionType.Call, finalExpression.NodeType);
            Assert.Equal(methodName, ((MethodCallExpression)finalExpression).Method.Name);
        }
    }
}