// Licensed under the Apache 2.0 License. See LICENSE.txt in the project root for more information.

using ElasticLinq.Async;
using ElasticLinq.Test.TestSupport;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace ElasticLinq.Test.Async
{
    public static class AsyncQueryableSumTests
    {
        static readonly TestableElasticContext context = new TestableElasticContext();

        static IQueryable<WithAllTypes> source => context.Query<WithAllTypes>();

        static AsyncQueryableSumTests()
        {
            context.SetData(WithAllTypes.CreateSequence(25));
        }

        [Fact]
        public static async Task SumIntAsyncReturnsSameResultAsSumInt()
        {
            var expected = source.Sum(r => r.Int);
            var actual = await source.SumAsync(r => r.Int).ConfigureAwait(false);

            Assert.Equal(expected, actual);
        }

        [Fact]
        public static async Task SumIntNullableAsyncReturnsSameResultAsSumIntNullable()
        {
            var expected = source.Sum(r => r.IntNullable);
            var actual = await source.SumAsync(r => r.IntNullable).ConfigureAwait(false);

            Assert.Equal(expected, actual);
        }

        [Fact]
        public static async Task SumLongAsyncReturnsSameResultAsSumLong()
        {
            var expected = source.Sum(r => r.Long);
            var actual = await source.SumAsync(r => r.Long).ConfigureAwait(false);

            Assert.Equal(expected, actual);
        }

        [Fact]
        public static async Task SumLongNullableAsyncReturnsSameResultAsSumLongNullable()
        {
            var expected = source.Sum(r => r.LongNullable);
            var actual = await source.SumAsync(r => r.LongNullable).ConfigureAwait(false);

            Assert.Equal(expected, actual);
        }

        [Fact]
        public static async Task SumFloatAsyncReturnsSameResultAsSumFloat()
        {
            var expected = source.Sum(r => r.Float);
            var actual = await source.SumAsync(r => r.Float).ConfigureAwait(false);

            Assert.Equal(expected, actual);
        }

        [Fact]
        public static async Task SumFloatNullableAsyncReturnsSameResultAsSumFloatNullable()
        {
            var expected = source.Sum(r => r.FloatNullable);
            var actual = await source.SumAsync(r => r.FloatNullable).ConfigureAwait(false);

            Assert.Equal(expected, actual);
        }

        [Fact]
        public static async Task SumDoubleAsyncReturnsSameResultAsSumDouble()
        {
            var expected = source.Sum(r => r.Double);
            var actual = await source.SumAsync(r => r.Double).ConfigureAwait(false);

            Assert.Equal(expected, actual);
        }

        [Fact]
        public static async Task SumDoubleNullableAsyncReturnsSameResultAsSumDoubleNullable()
        {
            var expected = source.Sum(r => r.DoubleNullable);
            var actual = await source.SumAsync(r => r.DoubleNullable).ConfigureAwait(false);

            Assert.Equal(expected, actual);
        }

        [Fact]
        public static async Task SumDecimalAsyncReturnsSameResultAsSumDecimal()
        {
            var expected = source.Sum(r => r.Decimal);
            var actual = await source.SumAsync(r => r.Decimal).ConfigureAwait(false);

            Assert.Equal(expected, actual);
        }

        [Fact]
        public static async Task SumDecimalNullableAsyncReturnsSameResultAsSumDecimalNullable()
        {
            var expected = source.Sum(r => r.DecimalNullable);
            var actual = await source.SumAsync(r => r.DecimalNullable).ConfigureAwait(false);

            Assert.Equal(expected, actual);
        }

        [Fact]
        public static async Task SumSelectIntAsyncReturnsSameResultAsSumInt()
        {
            var expected = source.Select(r => r.Int).Sum();
            var actual = await source.Select(r => r.Int).SumAsync().ConfigureAwait(false);

            Assert.Equal(expected, actual);
        }

        [Fact]
        public static async Task SumSelectIntNullableAsyncReturnsSameResultAsSumIntNullable()
        {
            var expected = source.Select(r => r.IntNullable).Sum();
            var actual = await source.Select(r => r.IntNullable).SumAsync().ConfigureAwait(false);

            Assert.Equal(expected, actual);
        }

        [Fact]
        public static async Task SumSelectLongAsyncReturnsSameResultAsSumLong()
        {
            var expected = source.Select(r => r.Long).Sum();
            var actual = await source.Select(r => r.Long).SumAsync().ConfigureAwait(false);

            Assert.Equal(expected, actual);
        }

        [Fact]
        public static async Task SumSelectLongNullableAsyncReturnsSameResultAsSumLongNullable()
        {
            var expected = source.Select(r => r.LongNullable).Sum();
            var actual = await source.Select(r => r.LongNullable).SumAsync().ConfigureAwait(false);

            Assert.Equal(expected, actual);
        }

        [Fact]
        public static async Task SumSelectFloatAsyncReturnsSameResultAsSumFloat()
        {
            var expected = source.Select(r => r.Float).Sum();
            var actual = await source.Select(r => r.Float).SumAsync().ConfigureAwait(false);

            Assert.Equal(expected, actual);
        }

        [Fact]
        public static async Task SumSelectFloatNullableAsyncReturnsSameResultAsSumFloatNullable()
        {
            var expected = source.Select(r => r.FloatNullable).Sum();
            var actual = await source.Select(r => r.FloatNullable).SumAsync().ConfigureAwait(false);

            Assert.Equal(expected, actual);
        }

        [Fact]
        public static async Task SumSelectDoubleAsyncReturnsSameResultAsSumDouble()
        {
            var expected = source.Select(r => r.Double).Sum();
            var actual = await source.Select(r => r.Double).SumAsync().ConfigureAwait(false);

            Assert.Equal(expected, actual);
        }

        [Fact]
        public static async Task SumSelectDoubleNullableAsyncReturnsSameResultAsSumDoubleNullable()
        {
            var expected = source.Select(r => r.DoubleNullable).Sum();
            var actual = await source.Select(r => r.DoubleNullable).SumAsync().ConfigureAwait(false);

            Assert.Equal(expected, actual);
        }

        [Fact]
        public static async Task SumSelectDecimalAsyncReturnsSameResultAsSumDecimal()
        {
            var expected = source.Select(r => r.Decimal).Sum();
            var actual = await source.Select(r => r.Decimal).SumAsync().ConfigureAwait(false);

            Assert.Equal(expected, actual);
        }

        [Fact]
        public static async Task SumSelectDecimalNullableAsyncReturnsSameResultAsSumDecimalNullable()
        {
            var expected = source.Select(r => r.DecimalNullable).Sum();
            var actual = await source.Select(r => r.DecimalNullable).SumAsync().ConfigureAwait(false);

            Assert.Equal(expected, actual);
        }
    }
}