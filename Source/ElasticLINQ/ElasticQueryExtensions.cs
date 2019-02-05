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
        /// Queries an Elasticsearch index based on a query string.
        /// </summary>
        /// <returns>
        /// An <see cref="IQueryable{T}"/> that contains elements from the input sequence that satisfy the condition specified by <paramref name="query"/>.
        /// </returns>
        /// <param name="source">An <see cref="IQueryable{T}"/> to query.</param>
        /// <param name="query">A query string to test each element for.</param>
        /// <typeparam name="TSource">The type of the elements of <paramref name="source"/>.</typeparam>
        /// <exception cref="ArgumentNullException"><paramref name="source"/> or <paramref name="query"/> is null.</exception>
        public static IQueryable<TSource> QueryString<TSource>(this IQueryable<TSource> source, string query)
        {
            Argument.EnsureNotNull(nameof(query), query);
            return CreateQueryMethodCall(source, queryStringMethodInfo, Expression.Constant(query));
        }

        /// <summary>
        /// Queries an Elasticsearch index based on a query string for specific field partterns.
        /// </summary>
        /// <returns>
        /// An <see cref="IQueryable{T}"/> that contains elements from the input sequence that satisfy the condition specified by <paramref name="query"/>.
        /// </returns>
        /// <param name="source">An <see cref="IQueryable{T}"/> to query.</param>
        /// <param name="query">A query string to test each element for.</param>
        /// <param name="fields">A list of field name patterns to search.</param>
        /// <typeparam name="TSource">The type of the elements of <paramref name="source"/>.</typeparam>
        /// <exception cref="ArgumentNullException"><paramref name="source"/>, <paramref name="query"/> or <paramref name="fields"/> is null.</exception>
        public static IQueryable<TSource> QueryString<TSource>(this IQueryable<TSource> source, string query, string[] fields)
        {
            Argument.EnsureNotNull(nameof(query), query);
            Argument.EnsureNotEmpty(nameof(fields), fields);
            return CreateQueryMethodCall(source, queryStringWithFieldsMethodInfo, Expression.Constant(query), Expression.Constant(fields));
        }

        /// <summary>
        /// Sorts the elements of a sequence in ascending order by their Elasticsearch score.
        /// </summary>
        /// <returns>
        /// An <see cref="IOrderedQueryable{T}"/> whose elements are sorted according to score.
        /// </returns>
        /// <param name="source">A sequence of values to order.</param>
        /// <exception cref="ArgumentNullException"><paramref name="source"/> is null.</exception>
        public static IOrderedQueryable<TSource> OrderByScore<TSource>(this IQueryable<TSource> source)
        {
            return (IOrderedQueryable<TSource>)CreateQueryMethodCall(source, orderByScoreMethodInfo);
        }

        /// <summary>
        /// Sorts the elements of a sequence in descending order by their Elasticsearch score.
        /// </summary>
        /// <returns>
        /// An <see cref="IOrderedQueryable{T}"/> whose elements are sorted according to score.
        /// </returns>
        /// <param name="source">A sequence of values to order.</param>
        /// <exception cref="ArgumentNullException"><paramref name="source"/> is null.</exception>
        public static IOrderedQueryable<TSource> OrderByScoreDescending<TSource>(this IQueryable<TSource> source)
        {
            return (IOrderedQueryable<TSource>)CreateQueryMethodCall(source, orderByScoreDescendingMethodInfo);
        }

        /// <summary>
        /// Performs a subsequent ordering of the elements in a sequence in ascending order by their Elasticsearch score.
        /// </summary>
        /// <returns>
        /// An <see cref="IOrderedQueryable{T}"/> whose elements are sorted according to score.
        /// </returns>
        /// <param name="source">A sequence of values to order.</param>
        /// <exception cref="ArgumentNullException"><paramref name="source"/> is null.</exception>
        public static IOrderedQueryable<TSource> ThenByScore<TSource>(this IOrderedQueryable<TSource> source)
        {
            return (IOrderedQueryable<TSource>)CreateQueryMethodCall(source, thenByScoreMethodInfo);
        }

        /// <summary>
        /// Performs a subsequent ordering of the elements in a sequence in descending order by their Elasticsearch score.
        /// </summary>
        /// <returns>
        /// An <see cref="IOrderedQueryable{T}"/> whose elements are sorted according to score.
        /// </returns>
        /// <param name="source">A sequence of values to order.</param>
        /// <exception cref="ArgumentNullException"><paramref name="source"/> is null.</exception>
        public static IOrderedQueryable<TSource> ThenByScoreDescending<TSource>(this IOrderedQueryable<TSource> source)
        {
            return (IOrderedQueryable<TSource>)CreateQueryMethodCall(source, thenByScoreDescendingMethodInfo);
        }

        /// <summary>
        /// Specifies a minimum Elasticsearch score.
        /// </summary>
        /// <returns>
        /// An <see cref="IQueryable{T}"/> whose elements have a score greater or equal to the score specified.
        /// </returns>
        /// <param name="source">A sequence of values to apply a minimum score to.</param>
        /// <param name="score">The minimal acceptable score for results.</param>
        /// <exception cref="ArgumentNullException"><paramref name="source"/> is null.</exception>
        public static IQueryable<TSource> MinScore<TSource>(this IQueryable<TSource> source, double score)
        {
            return CreateQueryMethodCall(source, minimumScoreMethodInfo, Expression.Constant(score));
        }

        /// <summary>
        /// Specifies highlighting for search results.
        /// </summary>
        /// <returns>
        /// An <see cref="IQueryable{T}"/> with elements containing hightlights as specified.
        /// </returns>
        /// <param name="source">A sequence of values to order.</param>
        /// <param name="predicate">A function to test each element for a condition.</param>
        /// <param name="highlight">Highlight specification to apply to search.</param>
        /// <exception cref="ArgumentNullException"><paramref name="source"/> or <paramref name="predicate"/> is null.</exception>
        public static IQueryable<TSource> Highlight<TSource, TKey>(this IQueryable<TSource> source, Expression<Func<TSource, TKey>> predicate, Highlight highlight = null)
        {
            Argument.EnsureNotNull(nameof(source), source);
            Argument.EnsureNotNull(nameof(predicate), predicate);
            return CreateQueryMethodCall<TSource, TKey>(source, highlightScoreMethodInfo, Expression.Quote(predicate), Expression.Constant(highlight ?? new Highlight()));
        }

        /// <summary>
        /// Return information about a <see cref="IElasticQuery{T}"/> including the JSON that would be submitted to Elasticsearch.
        /// </summary>
        /// <param name="source">An <see cref="IQueryable{T}"/> to query.</param>
        /// <typeparam name="TSource">The type of the elements of <paramref name="source"/>.</typeparam>
        /// <returns>QueryInfo including the Uri and Elasticsearch DSL JSON representing this query.</returns>
        /// <exception cref="ArgumentException"><paramref name="source"/> is not of type <see cref="IElasticQuery{T}"/>.</exception>
        public static QueryInfo ToQueryInfo<TSource>(this IQueryable<TSource> source)
        {
            var elasticQuery = source as IElasticQuery<TSource>;
            if (elasticQuery == null)
                throw new ArgumentException("Query must be of type IElasticQuery<> to call ToQueryInfo()", nameof(source));

            return elasticQuery.ToQueryInfo();
        }

        static readonly MethodInfo queryStringMethodInfo = typeof(ElasticQueryExtensions).GetMethodInfo(m => m.Name == "QueryString" && m.GetParameters().Length == 2);
        static readonly MethodInfo queryStringWithFieldsMethodInfo = typeof(ElasticQueryExtensions).GetMethodInfo(m => m.Name == "QueryString" && m.GetParameters().Length > 2);
        static readonly MethodInfo orderByScoreMethodInfo = typeof(ElasticQueryExtensions).GetMethodInfo(m => m.Name == "OrderByScore");
        static readonly MethodInfo orderByScoreDescendingMethodInfo = typeof(ElasticQueryExtensions).GetMethodInfo(m => m.Name == "OrderByScoreDescending");
        static readonly MethodInfo thenByScoreMethodInfo = typeof(ElasticQueryExtensions).GetMethodInfo(m => m.Name == "ThenByScore");
        static readonly MethodInfo thenByScoreDescendingMethodInfo = typeof(ElasticQueryExtensions).GetMethodInfo(m => m.Name == "ThenByScoreDescending");
        static readonly MethodInfo minimumScoreMethodInfo = typeof(ElasticQueryExtensions).GetMethodInfo(m => m.Name == "MinScore");
        static readonly MethodInfo highlightScoreMethodInfo = typeof(ElasticQueryExtensions).GetMethodInfo(m => m.Name == "Highlight");

        /// <summary>
        /// Creates an expression to call a generic version of the given method with the source and arguments as parameters..
        /// </summary>
        /// <typeparam name="TSource">Element type of the query derived from the IQueryable source.</typeparam>
        /// <param name="source">IQueryable source to use as the first parameter for the given method.</param>
        /// <param name="method">MethodInfo of the method to call.</param>
        /// <param name="arguments">Expressions that should be passed to the method as arguments.</param>
        /// <returns>IQueryable that contains the query with the method call inserted into the query chain.</returns>
        static IQueryable<TSource> CreateQueryMethodCall<TSource>(IQueryable<TSource> source, MethodInfo method, params Expression[] arguments)
        {
            Argument.EnsureNotNull(nameof(source), source);
            Argument.EnsureNotNull(nameof(method), source);

            var callExpression = Expression.Call(null, method.MakeGenericMethod(typeof(TSource)), new[] { source.Expression }.Concat(arguments));
            return source.Provider.CreateQuery<TSource>(callExpression);
        }

        static IQueryable<TSource> CreateQueryMethodCall<TSource, TKey>(IQueryable<TSource> source, MethodInfo method, params Expression[] arguments)
        {
            Argument.EnsureNotNull(nameof(source), source);
            Argument.EnsureNotNull(nameof(method), source);

            var callExpression = Expression.Call(null, method.MakeGenericMethod(typeof(TSource), typeof(TKey)), new[] { source.Expression }.Concat(arguments));
            return source.Provider.CreateQuery<TSource>(callExpression);
        }
    }
}