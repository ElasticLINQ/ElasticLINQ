﻿// Licensed under the Apache 2.0 License. See LICENSE.txt in the project root for more information.

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

        /// <summary>
        /// Initializes a new instance of the <see cref="ElasticQuery{T}"/> class.
        /// </summary>
        /// <param name="provider">The <see cref="ElasticQueryProvider"/> used to execute the queries.</param>
        public ElasticQuery(ElasticQueryProvider provider)
        {
            Argument.EnsureNotNull(nameof(provider), provider);

            this.provider = provider;
            Expression = Expression.Constant(this);
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
                throw new ArgumentOutOfRangeException(nameof(expression));

            this.provider = provider;
            Expression = expression;
        }

        /// <inheritdoc/>
        public IEnumerator<T> GetEnumerator()
        {
            return ((IEnumerable<T>)provider.Execute(Expression)).GetEnumerator();
        }

        /// <inheritdoc/>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable)provider.Execute(Expression)).GetEnumerator();
        }

        /// <inheritdoc/>
        public Type ElementType
        {
            get { return typeof(T); }
        }

        /// <inheritdoc/>
        public Expression Expression { get; }

        /// <inheritdoc/>
        public IQueryProvider Provider
        {
            get { return provider; }
        }

        /// <inheritdoc/>
        public QueryInfo ToQueryInfo()
        {
            var request = ElasticQueryTranslator.Translate(provider.Mapping, Expression);
            var formatter = new SearchRequestFormatter(provider.Connection, provider.Mapping, request.SearchRequest);

            return new QueryInfo(formatter.Body, provider.Connection.GetSearchUri(request.SearchRequest));
        }
    }
}