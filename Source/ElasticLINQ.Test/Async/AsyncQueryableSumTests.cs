// Licensed under the Apache 2.0 License. See LICENSE.txt in the project root for more information.

using ElasticLinq.Async;
using ElasticLinq.Test.TestSupport;
using System.Linq;
using Xunit;

namespace ElasticLinq.Test.Async
{
    public static class AsyncQueryableSumTests
    {
        static readonly TestableElasticContext context = new TestableElasticContext();

        static IQueryable<WithAllTypes> source
        {
            get { return context.Query<WithAllTypes>(); }
        }

        static AsyncQueryableSumTests()
        {
            context.SetData(WithAllTypes.CreateSequence(25));
        }

        [Fact]
        public static async void SumIntAsyncReturnsSameResultAsSumInt()
        {
            var expected = source.Sum(r => r.Int);
            var actual = await source.SumAsync(r => r.Int);

            Assert.Equal<object>(expected, actual);
        }

        [Fact]
        public static async void SumIntNullableAsyncReturnsSameResultAsSumIntNullable()
        {
            var expected = source.Sum(r => r.IntNullable);
            var actual = await source.SumAsync(r => r.IntNullable);

            Assert.Equal<object>(expected, actual);
        }

        [Fact]
        public static async void SumLongAsyncReturnsSameResultAsSumLong()
        {
            var expected = source.Sum(r => r.Long);
            var actual = await source.SumAsync(r => r.Long);

            Assert.Equal<object>(expected, actual);
        }

        [Fact]
        public static async void SumLongNullableAsyncReturnsSameResultAsSumLongNullable()
        {
            var expected = source.Sum(r => r.LongNullable);
            var actual = await source.SumAsync(r => r.LongNullable);

            Assert.Equal<object>(expected, actual);
        }

        [Fact]
        public static async void SumFloatAsyncReturnsSameResultAsSumFloat()
        {
            var expected = source.Sum(r => r.Float);
            var actual = await source.SumAsync(r => r.Float);

            Assert.Equal<object>(expected, actual);
        }

        [Fact]
        public static async void SumFloatNullableAsyncReturnsSameResultAsSumFloatNullable()
        {
            var expected = source.Sum(r => r.FloatNullable);
            var actual = await source.SumAsync(r => r.FloatNullable);

            Assert.Equal<object>(expected, actual);
        }

        [Fact]
        public static async void SumDoubleAsyncReturnsSameResultAsSumDouble()
        {
            var expected = source.Sum(r => r.Double);
            var actual = await source.SumAsync(r => r.Double);

            Assert.Equal<object>(expected, actual);
        }

        [Fact]
        public static async void SumDoubleNullableAsyncReturnsSameResultAsSumDoubleNullable()
        {
            var expected = source.Sum(r => r.DoubleNullable);
            var actual = await source.SumAsync(r => r.DoubleNullable);

            Assert.Equal<object>(expected, actual);
        }

        [Fact]
        public static async void SumDecimalAsyncReturnsSameResultAsSumDecimal()
        {
            var expected = source.Sum(r => r.Decimal);
            var actual = await source.SumAsync(r => r.Decimal);

            Assert.Equal<object>(expected, actual);
        }

        [Fact]
        public static async void SumDecimalNullableAsyncReturnsSameResultAsSumDecimalNullable()
        {
            var expected = source.Sum(r => r.DecimalNullable);
            var actual = await source.SumAsync(r => r.DecimalNullable);

            Assert.Equal<object>(expected, actual);
        }
    }
}