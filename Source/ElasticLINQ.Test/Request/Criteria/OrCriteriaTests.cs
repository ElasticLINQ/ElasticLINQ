// Licensed under the Apache 2.0 License. See LICENSE.txt in the project root for more information.

using ElasticLinq.Request.Criteria;
using System.Linq;
using Xunit;
using Xunit.Extensions;

namespace ElasticLinq.Test.Request.Criteria
{
    public class OrCriteriaTests
    {
        private readonly ITermsCriteria salutationMr = TermsCriteria.Build("salutation", "Mr");
        private readonly ITermsCriteria salutationMrs = TermsCriteria.Build("salutation", "Mrs");
        private readonly ITermsCriteria area408 = TermsCriteria.Build("area", "408");

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
        public void CombineWithEmptyListReturnsEmptyOr()
        {
            var criteria = OrCriteria.Combine(new ICriteria[] { });

            Assert.IsType<OrCriteria>(criteria);
            Assert.Empty(((OrCriteria)criteria).Criteria);
        }

        [Theory]
        [InlineData(null)]
        [InlineData(TermsExecutionMode.@bool)]
        [InlineData(TermsExecutionMode.or)]
        [InlineData(TermsExecutionMode.plain)]
        public void CombineWithAllTermFieldsSame_OrCompatibleExecutionMode_CombinesIntoSingleTerm(TermsExecutionMode? executionMode)
        {
            var salutationMs = TermsCriteria.Build(executionMode, "salutation", "Miss", "Ms");
            var criteria = OrCriteria.Combine(salutationMr, salutationMrs, salutationMs);

            var termsCriteria = Assert.IsType<TermsCriteria>(criteria);
            Assert.Equal(termsCriteria.Field, salutationMr.Field);

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
            var salutationMs = TermsCriteria.Build(executionMode, "salutation", "Miss", "Ms");
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
            var termCriteria = TermsCriteria.Build("termField", "some value");

            var orCriteria = OrCriteria.Combine(existsCriteria, termCriteria);
            var result = orCriteria.ToString();

            Assert.Contains(existsCriteria.ToString(), result);
            Assert.Contains(termCriteria.ToString(), result);
        }
    }
}