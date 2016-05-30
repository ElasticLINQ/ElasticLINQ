// Licensed under the Apache 2.0 License. See LICENSE.txt in the project root for more information.

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Linq.Expressions;

namespace ElasticLinq.Request.Visitors
{
    class RebindCollectionResult<T>
    {
        readonly Expression expression;
        readonly ReadOnlyCollection<T> collected;
        readonly ParameterExpression parameter;

        public RebindCollectionResult(Expression expression, IEnumerable<T> collected, ParameterExpression parameter)
        {
            this.expression = expression;
            this.collected = new ReadOnlyCollection<T>(collected.ToArray());
            this.parameter = parameter;
        }

        public Expression Expression { get { return expression; } }

        public ParameterExpression Parameter { get { return parameter; } }

        public ReadOnlyCollection<T> Collected { get { return collected; } }
    }
}
