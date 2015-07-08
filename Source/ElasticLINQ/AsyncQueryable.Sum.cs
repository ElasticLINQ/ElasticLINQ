// Licensed under the Apache 2.0 License. See LICENSE.txt in the project root for more information.

using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace ElasticLinq
{
    public static partial class AsyncQueryable
    {
        private static readonly Lazy<MethodInfo> sumIntMethodInfo = GetLazyQueryableMethod("Sum", 2, typeof(int));
        private static readonly Lazy<MethodInfo> sumIntNullableMethodInfo = GetLazyQueryableMethod("Sum", 2, typeof(int?));
        private static readonly Lazy<MethodInfo> sumLongMethodInfo = GetLazyQueryableMethod("Sum", 2, typeof(long));
        private static readonly Lazy<MethodInfo> sumLongNullableMethodInfo = GetLazyQueryableMethod("Sum", 2, typeof(long?));
        private static readonly Lazy<MethodInfo> sumFloatMethodInfo = GetLazyQueryableMethod("Sum", 2, typeof(float));
        private static readonly Lazy<MethodInfo> sumFloatNullableMethodInfo = GetLazyQueryableMethod("Sum", 2, typeof(float?));
        private static readonly Lazy<MethodInfo> sumDoubleMethodInfo = GetLazyQueryableMethod("Sum", 2, typeof(double));
        private static readonly Lazy<MethodInfo> sumDoubleNullableMethodInfo = GetLazyQueryableMethod("Sum", 2, typeof(double?));
        private static readonly Lazy<MethodInfo> sumDecimalMethodInfo = GetLazyQueryableMethod("Sum", 2, typeof(decimal));
        private static readonly Lazy<MethodInfo> sumDecimalNullableMethodInfo = GetLazyQueryableMethod("Sum", 2, typeof(decimal));

        private static readonly Lazy<MethodInfo> sumIntSelectorMethodInfo = GetLazyQueryableMethod("Sum", 3, typeof(int));
        private static readonly Lazy<MethodInfo> sumIntNullableSelectorMethodInfo = GetLazyQueryableMethod("Sum", 3, typeof(int?));
        private static readonly Lazy<MethodInfo> sumLongSelectorMethodInfo = GetLazyQueryableMethod("Sum", 3, typeof(long));
        private static readonly Lazy<MethodInfo> sumLongNullableSelectorMethodInfo = GetLazyQueryableMethod("Sum", 3, typeof(long?));
        private static readonly Lazy<MethodInfo> sumFloatSelectorMethodInfo = GetLazyQueryableMethod("Sum", 3, typeof(float));
        private static readonly Lazy<MethodInfo> sumFloatNullableSelectorMethodInfo = GetLazyQueryableMethod("Sum", 3, typeof(float?));
        private static readonly Lazy<MethodInfo> sumDoubleSelectorMethodInfo = GetLazyQueryableMethod("Sum", 3, typeof(double));
        private static readonly Lazy<MethodInfo> sumDoubleNullableSelectorMethodInfo = GetLazyQueryableMethod("Sum", 3, typeof(double?));
        private static readonly Lazy<MethodInfo> sumDecimalSelectorMethodInfo = GetLazyQueryableMethod("Sum", 3, typeof(decimal));
        private static readonly Lazy<MethodInfo> sumDecimalNullableSelectorMethodInfo = GetLazyQueryableMethod("Sum", 3, typeof(decimal));

        /// <summary>
        /// Asynchronously computes the sum of a sequence of <see cref="T:System.Int32"/> values.
        /// </summary>
        /// <returns>
        /// A task that returns the sum of the values in the sequence.
        /// </returns>
        /// <param name="source">A sequence of <see cref="T:System.Int32"/> values to calculate the sum of.</param>
        /// <param name="cancellationToken">The optional <see cref="T:System.Threading.CancellationToken"/> which can be used to cancel this task.</param>
        /// <exception cref="T:System.ArgumentNullException">
        /// <paramref name="source"/> is null.</exception>
        /// <exception cref="T:System.OverflowException">The sum is larger than <see cref="F:System.Int32.MaxValue"/>.</exception>
        public static async Task<int> SumAsync(this IQueryable<int> source, CancellationToken cancellationToken = default(CancellationToken))
        {
            return (int)await ExecuteAsync(source.Provider, FinalExpression(source, sumIntMethodInfo.Value), cancellationToken);
        }

        /// <summary>
        /// Asynchronously computes the sum of a sequence of nullable <see cref="T:System.Int32"/> values.
        /// </summary>
        /// <returns>
        /// A task that returns the sum of the values in the sequence.
        /// </returns>
        /// <param name="source">A sequence of nullable <see cref="T:System.Int32"/> values to calculate the sum of.</param>
        /// <param name="cancellationToken">The optional <see cref="T:System.Threading.CancellationToken"/> which can be used to cancel this task.</param>
        /// <exception cref="T:System.ArgumentNullException">
        /// <paramref name="source"/> is null.</exception>
        /// <exception cref="T:System.OverflowException">The sum is larger than <see cref="F:System.Int32.MaxValue"/>.</exception>
        public static async Task<int?> SumAsync(this IQueryable<int?> source, CancellationToken cancellationToken = default(CancellationToken))
        {
            return (int)await ExecuteAsync(source.Provider, FinalExpression(source, sumIntNullableMethodInfo.Value), cancellationToken);
        }

        /// <summary>
        /// Asynchronously computes the sum of a sequence of <see cref="T:System.Int64"/> values.
        /// </summary>
        /// <returns>
        /// A task that returns the sum of the values in the sequence.
        /// </returns>
        /// <param name="source">A sequence of <see cref="T:System.Int64"/> values to calculate the sum of.</param>
        /// <param name="cancellationToken">The optional <see cref="T:System.Threading.CancellationToken"/> which can be used to cancel this task.</param>
        /// <exception cref="T:System.ArgumentNullException">
        /// <paramref name="source"/> is null.</exception>
        /// <exception cref="T:System.OverflowException">The sum is larger than <see cref="F:System.Int64.MaxValue"/>.</exception>
        public static async Task<long> SumAsync(this IQueryable<long> source, CancellationToken cancellationToken = default(CancellationToken))
        {
            return (long)await ExecuteAsync(source.Provider, FinalExpression(source, sumLongMethodInfo.Value), cancellationToken);
        }

        /// <summary>
        /// Asynchronously computes the sum of a sequence of nullable <see cref="T:System.Int64"/> values.
        /// </summary>
        /// <returns>
        /// A task that returns the sum of the values in the sequence.
        /// </returns>
        /// <param name="source">A sequence of nullable <see cref="T:System.Int64"/> values to calculate the sum of.</param>
        /// <param name="cancellationToken">The optional <see cref="T:System.Threading.CancellationToken"/> which can be used to cancel this task.</param>
        /// <exception cref="T:System.ArgumentNullException">
        /// <paramref name="source"/> is null.</exception>
        /// <exception cref="T:System.OverflowException">The sum is larger than <see cref="F:System.Int64.MaxValue"/>.</exception>
        public static async Task<long?> SumAsync(this IQueryable<long?> source, CancellationToken cancellationToken = default(CancellationToken))
        {
            return (long?)await ExecuteAsync(source.Provider, FinalExpression(source, sumLongNullableMethodInfo.Value), cancellationToken);
        }

        /// <summary>
        /// Asynchronously computes the sum of a sequence of <see cref="T:System.Single"/> values.
        /// </summary>
        /// <returns>
        /// A task that returns the sum of the values in the sequence.
        /// </returns>
        /// <param name="source">A sequence of <see cref="T:System.Single"/> values to calculate the sum of.</param>
        /// <param name="cancellationToken">The optional <see cref="T:System.Threading.CancellationToken"/> which can be used to cancel this task.</param>
        /// <exception cref="T:System.ArgumentNullException">
        /// <paramref name="source"/> is null.</exception>
        public static async Task<float> SumAsync(this IQueryable<float> source, CancellationToken cancellationToken = default(CancellationToken))
        {
            return (float)await ExecuteAsync(source.Provider, FinalExpression(source, sumFloatMethodInfo.Value), cancellationToken);
        }

        /// <summary>
        /// Asynchronously computes the sum of a sequence of nullable <see cref="T:System.Single"/> values.
        /// </summary>
        /// <returns>
        /// A task that returns the sum of the values in the sequence.
        /// </returns>
        /// <param name="source">A sequence of nullable <see cref="T:System.Single"/> values to calculate the sum of.</param>
        /// <param name="cancellationToken">The optional <see cref="T:System.Threading.CancellationToken"/> which can be used to cancel this task.</param>
        /// <exception cref="T:System.ArgumentNullException">
        /// <paramref name="source"/> is null.</exception>
        public static async Task<float?> SumAsync(this IQueryable<float?> source, CancellationToken cancellationToken = default(CancellationToken))
        {
            return (float?)await ExecuteAsync(source.Provider, FinalExpression(source, sumFloatNullableMethodInfo.Value), cancellationToken);
        }

        /// <summary>
        /// Asynchronously computes the sum of a sequence of <see cref="T:System.Double"/> values.
        /// </summary>
        /// <returns>
        /// A task that returns the sum of the values in the sequence.
        /// </returns>
        /// <param name="source">A sequence of <see cref="T:System.Double"/> values to calculate the sum of.</param>
        /// <param name="cancellationToken">The optional <see cref="T:System.Threading.CancellationToken"/> which can be used to cancel this task.</param>
        /// <exception cref="T:System.ArgumentNullException">
        /// <paramref name="source"/> is null.</exception>
        public static async Task<double> SumAsync(this IQueryable<double> source, CancellationToken cancellationToken = default(CancellationToken))
        {
            return (double)await ExecuteAsync(source.Provider, FinalExpression(source, sumDoubleMethodInfo.Value), cancellationToken);
        }

        /// <summary>
        /// Asynchronously computes the sum of a sequence of nullable <see cref="T:System.Double"/> values.
        /// </summary>
        /// <returns>
        /// A task that returns the sum of the values in the sequence.
        /// </returns>
        /// <param name="source">A sequence of nullable <see cref="T:System.Double"/> values to calculate the sum of.</param>
        /// <param name="cancellationToken">The optional <see cref="T:System.Threading.CancellationToken"/> which can be used to cancel this task.</param>
        /// <exception cref="T:System.ArgumentNullException">
        /// <paramref name="source"/> is null.</exception>
        public static async Task<double?> SumAsync(this IQueryable<double?> source, CancellationToken cancellationToken = default(CancellationToken))
        {
            return (double?)await ExecuteAsync(source.Provider, FinalExpression(source, sumDoubleNullableMethodInfo.Value), cancellationToken);
        }

        /// <summary>
        /// Asynchronously computes the sum of a sequence of <see cref="T:System.Decimal"/> values.
        /// </summary>
        /// <returns>
        /// A task that returns the sum of the values in the sequence.
        /// </returns>
        /// <param name="source">A sequence of <see cref="T:System.Decimal"/> values to calculate the sum of.</param>
        /// <param name="cancellationToken">The optional <see cref="T:System.Threading.CancellationToken"/> which can be used to cancel this task.</param>
        /// <exception cref="T:System.ArgumentNullException">
        /// <paramref name="source"/> is null.</exception>
        /// <exception cref="T:System.OverflowException">The sum is larger than <see cref="F:System.Decimal.MaxValue"/>.</exception>
        public static async Task<decimal> SumAsync(this IQueryable<decimal> source, CancellationToken cancellationToken = default(CancellationToken))
        {
            return (decimal)await ExecuteAsync(source.Provider, FinalExpression(source, sumDecimalMethodInfo.Value), cancellationToken);
        }

        /// <summary>
        /// Asynchronously computes the sum of a sequence of nullable <see cref="T:System.Decimal"/> values.
        /// </summary>
        /// <returns>
        /// A task that returns the sum of the values in the sequence.
        /// </returns>
        /// <param name="source">A sequence of nullable <see cref="T:System.Decimal"/> values to calculate the sum of.</param>
        /// <param name="cancellationToken">The optional <see cref="T:System.Threading.CancellationToken"/> which can be used to cancel this task.</param>
        /// <exception cref="T:System.ArgumentNullException">
        /// <paramref name="source"/> is null.</exception>
        /// <exception cref="T:System.OverflowException">The sum is larger than <see cref="F:System.Decimal.MaxValue"/>.</exception>
        public static async Task<decimal?> SumAsync(this IQueryable<decimal?> source, CancellationToken cancellationToken = default(CancellationToken))
        {
            return (decimal?)await ExecuteAsync(source.Provider, FinalExpression(source, sumDecimalNullableMethodInfo.Value), cancellationToken);
        }

        /// <summary>
        /// Asynchronously computes the sum of the sequence of <see cref="T:System.Int32"/> values that is obtained by invoking a projection function on each element of the input sequence.
        /// </summary>
        /// <returns>
        /// A task that returns the sum of the projected values.
        /// </returns>
        /// <param name="source">A sequence of values of type <typeparamref name="TSource"/>.</param>
        /// <param name="selector">A projection function to apply to each element.</param>
        /// <param name="cancellationToken">The optional <see cref="T:System.Threading.CancellationToken"/> which can be used to cancel this task.</param>
        /// <typeparam name="TSource">The type of the elements of <paramref name="source"/>.</typeparam>
        /// <exception cref="T:System.ArgumentNullException">
        /// <paramref name="source"/> or <paramref name="selector"/> is null.</exception>
        /// <exception cref="T:System.OverflowException">The sum is larger than <see cref="F:System.Int32.MaxValue"/>.</exception>
        public static async Task<int> SumAsync<TSource>(this IQueryable<TSource> source, Expression<Func<TSource, int>> selector, CancellationToken cancellationToken = default(CancellationToken))
        {
            return (int)await ExecuteAsync(source.Provider, FinalExpression(source, sumIntSelectorMethodInfo.Value, selector), cancellationToken);
        }

        /// <summary>
        /// Asynchronously computes the sum of the sequence of nullable <see cref="T:System.Int32"/> values that is obtained by invoking a projection function on each element of the input sequence.
        /// </summary>
        /// <returns>
        /// A task that returns the sum of the projected values.
        /// </returns>
        /// <param name="source">A sequence of values of type <typeparamref name="TSource"/>.</param>
        /// <param name="selector">A projection function to apply to each element.</param>
        /// <param name="cancellationToken">The optional <see cref="T:System.Threading.CancellationToken"/> which can be used to cancel this task.</param>
        /// <typeparam name="TSource">The type of the elements of <paramref name="source"/>.</typeparam>
        /// <exception cref="T:System.ArgumentNullException">
        /// <paramref name="source"/> or <paramref name="selector"/> is null.</exception>
        /// <exception cref="T:System.OverflowException">The sum is larger than <see cref="F:System.Int32.MaxValue"/>.</exception>
        public static async Task<int?> SumAsync<TSource>(this IQueryable<TSource> source, Expression<Func<TSource, int?>> selector, CancellationToken cancellationToken = default(CancellationToken))
        {
            return (int?)await ExecuteAsync(source.Provider, FinalExpression(source, sumIntNullableSelectorMethodInfo.Value, selector), cancellationToken);
        }

        /// <summary>
        /// Asynchronously computes the sum of the sequence of <see cref="T:System.Int64"/> values that is obtained by invoking a projection function on each element of the input sequence.
        /// </summary>
        /// <returns>
        /// A task that returns the sum of the projected values.
        /// </returns>
        /// <param name="source">A sequence of values of type <typeparamref name="TSource"/>.</param>
        /// <param name="selector">A projection function to apply to each element.</param>
        /// <param name="cancellationToken">The optional <see cref="T:System.Threading.CancellationToken"/> which can be used to cancel this task.</param>
        /// <typeparam name="TSource">The type of the elements of <paramref name="source"/>.</typeparam>
        /// <exception cref="T:System.ArgumentNullException">
        /// <paramref name="source"/> or <paramref name="selector"/> is null.</exception>
        /// <exception cref="T:System.OverflowException">The sum is larger than <see cref="F:System.Int64.MaxValue"/>.</exception>
        public static async Task<long> SumAsync<TSource>(this IQueryable<TSource> source, Expression<Func<TSource, long>> selector, CancellationToken cancellationToken = default(CancellationToken))
        {
            return (long)await ExecuteAsync(source.Provider, FinalExpression(source, sumLongSelectorMethodInfo.Value, selector), cancellationToken);
        }

        /// <summary>
        /// Asynchronously computes the sum of the sequence of nullable <see cref="T:System.Int64"/> values that is obtained by invoking a projection function on each element of the input sequence.
        /// </summary>
        /// <returns>
        /// A task that returns the sum of the projected values.
        /// </returns>
        /// <param name="source">A sequence of values of type <typeparamref name="TSource"/>.</param>
        /// <param name="selector">A projection function to apply to each element.</param>
        /// <param name="cancellationToken">The optional <see cref="T:System.Threading.CancellationToken"/> which can be used to cancel this task.</param>
        /// <typeparam name="TSource">The type of the elements of <paramref name="source"/>.</typeparam>
        /// <exception cref="T:System.ArgumentNullException">
        /// <paramref name="source"/> or <paramref name="selector"/> is null.</exception>
        /// <exception cref="T:System.OverflowException">The sum is larger than <see cref="F:System.Int64.MaxValue"/>.</exception>
        public static async Task<long?> SumAsync<TSource>(this IQueryable<TSource> source, Expression<Func<TSource, long?>> selector, CancellationToken cancellationToken = default(CancellationToken))
        {
            return (long?)await ExecuteAsync(source.Provider, FinalExpression(source, sumLongNullableSelectorMethodInfo.Value, selector), cancellationToken);
        }

        /// <summary>
        /// Asynchronously computes the sum of the sequence of <see cref="T:System.Single"/> values that is obtained by invoking a projection function on each element of the input sequence.
        /// </summary>
        /// <returns>
        /// A task that returns the sum of the projected values.
        /// </returns>
        /// <param name="source">A sequence of values of type <typeparamref name="TSource"/>.</param>
        /// <param name="selector">A projection function to apply to each element.</param>
        /// <param name="cancellationToken">The optional <see cref="T:System.Threading.CancellationToken"/> which can be used to cancel this task.</param>
        /// <typeparam name="TSource">The type of the elements of <paramref name="source"/>.</typeparam>
        /// <exception cref="T:System.ArgumentNullException">
        /// <paramref name="source"/> or <paramref name="selector"/> is null.</exception>
        public static async Task<float> SumAsync<TSource>(this IQueryable<TSource> source, Expression<Func<TSource, float>> selector, CancellationToken cancellationToken = default(CancellationToken))
        {
            return (float)await ExecuteAsync(source.Provider, FinalExpression(source, sumFloatSelectorMethodInfo.Value, selector), cancellationToken);
        }

        /// <summary>
        /// Asynchronously computes the sum of the sequence of nullable <see cref="T:System.Single"/> values that is obtained by invoking a projection function on each element of the input sequence.
        /// </summary>
        /// <returns>
        /// A task that returns the sum of the projected values.
        /// </returns>
        /// <param name="source">A sequence of values of type <typeparamref name="TSource"/>.</param>
        /// <param name="selector">A projection function to apply to each element.</param>
        /// <param name="cancellationToken">The optional <see cref="T:System.Threading.CancellationToken"/> which can be used to cancel this task.</param>
        /// <typeparam name="TSource">The type of the elements of <paramref name="source"/>.</typeparam>
        /// <exception cref="T:System.ArgumentNullException">
        /// <paramref name="source"/> or <paramref name="selector"/> is null.</exception>
        public static async Task<float?> SumAsync<TSource>(this IQueryable<TSource> source, Expression<Func<TSource, float?>> selector, CancellationToken cancellationToken = default(CancellationToken))
        {
            return (float?)await ExecuteAsync(source.Provider, FinalExpression(source, sumFloatNullableSelectorMethodInfo.Value, selector), cancellationToken);
        }

        /// <summary>
        /// Asynchronously computes the sum of the sequence of <see cref="T:System.Double"/> values that is obtained by invoking a projection function on each element of the input sequence.
        /// </summary>
        /// <returns>
        /// A task that returns the sum of the projected values.
        /// </returns>
        /// <param name="source">A sequence of values of type <typeparamref name="TSource"/>.</param>
        /// <param name="selector">A projection function to apply to each element.</param>
        /// <param name="cancellationToken">The optional <see cref="T:System.Threading.CancellationToken"/> which can be used to cancel this task.</param>
        /// <typeparam name="TSource">The type of the elements of <paramref name="source"/>.</typeparam>
        /// <exception cref="T:System.ArgumentNullException">
        /// <paramref name="source"/> or <paramref name="selector"/> is null.</exception>
        public static async Task<double> SumAsync<TSource>(this IQueryable<TSource> source, Expression<Func<TSource, double>> selector, CancellationToken cancellationToken = default(CancellationToken))
        {
            return (double)await ExecuteAsync(source.Provider, FinalExpression(source, sumDoubleSelectorMethodInfo.Value, selector), cancellationToken);
        }

        /// <summary>
        /// Asynchronously computes the sum of the sequence of nullable <see cref="T:System.Double"/> values that is obtained by invoking a projection function on each element of the input sequence.
        /// </summary>
        /// <returns>
        /// A task that returns the sum of the projected values.
        /// </returns>
        /// <param name="source">A sequence of values of type <typeparamref name="TSource"/>.</param>
        /// <param name="selector">A projection function to apply to each element.</param>
        /// <param name="cancellationToken">The optional <see cref="T:System.Threading.CancellationToken"/> which can be used to cancel this task.</param>
        /// <typeparam name="TSource">The type of the elements of <paramref name="source"/>.</typeparam>
        /// <exception cref="T:System.ArgumentNullException">
        /// <paramref name="source"/> or <paramref name="selector"/> is null.</exception>
        public static async Task<double?> SumAsync<TSource>(this IQueryable<TSource> source, Expression<Func<TSource, double?>> selector, CancellationToken cancellationToken = default(CancellationToken))
        {
            return (double?)await ExecuteAsync(source.Provider, FinalExpression(source, sumDoubleNullableSelectorMethodInfo.Value, selector), cancellationToken);
        }

        /// <summary>
        /// Asynchronously computes the sum of the sequence of <see cref="T:System.Decimal"/> values that is obtained by invoking a projection function on each element of the input sequence.
        /// </summary>
        /// <returns>
        /// A task that returns the sum of the projected values.
        /// </returns>
        /// <param name="source">A sequence of values of type <typeparamref name="TSource"/>.</param>
        /// <param name="selector">A projection function to apply to each element.</param>
        /// <param name="cancellationToken">The optional <see cref="T:System.Threading.CancellationToken"/> which can be used to cancel this task.</param>
        /// <typeparam name="TSource">The type of the elements of <paramref name="source"/>.</typeparam>
        /// <exception cref="T:System.ArgumentNullException">
        /// <paramref name="source"/> or <paramref name="selector"/> is null.</exception>
        /// <exception cref="T:System.OverflowException">The sum is larger than <see cref="F:System.Decimal.MaxValue"/>.</exception>
        public static async Task<decimal> SumAsync<TSource>(this IQueryable<TSource> source, Expression<Func<TSource, decimal>> selector, CancellationToken cancellationToken = default(CancellationToken))
        {
            return (decimal)await ExecuteAsync(source.Provider, FinalExpression(source, sumDecimalSelectorMethodInfo.Value, selector), cancellationToken);
        }

        /// <summary>
        /// Asynchronously computes the sum of the sequence of nullable <see cref="T:System.Decimal"/> values that is obtained by invoking a projection function on each element of the input sequence.
        /// </summary>
        /// <returns>
        /// A task that returns the sum of the projected values.
        /// </returns>
        /// <param name="source">A sequence of values of type <typeparamref name="TSource"/>.</param>
        /// <param name="selector">A projection function to apply to each element.</param>
        /// <param name="cancellationToken">The optional <see cref="T:System.Threading.CancellationToken"/> which can be used to cancel this task.</param>
        /// <typeparam name="TSource">The type of the elements of <paramref name="source"/>.</typeparam>
        /// <exception cref="T:System.ArgumentNullException">
        /// <paramref name="source"/> or <paramref name="selector"/> is null.</exception>
        /// <exception cref="T:System.OverflowException">The sum is larger than <see cref="F:System.Decimal.MaxValue"/>.</exception>
        public static async Task<decimal?> SumAsync<TSource>(this IQueryable<TSource> source, Expression<Func<TSource, decimal?>> selector, CancellationToken cancellationToken = default(CancellationToken))
        {
            return (decimal?)await ExecuteAsync(source.Provider, FinalExpression(source, sumDecimalNullableSelectorMethodInfo.Value, selector), cancellationToken);
        }
    }
}