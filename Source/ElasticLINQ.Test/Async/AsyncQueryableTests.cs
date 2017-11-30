// Licensed under the Apache 2.0 License. See LICENSE.txt in the project root for more information.

using ElasticLinq.Async;
using ElasticLinq.Test.TestSupport;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace ElasticLinq.Test.Async
{
    public static class AsyncQueryableTests
    {
        static readonly TestableElasticContext context = new TestableElasticContext();

        static AsyncQueryableTests()
        {
            context.SetData<Robot>(RobotFactory.Inventory);
        }

        [Fact]
        public static async Task CountAsyncReturnsSameResultAsCount()
        {
            var expected = context.Query<Robot>().Count();
            var actual = await context.Query<Robot>().CountAsync().ConfigureAwait(false);

            Assert.Equal(expected, actual);
        }

        [Fact]
        public static async Task CountPredicateAsyncReturnsSameResultAsCountPredicate()
        {
            var expected = context.Query<Robot>().Count(r => r.Zone == 3);
            var actual = await context.Query<Robot>().CountAsync(r => r.Zone == 3).ConfigureAwait(false);

            Assert.Equal(expected, actual);
        }

        [Fact]
        public static async Task LongCountAsyncReturnsSameResultAsCount()
        {
            var expected = context.Query<Robot>().LongCount();
            var actual = await context.Query<Robot>().LongCountAsync().ConfigureAwait(false);

            Assert.Equal(expected, actual);
        }

        [Fact]
        public static async Task LongCountPredicateAsyncReturnsSameResultAsCountPredicate()
        {
            var expected = context.Query<Robot>().LongCount(r => r.Zone == 3);
            var actual = await context.Query<Robot>().LongCountAsync(r => r.Zone == 3).ConfigureAwait(false);

            Assert.Equal(expected, actual);
        }

        [Fact]
        public static async Task MinAsyncReturnsSameResultAsMin()
        {
            var expected = context.Query<Robot>().Select(r => r.Cost).Min();
            var actual = await context.Query<Robot>().Select(r => r.Cost).MinAsync().ConfigureAwait(false);

            Assert.Equal(expected, actual);
        }

        [Fact]
        public static async Task MinSelectorAsyncReturnsSameResultAsMinSelector()
        {
            var expected = context.Query<Robot>().Min(r => r.Cost);
            var actual = await context.Query<Robot>().MinAsync(r => r.Cost).ConfigureAwait(false);

            Assert.Equal(expected, actual);
        }

        [Fact]
        public static async Task MaxAsyncReturnsSameResultAsMax()
        {
            var expected = context.Query<Robot>().Select(r => r.Cost).Max();
            var actual = await context.Query<Robot>().Select(r => r.Cost).MaxAsync().ConfigureAwait(false);

            Assert.Equal(expected, actual);
        }

        [Fact]
        public static async Task MaxSelectorAsyncReturnsSameResultAsMaxSelector()
        {
            var expected = context.Query<Robot>().Max(r => r.Cost);
            var actual = await context.Query<Robot>().MaxAsync(r => r.Cost).ConfigureAwait(false);

            Assert.Equal(expected, actual);
        }

        [Fact]
        public static async Task ToArrayAsyncReturnsSameResultAsToArray()
        {
            var expected = context.Query<Robot>().ToArray();
            var actual = await context.Query<Robot>().ToArrayAsync().ConfigureAwait(false);

            Assert.Equal(expected, actual);
        }

        [Fact]
        public static async Task ToDictionaryAsyncReturnsSameResultAsToDictionary()
        {
            var expected = context.Query<Robot>().ToDictionary(r => r.Id);
            var actual = await context.Query<Robot>().ToDictionaryAsync(r => r.Id).ConfigureAwait(false);

            Assert.Equal(expected, actual);
        }

        [Fact]
        public static async Task ToDictionaryElementSelectorAsyncReturnsSameResultAsToDictionaryElementSelector()
        {
            var expected = context.Query<Robot>().ToDictionary(r => r.Id, v => v.Started);
            var actual = await context.Query<Robot>().ToDictionaryAsync(r => r.Id, v => v.Started).ConfigureAwait(false);

            Assert.Equal(expected, actual);
        }
    }
}