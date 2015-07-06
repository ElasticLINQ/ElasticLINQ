// Licensed under the Apache 2.0 License. See LICENSE.txt in the project root for more information.

using ElasticLinq.Request.Visitors;
using System;
using System.Linq;
using Xunit;

namespace ElasticLinq.Test.Request.Visitors.ElasticQueryTranslation
{
    public class ElasticQueryTranslationNotSupportedTests : ElasticQueryTranslationTestsBase
    {
        [Fact]
        public static void BinaryExpressionsThrowNotSupportedException()
        {
            // These tests are not exhaustive.
            {
                var ex = Assert.Throws<NotSupportedException>(() => Translate(Robots.Where(r => r.Cost * 2 > 1)));
                Assert.Contains("Multiply", ex.Message);
            }

            {
                var ex = Assert.Throws<NotSupportedException>(() => Translate(Robots.Where(r => r.Cost / 2 > 1)));
                Assert.Contains("Divide", ex.Message);
            }

            {
                var ex = Assert.Throws<NotSupportedException>(() => Translate(Robots.Where(r => r.Cost - 2 > 1)));
                Assert.Contains("Subtract", ex.Message);
            }

            {
                var ex = Assert.Throws<NotSupportedException>(() => Translate(Robots.Where(r => r.Cost + 2 > 1)));
                Assert.Contains("Add", ex.Message);
            }

            {
                var ex = Assert.Throws<NotSupportedException>(() => Translate(Robots.Where(r => r.Id >> 2 > 1)));
                Assert.Contains("RightShift", ex.Message);
            }

            {
                var ex = Assert.Throws<NotSupportedException>(() => Translate(Robots.Where(r => r.Id << 2 > 1)));
                Assert.Contains("LeftShift", ex.Message);
            }

            {
                var ex = Assert.Throws<NotSupportedException>(() => Translate(Robots.Where(r => (r.Id | 2) > 1)));
                Assert.Contains("Or", ex.Message); // User probably meant ||
            }

            {
                var ex = Assert.Throws<NotSupportedException>(() => Translate(Robots.Where(r => (r.Id & 2) > 1)));
                Assert.Contains("And", ex.Message); // User probably meant &&
            }
        }

        [Fact]
        public static void EqualsMustBeBetweenMemberAndConstant()
        {
            var ex = Assert.Throws<NotSupportedException>(() => Translate(Robots.Where(r => r.Zone == r.Id)));
            Assert.Contains("Equality must be between", ex.Message);
        }

        [Fact]
        public static void NotEqualsMustBeBetweenMemberAndConstant()
        {
            var ex = Assert.Throws<NotSupportedException>(() => Translate(Robots.Where(r => r.Zone != r.Id)));
            Assert.Contains("not-equal expression must be between", ex.Message);
        }

        [Fact]
        public static void RangeMustBeBetweenMemberAndConstant()
        {
            var ex = Assert.Throws<NotSupportedException>(() => Translate(Robots.Where(r => r.Zone > r.Id)));
            Assert.Contains("must test a constant against a member", ex.Message);
        }

        [Fact]
        public static void PrefixMustBeBetweenMemberAndConstant()
        {
            const string expectedMessageFragment = "Prefix must take a member";

            {
                var ex = Assert.Throws<NotSupportedException>(() => Translate(Robots.Where(r => ElasticMethods.Prefix(r.Name, r.Name))));
                Assert.Contains(expectedMessageFragment, ex.Message);
            }

            {
                var ex = Assert.Throws<NotSupportedException>(() => Translate(Robots.Where(r => ElasticMethods.Prefix("", r.Name))));
                Assert.Contains(expectedMessageFragment, ex.Message);
            }

            {
                var ex = Assert.Throws<NotSupportedException>(() => Translate(Robots.Where(r => ElasticMethods.Prefix("", ""))));
                Assert.Contains(expectedMessageFragment, ex.Message);
            }
        }

        [Fact]
        public static void RegexpMustBeBetweenMemberAndConstant()
        {
            const string expectedMessageFragment = "Regexp must take a member";

            {
                var ex = Assert.Throws<NotSupportedException>(() => Translate(Robots.Where(r => ElasticMethods.Regexp(r.Name, r.Name))));
                Assert.Contains(expectedMessageFragment, ex.Message);
            }

            {
                var ex = Assert.Throws<NotSupportedException>(() => Translate(Robots.Where(r => ElasticMethods.Regexp("", r.Name))));
                Assert.Contains(expectedMessageFragment, ex.Message);
            }

            {
                var ex = Assert.Throws<NotSupportedException>(() => Translate(Robots.Where(r => ElasticMethods.Regexp("", ""))));
                Assert.Contains(expectedMessageFragment, ex.Message);
            }
        }

        [Fact]
        public static void StringContainsMustBeBetweenMemberAndConstant()
        {
            var ex = Assert.Throws<NotSupportedException>(() => Translate(Robots.Query(r => r.Name.Contains(r.Name))));
            Assert.Contains("Contains operation must be a constant", ex.Message);
        }

        [Fact]
        public static void StringContainsMustNotBeInAWhere()
        {
            var ex = Assert.Throws<NotSupportedException>(() => Translate(Robots.Where(r => r.Name.Contains("a"))));
            Assert.Contains("String.Contains can only be used within .Query()", ex.Message);
        }

        [Fact]
        public static void ContainsOnEntityPropertyMustBeMatchedWithConstant()
        {
            var ex = Assert.Throws<NotSupportedException>(() => Translate(Robots.Where(r => r.Aliases.Contains(r.Name))));
            Assert.Contains("must be a constant", ex.Message);
        }

        [Fact]
        public static void ContainsOnConstantMustBeMatchedWithEntity()
        {
            var names = new[] { "a", "b", "c" };

            {
                // TODO: Consider supporting this particular case via partial evaluation
                var ex = Assert.Throws<NotSupportedException>(() => Translate(Robots.Where(r => names.Contains("a"))));
                Assert.Contains("Unknown source", ex.Message);
            }

            {
                var ex = Assert.Throws<NotSupportedException>(() => Translate(Robots.Where(r => names.Contains(r.Name.ToLower()))));
                Assert.Contains("Unknown source", ex.Message);
            }
        }

        [Fact]
        public static void GroupByCannotBeExpression()
        {
            var ex = Assert.Throws<NotSupportedException>(() => Translate(Robots.GroupBy(r => r.Id / 2)));
            Assert.Contains("GroupBy must be either a", ex.Message);
        }

        [Fact]
        public static void WhereCannotTakeAFunc()
        {
            Func<Robot, bool> wherePredicateFunc = r => r.Name.Contains("a");
            var ex = Assert.Throws<NotSupportedException>(() => Translate(Robots.Where(r => wherePredicateFunc(r))));
            Assert.Contains("Where expression ", ex.Message);
        }

        [Fact]
        public static void QueryCannotTakeAFunc()
        {
            Func<Robot, bool> wherePredicateFunc = r => r.Name.Contains("a");
            var ex = Assert.Throws<NotSupportedException>(() => Translate(Robots.Query(r => wherePredicateFunc(r))));
            Assert.Contains("Query expression ", ex.Message);
        }

        [Fact]
        public static void SelectManyCannotBeTranslated()
        {
            var ex = Assert.Throws<NotSupportedException>(() => Translate(Robots.SelectMany(r => r.Aliases)));
            Assert.Contains("Queryable.SelectMany method is not supported", ex.Message);
        }

        [Fact]
        public static void SelectWithIndexCannotBeTranslated()
        {
            var ex = Assert.Throws<NotSupportedException>(() => Translate(Robots.Select((r, i) => new { r.Name, i })));
            Assert.Contains("Select method with T", ex.Message);
        }

        [Theory]
        [InlineData("Last")]
        [InlineData("LastOrDefault")]
        public static void LastCannotBeTranslated(string method)
        {
            // TODO: Consider supporting these in conjunction with OrderBy/ThenBy by reversing all ordering
            var first = MakeQueryableExpression(method, Robots);
            var ex = Assert.Throws<NotSupportedException>(() => ElasticQueryTranslator.Translate(Mapping, first));
            Assert.Contains("Queryable." + method + " method is not supported", ex.Message);
        }

        [Fact]
        public static void WhereContainsFailsAfterQueryTransition()
        {
            var query = Robots.Query(r => r.Name.StartsWith("a")).Where(r => r.Name.Contains("b"));

            var ex = Assert.Throws<NotSupportedException>(() => Translate(query));
            Assert.Contains("String.Contains can only be used within .Query()", ex.Message);
        }

        private static ElasticTranslateResult Translate(IQueryable query)
        {
            return ElasticQueryTranslator.Translate(Mapping, query.Expression);
        }
    }
}