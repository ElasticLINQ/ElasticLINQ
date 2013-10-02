// Copyright (c) Tier 3 Inc. All rights reserved.
// This source code is made available under the terms of the Microsoft Public License (MS-PL)

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace ElasticLinq
{
    public class ElasticQuery<T> : IOrderedQueryable<T>
    {
        private readonly ElasticQueryProvider provider;
        private readonly Expression expression;

        public ElasticQuery(ElasticQueryProvider provider)
        {
            if (provider == null)
                throw new ArgumentNullException("provider");

            this.provider = provider;
            expression = Expression.Constant(this);
        }

        public ElasticQuery(ElasticQueryProvider provider, Expression expression)
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

        public IEnumerator<T> GetEnumerator()
        {
            return ((IEnumerable<T>)provider.Execute(expression)).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable)provider.Execute(expression)).GetEnumerator();
        }

        public Type ElementType
        {
            get { return typeof(T); }
        }

        public Expression Expression
        {
            get { return expression; }
        }

        public IQueryProvider Provider
        {
            get { return provider; }
        }
    }
}
