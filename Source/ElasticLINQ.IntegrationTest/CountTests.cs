// Licensed under the Apache 2.0 License. See LICENSE.txt in the project root for more information.

using System.Collections.Generic;
using System.Linq;
using ElasticLinq.IntegrationTest.Models;
using Xunit;

namespace ElasticLinq.IntegrationTest
{
    public class CountTests
    {
        static readonly Data data = new Data();

        static IQueryable<JobOpening> ElasticJobs { get; }

        static List<JobOpening> MemoryJobs { get; }

        static long MidPoint { get; }

        static CountTests()
        {
            ElasticJobs = data.Elastic<JobOpening>();
            data.LoadMemoryFromElastic();
            MemoryJobs = data.Memory<JobOpening>().ToList();
            MidPoint = MemoryJobs.OrderBy(o => o.Id).Skip(MemoryJobs.Count / 2).First().Id;
        }

        [Fact]
        public void Count()
        {
            Assert.Equal(ElasticJobs.Count(), MemoryJobs.Count());
        }

        [Fact]
        public void LongCount()
        {
            Assert.Equal(ElasticJobs.LongCount(), MemoryJobs.LongCount());
        }

        [Fact]
        public void CountPredicate()
        {
            Assert.Equal(
                data.Elastic<JobOpening>().Count(j => j.Id > MidPoint),
                data.Memory<JobOpening>().Count(j => j.Id > MidPoint));
        }

        [Fact]
        public void LongCountPredicate()
        {
            Assert.Equal(
                data.Elastic<JobOpening>().LongCount(j => j.Id > MidPoint),
                data.Memory<JobOpening>().LongCount(j => j.Id > MidPoint));
        }

        [Fact]
        public void WhereCount()
        {
            var midPoint = MemoryJobs.OrderBy(o => o.Id).Skip(MemoryJobs.Count / 2).First().Id;
            Assert.Equal(
                data.Elastic<JobOpening>().Where(j => j.Id > midPoint).Count(),
                data.Memory<JobOpening>().Where(j => j.Id > midPoint).Count());
        }

        [Fact]
        public void WhereLongCount()
        {
            var midPoint = MemoryJobs.OrderBy(o => o.Id).Skip(MemoryJobs.Count / 2).First().Id;
            Assert.Equal(
                data.Elastic<JobOpening>().Where(j => j.Id > midPoint).LongCount(),
                data.Memory<JobOpening>().Where(j => j.Id > midPoint).LongCount());
        }

        [Fact]
        public void QueryCount()
        {
            Assert.Equal(
                data.Elastic<JobOpening>().Query(j => j.JobTitle.Contains("a")).Count(),
                data.Memory<JobOpening>().Where(j => j.JobTitle.ToLowerInvariant().Contains("a")).Count());
        }

        [Fact]
        public void QueryLongCount()
        {
            Assert.Equal(
                data.Elastic<JobOpening>().Query(j => j.JobTitle.Contains("a")).LongCount(),
                data.Memory<JobOpening>().Where(j => j.JobTitle.ToLowerInvariant().Contains("a")).LongCount());
        }

        [Fact]
        public void QueryCountPredicate()
        {
            Assert.Equal(
                data.Elastic<JobOpening>().Query(j => j.JobTitle.Contains("a")).Count(j => j.Id > MidPoint),
                data.Memory<JobOpening>().Where(j => j.JobTitle.ToLowerInvariant().Contains("a")).Count(j => j.Id > MidPoint));
        }

        [Fact]
        public void QueryLongCountPredicate()
        {
            Assert.Equal(
                data.Elastic<JobOpening>().Query(j => j.JobTitle.Contains("a")).LongCount(j => j.Id > MidPoint),
                data.Memory<JobOpening>().Where(j => j.JobTitle.ToLowerInvariant().Contains("a")).LongCount(j => j.Id > MidPoint));
        }
    }
}
