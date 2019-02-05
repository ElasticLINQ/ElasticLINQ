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
        static readonly Lazy<MethodInfo> averageIntMethodInfo = QueryableMethodByQueryableParameterType("Average", 1, typeof(int));
        static readonly Lazy<MethodInfo> averageIntNullableMethodInfo = QueryableMethodByQueryableParameterType("Average", 1, typeof(int?));
        static readonly Lazy<MethodInfo> averageLongMethodInfo = QueryableMethodByQueryableParameterType("Average", 1, typeof(long));
        static readonly Lazy<MethodInfo> averageLongNullableMethodInfo = QueryableMethodByQueryableParameterType("Average", 1, typeof(long?));
        static readonly Lazy<MethodInfo> averageFloatMethodInfo = QueryableMethodByQueryableParameterType("Average", 1, typeof(float));
        static readonly Lazy<MethodInfo> averageFloatNullableMethodInfo = QueryableMethodByQueryableParameterType("Average", 1, typeof(float?));
        static readonly Lazy<MethodInfo> averageDoubleMethodInfo = QueryableMethodByQueryableParameterType("Average", 1, typeof(double));
        static readonly Lazy<MethodInfo> averageDoubleNullableMethodInfo = QueryableMethodByQueryableParameterType("Average", 1, typeof(double?));
        static readonly Lazy<MethodInfo> averageDecimalMethodInfo = QueryableMethodByQueryableParameterType("Average", 1, typeof(decimal));
        static readonly Lazy<MethodInfo> averageDecimalNullableMethodInfo = QueryableMethodByQueryableParameterType("Average", 1, typeof(decimal?));

        static readonly Lazy<MethodInfo> averageIntSelectorMethodInfo = QueryableMethodBySelectorParameterType("Average", 2, typeof(int));
        static readonly Lazy<MethodInfo> averageIntNullableSelectorMethodInfo = QueryableMethodBySelectorParameterType("Average", 2, typeof(int?));
        static readonly Lazy<MethodInfo> averageLongSelectorMethodInfo = QueryableMethodBySelectorParameterType("Average", 2, typeof(long));
        static readonly Lazy<MethodInfo> averageLongNullableSelectorMethodInfo = QueryableMethodBySelectorParameterType("Average", 2, typeof(long?));
        static readonly Lazy<MethodInfo> averageFloatSelectorMethodInfo = QueryableMethodBySelectorParameterType("Average", 2, typeof(float));
        static readonly Lazy<MethodInfo> averageFloatNullableSelectorMethodInfo = QueryableMethodBySelectorParameterType("Average", 2, typeof(float?));
        static readonly Lazy<MethodInfo> averageDoubleSelectorMethodInfo = QueryableMethodBySelectorParameterType("Average", 2, typeof(double));
        static readonly Lazy<MethodInfo> averageDoubleNullableSelectorMethodInfo = QueryableMethodBySelectorParameterType("Average", 2, typeof(double?));
        static readonly Lazy<MethodInfo> averageDecimalSelectorMethodInfo = QueryableMethodBySelectorParameterType("Average", 2, typeof(decimal));
        static readonly Lazy<MethodInfo> averageDecimalNullableSelectorMethodInfo = QueryableMethodBySelectorParameterType("Average", 2, typeof(decimal?));

        /// <summary>
        /// Asynchronously computes the average of a sequence of <see cref="Int32"/> values.
        /// </summary>
        /// <returns>
        /// A task that returns the average of the sequence of values.
        /// </returns>
        /// <param name="source">A sequence of <see cref="Int32"/> values to calculate the average of.</param>
        /// <param name="cancellationToken">The optional <see cref="CancellationToken"/> which can be used to cancel this task.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="source"/> is null.</exception>
        /// <exception cref="InvalidOperationException">
        /// <paramref name="source"/> contains no elements.</exception>
        public static async Task<double> AverageAsync(this IQueryable<int> source, CancellationToken cancellationToken = default(CancellationToken))
        {
            return (double)await ExecuteAsync(source.Provider, FinalExpression(source, averageIntMethodInfo.Value), cancellationToken).ConfigureAwait(false);
        }

        /// <summary>
        /// Asynchronously computes the average of a sequence of nullable <see cref="Int32"/> values.
        /// </summary>
        /// <returns>
        /// A task that returns the average of the sequence of values, or null if the source sequence is empty or contains only null values.
        /// </returns>
        /// <param name="source">A sequence of nullable <see cref="Int32"/> values to calculate the average of.</param>
        /// <param name="cancellationToken">The optional <see cref="CancellationToken"/> which can be used to cancel this task.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="source"/> is null.</exception>
        public static async Task<double?> AverageAsync(this IQueryable<int?> source, CancellationToken cancellationToken = default(CancellationToken))
        {
            return (double?)await ExecuteAsync(source.Provider, FinalExpression(source, averageIntNullableMethodInfo.Value), cancellationToken).ConfigureAwait(false);
        }

        /// <summary>
        /// Asynchronously computes the average of a sequence of <see cref="Int64"/> values.
        /// </summary>
        /// <returns>
        /// A task that returns the average of the sequence of values.
        /// </returns>
        /// <param name="source">A sequence of <see cref="Int64"/> values to calculate the average of.</param>
        /// <param name="cancellationToken">The optional <see cref="CancellationToken"/> which can be used to cancel this task.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="source"/> is null.</exception>
        /// <exception cref="InvalidOperationException">
        /// <paramref name="source"/> contains no elements.</exception>
        public static async Task<double> AverageAsync(this IQueryable<long> source, CancellationToken cancellationToken = default(CancellationToken))
        {
            return (double)await ExecuteAsync(source.Provider, FinalExpression(source, averageLongMethodInfo.Value), cancellationToken).ConfigureAwait(false);
        }

        /// <summary>
        /// Asynchronously computes the average of a sequence of nullable <see cref="Int64"/> values.
        /// </summary>
        /// <returns>
        /// A task that returns the average of the sequence of values, or null if the source sequence is empty or contains only null values.
        /// </returns>
        /// <param name="source">A sequence of nullable <see cref="Int64"/> values to calculate the average of.</param>
        /// <param name="cancellationToken">The optional <see cref="CancellationToken"/> which can be used to cancel this task.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="source"/> is null.</exception>
        public static async Task<double?> AverageAsync(this IQueryable<long?> source, CancellationToken cancellationToken = default(CancellationToken))
        {
            return (double)await ExecuteAsync(source.Provider, FinalExpression(source, averageLongNullableMethodInfo.Value), cancellationToken).ConfigureAwait(false);
        }

        /// <summary>
        /// Asynchronously computes the average of a sequence of <see cref="Single"/> values.
        /// </summary>
        /// <returns>
        /// A task that returns the average of the sequence of values.
        /// </returns>
        /// <param name="source">A sequence of <see cref="Single"/> values to calculate the average of.</param>
        /// <param name="cancellationToken">The optional <see cref="CancellationToken"/> which can be used to cancel this task.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="source"/> is null.</exception>
        /// <exception cref="InvalidOperationException">
        /// <paramref name="source"/> contains no elements.</exception>
        public static async Task<float> AverageAsync(this IQueryable<float> source, CancellationToken cancellationToken = default(CancellationToken))
        {
            return (float)await ExecuteAsync(source.Provider, FinalExpression(source, averageFloatMethodInfo.Value), cancellationToken).ConfigureAwait(false);
        }

        /// <summary>
        /// Asynchronously computes the average of a sequence of nullable <see cref="Single"/> values.
        /// </summary>
        /// <returns>
        /// A task that returns the average of the sequence of values, or null if the source sequence is empty or contains only null values.
        /// </returns>
        /// <param name="source">A sequence of nullable <see cref="Single"/> values to calculate the average of.</param>
        /// <param name="cancellationToken">The optional <see cref="CancellationToken"/> which can be used to cancel this task.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="source"/> is null.</exception>
        public static async Task<float?> AverageAsync(this IQueryable<float?> source, CancellationToken cancellationToken = default(CancellationToken))
        {
            return (float?)await ExecuteAsync(source.Provider, FinalExpression(source, averageFloatNullableMethodInfo.Value), cancellationToken).ConfigureAwait(false);
        }

        /// <summary>
        /// Asynchronously computes the average of a sequence of <see cref="Double"/> values.
        /// </summary>
        /// <returns>
        /// A task that returns the average of the sequence of values.
        /// </returns>
        /// <param name="source">A sequence of <see cref="Double"/> values to calculate the average of.</param>
        /// <param name="cancellationToken">The optional <see cref="CancellationToken"/> which can be used to cancel this task.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="source"/> is null.</exception>
        /// <exception cref="InvalidOperationException">
        /// <paramref name="source"/> contains no elements.</exception>
        public static async Task<double> AverageAsync(this IQueryable<double> source, CancellationToken cancellationToken = default(CancellationToken))
        {
            return (double)await ExecuteAsync(source.Provider, FinalExpression(source, averageDoubleMethodInfo.Value), cancellationToken).ConfigureAwait(false);
        }

        /// <summary>
        /// Asynchronously computes the average of a sequence of nullable <see cref="Double"/> values.
        /// </summary>
        /// <returns>
        /// A task that returns the average of the sequence of values, or null if the source sequence is empty or contains only null values.
        /// </returns>
        /// <param name="source">A sequence of nullable <see cref="Double"/> values to calculate the average of.</param>
        /// <param name="cancellationToken">The optional <see cref="CancellationToken"/> which can be used to cancel this task.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="source"/> is null.</exception>
        public static async Task<double?> AverageAsync(this IQueryable<double?> source, CancellationToken cancellationToken = default(CancellationToken))
        {
            return (double?)await ExecuteAsync(source.Provider, FinalExpression(source, averageDoubleNullableMethodInfo.Value), cancellationToken).ConfigureAwait(false);
        }

        /// <summary>
        /// Asynchronously computes the average of a sequence of <see cref="Decimal"/> values.
        /// </summary>
        /// <returns>
        /// A task that returns the average of the sequence of values.
        /// </returns>
        /// <param name="source">A sequence of <see cref="Decimal"/> values to calculate the average of.</param>
        /// <param name="cancellationToken">The optional <see cref="CancellationToken"/> which can be used to cancel this task.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="source"/> is null.</exception>
        /// <exception cref="InvalidOperationException">
        /// <paramref name="source"/> contains no elements.</exception>
        public static async Task<decimal> AverageAsync(this IQueryable<decimal> source, CancellationToken cancellationToken = default(CancellationToken))
        {
            return (decimal)await ExecuteAsync(source.Provider, FinalExpression(source, averageDecimalMethodInfo.Value), cancellationToken).ConfigureAwait(false);
        }

        /// <summary>
        /// Asynchronously computes the average of a sequence of nullable <see cref="Decimal"/> values.
        /// </summary>
        /// <returns>
        /// A task that returns the average of the sequence of values, or null if the source sequence is empty or contains only null values.
        /// </returns>
        /// <param name="source">A sequence of nullable <see cref="Decimal"/> values to calculate the average of.</param>
        /// <param name="cancellationToken">The optional <see cref="CancellationToken"/> which can be used to cancel this task.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="source"/> is null.</exception>
        public static async Task<decimal?> AverageAsync(this IQueryable<decimal?> source, CancellationToken cancellationToken = default(CancellationToken))
        {
            return (decimal?)await ExecuteAsync(source.Provider, FinalExpression(source, averageDecimalNullableMethodInfo.Value), cancellationToken).ConfigureAwait(false);
        }

        /// <summary>
        /// Asynchronously computes the average of a sequence of <see cref="Int32"/> values that is obtained by invoking a projection function on each element of the input sequence.
        /// </summary>
        /// <returns>
        /// A task that returns the average of the sequence of values.
        /// </returns>
        /// <param name="source">A sequence of values to calculate the average of.</param>
        /// <param name="selector">A projection function to apply to each element.</param>
        /// <param name="cancellationToken">The optional <see cref="CancellationToken"/> which can be used to cancel this task.</param>
        /// <typeparam name="TSource">The type of the elements of <paramref name="source"/>.</typeparam>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="source"/> or <paramref name="selector"/> is null.</exception>
        /// <exception cref="InvalidOperationException">
        /// <paramref name="source"/> contains no elements.</exception>
        public static async Task<double> AverageAsync<TSource>(this IQueryable<TSource> source, Expression<Func<TSource, int>> selector, CancellationToken cancellationToken = default(CancellationToken))
        {
            return (double)await ExecuteAsync(source.Provider, FinalExpression(source, averageIntSelectorMethodInfo.Value, selector), cancellationToken).ConfigureAwait(false);
        }

        /// <summary>
        /// Asynchronously computes the average of a sequence of nullable <see cref="Int32"/> values that is obtained by invoking a projection function on each element of the input sequence.
        /// </summary>
        /// <returns>
        /// A task that returns the average of the sequence of values, or null if the <paramref name="source"/> sequence is empty or contains only null values.
        /// </returns>
        /// <param name="source">A sequence of values to calculate the average of.</param>
        /// <param name="selector">A projection function to apply to each element.</param>
        /// <param name="cancellationToken">The optional <see cref="CancellationToken"/> which can be used to cancel this task.</param>
        /// <typeparam name="TSource">The type of the elements of <paramref name="source"/>.</typeparam>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="source"/> or <paramref name="selector"/> is null.</exception>
        public static async Task<double?> AverageAsync<TSource>(this IQueryable<TSource> source, Expression<Func<TSource, int?>> selector, CancellationToken cancellationToken = default(CancellationToken))
        {
            return (double?)await ExecuteAsync(source.Provider, FinalExpression(source, averageIntNullableSelectorMethodInfo.Value, selector), cancellationToken).ConfigureAwait(false);
        }

        /// <summary>
        /// Asynchronously computes the average of a sequence of <see cref="Single"/> values that is obtained by invoking a projection function on each element of the input sequence.
        /// </summary>
        /// <returns>
        /// A task that returns the average of the sequence of values.
        /// </returns>
        /// <param name="source">A sequence of values to calculate the average of.</param>
        /// <param name="selector">A projection function to apply to each element.</param>
        /// <param name="cancellationToken">The optional <see cref="CancellationToken"/> which can be used to cancel this task.</param>
        /// <typeparam name="TSource">The type of the elements of <paramref name="source"/>.</typeparam>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="source"/> or <paramref name="selector"/> is null.</exception>
        /// <exception cref="InvalidOperationException">
        /// <paramref name="source"/> contains no elements.</exception>
        public static async Task<float> AverageAsync<TSource>(this IQueryable<TSource> source, Expression<Func<TSource, float>> selector, CancellationToken cancellationToken = default(CancellationToken))
        {
            return (float)await ExecuteAsync(source.Provider, FinalExpression(source, averageFloatSelectorMethodInfo.Value, selector), cancellationToken).ConfigureAwait(false);
        }

        /// <summary>
        /// Asynchronously computes the average of a sequence of nullable <see cref="Single"/> values that is obtained by invoking a projection function on each element of the input sequence.
        /// </summary>
        /// <returns>
        /// A task that returns the average of the sequence of values, or null if the <paramref name="source"/> sequence is empty or contains only null values.
        /// </returns>
        /// <param name="source">A sequence of values to calculate the average of.</param>
        /// <param name="selector">A projection function to apply to each element.</param>
        /// <param name="cancellationToken">The optional <see cref="CancellationToken"/> which can be used to cancel this task.</param>
        /// <typeparam name="TSource">The type of the elements of <paramref name="source"/>.</typeparam>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="source"/> or <paramref name="selector"/> is null.</exception>
        public static async Task<float?> AverageAsync<TSource>(this IQueryable<TSource> source, Expression<Func<TSource, float?>> selector, CancellationToken cancellationToken = default(CancellationToken))
        {
            return (float?)await ExecuteAsync(source.Provider, FinalExpression(source, averageFloatNullableSelectorMethodInfo.Value, selector), cancellationToken).ConfigureAwait(false);
        }

        /// <summary>
        /// Asynchronously computes the average of a sequence of <see cref="Int64"/> values that is obtained by invoking a projection function on each element of the input sequence.
        /// </summary>
        /// <returns>
        /// A task that returns the average of the sequence of values.
        /// </returns>
        /// <param name="source">A sequence of values to calculate the average of.</param>
        /// <param name="selector">A projection function to apply to each element.</param>
        /// <param name="cancellationToken">The optional <see cref="CancellationToken"/> which can be used to cancel this task.</param>
        /// <typeparam name="TSource">The type of the elements of <paramref name="source"/>.</typeparam>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="source"/> or <paramref name="selector"/> is null.</exception>
        /// <exception cref="InvalidOperationException">
        /// <paramref name="source"/> contains no elements.</exception>
        public static async Task<double> AverageAsync<TSource>(this IQueryable<TSource> source, Expression<Func<TSource, long>> selector, CancellationToken cancellationToken = default(CancellationToken))
        {
            return (double)await ExecuteAsync(source.Provider, FinalExpression(source, averageLongSelectorMethodInfo.Value, selector), cancellationToken).ConfigureAwait(false);
        }

        /// <summary>
        /// Asynchronously computes the average of a sequence of nullable <see cref="Int64"/> values that is obtained by invoking a projection function on each element of the input sequence.
        /// </summary>
        /// <returns>
        /// A task that returns the average of the sequence of values, or null if the <paramref name="source"/> sequence is empty or contains only null values.
        /// </returns>
        /// <param name="source">A sequence of values to calculate the average of.</param>
        /// <param name="selector">A projection function to apply to each element.</param>
        /// <param name="cancellationToken">The optional <see cref="CancellationToken"/> which can be used to cancel this task.</param>
        /// <typeparam name="TSource">The type of the elements of <paramref name="source"/>.</typeparam>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="source"/> or <paramref name="selector"/> is null.</exception>
        public static async Task<double?> AverageAsync<TSource>(this IQueryable<TSource> source, Expression<Func<TSource, long?>> selector, CancellationToken cancellationToken = default(CancellationToken))
        {
            return (double?)await ExecuteAsync(source.Provider, FinalExpression(source, averageLongNullableSelectorMethodInfo.Value, selector), cancellationToken).ConfigureAwait(false);
        }

        /// <summary>
        /// Asynchronously computes the average of a sequence of <see cref="Double"/> values that is obtained by invoking a projection function on each element of the input sequence.
        /// </summary>
        /// <returns>
        /// A task that returns the average of the sequence of values.
        /// </returns>
        /// <param name="source">A sequence of values to calculate the average of.</param>
        /// <param name="selector">A projection function to apply to each element.</param>
        /// <param name="cancellationToken">The optional <see cref="CancellationToken"/> which can be used to cancel this task.</param>
        /// <typeparam name="TSource">The type of the elements of <paramref name="source"/>.</typeparam>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="source"/> or <paramref name="selector"/> is null.</exception>
        /// <exception cref="InvalidOperationException">
        /// <paramref name="source"/> contains no elements.</exception>
        public static async Task<double> AverageAsync<TSource>(this IQueryable<TSource> source, Expression<Func<TSource, double>> selector, CancellationToken cancellationToken = default(CancellationToken))
        {
            return (double)await ExecuteAsync(source.Provider, FinalExpression(source, averageDoubleSelectorMethodInfo.Value, selector), cancellationToken).ConfigureAwait(false);
        }

        /// <summary>
        /// Asynchronously computes the average of a sequence of nullable <see cref="Double"/> values that is obtained by invoking a projection function on each element of the input sequence.
        /// </summary>
        /// <returns>
        /// A task that returns the average of the sequence of values, or null if the <paramref name="source"/> sequence is empty or contains only null values.
        /// </returns>
        /// <param name="source">A sequence of values to calculate the average of.</param>
        /// <param name="selector">A projection function to apply to each element.</param>
        /// <param name="cancellationToken">The optional <see cref="CancellationToken"/> which can be used to cancel this task.</param>
        /// <typeparam name="TSource">The type of the elements of <paramref name="source"/>.</typeparam>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="source"/> or <paramref name="selector"/> is null.</exception>
        public static async Task<double?> AverageAsync<TSource>(this IQueryable<TSource> source, Expression<Func<TSource, double?>> selector, CancellationToken cancellationToken = default(CancellationToken))
        {
            return (double?)await ExecuteAsync(source.Provider, FinalExpression(source, averageDoubleNullableSelectorMethodInfo.Value, selector), cancellationToken).ConfigureAwait(false);
        }

        /// <summary>
        /// Asynchronously computes the average of a sequence of <see cref="Decimal"/> values that is obtained by invoking a projection function on each element of the input sequence.
        /// </summary>
        /// <returns>
        /// A task that returns the average of the sequence of values.
        /// </returns>
        /// <param name="source">A sequence of values that are used to calculate an average.</param>
        /// <param name="selector">A projection function to apply to each element.</param>
        /// <param name="cancellationToken">The optional <see cref="CancellationToken"/> which can be used to cancel this task.</param>
        /// <typeparam name="TSource">The type of the elements of <paramref name="source"/>.</typeparam>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="source"/> or <paramref name="selector"/> is null.</exception>
        /// <exception cref="InvalidOperationException">
        /// <paramref name="source"/> contains no elements.</exception>
        public static async Task<decimal> AverageAsync<TSource>(this IQueryable<TSource> source, Expression<Func<TSource, decimal>> selector, CancellationToken cancellationToken = default(CancellationToken))
        {
            return (decimal)await ExecuteAsync(source.Provider, FinalExpression(source, averageDecimalSelectorMethodInfo.Value, selector), cancellationToken).ConfigureAwait(false);
        }

        /// <summary>
        /// Asynchronously computes the average of a sequence of nullable <see cref="Decimal"/> values that is obtained by invoking a projection function on each element of the input sequence.
        /// </summary>
        /// <returns>
        /// A task that returns the average of the sequence of values, or null if the <paramref name="source"/> sequence is empty or contains only null values.
        /// </returns>
        /// <param name="source">A sequence of values to calculate the average of.</param>
        /// <param name="selector">A projection function to apply to each element.</param>
        /// <param name="cancellationToken">The optional <see cref="CancellationToken"/> which can be used to cancel this task.</param>
        /// <typeparam name="TSource">The type of the elements of <paramref name="source"/>.</typeparam>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="source"/> or <paramref name="selector"/> is null.</exception>
        public static async Task<decimal?> AverageAsync<TSource>(this IQueryable<TSource> source, Expression<Func<TSource, decimal?>> selector, CancellationToken cancellationToken = default(CancellationToken))
        {
            return (decimal)await ExecuteAsync(source.Provider, FinalExpression(source, averageDecimalNullableSelectorMethodInfo.Value, selector), cancellationToken).ConfigureAwait(false);
        }
    }
}
