// Licensed under the Apache 2.0 License. See LICENSE.txt in the project root for more information.

using ElasticLinq.Request.Criteria;
using System;
using System.Linq;
using System.Reflection;
using Xunit;

namespace ElasticLinq.Test.Request.Criteria
{
    public class OrCriteriaTests
    {
        readonly static MemberInfo memberInfo = typeof(string).GetProperty("Length");
        readonly ITermsCriteria salutationMr = TermsCriteria.Build("salutation", memberInfo, "Mr");
        readonly ITermsCriteria salutationMrs = TermsCriteria.Build("salutation", memberInfo, "Mrs");
        readonly ITermsCriteria area408 = TermsCriteria.Build("area", memberInfo, "408");

        [Fact]
        public void NamePropertyIsOr()
        {
            var criteria = new OrCriteria();

            Assert.Equal("or", criteria.Name);
        }

        [Fact]
        public void ConstructorSetsCriteria()
        {
            var criteria = new OrCriteria(salutationMr, area408);

            Assert.Contains(salutationMr, criteria.Criteria);
            Assert.Contains(area408, criteria.Criteria);
            Assert.Equal(2, criteria.Criteria.Count);
        }

        [Fact]
        public void CombineThrowArgumentNullExceptionWhenCriteriaIsNull()
        {
            Assert.Throws<ArgumentNullException>(() => OrCriteria.Combine(null));
        }

        [Fact]
        public void CombineWithEmptyListReturnsNull()
        {
            var criteria = OrCriteria.Combine(new ICriteria[] { });

            Assert.Null(criteria);
        }

        [Fact]
        public void CombineWithSingleCriteriaReturnsThatCriteria()
        {
            var rangeCriteria = new RangeCriteria("field", memberInfo, RangeComparison.LessThan, 1);
            var andCriteria = OrCriteria.Combine(rangeCriteria);

            Assert.Same(rangeCriteria, andCriteria);
        }

        [Theory]
        [InlineData(null)]
        [InlineData(TermsExecutionMode.@bool)]
        [InlineData(TermsExecutionMode.or)]
        [InlineData(TermsExecutionMode.plain)]
        public void CombineWithAllTermFieldsSame_OrCompatibleExecutionMode_CombinesIntoSingleTerm(TermsExecutionMode? executionMode)
        {
            var salutationMs = TermsCriteria.Build(executionMode, "salutation", memberInfo, "Miss", "Ms");
            var criteria = OrCriteria.Combine(salutationMr, salutationMrs, salutationMs);

            var termsCriteria = Assert.IsType<TermsCriteria>(criteria);
            Assert.Equal(salutationMr.Field, termsCriteria.Field);
            Assert.Same(memberInfo, termsCriteria.Member);

            var allValues = salutationMr.Values.Concat(salutationMrs.Values).Concat(salutationMs.Values).Distinct().ToArray();
            foreach (var value in allValues)
                Assert.Contains(value, termsCriteria.Values);

            Assert.Equal(allValues.Length, termsCriteria.Values.Count);
        }

        [Theory]
        [InlineData(TermsExecutionMode.and)]
        [InlineData(TermsExecutionMode.fielddata)]
        public void CombineWithAllTermFieldsSame_OrIncompatibleExecutionMode_ReturnsOrCriteria(TermsExecutionMode executionMode)
        {
            var salutationMs = TermsCriteria.Build(executionMode, "salutation", memberInfo, "Miss", "Ms");
            var criteria = OrCriteria.Combine(salutationMr, salutationMrs, salutationMs);

            var orCriteria = Assert.IsType<OrCriteria>(criteria);
            Assert.Contains(salutationMr, orCriteria.Criteria);
            Assert.Contains(salutationMrs, orCriteria.Criteria);
            Assert.Contains(salutationMs, orCriteria.Criteria);
            Assert.Equal(3, orCriteria.Criteria.Count);
        }

        [Fact]
        public void CombineWithDifferTermCriteriaFieldsDoesNotCombine()
        {
            var criteria = OrCriteria.Combine(salutationMr, salutationMrs, area408);

            Assert.IsType<OrCriteria>(criteria);
            var orCriteria = (OrCriteria)criteria;
            Assert.Contains(salutationMr, orCriteria.Criteria);
            Assert.Contains(salutationMrs, orCriteria.Criteria);
            Assert.Contains(area408, orCriteria.Criteria);
            Assert.Equal(3, orCriteria.Criteria.Count);
        }

        [Fact]
        public void ToStringContainsSubfields()
        {
            var existsCriteria = new ExistsCriteria("thisIsAMissingField");
            var termCriteria = TermsCriteria.Build("termField", memberInfo, "some value");

            var orCriteria = OrCriteria.Combine(existsCriteria, termCriteria);
            var result = orCriteria.ToString();

            Assert.Contains(existsCriteria.ToString(), result);
            Assert.Contains(termCriteria.ToString(), result);
        }
    }
}