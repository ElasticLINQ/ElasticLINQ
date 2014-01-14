// Licensed under the Apache 2.0 License. See LICENSE.txt in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace ElasticLinq.Test
{
    public class TestableElasticQueryProvider : IQueryProvider
    {
        private readonly TestableElasticContext context;

        public TestableElasticQueryProvider(TestableElasticContext context)
        {
            this.context = context;
        }

        public IQueryable<TElement> CreateQuery<TElement>(Expression expression)
        {
            return new TestableElasticQuery<TElement>(context, expression);
        }

        public IQueryable CreateQuery(Expression expression)
        {
            return CreateQuery<object>(expression);
        }

        public TResult Execute<TResult>(Expression expression)
        {
            return (TResult)Execute(expression);
        }

        public object Execute(Expression expression)
        {
            return LambdaExpression.Lambda(expression).Compile().DynamicInvoke();
        }
    }
}
