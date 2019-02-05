// Licensed under the Apache 2.0 License. See LICENSE.txt in the project root for more information.

using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace ElasticLinq.Async
{
    public static partial class AsyncQueryable
    {
        static readonly Lazy<MethodInfo> firstMethodInfo = QueryableMethodByArgs("First", 1);
        static readonly Lazy<MethodInfo> firstPredicateMethodInfo = QueryableMethodByArgs("First", 2);
        static readonly Lazy<MethodInfo> firstOrDefaultMethodInfo = QueryableMethodByArgs("FirstOrDefault", 1);
        static readonly Lazy<MethodInfo> firstOrDefaultPredicateMethodInfo = QueryableMethodByArgs("FirstOrDefault", 2);
        
        static readonly Lazy<MethodInfo> singleMethodInfo = QueryableMethodByArgs("Single", 1);
        static readonly Lazy<MethodInfo> singlePredicateMethodInfo = QueryableMethodByArgs("Single", 2);
        static readonly Lazy<MethodInfo> singleOrDefaultMethodInfo = QueryableMethodByArgs("SingleOrDefault", 1);
        static readonly Lazy<MethodInfo> singleOrDefaultPredicateMethodInfo = QueryableMethodByArgs("SingleOrDefault", 2);

        /// <summary>
        /// Asynchronously returns the first element of a sequence.
        /// </summary>
        /// <returns>
        /// A task that returns the first element in <paramref name="source"/>.
        /// </returns>
        /// <param name="source">The <see cref="IQueryable{T}"/> to return the first element of.</param>
        /// <param name="cancellationToken">The optional <see cref="CancellationToken"/> which can be used to cancel this task.</param>
        /// <typeparam name="TSource">The type of the elements of <paramref name="source"/>.</typeparam>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="source"/> is null.</exception>
        /// <exception cref="InvalidOperationException">The source sequence is empty.</exception>
        public static async Task<TSource> FirstAsync<TSource>(this IQueryable<TSource> source, CancellationToken cancellationToken = default(CancellationToken))
        {
            return (TSource)await ExecuteAsync(source.Provider, FinalExpression(source, firstMethodInfo.Value), cancellationToken).ConfigureAwait(false);
        }

        /// <summary>
        /// Asynchronously returns the first element of a sequence that satisfies a specified condition.
        /// </summary>
        /// <returns>
        /// A task that returns the first element in <paramref name="source"/> that passes the test in <paramref name="predicate"/>.
        /// </returns>
        /// <param name="source">An <see cref="IQueryable{T}"/> to return an element from.</param>
        /// <param name="predicate">A function to test each element for a condition.</param>
        /// <param name="cancellationToken">The optional <see cref="CancellationToken"/> which can be used to cancel this task.</param>
        /// <typeparam name="TSource">The type of the elements of <paramref name="source"/>.</typeparam>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="source"/> or <paramref name="predicate"/> is null.</exception>
        /// <exception cref="InvalidOperationException">No element satisfies the condition in <paramref name="predicate"/>.-or-The source sequence is empty.</exception>
        public static async Task<TSource> FirstAsync<TSource>(this IQueryable<TSource> source, Expression<Func<TSource, bool>> predicate, CancellationToken cancellationToken = default(CancellationToken))
        {
            return (TSource)await ExecuteAsync(source.Provider, FinalExpression(source, firstPredicateMethodInfo.Value, predicate), cancellationToken).ConfigureAwait(false);
        }

        /// <summary>
        /// Asynchronously returns the first element of a sequence, or a default value if the sequence contains no elements.
        /// </summary>
        /// <returns>
        /// A task that returns default(<typeparamref name="TSource"/>) if <paramref name="source"/> is empty; otherwise, the first element in <paramref name="source"/>.
        /// </returns>
        /// <param name="source">The <see cref="IQueryable{T}"/> to return the first element of.</param>
        /// <param name="cancellationToken">The optional <see cref="CancellationToken"/> which can be used to cancel this task.</param>
        /// <typeparam name="TSource">The type of the elements of <paramref name="source"/>.</typeparam>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="source"/> is null.</exception>
        public static async Task<TSource> FirstOrDefaultAsync<TSource>(this IQueryable<TSource> source, CancellationToken cancellationToken = default(CancellationToken))
        {
            return (TSource)await ExecuteAsync(source.Provider, FinalExpression(source, firstOrDefaultMethodInfo.Value), cancellationToken).ConfigureAwait(false);
        }

        /// <summary>
        /// Asynchronously returns the first element of a sequence that satisfies a specified condition or a default value if no such element is found.
        /// </summary>
        /// <returns>
        /// A task that returns default(<typeparamref name="TSource"/>) if <paramref name="source"/> is empty or if no element passes the test specified by <paramref name="predicate"/>; otherwise, the first element in <paramref name="source"/> that passes the test specified by <paramref name="predicate"/>.
        /// </returns>
        /// <param name="source">An <see cref="IQueryable{T}"/> to return an element from.</param>
        /// <param name="predicate">A function to test each element for a condition.</param>
        /// <param name="cancellationToken">The optional <see cref="CancellationToken"/> which can be used to cancel this task.</param>
        /// <typeparam name="TSource">The type of the elements of <paramref name="source"/>.</typeparam>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="source"/> or <paramref name="predicate"/> is null.</exception>
        public static async Task<TSource> FirstOrDefaultAsync<TSource>(this IQueryable<TSource> source, Expression<Func<TSource, bool>> predicate, CancellationToken cancellationToken = default(CancellationToken))
        {
            return (TSource)await ExecuteAsync(source.Provider, FinalExpression(source, firstOrDefaultPredicateMethodInfo.Value, predicate), cancellationToken).ConfigureAwait(false);
        }

        /// <summary>
        /// Asynchronously returns the only element of a sequence, and throws an exception if there is not exactly one element in the sequence.
        /// </summary>
        /// <returns>
        /// A task that returns the single element of the input sequence.
        /// </returns>
        /// <param name="source">An <see cref="IQueryable{T}"/> to return the single element of.</param>
        /// <param name="cancellationToken">The optional <see cref="CancellationToken"/> which can be used to cancel this task.</param>
        /// <typeparam name="TSource">The type of the elements of <paramref name="source"/>.</typeparam>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="source"/> is null.</exception>
        /// <exception cref="InvalidOperationException">
        /// <paramref name="source"/> has more than one element.</exception>
        public static async Task<TSource> SingleAsync<TSource>(this IQueryable<TSource> source, CancellationToken cancellationToken = default(CancellationToken))
        {
            return (TSource)await ExecuteAsync(source.Provider, FinalExpression(source, singleMethodInfo.Value), cancellationToken).ConfigureAwait(false);
        }

        /// <summary>
        /// Asynchronously returns the only element of a sequence that satisfies a specified condition, and throws an exception if more than one such element exists.
        /// </summary>
        /// <returns>
        /// A task that returns the single element of the input sequence that satisfies the condition in <paramref name="predicate"/>.
        /// </returns>
        /// <param name="source">An <see cref="IQueryable{T}"/> to return a single element from.</param>
        /// <param name="predicate">A function to test an element for a condition.</param>
        /// <param name="cancellationToken">The optional <see cref="CancellationToken"/> which can be used to cancel this task.</param>
        /// <typeparam name="TSource">The type of the elements of <paramref name="source"/>.</typeparam>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="source"/> or <paramref name="predicate"/> is null.</exception>
        /// <exception cref="InvalidOperationException">No element satisfies the condition in <paramref name="predicate"/>.-or-More than one element satisfies the condition in <paramref name="predicate"/>.-or-The source sequence is empty.</exception>
        public static async Task<TSource> SingleAsync<TSource>(this IQueryable<TSource> source, Expression<Func<TSource, bool>> predicate, CancellationToken cancellationToken = default(CancellationToken))
        {
            return (TSource)await ExecuteAsync(source.Provider, FinalExpression(source, singlePredicateMethodInfo.Value, predicate), cancellationToken).ConfigureAwait(false);
        }

        /// <summary>
        /// Asynchronously returns the only element of a sequence, or a default value if the sequence is empty; this method throws an exception if there is more than one element in the sequence.
        /// </summary>
        /// <returns>
        /// A task that returns the single element of the input sequence, or default(<typeparamref name="TSource"/>) if the sequence contains no elements.
        /// </returns>
        /// <param name="source">An <see cref="IQueryable{T}"/> to return the single element of.</param>
        /// <param name="cancellationToken">The optional <see cref="CancellationToken"/> which can be used to cancel this task.</param>
        /// <typeparam name="TSource">The type of the elements of <paramref name="source"/>.</typeparam>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="source"/> is null.</exception>
        /// <exception cref="InvalidOperationException">
        /// <paramref name="source"/> has more than one element.</exception>
        public static async Task<TSource> SingleOrDefaultAsync<TSource>(this IQueryable<TSource> source, CancellationToken cancellationToken = default(CancellationToken))
        {
            return (TSource)await ExecuteAsync(source.Provider, FinalExpression(source, singleOrDefaultMethodInfo.Value), cancellationToken).ConfigureAwait(false);
        }

        /// <summary>
        /// Asynchronously returns the only element of a sequence that satisfies a specified condition or a default value if no such element exists; this method throws an exception if more than one element satisfies the condition.
        /// </summary>
        /// <returns>
        /// A task that returns the single element of the input sequence that satisfies the condition in <paramref name="predicate"/>, or default(<typeparamref name="TSource"/>) if no such element is found.
        /// </returns>
        /// <param name="source">An <see cref="IQueryable{T}"/> to return a single element from.</param>
        /// <param name="predicate">A function to test an element for a condition.</param>
        /// <param name="cancellationToken">The optional <see cref="CancellationToken"/> which can be used to cancel this task.</param>
        /// <typeparam name="TSource">The type of the elements of <paramref name="source"/>.</typeparam>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="source"/> or <paramref name="predicate"/> is null.</exception>
        /// <exception cref="InvalidOperationException">More than one element satisfies the condition in <paramref name="predicate"/>.</exception>
        public static async Task<TSource> SingleOrDefaultAsync<TSource>(this IQueryable<TSource> source, Expression<Func<TSource, bool>> predicate, CancellationToken cancellationToken = default(CancellationToken))
        {
            return (TSource)await ExecuteAsync(source.Provider, FinalExpression(source, singleOrDefaultPredicateMethodInfo.Value, predicate), cancellationToken).ConfigureAwait(false);
        }
    }
}