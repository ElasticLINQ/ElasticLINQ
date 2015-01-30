// Licensed under the Apache 2.0 License. See LICENSE.txt in the project root for more information.

using ElasticLinq.Request.Criteria;
using Xunit;

namespace ElasticLinq.Test.Request.Criteria
{
    public class ConstantCriteriaFilterReducerTests
    {
        [Fact]
        public void TrueRemainsTrue()
        {
            var actual = ConstantCriteriaFilterReducer.Reduce(ConstantCriteria.True);

            Assert.Same(ConstantCriteria.True, actual);
        }

        [Fact]
        public void TopLevelFalseRemainsFalse()
        {
            var actual = ConstantCriteriaFilterReducer.Reduce(ConstantCriteria.False);

            Assert.Same(ConstantCriteria.False, actual);
        }

        [Fact]
        public void OrWithFalseRemovesFalse()
        {
            var exists1 = new ExistsCriteria("1");
            var exists2 = new ExistsCriteria("2");
            var criteria = OrCriteria.Combine(exists1, ConstantCriteria.False, exists2);

            var actual = ConstantCriteriaFilterReducer.Reduce(criteria);

            var orActual = Assert.IsType<OrCriteria>(actual);
            Assert.DoesNotContain(orActual.Criteria, c => c == ConstantCriteria.False);
            Assert.Single(orActual.Criteria, exists1);
            Assert.Single(orActual.Criteria, exists2);
        }

        [Fact]
        public void AndWithTrueRemovesTrue()
        {
            var exists1 = new ExistsCriteria("1");
            var exists2 = new ExistsCriteria("2");
            var criteria = AndCriteria.Combine(exists1, ConstantCriteria.True, exists2);

            var actual = ConstantCriteriaFilterReducer.Reduce(criteria);

            var andActual = Assert.IsType<AndCriteria>(actual);
            Assert.DoesNotContain(andActual.Criteria, c => c == ConstantCriteria.True);
            Assert.Single(andActual.Criteria, exists1);
            Assert.Single(andActual.Criteria, exists2);
        }

        [Fact]
        public void OrWithTrueOptimizesToTrue()
        {
            var exists1 = new ExistsCriteria("1");
            var exists2 = new ExistsCriteria("2");
            var criteria = OrCriteria.Combine(exists1, ConstantCriteria.True, exists2);

            var actual = ConstantCriteriaFilterReducer.Reduce(criteria);

            Assert.Same(ConstantCriteria.True, actual);
        }

        [Fact]
        public void AndWithFalseOptimizesToFalse()
        {
            var exists1 = new ExistsCriteria("1");
            var exists2 = new ExistsCriteria("2");
            var criteria = AndCriteria.Combine(exists1, ConstantCriteria.False, exists2);

            var actual = ConstantCriteriaFilterReducer.Reduce(criteria);

            Assert.Same(ConstantCriteria.False, actual);
        }
    }
}