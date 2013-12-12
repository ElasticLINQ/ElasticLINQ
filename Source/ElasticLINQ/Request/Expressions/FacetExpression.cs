// Licensed under the Apache 2.0 License. See LICENSE.txt in the project root for more information.

using ElasticLinq.Request.Facets;
using System;
using System.Linq.Expressions;

namespace ElasticLinq.Request.Expressions
{
    /// <summary>
    /// An expression tree node that represents an ElasticSearch facet.
    /// </summary>
    internal class FacetExpression : Expression
    {
        private readonly IFacet facet;

        public FacetExpression(IFacet facet)
        {
            this.facet = facet;
        }

        public IFacet Facet { get { return facet; } }

        public override ExpressionType NodeType
        {
            get { return (ExpressionType)10001; }
        }

        public override Type Type
        {
            get { return typeof(bool); }
        }

        public override bool CanReduce
        {
            get { return false; }
        }

        public override string ToString()
        {
            return facet.ToString();
        }
    }
}