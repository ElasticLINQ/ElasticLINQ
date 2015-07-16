// Licensed under the Apache 2.0 License. See LICENSE.txt in the project root for more information.

using ElasticLinq.Async;
using ElasticLinq.IntegrationTest.Models;
using System.Linq;
using Xunit;

namespace ElasticLinq.IntegrationTest
{
    public class AsyncTests
    {
        [Fact]
        public async void ToListAsyncReturnsCorrectResults()
        {
            var memory = DataAssert.Data.Memory<WebUser>().Where(w => w.Id > 80).OrderBy(w => w.Id).ToList();
            var elastic = await DataAssert.Data.Elastic<WebUser>().Where(w => w.Id > 80).OrderBy(w => w.Id).ToListAsync();

            DataAssert.SameSequence(memory, elastic);
        }

        [Fact]
        public async void ToDictionaryAsyncReturnsCorrectResults()
        {
            var memory = DataAssert.Data.Memory<WebUser>().Where(w => w.Id > 75).ToDictionary(k => k.Id);
            var elastic = await DataAssert.Data.Elastic<WebUser>().Where(w => w.Id > 75).ToDictionaryAsync(k => k.Id);

            Assert.Equal(memory.Count, elastic.Count);
            foreach (var kvp in memory)
                Assert.Equal(elastic[kvp.Key], kvp.Value);
        }

        [Fact]
        public async void FirstAsyncReturnsCorrectResult()
        {
            var memory = DataAssert.Data.Memory<WebUser>().Where(w => w.Id > 80).First();
            var elastic = await DataAssert.Data.Elastic<WebUser>().Where(w => w.Id > 80).FirstAsync();

            Assert.Equal(memory, elastic);
        }

        [Fact]
        public async void FirstOrDefaultAsyncWithPredicateReturnsCorrectResult()
        {
            var memory = DataAssert.Data.Memory<WebUser>().FirstOrDefault(w => w.Id == 34);
            var elastic = await DataAssert.Data.Elastic<WebUser>().FirstOrDefaultAsync(w => w.Id == 34);

            Assert.Equal(memory, elastic);
        }
    }
}