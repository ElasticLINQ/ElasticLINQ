// Licensed under the Apache 2.0 License. See LICENSE.txt in the project root for more information.

using ElasticLinq.Request;
using ElasticLinq.Request.Formatters;
using ElasticLinq.Request.Visitors;
using ElasticLinq.Utility;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace ElasticLinq
{
    /// <summary>
    /// Represents a LINQ query object to be used with Elasticsearch.
    /// </summary>
    /// <typeparam name="T">Element type being queried.</typeparam>
    public class ElasticQuery<T> : IElasticQuery<T>
    {
        readonly ElasticQueryProvider provider;
        readonly Expression expression;

        /// <summary>
        /// Initializes a new instance of the <see cref="ElasticQuery{T}"/> class.
        /// </summary>
        /// <param name="provider">The <see cref="ElasticQueryProvider"/> used to execute the queries.</param>
        public ElasticQuery(ElasticQueryProvider provider)
        {
            Argument.EnsureNotNull(nameof(provider), provider);

            this.provider = provider;
            expression = Expression.Constant(this);
        }

        /// <summary>
        /// Initializes a new instance of this class.
        /// </summary>
        /// <param name="provider">The <see cref="ElasticQueryProvider"/> used to execute the queries.</param>
        /// <param name="expression">The <see cref="Expression"/> that represents the LINQ query so far.</param>
        public ElasticQuery(ElasticQueryProvider provider, Expression expression)
        {
            Argument.EnsureNotNull(nameof(provider), provider);
            Argument.EnsureNotNull(nameof(expression), expression);

            if (!typeof(IQueryable<T>).IsAssignableFrom(expression.Type))
                throw new ArgumentOutOfRangeException("expression");

            this.provider = provider;
            this.expression = expression;
        }

        /// <inheritdoc/>
        public IEnumerator<T> GetEnumerator()
        {
            return ((IEnumerable<T>)provider.Execute(expression)).GetEnumerator();
        }

        /// <inheritdoc/>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable)provider.Execute(expression)).GetEnumerator();
        }

        /// <inheritdoc/>
        public Type ElementType => typeof(T);

        /// <inheritdoc/>
        public Expression Expression => expression;

        /// <inheritdoc/>
        public IQueryProvider Provider => provider;

        /// <inheritdoc/>
        public QueryInfo ToQueryInfo()
        {
            var request = ElasticQueryTranslator.Translate(provider.Mapping, Expression);
            var formatter = new SearchRequestFormatter(provider.Connection, provider.Mapping, request.SearchRequest);

            return new QueryInfo(formatter.Body, provider.Connection.GetSearchUri(request.SearchRequest));
        }
    }
}