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
        static readonly Lazy<MethodInfo> sumIntMethodInfo = QueryableMethodByReturnType("Sum", 1, typeof(int));
        static readonly Lazy<MethodInfo> sumIntNullableMethodInfo = QueryableMethodByReturnType("Sum", 1, typeof(int?));
        static readonly Lazy<MethodInfo> sumLongMethodInfo = QueryableMethodByReturnType("Sum", 1, typeof(long));
        static readonly Lazy<MethodInfo> sumLongNullableMethodInfo = QueryableMethodByReturnType("Sum", 1, typeof(long?));
        static readonly Lazy<MethodInfo> sumFloatMethodInfo = QueryableMethodByReturnType("Sum", 1, typeof(float));
        static readonly Lazy<MethodInfo> sumFloatNullableMethodInfo = QueryableMethodByReturnType("Sum", 1, typeof(float?));
        static readonly Lazy<MethodInfo> sumDoubleMethodInfo = QueryableMethodByReturnType("Sum", 1, typeof(double));
        static readonly Lazy<MethodInfo> sumDoubleNullableMethodInfo = QueryableMethodByReturnType("Sum", 1, typeof(double?));
        static readonly Lazy<MethodInfo> sumDecimalMethodInfo = QueryableMethodByReturnType("Sum", 1, typeof(decimal));
        static readonly Lazy<MethodInfo> sumDecimalNullableMethodInfo = QueryableMethodByReturnType("Sum", 1, typeof(decimal?));

        static readonly Lazy<MethodInfo> sumIntSelectorMethodInfo = QueryableMethodByReturnType("Sum", 2, typeof(int));
        static readonly Lazy<MethodInfo> sumIntNullableSelectorMethodInfo = QueryableMethodByReturnType("Sum", 2, typeof(int?));
        static readonly Lazy<MethodInfo> sumLongSelectorMethodInfo = QueryableMethodByReturnType("Sum", 2, typeof(long));
        static readonly Lazy<MethodInfo> sumLongNullableSelectorMethodInfo = QueryableMethodByReturnType("Sum", 2, typeof(long?));
        static readonly Lazy<MethodInfo> sumFloatSelectorMethodInfo = QueryableMethodByReturnType("Sum", 2, typeof(float));
        static readonly Lazy<MethodInfo> sumFloatNullableSelectorMethodInfo = QueryableMethodByReturnType("Sum", 2, typeof(float?));
        static readonly Lazy<MethodInfo> sumDoubleSelectorMethodInfo = QueryableMethodByReturnType("Sum", 2, typeof(double));
        static readonly Lazy<MethodInfo> sumDoubleNullableSelectorMethodInfo = QueryableMethodByReturnType("Sum", 2, typeof(double?));
        static readonly Lazy<MethodInfo> sumDecimalSelectorMethodInfo = QueryableMethodByReturnType("Sum", 2, typeof(decimal));
        static readonly Lazy<MethodInfo> sumDecimalNullableSelectorMethodInfo = QueryableMethodByReturnType("Sum", 2, typeof(decimal?));

        /// <summary>
        /// Asynchronously computes the sum of a sequence of <see cref="Int32"/> values.
        /// </summary>
        /// <returns>
        /// A task that returns the sum of the values in the sequence.
        /// </returns>
        /// <param name="source">A sequence of <see cref="Int32"/> values to calculate the sum of.</param>
        /// <param name="cancellationToken">The optional <see cref="CancellationToken"/> which can be used to cancel this task.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="source"/> is null.</exception>
        /// <exception cref="OverflowException">The sum is larger than <see cref="Int32.MaxValue"/>.</exception>
        public static async Task<int> SumAsync(this IQueryable<int> source, CancellationToken cancellationToken = default(CancellationToken))
        {
            return (int)await ExecuteAsync(source.Provider, FinalExpression(source, sumIntMethodInfo.Value), cancellationToken).ConfigureAwait(false);
        }

        /// <summary>
        /// Asynchronously computes the sum of a sequence of nullable <see cref="Int32"/> values.
        /// </summary>
        /// <returns>
        /// A task that returns the sum of the values in the sequence.
        /// </returns>
        /// <param name="source">A sequence of nullable <see cref="Int32"/> values to calculate the sum of.</param>
        /// <param name="cancellationToken">The optional <see cref="CancellationToken"/> which can be used to cancel this task.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="source"/> is null.</exception>
        /// <exception cref="OverflowException">The sum is larger than <see cref="Int32.MaxValue"/>.</exception>
        public static async Task<int?> SumAsync(this IQueryable<int?> source, CancellationToken cancellationToken = default(CancellationToken))
        {
            return (int)await ExecuteAsync(source.Provider, FinalExpression(source, sumIntNullableMethodInfo.Value), cancellationToken).ConfigureAwait(false);
        }

        /// <summary>
        /// Asynchronously computes the sum of a sequence of <see cref="Int64"/> values.
        /// </summary>
        /// <returns>
        /// A task that returns the sum of the values in the sequence.
        /// </returns>
        /// <param name="source">A sequence of <see cref="Int64"/> values to calculate the sum of.</param>
        /// <param name="cancellationToken">The optional <see cref="CancellationToken"/> which can be used to cancel this task.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="source"/> is null.</exception>
        /// <exception cref="OverflowException">The sum is larger than <see cref="Int64.MaxValue"/>.</exception>
        public static async Task<long> SumAsync(this IQueryable<long> source, CancellationToken cancellationToken = default(CancellationToken))
        {
            return (long)await ExecuteAsync(source.Provider, FinalExpression(source, sumLongMethodInfo.Value), cancellationToken).ConfigureAwait(false);
        }

        /// <summary>
        /// Asynchronously computes the sum of a sequence of nullable <see cref="Int64"/> values.
        /// </summary>
        /// <returns>
        /// A task that returns the sum of the values in the sequence.
        /// </returns>
        /// <param name="source">A sequence of nullable <see cref="Int64"/> values to calculate the sum of.</param>
        /// <param name="cancellationToken">The optional <see cref="CancellationToken"/> which can be used to cancel this task.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="source"/> is null.</exception>
        /// <exception cref="OverflowException">The sum is larger than <see cref="Int64.MaxValue"/>.</exception>
        public static async Task<long?> SumAsync(this IQueryable<long?> source, CancellationToken cancellationToken = default(CancellationToken))
        {
            return (long?)await ExecuteAsync(source.Provider, FinalExpression(source, sumLongNullableMethodInfo.Value), cancellationToken).ConfigureAwait(false);
        }

        /// <summary>
        /// Asynchronously computes the sum of a sequence of <see cref="Single"/> values.
        /// </summary>
        /// <returns>
        /// A task that returns the sum of the values in the sequence.
        /// </returns>
        /// <param name="source">A sequence of <see cref="Single"/> values to calculate the sum of.</param>
        /// <param name="cancellationToken">The optional <see cref="CancellationToken"/> which can be used to cancel this task.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="source"/> is null.</exception>
        public static async Task<float> SumAsync(this IQueryable<float> source, CancellationToken cancellationToken = default(CancellationToken))
        {
            return (float)await ExecuteAsync(source.Provider, FinalExpression(source, sumFloatMethodInfo.Value), cancellationToken).ConfigureAwait(false);
        }

        /// <summary>
        /// Asynchronously computes the sum of a sequence of nullable <see cref="Single"/> values.
        /// </summary>
        /// <returns>
        /// A task that returns the sum of the values in the sequence.
        /// </returns>
        /// <param name="source">A sequence of nullable <see cref="Single"/> values to calculate the sum of.</param>
        /// <param name="cancellationToken">The optional <see cref="CancellationToken"/> which can be used to cancel this task.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="source"/> is null.</exception>
        public static async Task<float?> SumAsync(this IQueryable<float?> source, CancellationToken cancellationToken = default(CancellationToken))
        {
            return (float?)await ExecuteAsync(source.Provider, FinalExpression(source, sumFloatNullableMethodInfo.Value), cancellationToken).ConfigureAwait(false);
        }

        /// <summary>
        /// Asynchronously computes the sum of a sequence of <see cref="Double"/> values.
        /// </summary>
        /// <returns>
        /// A task that returns the sum of the values in the sequence.
        /// </returns>
        /// <param name="source">A sequence of <see cref="Double"/> values to calculate the sum of.</param>
        /// <param name="cancellationToken">The optional <see cref="CancellationToken"/> which can be used to cancel this task.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="source"/> is null.</exception>
        public static async Task<double> SumAsync(this IQueryable<double> source, CancellationToken cancellationToken = default(CancellationToken))
        {
            return (double)await ExecuteAsync(source.Provider, FinalExpression(source, sumDoubleMethodInfo.Value), cancellationToken).ConfigureAwait(false);
        }

        /// <summary>
        /// Asynchronously computes the sum of a sequence of nullable <see cref="Double"/> values.
        /// </summary>
        /// <returns>
        /// A task that returns the sum of the values in the sequence.
        /// </returns>
        /// <param name="source">A sequence of nullable <see cref="Double"/> values to calculate the sum of.</param>
        /// <param name="cancellationToken">The optional <see cref="CancellationToken"/> which can be used to cancel this task.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="source"/> is null.</exception>
        public static async Task<double?> SumAsync(this IQueryable<double?> source, CancellationToken cancellationToken = default(CancellationToken))
        {
            return (double?)await ExecuteAsync(source.Provider, FinalExpression(source, sumDoubleNullableMethodInfo.Value), cancellationToken).ConfigureAwait(false);
        }

        /// <summary>
        /// Asynchronously computes the sum of a sequence of <see cref="Decimal"/> values.
        /// </summary>
        /// <returns>
        /// A task that returns the sum of the values in the sequence.
        /// </returns>
        /// <param name="source">A sequence of <see cref="Decimal"/> values to calculate the sum of.</param>
        /// <param name="cancellationToken">The optional <see cref="CancellationToken"/> which can be used to cancel this task.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="source"/> is null.</exception>
        /// <exception cref="OverflowException">The sum is larger than <see cref="Decimal.MaxValue"/>.</exception>
        public static async Task<decimal> SumAsync(this IQueryable<decimal> source, CancellationToken cancellationToken = default(CancellationToken))
        {
            return (decimal)await ExecuteAsync(source.Provider, FinalExpression(source, sumDecimalMethodInfo.Value), cancellationToken).ConfigureAwait(false);
        }

        /// <summary>
        /// Asynchronously computes the sum of a sequence of nullable <see cref="Decimal"/> values.
        /// </summary>
        /// <returns>
        /// A task that returns the sum of the values in the sequence.
        /// </returns>
        /// <param name="source">A sequence of nullable <see cref="Decimal"/> values to calculate the sum of.</param>
        /// <param name="cancellationToken">The optional <see cref="CancellationToken"/> which can be used to cancel this task.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="source"/> is null.</exception>
        /// <exception cref="OverflowException">The sum is larger than <see cref="Decimal.MaxValue"/>.</exception>
        public static async Task<decimal?> SumAsync(this IQueryable<decimal?> source, CancellationToken cancellationToken = default(CancellationToken))
        {
            return (decimal?)await ExecuteAsync(source.Provider, FinalExpression(source, sumDecimalNullableMethodInfo.Value), cancellationToken).ConfigureAwait(false);
        }

        /// <summary>
        /// Asynchronously computes the sum of the sequence of <see cref="Int32"/> values that is obtained by invoking a projection function on each element of the input sequence.
        /// </summary>
        /// <returns>
        /// A task that returns the sum of the projected values.
        /// </returns>
        /// <param name="source">A sequence of values of type <typeparamref name="TSource"/>.</param>
        /// <param name="selector">A projection function to apply to each element.</param>
        /// <param name="cancellationToken">The optional <see cref="CancellationToken"/> which can be used to cancel this task.</param>
        /// <typeparam name="TSource">The type of the elements of <paramref name="source"/>.</typeparam>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="source"/> or <paramref name="selector"/> is null.</exception>
        /// <exception cref="OverflowException">The sum is larger than <see cref="Int32.MaxValue"/>.</exception>
        public static async Task<int> SumAsync<TSource>(this IQueryable<TSource> source, Expression<Func<TSource, int>> selector, CancellationToken cancellationToken = default(CancellationToken))
        {
            return (int)await ExecuteAsync(source.Provider, FinalExpression(source, sumIntSelectorMethodInfo.Value, selector), cancellationToken).ConfigureAwait(false);
        }

        /// <summary>
        /// Asynchronously computes the sum of the sequence of nullable <see cref="Int32"/> values that is obtained by invoking a projection function on each element of the input sequence.
        /// </summary>
        /// <returns>
        /// A task that returns the sum of the projected values.
        /// </returns>
        /// <param name="source">A sequence of values of type <typeparamref name="TSource"/>.</param>
        /// <param name="selector">A projection function to apply to each element.</param>
        /// <param name="cancellationToken">The optional <see cref="CancellationToken"/> which can be used to cancel this task.</param>
        /// <typeparam name="TSource">The type of the elements of <paramref name="source"/>.</typeparam>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="source"/> or <paramref name="selector"/> is null.</exception>
        /// <exception cref="OverflowException">The sum is larger than <see cref="Int32.MaxValue"/>.</exception>
        public static async Task<int?> SumAsync<TSource>(this IQueryable<TSource> source, Expression<Func<TSource, int?>> selector, CancellationToken cancellationToken = default(CancellationToken))
        {
            return (int?)await ExecuteAsync(source.Provider, FinalExpression(source, sumIntNullableSelectorMethodInfo.Value, selector), cancellationToken).ConfigureAwait(false);
        }

        /// <summary>
        /// Asynchronously computes the sum of the sequence of <see cref="Int64"/> values that is obtained by invoking a projection function on each element of the input sequence.
        /// </summary>
        /// <returns>
        /// A task that returns the sum of the projected values.
        /// </returns>
        /// <param name="source">A sequence of values of type <typeparamref name="TSource"/>.</param>
        /// <param name="selector">A projection function to apply to each element.</param>
        /// <param name="cancellationToken">The optional <see cref="CancellationToken"/> which can be used to cancel this task.</param>
        /// <typeparam name="TSource">The type of the elements of <paramref name="source"/>.</typeparam>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="source"/> or <paramref name="selector"/> is null.</exception>
        /// <exception cref="OverflowException">The sum is larger than <see cref="Int64.MaxValue"/>.</exception>
        public static async Task<long> SumAsync<TSource>(this IQueryable<TSource> source, Expression<Func<TSource, long>> selector, CancellationToken cancellationToken = default(CancellationToken))
        {
            return (long)await ExecuteAsync(source.Provider, FinalExpression(source, sumLongSelectorMethodInfo.Value, selector), cancellationToken).ConfigureAwait(false);
        }

        /// <summary>
        /// Asynchronously computes the sum of the sequence of nullable <see cref="Int64"/> values that is obtained by invoking a projection function on each element of the input sequence.
        /// </summary>
        /// <returns>
        /// A task that returns the sum of the projected values.
        /// </returns>
        /// <param name="source">A sequence of values of type <typeparamref name="TSource"/>.</param>
        /// <param name="selector">A projection function to apply to each element.</param>
        /// <param name="cancellationToken">The optional <see cref="CancellationToken"/> which can be used to cancel this task.</param>
        /// <typeparam name="TSource">The type of the elements of <paramref name="source"/>.</typeparam>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="source"/> or <paramref name="selector"/> is null.</exception>
        /// <exception cref="OverflowException">The sum is larger than <see cref="Int64.MaxValue"/>.</exception>
        public static async Task<long?> SumAsync<TSource>(this IQueryable<TSource> source, Expression<Func<TSource, long?>> selector, CancellationToken cancellationToken = default(CancellationToken))
        {
            return (long?)await ExecuteAsync(source.Provider, FinalExpression(source, sumLongNullableSelectorMethodInfo.Value, selector), cancellationToken).ConfigureAwait(false);
        }

        /// <summary>
        /// Asynchronously computes the sum of the sequence of <see cref="Single"/> values that is obtained by invoking a projection function on each element of the input sequence.
        /// </summary>
        /// <returns>
        /// A task that returns the sum of the projected values.
        /// </returns>
        /// <param name="source">A sequence of values of type <typeparamref name="TSource"/>.</param>
        /// <param name="selector">A projection function to apply to each element.</param>
        /// <param name="cancellationToken">The optional <see cref="CancellationToken"/> which can be used to cancel this task.</param>
        /// <typeparam name="TSource">The type of the elements of <paramref name="source"/>.</typeparam>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="source"/> or <paramref name="selector"/> is null.</exception>
        public static async Task<float> SumAsync<TSource>(this IQueryable<TSource> source, Expression<Func<TSource, float>> selector, CancellationToken cancellationToken = default(CancellationToken))
        {
            return (float)await ExecuteAsync(source.Provider, FinalExpression(source, sumFloatSelectorMethodInfo.Value, selector), cancellationToken).ConfigureAwait(false);
        }

        /// <summary>
        /// Asynchronously computes the sum of the sequence of nullable <see cref="Single"/> values that is obtained by invoking a projection function on each element of the input sequence.
        /// </summary>
        /// <returns>
        /// A task that returns the sum of the projected values.
        /// </returns>
        /// <param name="source">A sequence of values of type <typeparamref name="TSource"/>.</param>
        /// <param name="selector">A projection function to apply to each element.</param>
        /// <param name="cancellationToken">The optional <see cref="CancellationToken"/> which can be used to cancel this task.</param>
        /// <typeparam name="TSource">The type of the elements of <paramref name="source"/>.</typeparam>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="source"/> or <paramref name="selector"/> is null.</exception>
        public static async Task<float?> SumAsync<TSource>(this IQueryable<TSource> source, Expression<Func<TSource, float?>> selector, CancellationToken cancellationToken = default(CancellationToken))
        {
            return (float?)await ExecuteAsync(source.Provider, FinalExpression(source, sumFloatNullableSelectorMethodInfo.Value, selector), cancellationToken).ConfigureAwait(false);
        }

        /// <summary>
        /// Asynchronously computes the sum of the sequence of <see cref="Double"/> values that is obtained by invoking a projection function on each element of the input sequence.
        /// </summary>
        /// <returns>
        /// A task that returns the sum of the projected values.
        /// </returns>
        /// <param name="source">A sequence of values of type <typeparamref name="TSource"/>.</param>
        /// <param name="selector">A projection function to apply to each element.</param>
        /// <param name="cancellationToken">The optional <see cref="CancellationToken"/> which can be used to cancel this task.</param>
        /// <typeparam name="TSource">The type of the elements of <paramref name="source"/>.</typeparam>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="source"/> or <paramref name="selector"/> is null.</exception>
        public static async Task<double> SumAsync<TSource>(this IQueryable<TSource> source, Expression<Func<TSource, double>> selector, CancellationToken cancellationToken = default(CancellationToken))
        {
            return (double)await ExecuteAsync(source.Provider, FinalExpression(source, sumDoubleSelectorMethodInfo.Value, selector), cancellationToken).ConfigureAwait(false);
        }

        /// <summary>
        /// Asynchronously computes the sum of the sequence of nullable <see cref="Double"/> values that is obtained by invoking a projection function on each element of the input sequence.
        /// </summary>
        /// <returns>
        /// A task that returns the sum of the projected values.
        /// </returns>
        /// <param name="source">A sequence of values of type <typeparamref name="TSource"/>.</param>
        /// <param name="selector">A projection function to apply to each element.</param>
        /// <param name="cancellationToken">The optional <see cref="CancellationToken"/> which can be used to cancel this task.</param>
        /// <typeparam name="TSource">The type of the elements of <paramref name="source"/>.</typeparam>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="source"/> or <paramref name="selector"/> is null.</exception>
        public static async Task<double?> SumAsync<TSource>(this IQueryable<TSource> source, Expression<Func<TSource, double?>> selector, CancellationToken cancellationToken = default(CancellationToken))
        {
            return (double?)await ExecuteAsync(source.Provider, FinalExpression(source, sumDoubleNullableSelectorMethodInfo.Value, selector), cancellationToken).ConfigureAwait(false);
        }

        /// <summary>
        /// Asynchronously computes the sum of the sequence of <see cref="Decimal"/> values that is obtained by invoking a projection function on each element of the input sequence.
        /// </summary>
        /// <returns>
        /// A task that returns the sum of the projected values.
        /// </returns>
        /// <param name="source">A sequence of values of type <typeparamref name="TSource"/>.</param>
        /// <param name="selector">A projection function to apply to each element.</param>
        /// <param name="cancellationToken">The optional <see cref="CancellationToken"/> which can be used to cancel this task.</param>
        /// <typeparam name="TSource">The type of the elements of <paramref name="source"/>.</typeparam>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="source"/> or <paramref name="selector"/> is null.</exception>
        /// <exception cref="OverflowException">The sum is larger than <see cref="Decimal.MaxValue"/>.</exception>
        public static async Task<decimal> SumAsync<TSource>(this IQueryable<TSource> source, Expression<Func<TSource, decimal>> selector, CancellationToken cancellationToken = default(CancellationToken))
        {
            return (decimal)await ExecuteAsync(source.Provider, FinalExpression(source, sumDecimalSelectorMethodInfo.Value, selector), cancellationToken).ConfigureAwait(false);
        }

        /// <summary>
        /// Asynchronously computes the sum of the sequence of nullable <see cref="Decimal"/> values that is obtained by invoking a projection function on each element of the input sequence.
        /// </summary>
        /// <returns>
        /// A task that returns the sum of the projected values.
        /// </returns>
        /// <param name="source">A sequence of values of type <typeparamref name="TSource"/>.</param>
        /// <param name="selector">A projection function to apply to each element.</param>
        /// <param name="cancellationToken">The optional <see cref="CancellationToken"/> which can be used to cancel this task.</param>
        /// <typeparam name="TSource">The type of the elements of <paramref name="source"/>.</typeparam>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="source"/> or <paramref name="selector"/> is null.</exception>
        /// <exception cref="OverflowException">The sum is larger than <see cref="Decimal.MaxValue"/>.</exception>
        public static async Task<decimal?> SumAsync<TSource>(this IQueryable<TSource> source, Expression<Func<TSource, decimal?>> selector, CancellationToken cancellationToken = default(CancellationToken))
        {
            return (decimal?)await ExecuteAsync(source.Provider, FinalExpression(source, sumDecimalNullableSelectorMethodInfo.Value, selector), cancellationToken).ConfigureAwait(false);
        }
    }
}