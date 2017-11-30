// Licensed under the Apache 2.0 License. See LICENSE.txt in the project root for more information.

using System.Linq;
using System.Threading.Tasks;
using ElasticLinq.Async;
using ElasticLinq.Test.TestSupport;
using Xunit;

namespace ElasticLinq.Test.Async
{
    public static class AsyncQueryableEntityTests
    {
        static readonly TestableElasticContext context = new TestableElasticContext();

        static AsyncQueryableEntityTests()
        {
            context.SetData<Robot>(RobotFactory.Inventory);
        }

        [Fact]
        public static async Task FirstAsyncReturnsSameResultAsFirst()
        {
            var expected = context.Query<Robot>().OrderBy(r => r.Started).First();
            var actual = await context.Query<Robot>().OrderBy(r => r.Started).FirstAsync().ConfigureAwait(false);

            Assert.Equal<object>(expected, actual);
        }

        [Fact]
        public static async Task FirstPredicateAsyncReturnsSameResultAsFirstPredicate()
        {
            var expected = context.Query<Robot>().First(r => r.Zone == 2);
            var actual = await context.Query<Robot>().FirstAsync(r => r.Zone == 2).ConfigureAwait(false);

            Assert.Equal<object>(expected, actual);
        }


        [Fact]
        public static async Task FirstOrDefaultAsyncReturnsSameResultAsFirstOrDefault()
        {
            var expected = context.Query<Robot>().OrderBy(r => r.Started).FirstOrDefault();
            var actual = await context.Query<Robot>().OrderBy(r => r.Started).FirstOrDefaultAsync().ConfigureAwait(false);

            Assert.Equal<object>(expected, actual);
        }

        [Fact]
        public static async Task FirstOrDefaultPredicateAsyncReturnsSameResultAsFirstOrDefaultPredicate()
        {
            var expected = context.Query<Robot>().FirstOrDefault(r => r.Zone == 2);
            var actual = await context.Query<Robot>().FirstOrDefaultAsync(r => r.Zone == 2).ConfigureAwait(false);

            Assert.Equal<object>(expected, actual);
        }

        [Fact]
        public static async Task SingleAsyncReturnsSameResultAsSingle()
        {
            var expected = context.Query<Robot>().Where(r => r.Id == 1).Single();
            var actual = await context.Query<Robot>().Where(r => r.Id == 1).SingleAsync().ConfigureAwait(false);

            Assert.Equal<object>(expected, actual);
        }

        [Fact]
        public static async Task SinglePredicateAsyncReturnsSameResultAsSinglePredicate()
        {
            var expected = context.Query<Robot>().Single(r => r.Id == 1);
            var actual = await context.Query<Robot>().SingleAsync(r => r.Id == 1).ConfigureAwait(false);

            Assert.Equal<object>(expected, actual);
        }


        [Fact]
        public static async Task SingleOrDefaultAsyncReturnsSameResultAsSingleOrDefault()
        {
            var expected = context.Query<Robot>().Where(r => r.Id == 1).SingleOrDefault();
            var actual = await context.Query<Robot>().Where(r => r.Id == 1).SingleOrDefaultAsync().ConfigureAwait(false);

            Assert.Equal<object>(expected, actual);
        }

        [Fact]
        public static async Task SingleOrDefaultPredicateAsyncReturnsSameResultAsSingleOrDefaultPredicate()
        {
            var expected = context.Query<Robot>().SingleOrDefault(r => r.Id == 1);
            var actual = await context.Query<Robot>().SingleOrDefaultAsync(r => r.Id == 1).ConfigureAwait(false);

            Assert.Equal<object>(expected, actual);
        }
   }
}