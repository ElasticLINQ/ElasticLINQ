// Licensed under the Apache 2.0 License. See LICENSE.txt in the project root for more information.

using ElasticLinq.Utility;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace ElasticLinq
{
    /// <summary>
    /// Extension methods that extend LINQ functionality for ElasticSearch queries.
    /// </summary>
    /// <remarks>
    /// Using these methods against any provider except <see cref="ElasticQueryProvider"/> will fail.
    /// </remarks>
    public static class ElasticQueryExtensions
    {
        /// <summary>
        /// Queries an ElasticSearch index based on a predicate.
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
            return CreateQueryMethodCall(source, (MethodInfo)MethodBase.GetCurrentMethod(), Expression.Quote(predicate));
        }

        /// <summary>
        /// Queries an ElasticSearch index based on a query string.
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
            return CreateQueryMethodCall(source, (MethodInfo)MethodBase.GetCurrentMethod(), Expression.Constant(query));
        }

        /// <summary>
        /// Sorts the elements of a sequence in ascending order by their ElasticSearch score.
        /// </summary>
        /// <returns>
        /// An <see cref="T:System.Linq.IOrderedQueryable`1"/> whose elements are sorted according to score.
        /// </returns>
        /// <param name="source">A sequence of values to order.</param>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="source"/> is null.</exception>
        public static IOrderedQueryable<TSource> OrderByScore<TSource>(this IQueryable<TSource> source)
        {
            return (IOrderedQueryable<TSource>)CreateQueryMethodCall(source, (MethodInfo)MethodBase.GetCurrentMethod());
        }

        /// <summary>
        /// Sorts the elements of a sequence in descending order by their ElasticSearch score.
        /// </summary>
        /// <returns>
        /// An <see cref="T:System.Linq.IOrderedQueryable`1"/> whose elements are sorted according to score.
        /// </returns>
        /// <param name="source">A sequence of values to order.</param>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="source"/> is null.</exception>
        public static IOrderedQueryable<TSource> OrderByScoreDescending<TSource>(this IQueryable<TSource> source)
        {
            return (IOrderedQueryable<TSource>)CreateQueryMethodCall(source, (MethodInfo)MethodBase.GetCurrentMethod());
        }

        /// <summary>
        /// Performs a subsequent ordering of the elements in a sequence in ascending order by their ElasticSearch score.
        /// </summary>
        /// <returns>
        /// An <see cref="T:System.Linq.IOrderedQueryable`1"/> whose elements are sorted according to score.
        /// </returns>
        /// <param name="source">A sequence of values to order.</param>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="source"/> is null.</exception>
        public static IOrderedQueryable<TSource> ThenByScore<TSource>(this IOrderedQueryable<TSource> source)
        {
            return (IOrderedQueryable<TSource>)CreateQueryMethodCall(source, (MethodInfo)MethodBase.GetCurrentMethod());
        }

        /// <summary>
        /// Performs a subsequent ordering of the elements in a sequence in descending order by their ElasticSearch score.
        /// </summary>
        /// <returns>
        /// An <see cref="T:System.Linq.IOrderedQueryable`1"/> whose elements are sorted according to score.
        /// </returns>
        /// <param name="source">A sequence of values to order.</param>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="source"/> is null.</exception>
        public static IOrderedQueryable<TSource> ThenByScoreDescending<TSource>(this IOrderedQueryable<TSource> source)
        {
            return (IOrderedQueryable<TSource>)CreateQueryMethodCall(source, (MethodInfo)MethodBase.GetCurrentMethod());
        }

        /// <summary>
        /// Converts an <see cref="IElasticQuery{T}"/> into the JSON that would be submitted
        /// to ElasticSearch.
        /// </summary>
        /// <param name="source">An <see cref="T:System.Linq.IQueryable{T}"/> to query.</param>
        /// <typeparam name="TSource">The type of the elements of <paramref name="source"/>.</typeparam>
        /// <returns>The ElasticSearch JSON representing this query.</returns>
        /// <exception cref="ArgumentException"><paramref name="source"/> is not of type <see cref="IElasticQuery{T}"/>.</exception>
        public static string ToElasticSearchQuery<TSource>(this IQueryable<TSource> source)
        {
            var elasticQuery = source as IElasticQuery<TSource>;
            if (elasticQuery == null)
                throw new ArgumentException("Query must be of type IElasticQuery<> to call ToElasticSearchQuery()", "source");

            return elasticQuery.ToElasticSearchQuery();
        }

        private static IQueryable<TSource> CreateQueryMethodCall<TSource>(IQueryable<TSource> source, MethodInfo method, params Expression[] arguments)
        {
            Argument.EnsureNotNull("source", source);
            Argument.EnsureNotNull("method", source);

            var callExpression = Expression.Call(null, method.MakeGenericMethod(typeof(TSource)), new[] { source.Expression }.Concat(arguments));
            return source.Provider.CreateQuery<TSource>(callExpression);
        }
    }
}