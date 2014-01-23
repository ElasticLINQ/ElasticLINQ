// Licensed under the Apache 2.0 License. See LICENSE.txt in the project root for more information.

using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace ElasticLinq.Request.Visitors
{
    internal class RebindCollectionResult<T>
    {
        private readonly Expression expression;
        private readonly HashSet<T> collected;
        private readonly ParameterExpression parameter;
        private readonly LambdaExpression projection;

        public RebindCollectionResult(Expression expression, HashSet<T> collected, ParameterExpression parameter, LambdaExpression projection)
        {
            this.expression = expression;
            this.collected = collected;
            this.parameter = parameter;
            this.projection = projection;
        }

        public Expression Expression { get { return expression; } }
        public ParameterExpression Parameter { get { return parameter; } }
        public IReadOnlyList<T> Collected { get { return collected.ToList().AsReadOnly(); } }
        public LambdaExpression Projection { get { return projection; } }
    }
}
