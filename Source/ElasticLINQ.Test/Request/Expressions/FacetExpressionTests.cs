// Licensed under the Apache 2.0 License. See LICENSE.txt in the project root for more information.

using System;
using System.Linq;
using System.Linq.Expressions;
using ElasticLinq.Request.Expressions;
using ElasticLinq.Request.Facets;
using Xunit;

namespace ElasticLinq.Test.Request.Expressions
{
    public class FacetExpressionTests
    {
        private readonly TermsFacet termsFacet = new TermsFacet("field", 123, "value");

        [Fact]
        public void ConstructorSetsFacet()
        {
            var expression = new FacetExpression(termsFacet);

            Assert.Same(termsFacet, expression.Facet);
        }

        [Fact]
        public void ExpressionsTypeIsBool()
        {
            var expression = new FacetExpression(termsFacet);

            Assert.Equal(typeof(bool), expression.Type);
        }

        [Fact]
        public void CanReduceIsAlwaysFalse()
        {
            var expression = new FacetExpression(termsFacet);

            Assert.False(expression.CanReduce);
        }

        [Fact]
        public void ToStringReturnsFacetToString()
        {
            var expression = new FacetExpression(termsFacet);

            Assert.Equal(termsFacet.ToString(), expression.ToString());
        }

        [Fact]
        public void NodeTypeDoesNotConflictWithSystemNodeTypes()
        {
            var systemNodeTypes = Enum.GetValues(typeof(ExpressionType)).OfType<ExpressionType>();

            var expression = new FacetExpression(termsFacet);

            Assert.DoesNotContain(expression.NodeType, systemNodeTypes);
        }
    }
}