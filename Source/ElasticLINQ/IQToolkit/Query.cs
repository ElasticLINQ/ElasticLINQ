// Copyright (c) Microsoft Corporation.  All rights reserved.
// This source code is made available under the terms of the Microsoft Public License (MS-PL)

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace IQToolkit
{
    public interface IQueryText
    {
        string GetQueryText(Expression expression);
    }

    public class Query<T> : IOrderedQueryable<T>
    {
        private readonly Expression expression;
        private readonly IQueryProvider provider;

        public Query(IQueryProvider provider)
            : this(provider, (Type)null)
        {
        }

        public Query(IQueryProvider provider, Type staticType)
        {
            if (provider == null)
                throw new ArgumentNullException("provider");

            this.provider = provider;
            expression = staticType != null ? Expression.Constant(this, staticType) : Expression.Constant(this);
        }

        public Query(IQueryProvider provider, Expression expression)
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

        public string QueryText
        {
            get { return provider is IQueryText ? ((IQueryText)provider).GetQueryText(expression) : ""; }
        }

        public Expression Expression
        {
            get { return expression; }
        }

        public Type ElementType
        {
            get { return typeof(T); }
        }

        public IQueryProvider Provider
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
            if (expression.NodeType == ExpressionType.Constant && ((ConstantExpression)expression).Value == this)
                return "Query(" + typeof(T) + ")";
            
            return expression.ToString();
        }
    }
}