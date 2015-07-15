// Licensed under the Apache 2.0 License. See LICENSE.txt in the project root for more information.

using ElasticLinq.Async;
using ElasticLinq.Test.TestSupport;
using System.Linq;
using Xunit;

namespace ElasticLinq.Test.Async
{
    public static class AsyncQueryableAverageTests
    {
        static readonly TestableElasticContext context = new TestableElasticContext();

        static IQueryable<WithAllTypes> source
        {
            get { return context.Query<WithAllTypes>(); }
        }

        static AsyncQueryableAverageTests()
        {
            context.SetData(WithAllTypes.CreateSequence(25));
        }

        [Fact]
        public static async void AverageIntAsyncReturnsSameResultAsAverageInt()
        {
            var expected = source.Average(r => r.Int);
            var actual = await source.AverageAsync(r => r.Int);

            Assert.Equal<object>(expected, actual);
        }

        [Fact]
        public static async void AverageIntNullableAsyncReturnsSameResultAsAverageIntNullable()
        {
            var expected = source.Average(r => r.IntNullable);
            var actual = await source.AverageAsync(r => r.IntNullable);

            Assert.Equal<object>(expected, actual);
        }

        [Fact]
        public static async void AverageLongAsyncReturnsSameResultAsAverageLong()
        {
            var expected = source.Average(r => r.Long);
            var actual = await source.AverageAsync(r => r.Long);

            Assert.Equal<object>(expected, actual);
        }

        [Fact]
        public static async void AverageLongNullableAsyncReturnsSameResultAsAverageLongNullable()
        {
            var expected = source.Average(r => r.LongNullable);
            var actual = await source.AverageAsync(r => r.LongNullable);

            Assert.Equal<object>(expected, actual);
        }

        [Fact]
        public static async void AverageFloatAsyncReturnsSameResultAsAverageFloat()
        {
            var expected = source.Average(r => r.Float);
            var actual = await source.AverageAsync(r => r.Float);

            Assert.Equal<object>(expected, actual);
        }

        [Fact]
        public static async void AverageFloatNullableAsyncReturnsSameResultAsAverageFloatNullable()
        {
            var expected = source.Average(r => r.FloatNullable);
            var actual = await source.AverageAsync(r => r.FloatNullable);

            Assert.Equal<object>(expected, actual);
        }

        [Fact]
        public static async void AverageDoubleAsyncReturnsSameResultAsAverageDouble()
        {
            var expected = source.Average(r => r.Double);
            var actual = await source.AverageAsync(r => r.Double);

            Assert.Equal<object>(expected, actual);
        }

        [Fact]
        public static async void AverageDoubleNullableAsyncReturnsSameResultAsAverageDoubleNullable()
        {
            var expected = source.Average(r => r.DoubleNullable);
            var actual = await source.AverageAsync(r => r.DoubleNullable);

            Assert.Equal<object>(expected, actual);
        }

        [Fact]
        public static async void AverageDecimalAsyncReturnsSameResultAsAverageDecimal()
        {
            var expected = source.Average(r => r.Decimal);
            var actual = await source.AverageAsync(r => r.Decimal);

            Assert.Equal<object>(expected, actual);
        }

        [Fact]
        public static async void AverageDecimalNullableAsyncReturnsSameResultAsAverageDecimalNullable()
        {
            var expected = source.Average(r => r.DecimalNullable);
            var actual = await source.AverageAsync(r => r.DecimalNullable);

            Assert.Equal<object>(expected, actual);
        }

        [Fact]
        public static async void AverageSelectIntAsyncReturnsSameResultAsAverageInt()
        {
            var expected = source.Select(r => r.Int).Average();
            var actual = await source.Select(r => r.Int).AverageAsync();

            Assert.Equal<object>(expected, actual);
        }

        [Fact]
        public static async void AverageSelectIntNullableAsyncReturnsSameResultAsAverageIntNullable()
        {
            var expected = source.Select(r => r.IntNullable).Average();
            var actual = await source.Select(r => r.IntNullable).AverageAsync();

            Assert.Equal<object>(expected, actual);
        }

        [Fact]
        public static async void AverageSelectLongAsyncReturnsSameResultAsAverageLong()
        {
            var expected = source.Select(r => r.Long).Average();
            var actual = await source.Select(r => r.Long).AverageAsync();

            Assert.Equal<object>(expected, actual);
        }

        [Fact]
        public static async void AverageSelectLongNullableAsyncReturnsSameResultAsAverageLongNullable()
        {
            var expected = source.Select(r => r.LongNullable).Average();
            var actual = await source.Select(r => r.LongNullable).AverageAsync();

            Assert.Equal<object>(expected, actual);
        }

        [Fact]
        public static async void AverageSelectFloatAsyncReturnsSameResultAsAverageFloat()
        {
            var expected = source.Select(r => r.Float).Average();
            var actual = await source.Select(r => r.Float).AverageAsync();

            Assert.Equal<object>(expected, actual);
        }

        [Fact]
        public static async void AverageSelectFloatNullableAsyncReturnsSameResultAsAverageFloatNullable()
        {
            var expected = source.Select(r => r.FloatNullable).Average();
            var actual = await source.Select(r => r.FloatNullable).AverageAsync();

            Assert.Equal<object>(expected, actual);
        }

        [Fact]
        public static async void AverageSelectDoubleAsyncReturnsSameResultAsAverageDouble()
        {
            var expected = source.Select(r => r.Double).Average();
            var actual = await source.Select(r => r.Double).AverageAsync();

            Assert.Equal<object>(expected, actual);
        }

        [Fact]
        public static async void AverageSelectDoubleNullableAsyncReturnsSameResultAsAverageDoubleNullable()
        {
            var expected = source.Select(r => r.DoubleNullable).Average();
            var actual = await source.Select(r => r.DoubleNullable).AverageAsync();

            Assert.Equal<object>(expected, actual);
        }

        [Fact]
        public static async void AverageSelectDecimalAsyncReturnsSameResultAsAverageDecimal()
        {
            var expected = source.Select(r => r.Decimal).Average();
            var actual = await source.Select(r => r.Decimal).AverageAsync();

            Assert.Equal<object>(expected, actual);
        }

        [Fact]
        public static async void AverageSelectDecimalNullableAsyncReturnsSameResultAsAverageDecimalNullable()
        {
            var expected = source.Select(r => r.DecimalNullable).Average();
            var actual = await source.Select(r => r.DecimalNullable).AverageAsync();

            Assert.Equal<object>(expected, actual);
        }

    }
}