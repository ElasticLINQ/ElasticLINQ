// Licensed under the Apache 2.0 License. See LICENSE.txt in the project root for more information.

using System.Linq;
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
        public async static void FirstAsyncReturnsSameResultAsFirst()
        {
            var expected = context.Query<Robot>().OrderBy(r => r.Started).First();
            var actual = await context.Query<Robot>().OrderBy(r => r.Started).FirstAsync();

            Assert.Equal<object>(expected, actual);
        }

        [Fact]
        public async static void FirstPredicateAsyncReturnsSameResultAsFirstPredicate()
        {
            var expected = context.Query<Robot>().First(r => r.Zone == 2);
            var actual = await context.Query<Robot>().FirstAsync(r => r.Zone == 2);

            Assert.Equal<object>(expected, actual);
        }


        [Fact]
        public async static void FirstOrDefaultAsyncReturnsSameResultAsFirstOrDefault()
        {
            var expected = context.Query<Robot>().OrderBy(r => r.Started).FirstOrDefault();
            var actual = await context.Query<Robot>().OrderBy(r => r.Started).FirstOrDefaultAsync();

            Assert.Equal<object>(expected, actual);
        }

        [Fact]
        public async static void FirstOrDefaultPredicateAsyncReturnsSameResultAsFirstOrDefaultPredicate()
        {
            var expected = context.Query<Robot>().FirstOrDefault(r => r.Zone == 2);
            var actual = await context.Query<Robot>().FirstOrDefaultAsync(r => r.Zone == 2);

            Assert.Equal<object>(expected, actual);
        }

        [Fact]
        public async static void SingleAsyncReturnsSameResultAsSingle()
        {
            var expected = context.Query<Robot>().Where(r => r.Id == 1).Single();
            var actual = await context.Query<Robot>().Where(r => r.Id == 1).SingleAsync();

            Assert.Equal<object>(expected, actual);
        }

        [Fact]
        public async static void SinglePredicateAsyncReturnsSameResultAsSinglePredicate()
        {
            var expected = context.Query<Robot>().Single(r => r.Id == 1);
            var actual = await context.Query<Robot>().SingleAsync(r => r.Id == 1);

            Assert.Equal<object>(expected, actual);
        }


        [Fact]
        public async static void SingleOrDefaultAsyncReturnsSameResultAsSingleOrDefault()
        {
            var expected = context.Query<Robot>().Where(r => r.Id == 1).SingleOrDefault();
            var actual = await context.Query<Robot>().Where(r => r.Id == 1).SingleOrDefaultAsync();

            Assert.Equal<object>(expected, actual);
        }

        [Fact]
        public async static void SingleOrDefaultPredicateAsyncReturnsSameResultAsSingleOrDefaultPredicate()
        {
            var expected = context.Query<Robot>().SingleOrDefault(r => r.Id == 1);
            var actual = await context.Query<Robot>().SingleOrDefaultAsync(r => r.Id == 1);

            Assert.Equal<object>(expected, actual);
        }
   }
}