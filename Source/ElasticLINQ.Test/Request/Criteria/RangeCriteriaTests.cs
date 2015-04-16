// Licensed under the Apache 2.0 License. See LICENSE.txt in the project root for more information.

using ElasticLinq.Request.Criteria;
using System;
using System.Linq;
using System.Reflection;
using Xunit;

namespace ElasticLinq.Test.Request.Criteria
{
    public class RangeCriteriaTests
    {
        private readonly static MemberInfo memberInfo = typeof(string).GetProperty("Length");

        [Fact]
        public void SetsNameAndMember()
        {
            var criteria = new RangeCriteria("field", memberInfo, RangeComparison.GreaterThan, 1);

            Assert.Equal("range", criteria.Name);
            Assert.Equal(memberInfo, criteria.Member);
        }

        [Fact]
        public void SingleConstructorSetsSingleSpecification()
        {
            var criteria = new RangeCriteria("field", memberInfo, RangeComparison.GreaterThan, 1);

            Assert.Equal("field", criteria.Field);
            Assert.Equal(1, criteria.Specifications.Count);

            var specification = criteria.Specifications.Single();
            Assert.Equal("gt", specification.Name);
            Assert.Equal(1, specification.Value);
        }

        [Fact]
        public void EnumerableConstructorSetsMultipleSpecifications()
        {
            var greater = new RangeSpecificationCriteria(RangeComparison.GreaterThanOrEqual, "D");
            var less = new RangeSpecificationCriteria(RangeComparison.LessThan, "H");

            var criteria = new RangeCriteria("initials", memberInfo, new[] { greater, less });

            Assert.Equal("initials", criteria.Field);
            Assert.Equal(memberInfo, criteria.Member);
            Assert.Contains(greater, criteria.Specifications);
            Assert.Contains(less, criteria.Specifications);
            Assert.Equal(2, criteria.Specifications.Count);
        }

        [Fact]
        public void Constructor_GuardClauses()
        {
            Assert.Throws<ArgumentNullException>(() => new RangeCriteria(null, memberInfo, RangeComparison.GreaterThan, 1));
            Assert.Throws<ArgumentException>(() => new RangeCriteria(" ", memberInfo, RangeComparison.GreaterThan, 1));
            Assert.Throws<ArgumentNullException>(() => new RangeCriteria("field", null, Enumerable.Empty<RangeSpecificationCriteria>()));
            Assert.Throws<ArgumentNullException>(() => new RangeCriteria("field", memberInfo, null));
            Assert.Throws<ArgumentOutOfRangeException>(() => new RangeCriteria(" ", memberInfo, (RangeComparison)99, 1));
        }

        [Fact]
        public void ConstructorThrowsArgumentNullExceptionWhenValueIsNull()
        {
            Assert.Throws<ArgumentNullException>(() => new RangeCriteria("field", memberInfo, RangeComparison.GreaterThan, null));
        }

        [Fact]
        public void RangeComparisonsMapToExpectedNames()
        {
            var gt = new RangeSpecificationCriteria(RangeComparison.GreaterThan, 1);
            var gte = new RangeSpecificationCriteria(RangeComparison.GreaterThanOrEqual, 1);
            var lt = new RangeSpecificationCriteria(RangeComparison.LessThan, 1);
            var lte = new RangeSpecificationCriteria(RangeComparison.LessThanOrEqual, 1);

            Assert.Equal(gt.Name, "gt");
            Assert.Equal(gte.Name, "gte");
            Assert.Equal(lt.Name, "lt");
            Assert.Equal(lte.Name, "lte");
        }

        [Fact]
        public void ToStringContainsFieldComparisonAndValue()
        {
            var criteria = new RangeCriteria("thisIsMyFieldName", memberInfo, RangeComparison.LessThan, "500");
            var result = criteria.ToString();

            Assert.Contains(criteria.Field, result);
            Assert.Contains(criteria.Specifications[0].Comparison.ToString(), result);
            Assert.Contains((string)criteria.Specifications[0].Value, result);
        }
    }
}