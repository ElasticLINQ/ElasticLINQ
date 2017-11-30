// Licensed under the Apache 2.0 License. See LICENSE.txt in the project root for more information.

using ElasticLinq.Async;
using ElasticLinq.Test.TestSupport;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace ElasticLinq.Test.Async
{
    public static class AsyncQueryableAverageTests
    {
        static readonly TestableElasticContext context = new TestableElasticContext();

        static IQueryable<WithAllTypes> source => context.Query<WithAllTypes>();

        static AsyncQueryableAverageTests()
        {
            context.SetData(WithAllTypes.CreateSequence(25));
        }

        [Fact]
        public static async Task AverageIntAsyncReturnsSameResultAsAverageInt()
        {
            var expected = source.Average(r => r.Int);
            var actual = await source.AverageAsync(r => r.Int).ConfigureAwait(false);

            Assert.Equal<object>(expected, actual);
        }

        [Fact]
        public static async Task AverageIntNullableAsyncReturnsSameResultAsAverageIntNullable()
        {
            var expected = source.Average(r => r.IntNullable);
            var actual = await source.AverageAsync(r => r.IntNullable).ConfigureAwait(false);

            Assert.Equal<object>(expected, actual);
        }

        [Fact]
        public static async Task AverageLongAsyncReturnsSameResultAsAverageLong()
        {
            var expected = source.Average(r => r.Long);
            var actual = await source.AverageAsync(r => r.Long).ConfigureAwait(false);

            Assert.Equal<object>(expected, actual);
        }

        [Fact]
        public static async Task AverageLongNullableAsyncReturnsSameResultAsAverageLongNullable()
        {
            var expected = source.Average(r => r.LongNullable);
            var actual = await source.AverageAsync(r => r.LongNullable).ConfigureAwait(false);

            Assert.Equal<object>(expected, actual);
        }

        [Fact]
        public static async Task AverageFloatAsyncReturnsSameResultAsAverageFloat()
        {
            var expected = source.Average(r => r.Float);
            var actual = await source.AverageAsync(r => r.Float).ConfigureAwait(false);

            Assert.Equal<object>(expected, actual);
        }

        [Fact]
        public static async Task AverageFloatNullableAsyncReturnsSameResultAsAverageFloatNullable()
        {
            var expected = source.Average(r => r.FloatNullable);
            var actual = await source.AverageAsync(r => r.FloatNullable).ConfigureAwait(false);

            Assert.Equal<object>(expected, actual);
        }

        [Fact]
        public static async Task AverageDoubleAsyncReturnsSameResultAsAverageDouble()
        {
            var expected = source.Average(r => r.Double);
            var actual = await source.AverageAsync(r => r.Double).ConfigureAwait(false);

            Assert.Equal<object>(expected, actual);
        }

        [Fact]
        public static async Task AverageDoubleNullableAsyncReturnsSameResultAsAverageDoubleNullable()
        {
            var expected = source.Average(r => r.DoubleNullable);
            var actual = await source.AverageAsync(r => r.DoubleNullable).ConfigureAwait(false);

            Assert.Equal<object>(expected, actual);
        }

        [Fact]
        public static async Task AverageDecimalAsyncReturnsSameResultAsAverageDecimal()
        {
            var expected = source.Average(r => r.Decimal);
            var actual = await source.AverageAsync(r => r.Decimal).ConfigureAwait(false);

            Assert.Equal<object>(expected, actual);
        }

        [Fact]
        public static async Task AverageDecimalNullableAsyncReturnsSameResultAsAverageDecimalNullable()
        {
            var expected = source.Average(r => r.DecimalNullable);
            var actual = await source.AverageAsync(r => r.DecimalNullable).ConfigureAwait(false);

            Assert.Equal<object>(expected, actual);
        }

        [Fact]
        public static async Task AverageSelectIntAsyncReturnsSameResultAsAverageInt()
        {
            var expected = source.Select(r => r.Int).Average();
            var actual = await source.Select(r => r.Int).AverageAsync().ConfigureAwait(false);

            Assert.Equal<object>(expected, actual);
        }

        [Fact]
        public static async Task AverageSelectIntNullableAsyncReturnsSameResultAsAverageIntNullable()
        {
            var expected = source.Select(r => r.IntNullable).Average();
            var actual = await source.Select(r => r.IntNullable).AverageAsync().ConfigureAwait(false);

            Assert.Equal<object>(expected, actual);
        }

        [Fact]
        public static async Task AverageSelectLongAsyncReturnsSameResultAsAverageLong()
        {
            var expected = source.Select(r => r.Long).Average();
            var actual = await source.Select(r => r.Long).AverageAsync().ConfigureAwait(false);

            Assert.Equal<object>(expected, actual);
        }

        [Fact]
        public static async Task AverageSelectLongNullableAsyncReturnsSameResultAsAverageLongNullable()
        {
            var expected = source.Select(r => r.LongNullable).Average();
            var actual = await source.Select(r => r.LongNullable).AverageAsync().ConfigureAwait(false);

            Assert.Equal<object>(expected, actual);
        }

        [Fact]
        public static async Task AverageSelectFloatAsyncReturnsSameResultAsAverageFloat()
        {
            var expected = source.Select(r => r.Float).Average();
            var actual = await source.Select(r => r.Float).AverageAsync().ConfigureAwait(false);

            Assert.Equal<object>(expected, actual);
        }

        [Fact]
        public static async Task AverageSelectFloatNullableAsyncReturnsSameResultAsAverageFloatNullable()
        {
            var expected = source.Select(r => r.FloatNullable).Average();
            var actual = await source.Select(r => r.FloatNullable).AverageAsync().ConfigureAwait(false);

            Assert.Equal<object>(expected, actual);
        }

        [Fact]
        public static async Task AverageSelectDoubleAsyncReturnsSameResultAsAverageDouble()
        {
            var expected = source.Select(r => r.Double).Average();
            var actual = await source.Select(r => r.Double).AverageAsync().ConfigureAwait(false);

            Assert.Equal<object>(expected, actual);
        }

        [Fact]
        public static async Task AverageSelectDoubleNullableAsyncReturnsSameResultAsAverageDoubleNullable()
        {
            var expected = source.Select(r => r.DoubleNullable).Average();
            var actual = await source.Select(r => r.DoubleNullable).AverageAsync().ConfigureAwait(false);

            Assert.Equal<object>(expected, actual);
        }

        [Fact]
        public static async Task AverageSelectDecimalAsyncReturnsSameResultAsAverageDecimal()
        {
            var expected = source.Select(r => r.Decimal).Average();
            var actual = await source.Select(r => r.Decimal).AverageAsync().ConfigureAwait(false);

            Assert.Equal<object>(expected, actual);
        }

        [Fact]
        public static async Task AverageSelectDecimalNullableAsyncReturnsSameResultAsAverageDecimalNullable()
        {
            var expected = source.Select(r => r.DecimalNullable).Average();
            var actual = await source.Select(r => r.DecimalNullable).AverageAsync().ConfigureAwait(false);

            Assert.Equal<object>(expected, actual);
        }

    }
}