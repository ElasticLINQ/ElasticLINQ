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

            Assert.Equal(expected, actual);
        }

        [Fact]
        public static async void SumIntNullableAsyncReturnsSameResultAsSumIntNullable()
        {
            var expected = source.Sum(r => r.IntNullable);
            var actual = await source.SumAsync(r => r.IntNullable);

            Assert.Equal(expected, actual);
        }

        [Fact]
        public static async void SumLongAsyncReturnsSameResultAsSumLong()
        {
            var expected = source.Sum(r => r.Long);
            var actual = await source.SumAsync(r => r.Long);

            Assert.Equal(expected, actual);
        }

        [Fact]
        public static async void SumLongNullableAsyncReturnsSameResultAsSumLongNullable()
        {
            var expected = source.Sum(r => r.LongNullable);
            var actual = await source.SumAsync(r => r.LongNullable);

            Assert.Equal(expected, actual);
        }

        [Fact]
        public static async void SumFloatAsyncReturnsSameResultAsSumFloat()
        {
            var expected = source.Sum(r => r.Float);
            var actual = await source.SumAsync(r => r.Float);

            Assert.Equal(expected, actual);
        }

        [Fact]
        public static async void SumFloatNullableAsyncReturnsSameResultAsSumFloatNullable()
        {
            var expected = source.Sum(r => r.FloatNullable);
            var actual = await source.SumAsync(r => r.FloatNullable);

            Assert.Equal(expected, actual);
        }

        [Fact]
        public static async void SumDoubleAsyncReturnsSameResultAsSumDouble()
        {
            var expected = source.Sum(r => r.Double);
            var actual = await source.SumAsync(r => r.Double);

            Assert.Equal(expected, actual);
        }

        [Fact]
        public static async void SumDoubleNullableAsyncReturnsSameResultAsSumDoubleNullable()
        {
            var expected = source.Sum(r => r.DoubleNullable);
            var actual = await source.SumAsync(r => r.DoubleNullable);

            Assert.Equal(expected, actual);
        }

        [Fact]
        public static async void SumDecimalAsyncReturnsSameResultAsSumDecimal()
        {
            var expected = source.Sum(r => r.Decimal);
            var actual = await source.SumAsync(r => r.Decimal);

            Assert.Equal(expected, actual);
        }

        [Fact]
        public static async void SumDecimalNullableAsyncReturnsSameResultAsSumDecimalNullable()
        {
            var expected = source.Sum(r => r.DecimalNullable);
            var actual = await source.SumAsync(r => r.DecimalNullable);

            Assert.Equal(expected, actual);
        }

        [Fact]
        public static async void SumSelectIntAsyncReturnsSameResultAsSumInt()
        {
            var expected = source.Select(r => r.Int).Sum();
            var actual = await source.Select(r => r.Int).SumAsync();

            Assert.Equal(expected, actual);
        }

        [Fact]
        public static async void SumSelectIntNullableAsyncReturnsSameResultAsSumIntNullable()
        {
            var expected = source.Select(r => r.IntNullable).Sum();
            var actual = await source.Select(r => r.IntNullable).SumAsync();

            Assert.Equal(expected, actual);
        }

        [Fact]
        public static async void SumSelectLongAsyncReturnsSameResultAsSumLong()
        {
            var expected = source.Select(r => r.Long).Sum();
            var actual = await source.Select(r => r.Long).SumAsync();

            Assert.Equal(expected, actual);
        }

        [Fact]
        public static async void SumSelectLongNullableAsyncReturnsSameResultAsSumLongNullable()
        {
            var expected = source.Select(r => r.LongNullable).Sum();
            var actual = await source.Select(r => r.LongNullable).SumAsync();

            Assert.Equal(expected, actual);
        }

        [Fact]
        public static async void SumSelectFloatAsyncReturnsSameResultAsSumFloat()
        {
            var expected = source.Select(r => r.Float).Sum();
            var actual = await source.Select(r => r.Float).SumAsync();

            Assert.Equal(expected, actual);
        }

        [Fact]
        public static async void SumSelectFloatNullableAsyncReturnsSameResultAsSumFloatNullable()
        {
            var expected = source.Select(r => r.FloatNullable).Sum();
            var actual = await source.Select(r => r.FloatNullable).SumAsync();

            Assert.Equal(expected, actual);
        }

        [Fact]
        public static async void SumSelectDoubleAsyncReturnsSameResultAsSumDouble()
        {
            var expected = source.Select(r => r.Double).Sum();
            var actual = await source.Select(r => r.Double).SumAsync();

            Assert.Equal(expected, actual);
        }

        [Fact]
        public static async void SumSelectDoubleNullableAsyncReturnsSameResultAsSumDoubleNullable()
        {
            var expected = source.Select(r => r.DoubleNullable).Sum();
            var actual = await source.Select(r => r.DoubleNullable).SumAsync();

            Assert.Equal(expected, actual);
        }

        [Fact]
        public static async void SumSelectDecimalAsyncReturnsSameResultAsSumDecimal()
        {
            var expected = source.Select(r => r.Decimal).Sum();
            var actual = await source.Select(r => r.Decimal).SumAsync();

            Assert.Equal(expected, actual);
        }

        [Fact]
        public static async void SumSelectDecimalNullableAsyncReturnsSameResultAsSumDecimalNullable()
        {
            var expected = source.Select(r => r.DecimalNullable).Sum();
            var actual = await source.Select(r => r.DecimalNullable).SumAsync();

            Assert.Equal(expected, actual);
        }
    }
}