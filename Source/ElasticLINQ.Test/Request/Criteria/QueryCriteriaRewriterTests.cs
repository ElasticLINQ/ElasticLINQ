// Licensed under the Apache 2.0 License. See LICENSE.txt in the project root for more information.

using System.Linq;
using System.Reflection;
using ElasticLinq.Request.Criteria;
using Xunit;

namespace ElasticLinq.Test.Request.Criteria
{
    public class QueryCriteriaRewriterTests
    {
        readonly static MemberInfo memberInfo = typeof(string).GetProperty("Length");

        [Fact]
        public void AndBecomesBoolWithMust()
        {
            var expected = new [] { new RangeCriteria("fieldOne", memberInfo, RangeComparison.LessThan, 2), new RangeCriteria("fieldTwo", memberInfo, RangeComparison.GreaterThan, 4) };

            var actual = QueryCriteriaRewriter.Compensate(AndCriteria.Combine(expected));

            var boolActual = Assert.IsType<BoolCriteria>(actual);

            Assert.Equal(boolActual.Must.AsEnumerable(), expected);
            Assert.Empty(boolActual.Should);
            Assert.Empty(boolActual.MustNot);
        }

        [Fact]
        public void AndWithOrsBecomesBoolWithShould()
        {
            var expected1 = new[] { new RangeCriteria("field1", memberInfo, RangeComparison.LessThan, 2), new RangeCriteria("field2", memberInfo, RangeComparison.GreaterThan, 4) };
            var expected2 = new[] { new RangeCriteria("field3", memberInfo, RangeComparison.LessThan, 2), new RangeCriteria("field4", memberInfo, RangeComparison.GreaterThan, 4) };

            var actual = QueryCriteriaRewriter.Compensate(AndCriteria.Combine(OrCriteria.Combine(expected1), OrCriteria.Combine(expected2)));

            var boolActual = Assert.IsType<BoolCriteria>(actual);

            Assert.Equal(boolActual.Should.AsEnumerable(), expected1.Concat(expected2));
            Assert.Empty(boolActual.Must);
            Assert.Empty(boolActual.MustNot);
        }

        [Fact]
        public void AndWithNotsBecomesBoolWithMustNots()
        {
            var expectedMustNots = new[] { new RangeCriteria("field1", memberInfo, RangeComparison.LessThan, 2), new RangeCriteria("field2", memberInfo, RangeComparison.GreaterThan, 4) };

            var actual = QueryCriteriaRewriter.Compensate(AndCriteria.Combine(expectedMustNots.Select(NotCriteria.Create).ToArray()));

            var boolActual = Assert.IsType<BoolCriteria>(actual);

            Assert.Equal(boolActual.MustNot.AsEnumerable(), expectedMustNots);
            Assert.Empty(boolActual.Should);
            Assert.Empty(boolActual.Must);
        }

        [Fact]
        public void AndWithMixedContentBecomesBoolWithShouldMustAndMustNot()
        {
            var expectedShould = new[] { new RangeCriteria("field1", memberInfo, RangeComparison.LessThan, 2), new RangeCriteria("field2", memberInfo, RangeComparison.GreaterThan, 4) };
            var expectedMust = new RangeCriteria("field3", memberInfo, RangeComparison.LessThan, 2);
            var expectedMustNot = new PrefixCriteria("field5", "prefix");

            var actual = QueryCriteriaRewriter.Compensate(AndCriteria.Combine(OrCriteria.Combine(expectedShould), expectedMust, NotCriteria.Create(expectedMustNot)));

            var boolActual = Assert.IsType<BoolCriteria>(actual);

            Assert.Equal(boolActual.Should.AsEnumerable(), expectedShould);
            Assert.Single(boolActual.Must, expectedMust);
            Assert.Single(boolActual.MustNot, expectedMustNot);
        }

        [Fact]
        public void OrBecomesBoolWithShould()
        {
            var expected = new[] { new RangeCriteria("fieldOne", memberInfo, RangeComparison.LessThan, 2), new RangeCriteria("fieldTwo", memberInfo, RangeComparison.GreaterThan, 4) };

            var actual = QueryCriteriaRewriter.Compensate(OrCriteria.Combine(expected));

            var boolActual = Assert.IsType<BoolCriteria>(actual);

            Assert.Equal(boolActual.Should.AsEnumerable(), expected);
            Assert.Empty(boolActual.Must);
            Assert.Empty(boolActual.MustNot);
        }

        [Fact]
        public void NotWithOrBecomesBoolWithMustNot()
        {
            var expected = new[] { new RangeCriteria("fieldOne", memberInfo, RangeComparison.LessThan, 2), new RangeCriteria("fieldTwo", memberInfo, RangeComparison.GreaterThan, 4) };

            var actual = QueryCriteriaRewriter.Compensate(NotCriteria.Create(OrCriteria.Combine(expected)));

            var boolActual = Assert.IsType<BoolCriteria>(actual);

            Assert.Equal(boolActual.MustNot.AsEnumerable(), expected);
            Assert.Empty(boolActual.Should);
            Assert.Empty(boolActual.Must);
        }

        [Fact]
        public void NotWithAndBecomesBoolWithMustNotBool()
        {
            var expected = new[] { new RangeCriteria("fieldOne", memberInfo, RangeComparison.LessThan, 2), new RangeCriteria("fieldTwo", memberInfo, RangeComparison.GreaterThan, 4) };

            var actual = QueryCriteriaRewriter.Compensate(NotCriteria.Create(AndCriteria.Combine(expected)));

            var boolActual = Assert.IsType<BoolCriteria>(actual);
            Assert.Empty(boolActual.Should);
            Assert.Empty(boolActual.Must);
            Assert.Single(boolActual.MustNot);
            var subBoolActual = Assert.IsType<BoolCriteria>(boolActual.MustNot.First());

            Assert.Equal(subBoolActual.Must.AsEnumerable(), expected);
            Assert.Empty(subBoolActual.Should);
            Assert.Empty(subBoolActual.MustNot);
        }
   }
}