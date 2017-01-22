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
    /// Provides an <see cref="IElasticQuery{T}"/> that can be used by unit tests.
    /// </summary>
    /// <typeparam name="T">Element type this query is for.</typeparam>
    public class TestableElasticQuery<T> : IElasticQuery<T>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TestableElasticQuery{T}" /> class.
        /// </summary>
        /// <param name="context">The <see cref="TestableElasticContext"/> this query belongs to.</param>
        /// <param name="expression">The <see cref="Expression"/> that represents the LINQ query.</param>
        public TestableElasticQuery(TestableElasticContext context, Expression expression = null)
        {
            Context = context;
            ElementType = typeof(T);
            Expression = expression ?? Expression.Constant(context.Data<T>().AsQueryable());
        }

        /// <summary>
        /// The <see cref="TestableElasticContext"/> this query belongs to.
        /// </summary>
        public TestableElasticContext Context { get; private set; }

        /// <inheritdoc/>
        public Type ElementType { get; private set; }

        /// <inheritdoc/>
        public Expression Expression { get; private set; }

        /// <inheritdoc/>
        public IQueryProvider Provider => Context.Provider;

        /// <inheritdoc/>
        public IEnumerator<T> GetEnumerator()
        {
            Context.Requests.Add(ToQueryInfo());
            return ((IEnumerable<T>)Provider.Execute(Expression)).GetEnumerator();
        }

        /// <inheritdoc/>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        /// <inheritdoc/>
        public QueryInfo ToQueryInfo()
        {
            var request = ElasticQueryTranslator.Translate(Context.Mapping, Expression);
            var formatter = new SearchRequestFormatter(Context.Connection, Context.Mapping, request.SearchRequest);
            return new QueryInfo(formatter.Body, Context.Connection.GetSearchUri(request.SearchRequest));
        }
    }
}
