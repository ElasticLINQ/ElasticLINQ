// Licensed under the Apache 2.0 License. See LICENSE.txt in the project root for more information.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace ElasticLinq.Test.TestSupport
{
    class FakeQuery<T> : IOrderedQueryable<T>
    {
        readonly FakeQueryProvider provider;
        readonly Expression expression;

        public FakeQuery(FakeQueryProvider provider)
        {
            this.provider = provider;
            expression = Expression.Constant(this);
        }

        public FakeQuery(FakeQueryProvider provider, Expression expression)
        {
            this.provider = provider;
            this.expression = expression;
        }

        public IEnumerator<T> GetEnumerator()
        {
            return ((IEnumerable<T>)provider.Execute(expression)).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable)provider.Execute(expression)).GetEnumerator();
        }

        public Type ElementType => typeof(T);

        public Expression Expression => expression;

        public IQueryProvider Provider => provider;
    }
}