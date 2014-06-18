// Licensed under the Apache 2.0 License. See LICENSE.txt in the project root for more information.

using ElasticLinq.Request.Criteria;
using ElasticLinq.Request.Expressions;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Xunit;

namespace ElasticLinq.Test.Request.Expressions
{
    public class CriteriaExpressionTests
    {
        private readonly static MemberInfo memberInfo = typeof(string).GetProperty("Length");
        private readonly ITermsCriteria criteria = TermsCriteria.Build("field", memberInfo, "value");

        [Fact]
        public void ConstructorSetsCriteria()
        {
            var expression = new CriteriaExpression(criteria);

            Assert.Same(criteria, expression.Criteria);
        }

        [Fact]
        public void ExpressionsTypeIsBool()
        {
            var expression = new CriteriaExpression(criteria);

            Assert.Equal(typeof(bool), expression.Type);
        }

        [Fact]
        public void CanReduceIsAlwaysFalse()
        {
            var expression = new CriteriaExpression(criteria);

            Assert.False(expression.CanReduce);
        }

        [Fact]
        public void ToStringReturnsCriteriaToString()
        {
            var expression = new CriteriaExpression(criteria);

            Assert.Equal(criteria.ToString(), expression.ToString());
        }

        [Fact]
        public void NodeTypeDoesNotConflictWithSystemNodeTypes()
        {
            var systemNodeTypes = Enum.GetValues(typeof(ExpressionType)).OfType<ExpressionType>();

            var expression = new CriteriaExpression(criteria);

            Assert.DoesNotContain(expression.NodeType, systemNodeTypes);
        }
    }
}