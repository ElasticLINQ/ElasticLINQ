// Licensed under the Apache 2.0 License. See LICENSE.txt in the project root for more information.

using System.Linq;
using System.Linq.Expressions;
using ElasticLinq.Request.Expressions;
using ElasticLinq.Request.Facets;
using Xunit;

namespace ElasticLinq.Test.Request.Expressions
{
    public class FacetExpressionTests
    {
        [Fact]
        public void ConstructorSetsFacet()
        {
            var facet = new TermsFacet("field", "value");

            var expression = new FacetExpression(facet);

            Assert.Same(facet, expression.Facet);
        }

        [Fact]
        public void ExpressionsTypeIsBool()
        {
            var facet = new TermsFacet("field", "value");

            var expression = new FacetExpression(facet);

            Assert.Equal(typeof(bool), expression.Type);
        }

        [Fact]
        public void CanReduceIsAlwaysFalse()
        {
            var facet = new TermsFacet("field", "value");

            var expression = new FacetExpression(facet);

            Assert.False(expression.CanReduce);
        }

        [Fact]
        public void ToStringReturnsFacetToString()
        {
            var facet = new TermsFacet("field", "value");

            var expression = new FacetExpression(facet);

            Assert.Equal(facet.ToString(), expression.ToString());
        }
    }
}