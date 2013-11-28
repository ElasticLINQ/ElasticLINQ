// Licensed under the Apache 2.0 License. See LICENSE.txt in the project root for more information.

using ElasticLinq.Request.Criteria;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Xunit;

namespace ElasticLinq.Test.Request.Criteria
{
    public class AndCriteriaTests
    {
        private readonly ICriteria sampleCriteria1 = new TermCriteria("first", "1st");
        private readonly ICriteria sampleCriteria2 = new TermCriteria("second", "2nd");

        [Fact]
        public void NamePropertyIsAnd()
        {
            var criteria = new AndCriteria();

            Assert.Equal("and", criteria.Name);
        }

        [Fact]
        public void ConstructorSetsCriteria()
        {
            var criteria = new AndCriteria(sampleCriteria1, sampleCriteria2);

            Assert.Contains(sampleCriteria1, criteria.Criteria);
            Assert.Contains(sampleCriteria2, criteria.Criteria);
            Assert.Equal(2, criteria.Criteria.Count);
        }

        [Fact]
        [ExcludeFromCodeCoverage] // Expression isn't "executed"
        public void CombineThrowArgumentNullExceptionWhenCriteriaIsNull()
        {
            Assert.Throws<ArgumentNullException>(() => AndCriteria.Combine(null));
        }

        [Fact]
        public void CombineWithEmptyListReturnsNull()
        {
            var criteria = AndCriteria.Combine(new ICriteria[] { });

            Assert.Null(criteria);
        }

        [Fact]
        public void CombineWithSingleCriteriaReturnsThatCriteria()
        {
            var rangeCriteria = new RangeCriteria("field", RangeComparison.LessThan, 1);
            var criteria = AndCriteria.Combine(rangeCriteria);

            Assert.Same(rangeCriteria, criteria);
        }

        [Fact]
        public void CombineWithCriteriaCombinesIntoAndCriteria()
        {
            var criteria = AndCriteria.Combine(sampleCriteria1, sampleCriteria2);

            Assert.IsType<AndCriteria>(criteria);
            var andCriteria = (AndCriteria)criteria;
            Assert.Contains(sampleCriteria1, andCriteria.Criteria);
            Assert.Contains(sampleCriteria2, andCriteria.Criteria);
            Assert.Equal(2, andCriteria.Criteria.Count);
        }

        [Fact]
        public void CombineWithTwoSameFieldRangeCriteriaCombinesIntoSingleRangeCriteria()
        {
            var lowerFirstRange = new RangeCriteria("first", RangeComparison.GreaterThan, "lower");
            var upperFirstRange = new RangeCriteria("first", RangeComparison.LessThanOrEqual, "upper");

            var criteria = AndCriteria.Combine(lowerFirstRange, upperFirstRange);

            Assert.IsType<RangeCriteria>(criteria);
            var rangeCriteria = (RangeCriteria)criteria;
            Assert.Equal(rangeCriteria.Field, lowerFirstRange.Field);
            Assert.Single(rangeCriteria.Specifications, s => s.Comparison == lowerFirstRange.Specifications.First().Comparison);
            Assert.Single(rangeCriteria.Specifications, s => s.Comparison == upperFirstRange.Specifications.First().Comparison);
        }

        [Fact]
        public void CombineWithDifferentFieldRangeCriteriaCombinesRangesIntoAndCriteria()
        {
            var lowerFirstRange = new RangeCriteria("first", RangeComparison.GreaterThan, "lower");
            var upperFirstRange = new RangeCriteria("first", RangeComparison.LessThanOrEqual, "upper");
            var secondRange = new RangeCriteria("second", RangeComparison.GreaterThanOrEqual, "lower2");

            var criteria = AndCriteria.Combine(lowerFirstRange, secondRange, upperFirstRange);

            Assert.IsType<AndCriteria>(criteria);
            var andCriteria = (AndCriteria)criteria;
            Assert.Equal(2, andCriteria.Criteria.Count);
            Assert.Contains(secondRange, andCriteria.Criteria);

            var combinedRange = andCriteria.Criteria.OfType<RangeCriteria>().FirstOrDefault(r => r.Specifications.Count == 2);
            Assert.NotNull(combinedRange);
            Assert.Equal(lowerFirstRange.Field, combinedRange.Field);
            Assert.Single(combinedRange.Specifications, s => s.Comparison == lowerFirstRange.Specifications.First().Comparison);
            Assert.Single(combinedRange.Specifications, s => s.Comparison == upperFirstRange.Specifications.First().Comparison);
        }

        [Fact]
        public void ToStringContainsSubfields()
        {
            var existsCriteria = new ExistsCriteria("thisIsAMissingField");
            var termCriteria = new TermCriteria("termField", "some value");

            var andCriteria = AndCriteria.Combine(existsCriteria, termCriteria);
            var result = andCriteria.ToString();

            Assert.Contains(existsCriteria.ToString(), result);
            Assert.Contains(termCriteria.ToString(), result);
        }
    }
}