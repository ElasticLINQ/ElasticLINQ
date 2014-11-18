// Licensed under the Apache 2.0 License. See LICENSE.txt in the project root for more information.

using ElasticLinq.Request.Criteria;
using Xunit;

namespace ElasticLinq.Test.Request.Criteria
{
    public class ConstantCriteriaTests
    {
        [Fact]
        public void ConstantTrueIsAlwaysSameInstance()
        {
            var actual = ConstantCriteria.True;

            Assert.Same(actual, ConstantCriteria.True);
        }

        [Fact]
        public void ConstantFalseIsAlwaysSameInstance()
        {
            var actual = ConstantCriteria.False;

            Assert.Same(actual, ConstantCriteria.False);
        }

        [Fact]
        public void ConstantTrueReturnsNameTrue()
        {
            Assert.Equal("True", ConstantCriteria.True.Name);
        }

        [Fact]
        public void ConstantFalseReturnsNameTrue()
        {
            Assert.Equal("False", ConstantCriteria.False.Name);
        }

        [Fact]
        public void ConstantTrueToStringIsName()
        {
            Assert.Equal(ConstantCriteria.True.Name, ConstantCriteria.True.ToString());
        }

        [Fact]
        public void ConstantFalseToStringIsName()
        {
            Assert.Equal(ConstantCriteria.False.Name, ConstantCriteria.False.ToString());
        }
    }
}