// Licensed under the Apache 2.0 License. See LICENSE.txt in the project root for more information.

using ElasticLinq.Request;
using ElasticLinq.Request.Formatters;
using ElasticLinq.Request.Visitors;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace ElasticLinq.Test
{
    /// <summary>
    /// Provides an IElasticQuery that can be used by unit tests.
    /// </summary>
    /// <typeparam name="T">Element type this query is for.</typeparam>
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
            Context.Requests.Add(ToQueryInfo());

            return ((IEnumerable<T>)Provider.Execute(Expression)).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public QueryInfo ToQueryInfo()
        {
            var prefix = Context.Mapping.GetDocumentMappingPrefix(typeof(T));
            var request = ElasticQueryTranslator.Translate(Context.Mapping, prefix, Expression);
            var formatter = new SearchRequestFormatter(Context.Connection, Context.Mapping, request.SearchRequest);
            return new QueryInfo(formatter.Body, formatter.Uri);
        }
    }
}
