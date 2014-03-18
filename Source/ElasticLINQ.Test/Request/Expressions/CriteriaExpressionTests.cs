// Licensed under the Apache 2.0 License. See LICENSE.txt in the project root for more information.

using ElasticLinq.Request.Criteria;
using ElasticLinq.Request.Expressions;
using System.Reflection;
using Xunit;

namespace ElasticLinq.Test.Request.Expressions
{
    public class CriteriaExpressionTests
    {
        private readonly static MemberInfo memberInfo = typeof(string).GetProperty("Length");

        [Fact]
        public void ConstructorSetsCriteria()
        {
            var criteria = TermsCriteria.Build("field", memberInfo, "value");

            var expression = new CriteriaExpression(criteria);

            Assert.Same(criteria, expression.Criteria);
        }

        [Fact]
        public void ExpressionsTypeIsBool()
        {
            var criteria = TermsCriteria.Build("field", memberInfo, "value");

            var expression = new CriteriaExpression(criteria);

            Assert.Equal(typeof(bool), expression.Type);
        }

        [Fact]
        public void CanReduceIsAlwaysFalse()
        {
            var criteria = TermsCriteria.Build("field", memberInfo, "value");

            var expression = new CriteriaExpression(criteria);

            Assert.False(expression.CanReduce);
        }

        [Fact]
        public void ToStringReturnsCriteriaToString()
        {
            var criteria = TermsCriteria.Build("field", memberInfo, "value");

            var expression = new CriteriaExpression(criteria);

            Assert.Equal(criteria.ToString(), expression.ToString());
        }
    }
}