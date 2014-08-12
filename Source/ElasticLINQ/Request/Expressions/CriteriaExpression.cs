// Licensed under the Apache 2.0 License. See LICENSE.txt in the project root for more information.

using ElasticLinq.Request.Criteria;
using System;
using System.Linq.Expressions;

namespace ElasticLinq.Request.Expressions
{
    /// <summary>
    /// An expression tree node that represents Elasticsearch criteria.
    /// </summary>
    internal class CriteriaExpression : Expression
    {
        private readonly ICriteria criteria;

        public CriteriaExpression(ICriteria criteria)
        {
            this.criteria = criteria;
        }

        public ICriteria Criteria { get { return criteria; } }

        public override ExpressionType NodeType
        {
            get { return ElasticExpressionType.Criteria; }
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
            return criteria.ToString();
        }
    }
}