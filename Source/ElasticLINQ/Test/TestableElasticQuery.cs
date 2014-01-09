// Licensed under the Apache 2.0 License. See LICENSE.txt in the project root for more information.

using ElasticLinq.Request.Formatter;
using ElasticLinq.Request.Visitors;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace ElasticLinq.Test
{
    public class TestableElasticQuery<T> : IElasticQuery<T>
    {
        public TestableElasticQuery(TestableElasticContext context, Expression expression = null)
        {
            Context = context;
            ElementType = typeof(T);
            Expression = expression ?? Expression.Constant(context.Data<T>().AsQueryable());
        }

        public TestableElasticContext Context { get; private set; }

        public Type ElementType { get; private set; }

        public Expression Expression { get; private set; }

        public IQueryProvider Provider { get { return Context.Provider; } }

        public IEnumerator<T> GetEnumerator()
        {
            Context.Requests.Add(ToElasticSearchQuery());

            return ((IEnumerable<T>)Provider.Execute(Expression)).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public string ToElasticSearchQuery()
        {
            var request = ElasticQueryTranslator.Translate(Context.Mapping, Expression);
            var formatter = new PostBodyRequestFormatter(Context.Connection, request.SearchRequest);
            return formatter.Body;
        }
    }
}
