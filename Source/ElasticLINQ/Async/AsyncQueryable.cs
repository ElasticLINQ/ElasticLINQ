// Licensed under the Apache 2.0 License. See LICENSE.txt in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace ElasticLinq.Async
{
    /// <summary>
    /// Provides a set of static methods for querying data structures that implement <see cref="T:System.Linq.IQueryable`1"/> in an asynchronous manner.
    /// </summary>
    public static partial class AsyncQueryable
    {
        private static readonly Lazy<MethodInfo> countMethodInfo = QueryableMethodByArgs("Count", 1);
        private static readonly Lazy<MethodInfo> countPredicateMethodInfo = QueryableMethodByArgs("Count", 2);
        private static readonly Lazy<MethodInfo> longCountMethodInfo = QueryableMethodByArgs("LongCount", 1);
        private static readonly Lazy<MethodInfo> longCountPredicateMethodInfo = QueryableMethodByArgs("LongCount", 2);
        private static readonly Lazy<MethodInfo> minMethodInfo = QueryableMethodByArgs("Min", 1);
        private static readonly Lazy<MethodInfo> minSelectorMethodInfo = QueryableMethodByArgs("Min", 2);
        private static readonly Lazy<MethodInfo> maxMethodInfo = QueryableMethodByArgs("Max", 1);
        private static readonly Lazy<MethodInfo> maxSelectorMethodInfo = QueryableMethodByArgs("Max", 2);

        /// <summary>
        /// Asynchronously returns the number of elements in a sequence.
        /// </summary>
        /// <returns>
        /// A task that returns the number of elements in the input sequence.
        /// </returns>
        /// <param name="source">The <see cref="T:System.Linq.IQueryable`1"/> that contains the elements to be counted.</param>
        /// <param name="cancellationToken">The optional <see cref="T:System.Threading.CancellationToken"/> which can be used to cancel this task.</param>
        /// <typeparam name="TSource">The type of the elements of <paramref name="source"/>.</typeparam>
        /// <exception cref="T:System.ArgumentNullException">
        /// <paramref name="source"/> is null.</exception>
        /// <exception cref="T:System.OverflowException">The number of elements in <paramref name="source"/> is larger than <see cref="F:System.Int32.MaxValue"/>.</exception>
        public static async Task<int> CountAsync<TSource>(this IQueryable<TSource> source, CancellationToken cancellationToken = default(CancellationToken))
        {
            return (int)await ExecuteAsync(source.Provider, FinalExpression(source, countMethodInfo.Value), cancellationToken);
        }

        /// <summary>
        /// Asynchronously returns the number of elements in the specified sequence that satisfies a condition.
        /// </summary>
        /// <returns>
        /// A task that returns the number of elements in the sequence that satisfies the condition in the predicate function.
        /// </returns>
        /// <param name="source">An <see cref="T:System.Linq.IQueryable`1"/> that contains the elements to be counted.</param>
        /// <param name="predicate">A function to test each element for a condition.</param>
        /// <param name="cancellationToken">The optional <see cref="T:System.Threading.CancellationToken"/> which can be used to cancel this task.</param>
        /// <typeparam name="TSource">The type of the elements of <paramref name="source"/>.</typeparam>
        /// <exception cref="T:System.ArgumentNullException">
        /// <paramref name="source"/> or <paramref name="predicate"/> is null.</exception>
        /// <exception cref="T:System.OverflowException">The number of elements in <paramref name="source"/> is larger than <see cref="F:System.Int32.MaxValue"/>.</exception>
        public static async Task<int> CountAsync<TSource>(this IQueryable<TSource> source, Expression<Func<TSource, bool>> predicate, CancellationToken cancellationToken = default(CancellationToken))
        {
            return (int)await ExecuteAsync(source.Provider, FinalExpression(source, countPredicateMethodInfo.Value, predicate), cancellationToken);
        }

        /// <summary>
        /// Asynchronously returns an <see cref="T:System.Int64"/> that represents the total number of elements in a sequence.
        /// </summary>
        /// <returns>
        /// A task that returns the number of elements in <paramref name="source"/>.
        /// </returns>
        /// <param name="source">An <see cref="T:System.Linq.IQueryable`1"/> that contains the elements to be counted.</param>
        /// <param name="cancellationToken">The optional <see cref="T:System.Threading.CancellationToken"/> which can be used to cancel this task.</param>
        /// <typeparam name="TSource">The type of the elements of <paramref name="source"/>.</typeparam>
        /// <exception cref="T:System.ArgumentNullException">
        /// <paramref name="source"/> is null.</exception>
        /// <exception cref="T:System.OverflowException">The number of elements exceeds <see cref="F:System.Int64.MaxValue"/>.</exception>
        public static async Task<long> LongCountAsync<TSource>(this IQueryable<TSource> source, CancellationToken cancellationToken = default(CancellationToken))
        {
            return (long)await ExecuteAsync(source.Provider, FinalExpression(source, longCountMethodInfo.Value), cancellationToken);
        }

        /// <summary>
        /// Asynchronously returns an <see cref="T:System.Int64"/> that represents the number of elements in a sequence that satisfy a condition.
        /// </summary>
        /// <returns>
        /// A task that returns the number of elements in <paramref name="source"/> that satisfy the condition in the predicate function.
        /// </returns>
        /// <param name="source">An <see cref="T:System.Linq.IQueryable`1"/> that contains the elements to be counted.</param>
        /// <param name="predicate">A function to test each element for a condition.</param>
        /// <param name="cancellationToken">The optional <see cref="T:System.Threading.CancellationToken"/> which can be used to cancel this task.</param>
        /// <typeparam name="TSource">The type of the elements of <paramref name="source"/>.</typeparam>
        /// <exception cref="T:System.ArgumentNullException">
        /// <paramref name="source"/> or <paramref name="predicate"/> is null.</exception>
        /// <exception cref="T:System.OverflowException">The number of matching elements exceeds <see cref="F:System.Int64.MaxValue"/>.</exception>
        public static async Task<long> LongCountAsync<TSource>(this IQueryable<TSource> source, Expression<Func<TSource, bool>> predicate, CancellationToken cancellationToken = default(CancellationToken))
        {
            return (long)await ExecuteAsync(source.Provider, FinalExpression(source, longCountPredicateMethodInfo.Value, predicate), cancellationToken);
        }

        /// <summary>
        /// Asynchronously returns the minimum value of a generic <see cref="T:System.Linq.IQueryable`1"/>.
        /// </summary>
        /// <returns>
        /// A task that returns the minimum value in the sequence.
        /// </returns>
        /// <param name="source">A sequence of values to determine the minimum of.</param>
        /// <param name="cancellationToken">The optional <see cref="T:System.Threading.CancellationToken"/> which can be used to cancel this task.</param>
        /// <typeparam name="TSource">The type of the elements of <paramref name="source"/>.</typeparam>
        /// <exception cref="T:System.ArgumentNullException">
        /// <paramref name="source"/> is null.</exception>
        public static async Task<TSource> MinAsync<TSource>(this IQueryable<TSource> source, CancellationToken cancellationToken = default(CancellationToken))
        {
            return (TSource)await ExecuteAsync(source.Provider, FinalExpression(source, minMethodInfo.Value), cancellationToken);
        }

        /// <summary>
        /// Invokes a projection function on each element of a generic <see cref="T:System.Linq.IQueryable`1"/> and returns the minimum resulting value.
        /// </summary>
        /// <returns>
        /// A task that returns the minimum value in the sequence.
        /// </returns>
        /// <param name="source">A sequence of values to determine the minimum of.</param>
        /// <param name="selector">A projection function to apply to each element.</param>
        /// <param name="cancellationToken">The optional <see cref="T:System.Threading.CancellationToken"/> which can be used to cancel this task.</param>
        /// <typeparam name="TSource">The type of the elements of <paramref name="source"/>.</typeparam>
        /// <typeparam name="TResult">The type of the value returned by the function represented by <paramref name="selector"/>.</typeparam>
        /// <exception cref="T:System.ArgumentNullException">
        /// <paramref name="source"/> or <paramref name="selector"/> is null.</exception>
        public static async Task<TResult> MinAsync<TSource, TResult>(this IQueryable<TSource> source, Expression<Func<TSource, TResult>> selector, CancellationToken cancellationToken = default(CancellationToken))
        {
            return (TResult)await ExecuteAsync(source.Provider, FinalExpression<TSource, TResult>(source, minSelectorMethodInfo.Value, selector), cancellationToken);
        }

        /// <summary>
        /// Asynchronously returns the maximum value in a generic <see cref="T:System.Linq.IQueryable`1"/>.
        /// </summary>
        /// <returns>
        /// A task that returns the maximum value in the sequence.
        /// </returns>
        /// <param name="source">A sequence of values to determine the maximum of.</param>
        /// <param name="cancellationToken">The optional <see cref="T:System.Threading.CancellationToken"/> which can be used to cancel this task.</param>
        /// <typeparam name="TSource">The type of the elements of <paramref name="source"/>.</typeparam>
        /// <exception cref="T:System.ArgumentNullException">
        /// <paramref name="source"/> is null.</exception>
        public static async Task<TSource> MaxAsync<TSource>(this IQueryable<TSource> source, CancellationToken cancellationToken = default(CancellationToken))
        {
            return (TSource)await ExecuteAsync(source.Provider, FinalExpression(source, maxMethodInfo.Value), cancellationToken);
        }

        /// <summary>
        /// Invokes a projection function on each element of a generic <see cref="T:System.Linq.IQueryable`1"/> and returns the maximum resulting value.
        /// </summary>
        /// <returns>
        /// A task that returns the maximum value in the sequence.
        /// </returns>
        /// <param name="source">A sequence of values to determine the maximum of.</param>
        /// <param name="selector">A projection function to apply to each element.</param>
        /// <param name="cancellationToken">The optional <see cref="T:System.Threading.CancellationToken"/> which can be used to cancel this task.</param>
        /// <typeparam name="TSource">The type of the elements of <paramref name="source"/>.</typeparam>
        /// <typeparam name="TResult">The type of the value returned by the function represented by <paramref name="selector"/>.</typeparam>
        /// <exception cref="T:System.ArgumentNullException">
        /// <paramref name="source"/> or <paramref name="selector"/> is null.</exception>
        public static async Task<TResult> MaxAsync<TSource, TResult>(this IQueryable<TSource> source, Expression<Func<TSource, TResult>> selector, CancellationToken cancellationToken = default(CancellationToken))
        {
            return (TResult)await ExecuteAsync(source.Provider, FinalExpression<TSource, TResult>(source, maxSelectorMethodInfo.Value, selector), cancellationToken);
        }

        /// <summary>
        /// Creates a <see cref="T:System.Collection.List`1"/> from an <see cref="T:System.Linq.IQueryable`1"/> that is executed asyncronously.
        /// </summary>
        /// <returns>
        /// A task that returns the newly created list.
        /// </returns>
        /// <param name="source">A sequence of values to create a list from.</param>
        /// <param name="cancellationToken">The optional <see cref="T:System.Threading.CancellationToken"/> which can be used to cancel this task.</param>
        /// <typeparam name="TSource">The type of the elements of <paramref name="source"/>.</typeparam>
        public async static Task<List<TSource>> ToListAsync<TSource>(this IQueryable<TSource> source, CancellationToken cancellationToken = default(CancellationToken))
        {
            return ((IEnumerable<TSource>)await ExecuteAsync(source.Provider, source.Expression, cancellationToken)).ToList();
        }

        /// <summary>
        /// Creates an array from an <see cref="T:System.Linq.IQueryable`1"/> that is executed asyncronously.
        /// </summary>
        /// <returns>
        /// A task that returns the newly created array.
        /// </returns>
        /// <param name="source">A sequence of values to create an array from.</param>
        /// <param name="cancellationToken">The optional <see cref="T:System.Threading.CancellationToken"/> which can be used to cancel this task.</param>
        /// <typeparam name="TSource">The type of the elements of <paramref name="source"/>.</typeparam>
        public async static Task<TSource[]> ToArrayAsync<TSource>(this IQueryable<TSource> source, CancellationToken cancellationToken = default(CancellationToken))
        {
            return ((IEnumerable<TSource>)await ExecuteAsync(source.Provider, source.Expression, cancellationToken)).ToArray();
        }

        /// <summary>
        /// Creates a <see cref="T:System.Collections.Generic.Dictionary`2" /> from an <see cref="T:System.Collections.Generic.IEnumerable`1" /> according to a specified key selector function.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.Collections.Generic.Dictionary`2" /> that contains keys and values.
        /// </returns>
        /// <param name="source">An <see cref="T:System.Collections.Generic.IEnumerable`1" /> to create a <see cref="T:System.Collections.Generic.Dictionary`2" /> from.</param>
        /// <param name="keySelector">A function to extract a key from each element.</param>
        /// <param name="cancellationToken">The optional <see cref="T:System.Threading.CancellationToken"/> which can be used to cancel this task.</param>
        /// <typeparam name="TSource">The type of the elements of <paramref name="source" />.</typeparam>
        /// <typeparam name="TKey">The type of the key returned by <paramref name="keySelector" />.</typeparam>
        /// <exception cref="T:System.ArgumentNullException">
        /// <paramref name="source" /> or <paramref name="keySelector" /> is null.-or-<paramref name="keySelector" /> produces a key that is null.</exception>
        /// <exception cref="T:System.ArgumentException">
        /// <paramref name="keySelector" /> produces duplicate keys for two elements.</exception>
        public async static Task<Dictionary<TKey, TSource>> ToDictionaryAsync<TSource, TKey>(this IQueryable<TSource> source, Func<TSource, TKey> keySelector, CancellationToken cancellationToken = default(CancellationToken))
        {
            return ((IEnumerable<TSource>)await ExecuteAsync(source.Provider, source.Expression, cancellationToken)).ToDictionary(keySelector);
        }

        /// <summary>
        /// Creates a <see cref="T:System.Collections.Generic.Dictionary`2" /> from an <see cref="T:System.Collections.Generic.IEnumerable`1" /> according to specified key selector and element selector functions.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.Collections.Generic.Dictionary`2" /> that contains values of type <typeparamref name="TElement" /> selected from the input sequence.
        /// </returns>
        /// <param name="source">An <see cref="T:System.Collections.Generic.IEnumerable`1" /> to create a <see cref="T:System.Collections.Generic.Dictionary`2" /> from.</param>
        /// <param name="keySelector">A function to extract a key from each element.</param>
        /// <param name="elementSelector">A transform function to produce a result element value from each element.</param>
        /// <param name="cancellationToken">The optional <see cref="T:System.Threading.CancellationToken"/> which can be used to cancel this task.</param>
        /// <typeparam name="TSource">The type of the elements of <paramref name="source" />.</typeparam>
        /// <typeparam name="TKey">The type of the key returned by <paramref name="keySelector" />.</typeparam>
        /// <typeparam name="TElement">The type of the value returned by <paramref name="elementSelector" />.</typeparam>
        /// <exception cref="T:System.ArgumentNullException">
        /// <paramref name="source" /> or <paramref name="keySelector" /> or <paramref name="elementSelector" /> is null.-or-<paramref name="keySelector" /> produces a key that is null.</exception>
        /// <exception cref="T:System.ArgumentException">
        /// <paramref name="keySelector" /> produces duplicate keys for two elements.</exception>
        public async static Task<Dictionary<TKey, TElement>> ToDictionaryAsync<TSource, TKey, TElement>(this IQueryable<TSource> source, Func<TSource, TKey> keySelector, Func<TSource, TElement> elementSelector, CancellationToken cancellationToken = default(CancellationToken))
        {
            return ((IEnumerable<TSource>)await ExecuteAsync(source.Provider, source.Expression, cancellationToken)).ToDictionary(keySelector, elementSelector);
        }

        private static Lazy<MethodInfo> QueryableMethodByArgs(string name, int parameterCount, Type secondParameterType = null)
        {
            return new Lazy<MethodInfo>(() => typeof(Queryable).GetTypeInfo().DeclaredMethods
                .Single(m => m.Name == name && m.GetParameters().Length == parameterCount &&
                    (secondParameterType == null || m.GetParameters()[1].ParameterType == secondParameterType)));
        }

        private static Lazy<MethodInfo> QueryableMethodByReturnType(string name, int parameterCount, Type returnType)
        {
            return new Lazy<MethodInfo>(() => typeof(Queryable).GetTypeInfo().DeclaredMethods
                .Single(m => m.Name == name && m.ReturnType == returnType && m.GetParameters().Length == parameterCount));
        }

        private static Lazy<MethodInfo> QueryableMethodBySelectorParameterType(string name, int parameterCount, Type selectorParameterType)
        {
            return new Lazy<MethodInfo>(() =>
            {
                return typeof(Queryable).GetTypeInfo().DeclaredMethods
                    .Where(m => m.Name == name && m.IsGenericMethod)
                    .Select(m => Tuple.Create(m, m.GetParameters()))
                    .Where(m => m.Item2.Length == parameterCount)
                    .Single(m => m.Item2[1].ParameterType.GenericTypeArguments[0].GenericTypeArguments[1] == selectorParameterType).Item1;
            });
        }

        private static Lazy<MethodInfo> QueryableMethodByQueryableParameterType(string name, int parameterCount, Type sourceParameterType)
        {
            return new Lazy<MethodInfo>(() =>
            {
                var parameterType = typeof(IQueryable<>).MakeGenericType(sourceParameterType);

                return typeof(Queryable).GetTypeInfo().DeclaredMethods
                    .Single(m => m.Name == name && m.GetParameters().Length == parameterCount &&
                            m.GetParameters()[0].ParameterType == parameterType);
            });
        }

        private static Expression FinalExpression<TSource>(IQueryable<TSource> source, MethodInfo method, params Expression[] arguments)
        {
            var finalMethod = method.IsGenericMethod ? method.MakeGenericMethod(typeof (TSource)) : method;
            return Expression.Call(null, finalMethod, new[] { source.Expression }.Concat(arguments));
        }

        private static Expression FinalExpression<TSource, TResult>(IQueryable<TSource> source, MethodInfo method, params Expression[] arguments)
        {
            return Expression.Call(null, method.MakeGenericMethod(typeof(TSource), typeof(TResult)), new[] { source.Expression }.Concat(arguments));
        }

        private static Task<object> ExecuteAsync(IQueryProvider provider, Expression expression, CancellationToken cancellationToken)
        {
            return ((IAsyncQueryExecutor)provider).ExecuteAsync(expression, cancellationToken);
        }
    }
}