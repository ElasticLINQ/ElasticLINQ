// Licensed under the Apache 2.0 License. See LICENSE.txt in the project root for more information.

using ElasticLinq.Async;
using ElasticLinq.Test.TestSupport;
using System.Linq;
using Xunit;

namespace ElasticLinq.Test
{
    public static class AsyncQueryableTests
    {
        static readonly TestableElasticContext context = new TestableElasticContext();

        static AsyncQueryableTests()
        {
            context.SetData<Robot>(RobotFactory.Inventory);
        }

        [Fact]
        public async static void CountAsyncReturnsSameResultAsCount()
        {
            var expected = context.Query<Robot>().Count();
            var actual = await context.Query<Robot>().CountAsync();

            Assert.Equal(expected, actual);
        }

        [Fact]
        public async static void CountPredicateAsyncReturnsSameResultAsCountPredicate()
        {
            var expected = context.Query<Robot>().Count(r => r.Zone == 3);
            var actual = await context.Query<Robot>().CountAsync(r => r.Zone == 3);

            Assert.Equal(expected, actual);
        }

        [Fact]
        public async static void LongCountAsyncReturnsSameResultAsCount()
        {
            var expected = context.Query<Robot>().LongCount();
            var actual = await context.Query<Robot>().LongCountAsync();

            Assert.Equal(expected, actual);
        }

        [Fact]
        public async static void LongCountPredicateAsyncReturnsSameResultAsCountPredicate()
        {
            var expected = context.Query<Robot>().LongCount(r => r.Zone == 3);
            var actual = await context.Query<Robot>().LongCountAsync(r => r.Zone == 3);

            Assert.Equal(expected, actual);
        }

        [Fact]
        public async static void MinAsyncReturnsSameResultAsMin()
        {
            var expected = context.Query<Robot>().Select(r => r.Cost).Min();
            var actual = await context.Query<Robot>().Select(r => r.Cost).MinAsync();

            Assert.Equal(expected, actual);
        }

        [Fact]
        public async static void MinSelectorAsyncReturnsSameResultAsMinSelector()
        {
            var expected = context.Query<Robot>().Min(r => r.Cost);
            var actual = await context.Query<Robot>().MinAsync(r => r.Cost);

            Assert.Equal(expected, actual);
        }

        [Fact]
        public async static void MaxAsyncReturnsSameResultAsMax()
        {
            var expected = context.Query<Robot>().Select(r => r.Cost).Max();
            var actual = await context.Query<Robot>().Select(r => r.Cost).MaxAsync();

            Assert.Equal(expected, actual);
        }

        [Fact]
        public async static void MaxSelectorAsyncReturnsSameResultAsMaxSelector()
        {
            var expected = context.Query<Robot>().Max(r => r.Cost);
            var actual = await context.Query<Robot>().MaxAsync(r => r.Cost);

            Assert.Equal(expected, actual);
        }

        [Fact]
        public async static void ToArrayAsyncReturnsSameResultAsToArray()
        {
            var expected = context.Query<Robot>().ToArray();
            var actual = await context.Query<Robot>().ToArrayAsync();

            Assert.Equal(expected, actual);
        }

        [Fact]
        public async static void ToDictionaryAsyncReturnsSameResultAsToDictionary()
        {
            var expected = context.Query<Robot>().ToDictionary(r => r.Id);
            var actual = await context.Query<Robot>().ToDictionaryAsync(r => r.Id);

            Assert.Equal(expected, actual);
        }

        [Fact]
        public async static void ToDictionaryElementSelectorAsyncReturnsSameResultAsToDictionaryElementSelector()
        {
            var expected = context.Query<Robot>().ToDictionary(r => r.Id, v => v.Started);
            var actual = await context.Query<Robot>().ToDictionaryAsync(r => r.Id, v => v.Started);

            Assert.Equal(expected, actual);
        }
    }
}