// Licensed under the Apache 2.0 License. See LICENSE.txt in the project root for more information.

using ElasticLinq.Request;
using ElasticLinq.Utility;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace ElasticLinq
{
    /// <summary>
    /// Extension methods that extend LINQ functionality for Elasticsearch queries.
    /// </summary>
    /// <remarks>
    /// Using these methods against any provider except <see cref="ElasticQueryProvider"/> will fail.
    /// </remarks>
    public static class ElasticQueryExtensions
    {
        /// <summary>
        /// Queries an Elasticsearch index based on a predicate.
        /// </summary>
        /// <returns>
        /// An <see cref="T:System.Linq.IQueryable`1"/> that contains elements from the input sequence that satisfy the condition specified by <paramref name="predicate"/>.
        /// </returns>
        /// <param name="source">An <see cref="T:System.Linq.IQueryable`1"/> to query.</param>
        /// <param name="predicate">A function to test each element for a condition.</param>
        /// <typeparam name="TSource">The type of the elements of <paramref name="source"/>.</typeparam>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="source"/> or <paramref name="predicate"/> is null.</exception>
        public static IQueryable<TSource> Query<TSource>(this IQueryable<TSource> source, Expression<Func<TSource, bool>> predicate)
        {
            Argument.EnsureNotNull("predicate", predicate);
            return CreateQueryMethodCall(source, queryMethodInfo, Expression.Quote(predicate));
        }

        /// <summary>
        /// Queries an Elasticsearch index based on a query string.
        /// </summary>
        /// <returns>
        /// An <see cref="T:System.Linq.IQueryable`1"/> that contains elements from the input sequence that satisfy the condition specified by <paramref name="query"/>.
        /// </returns>
        /// <param name="source">An <see cref="T:System.Linq.IQueryable`1"/> to query.</param>
        /// <param name="query">A query string to test each element for.</param>
        /// <typeparam name="TSource">The type of the elements of <paramref name="source"/>.</typeparam>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="source"/> or <paramref name="query"/> is null.</exception>
        public static IQueryable<TSource> QueryString<TSource>(this IQueryable<TSource> source, string query)
        {
            Argument.EnsureNotNull("query", query);
            return CreateQueryMethodCall(source, queryStringMethodInfo, Expression.Constant(query));
        }

        /// <summary>
        /// Queries an Elasticsearch index based on a query string for specific field partterns.
        /// </summary>
        /// <returns>
        /// An <see cref="T:System.Linq.IQueryable`1"/> that contains elements from the input sequence that satisfy the condition specified by <paramref name="query"/>.
        /// </returns>
        /// <param name="source">An <see cref="T:System.Linq.IQueryable`1"/> to query.</param>
        /// <param name="query">A query string to test each element for.</param>
        /// <param name="fields">A list of field name patterns to search.</param>
        /// <typeparam name="TSource">The type of the elements of <paramref name="source"/>.</typeparam>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="source"/>, <paramref name="query"/> or <paramref name="fields"/> is null.</exception>
        public static IQueryable<TSource> QueryString<TSource>(this IQueryable<TSource> source, string query, string[] fields)
        {
            Argument.EnsureNotNull("query", query);
            Argument.EnsureNotEmpty("fields", fields);
            return CreateQueryMethodCall(source, queryStringWithFieldsMethodInfo, Expression.Constant(query), Expression.Constant(fields));
        }

        /// <summary>
        /// Sorts the elements of a sequence in ascending order by their Elasticsearch score.
        /// </summary>
        /// <returns>
        /// An <see cref="T:System.Linq.IOrderedQueryable`1"/> whose elements are sorted according to score.
        /// </returns>
        /// <param name="source">A sequence of values to order.</param>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="source"/> is null.</exception>
        public static IOrderedQueryable<TSource> OrderByScore<TSource>(this IQueryable<TSource> source)
        {
            return (IOrderedQueryable<TSource>)CreateQueryMethodCall(source, orderByScoreMethodInfo);
        }

        /// <summary>
        /// Sorts the elements of a sequence in descending order by their Elasticsearch score.
        /// </summary>
        /// <returns>
        /// An <see cref="T:System.Linq.IOrderedQueryable`1"/> whose elements are sorted according to score.
        /// </returns>
        /// <param name="source">A sequence of values to order.</param>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="source"/> is null.</exception>
        public static IOrderedQueryable<TSource> OrderByScoreDescending<TSource>(this IQueryable<TSource> source)
        {
            return (IOrderedQueryable<TSource>)CreateQueryMethodCall(source, orderByScoreDescendingMethodInfo);
        }

        /// <summary>
        /// Performs a subsequent ordering of the elements in a sequence in ascending order by their Elasticsearch score.
        /// </summary>
        /// <returns>
        /// An <see cref="T:System.Linq.IOrderedQueryable`1"/> whose elements are sorted according to score.
        /// </returns>
        /// <param name="source">A sequence of values to order.</param>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="source"/> is null.</exception>
        public static IOrderedQueryable<TSource> ThenByScore<TSource>(this IOrderedQueryable<TSource> source)
        {
            return (IOrderedQueryable<TSource>)CreateQueryMethodCall(source, thenByScoreMethodInfo);
        }

        /// <summary>
        /// Performs a subsequent ordering of the elements in a sequence in descending order by their Elasticsearch score.
        /// </summary>
        /// <returns>
        /// An <see cref="T:System.Linq.IOrderedQueryable`1"/> whose elements are sorted according to score.
        /// </returns>
        /// <param name="source">A sequence of values to order.</param>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="source"/> is null.</exception>
        public static IOrderedQueryable<TSource> ThenByScoreDescending<TSource>(this IOrderedQueryable<TSource> source)
        {
            return (IOrderedQueryable<TSource>)CreateQueryMethodCall(source, thenByScoreDescendingMethodInfo);
        }

        /// <summary>
        /// Return information about a <see cref="IElasticQuery{T}"/> including the JSON that would be submitted to Elasticsearch.
        /// </summary>
        /// <param name="source">An <see cref="T:System.Linq.IQueryable{T}"/> to query.</param>
        /// <typeparam name="TSource">The type of the elements of <paramref name="source"/>.</typeparam>
        /// <returns>QueryInfo including the Uri and Elasticsearch DSL JSON representing this query.</returns>
        /// <exception cref="ArgumentException"><paramref name="source"/> is not of type <see cref="IElasticQuery{T}"/>.</exception>
        public static QueryInfo ToQueryInfo<TSource>(this IQueryable<TSource> source)
        {
            var elasticQuery = source as IElasticQuery<TSource>;
            if (elasticQuery == null)
                throw new ArgumentException("Query must be of type IElasticQuery<> to call ToQueryInfo()", "source");

            return elasticQuery.ToQueryInfo();
        }

        private static readonly MethodInfo queryMethodInfo = typeof(ElasticQueryExtensions).GetMethodInfo(m => m.Name == "Query");
        private static readonly MethodInfo queryStringMethodInfo = typeof(ElasticQueryExtensions).GetMethodInfo(m => m.Name == "QueryString" && m.GetParameters().Count() == 2);
        private static readonly MethodInfo queryStringWithFieldsMethodInfo = typeof(ElasticQueryExtensions).GetMethodInfo(m => m.Name == "QueryString" && m.GetParameters().Count() > 2);
        private static readonly MethodInfo orderByScoreMethodInfo = typeof(ElasticQueryExtensions).GetMethodInfo(m => m.Name == "OrderByScore");
        private static readonly MethodInfo orderByScoreDescendingMethodInfo = typeof(ElasticQueryExtensions).GetMethodInfo(m => m.Name == "OrderByScoreDescending");
        private static readonly MethodInfo thenByScoreMethodInfo = typeof(ElasticQueryExtensions).GetMethodInfo(m => m.Name == "ThenByScore");
        private static readonly MethodInfo thenByScoreDescendingMethodInfo = typeof(ElasticQueryExtensions).GetMethodInfo(m => m.Name == "ThenByScoreDescending");

        private static IQueryable<TSource> CreateQueryMethodCall<TSource>(IQueryable<TSource> source, MethodInfo method, params Expression[] arguments)
        {
            Argument.EnsureNotNull("source", source);
            Argument.EnsureNotNull("method", source);

            var callExpression = Expression.Call(null, method.MakeGenericMethod(typeof(TSource)), new[] { source.Expression }.Concat(arguments));
            return source.Provider.CreateQuery<TSource>(callExpression);
        }
    }
}