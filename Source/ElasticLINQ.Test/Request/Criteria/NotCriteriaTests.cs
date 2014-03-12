// Licensed under the Apache 2.0 License. See LICENSE.txt in the project root for more information.

using ElasticLinq.Request.Criteria;
using System;
using System.Diagnostics.CodeAnalysis;
using Xunit;

namespace ElasticLinq.Test.Request.Criteria
{
    public class NotCriteriaTests
    {
        private readonly ICriteria sampleTerm = TermsCriteria.Build("field", "value");

        [Fact]
        public void NamePropertyIsNot()
        {
            var criteria = NotCriteria.Create(sampleTerm);

            Assert.Equal("not", criteria.Name);
        }

        [Fact]
        public void CreateReturnsNotCriteriaWithChildCriteriaSet()
        {
            var criteria = NotCriteria.Create(sampleTerm);

            Assert.IsType<NotCriteria>(criteria);
            Assert.Equal(sampleTerm, ((NotCriteria)criteria).Criteria);
        }

        [Fact]
        public void CreateUnwrapsNestedNotCriteria()
        {
            var criteria = NotCriteria.Create(NotCriteria.Create(sampleTerm));

            Assert.IsType<TermCriteria>(criteria);
            Assert.Equal(sampleTerm, criteria);
        }

        [Fact]
        [ExcludeFromCodeCoverage] // Expression isn't "executed"
        public void ConstructorThrowsArgumentNullExceptionWhenFieldIsNull()
        {
            Assert.Throws<ArgumentNullException>(() => NotCriteria.Create(null));
        }

        [Fact]
        public void ToStringContainsSubfields()
        {
            var termCriteria = TermsCriteria.Build("termField", "some value");

            var notCriteria = NotCriteria.Create(termCriteria);
            var result = notCriteria.ToString();

            Assert.Contains(termCriteria.ToString(), result);
        }
   }
}