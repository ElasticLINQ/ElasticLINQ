// Licensed under the Apache 2.0 License. See LICENSE.txt in the project root for more information.

using System;
using ElasticLinq.Request.Criteria;
using ElasticLinq.Request.Visitors;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Xunit;

namespace ElasticLinq.Test.Request.Visitors.ElasticQueryTranslation
{
    public class ElasticQueryTranslationSingularTests : ElasticQueryTranslationTestsBase
    {
        [Fact]
        public void FirstTranslatesToSizeOfOneWithNoFilter()
        {
            var first = MakeQueryableExpression("First", Robots);

            var request = ElasticQueryTranslator.Translate(Mapping, first).SearchRequest;

            Assert.Equal(1, request.Size);
            Assert.Null(request.Filter);
        }

        [Fact]
        public void FirstOrDefaultTranslatesToSizeOfOneWithNoFilter()
        {
            var first = MakeQueryableExpression("FirstOrDefault", Robots);

            var request = ElasticQueryTranslator.Translate(Mapping, first).SearchRequest;

            Assert.Equal(1, request.Size);
            Assert.Null(request.Filter);
        }

        [Fact]
        public void FirstWithPredicateTranslatesToSizeOfOneWithFilter()
        {
            const string expectedTermValue = "Josef";
            Expression<Func<Robot, bool>> lambda = r => r.Name == expectedTermValue;
            var first = MakeQueryableExpression("First", Robots, lambda);

            var request = ElasticQueryTranslator.Translate(Mapping, first).SearchRequest;

            Assert.Equal(1, request.Size);
            Assert.IsType<TermCriteria>(request.Filter);
            var termCriteria = (TermCriteria)request.Filter;
            Assert.Equal("term", termCriteria.Name);
            Assert.Equal("name", termCriteria.Field);
            Assert.Equal(1, termCriteria.Values.Count);
            Assert.Equal(expectedTermValue, termCriteria.Values[0]);
        }

        [Fact]
        public void FirstOrDefaultWithPredicateTranslatesToSizeOfOneWithFilter()
        {
            const string expectedTermValue = "Josef";
            Expression<Func<Robot, bool>> lambda = r => r.Name == expectedTermValue;
            var first = MakeQueryableExpression("FirstOrDefault", Robots, lambda);

            var request = ElasticQueryTranslator.Translate(Mapping, first).SearchRequest;

            Assert.Equal(1, request.Size);
            Assert.IsType<TermCriteria>(request.Filter);
            var termCriteria = (TermCriteria)request.Filter;
            Assert.Equal("term", termCriteria.Name);
            Assert.Equal("name", termCriteria.Field);
            Assert.Equal(1, termCriteria.Values.Count);
            Assert.Equal(expectedTermValue, termCriteria.Values[0]);
        }

        [Fact]
        public void SingleTranslatesToSizeOfTwoWithNoFilter()
        {
            var first = MakeQueryableExpression("Single", Robots);

            var request = ElasticQueryTranslator.Translate(Mapping, first).SearchRequest;

            Assert.Equal(2, request.Size);
            Assert.Null(request.Filter);
        }

        [Fact]
        public void SingleOrDefaultTranslatesToSizeOfTwoWithNoFilter()
        {
            var first = MakeQueryableExpression("SingleOrDefault", Robots);

            var request = ElasticQueryTranslator.Translate(Mapping, first).SearchRequest;

            Assert.Equal(2, request.Size);
            Assert.Null(request.Filter);
        }

        [Fact]
        public void SingleWithPredicateTranslatesToSizeOfTwoWithFilter()
        {
            const string expectedTermValue = "Josef";
            Expression<Func<Robot, bool>> lambda = r => r.Name == expectedTermValue;
            var first = MakeQueryableExpression("Single", Robots, lambda);

            var request = ElasticQueryTranslator.Translate(Mapping, first).SearchRequest;

            Assert.Equal(2, request.Size);
            Assert.IsType<TermCriteria>(request.Filter);
            var termCriteria = (TermCriteria)request.Filter;
            Assert.Equal("term", termCriteria.Name);
            Assert.Equal("name", termCriteria.Field);
            Assert.Equal(1, termCriteria.Values.Count);
            Assert.Equal(expectedTermValue, termCriteria.Values[0]);
        }

        [Fact]
        public void SingleOrDefaultWithPredicateTranslatesToSizeOfTwoWithFilter()
        {
            const string expectedTermValue = "Josef";
            Expression<Func<Robot, bool>> lambda = r => r.Name == expectedTermValue;
            var first = MakeQueryableExpression("SingleOrDefault", Robots, lambda);

            var request = ElasticQueryTranslator.Translate(Mapping, first).SearchRequest;

            Assert.Equal(2, request.Size);
            Assert.IsType<TermCriteria>(request.Filter);
            var termCriteria = (TermCriteria)request.Filter;
            Assert.Equal("term", termCriteria.Name);
            Assert.Equal("name", termCriteria.Field);
            Assert.Equal(1, termCriteria.Values.Count);
            Assert.Equal(expectedTermValue, termCriteria.Values[0]);
        }

        private static Expression MakeQueryableExpression<TSource>(string name, IQueryable<TSource> source, params Expression[] parameters)
        {
            var method = MakeQueryableMethod<TSource>(name, parameters.Length + 1);
            return Expression.Call(method, new[] { source.Expression }.Concat(parameters).ToArray());
        }

        private static MethodInfo MakeQueryableMethod<TSource>(string name, int parameterCount)
        {
            return typeof(Queryable).FindMembers
                (MemberTypes.Method,
                    BindingFlags.Static | BindingFlags.Public,
                    (info, criteria) => info.Name.Equals(criteria), name)
                .OfType<MethodInfo>()
                .Single(a => a.GetParameters().Length == parameterCount)
                .MakeGenericMethod(typeof(TSource));
        }
    }
}