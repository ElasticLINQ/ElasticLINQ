using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace ElasticSpiking.BasicProvider
{
    public class Query<T> : IOrderedQueryable<T>
    {
        private readonly QueryProvider provider;
        private readonly Expression expression;

        public Query(QueryProvider provider)
        {
            if (provider == null)
                throw new ArgumentNullException("provider");

            this.provider = provider;
            expression = Expression.Constant(this);
        }

        public Query(QueryProvider provider, Expression expression)
        {
            if (provider == null)
                throw new ArgumentNullException("provider");

            if (expression == null)
                throw new ArgumentNullException("expression");

            if (!typeof(IQueryable<T>).IsAssignableFrom(expression.Type))
                throw new ArgumentOutOfRangeException("expression");

            this.provider = provider;
            this.expression = expression;
        }

        Expression IQueryable.Expression
        {
            get { return expression; }
        }

        Type IQueryable.ElementType
        {
            get { return typeof(T); }
        }

        IQueryProvider IQueryable.Provider
        {
            get { return provider; }
        }

        public IEnumerator<T> GetEnumerator()
        {
            return ((IEnumerable<T>)provider.Execute(expression)).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable)provider.Execute(expression)).GetEnumerator();
        }

        public override string ToString()
        {
            return provider.GetQueryText(expression);
        }
    }
}