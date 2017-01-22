// Licensed under the Apache 2.0 License. See LICENSE.txt in the project root for more information.

using ElasticLinq.Request.Criteria;
using System;
using System.Linq.Expressions;

namespace ElasticLinq.Request.Expressions
{
    /// <summary>
    /// An expression tree node that represents criteria.
    /// </summary>
    class CriteriaExpression : Expression
    {
        readonly ICriteria criteria;

        /// <summary>
        /// Initializes a new instance of the <see cref="CriteriaExpression"/> class.
        /// </summary>
        /// <param name="criteria"><see cref="ICriteria" /> to represent with this expression.</param>
        public CriteriaExpression(ICriteria criteria)
        {
            this.criteria = criteria;
        }

        /// <summary>
        /// <see cref="ICriteria" /> that is represented by this expression.
        /// </summary>
        public ICriteria Criteria => criteria;

        /// <inheritdoc/>
        public override ExpressionType NodeType => ElasticExpressionType.Criteria;

        /// <inheritdoc/>
        public override Type Type => typeof(bool);

        /// <inheritdoc/>
        public override bool CanReduce => false;

        /// <inheritdoc/>
        public override string ToString()
        {
            return criteria.ToString();
        }
    }
}