using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace ElasticLinq.IntegrationTest
{
    static class DataAssert
    {
        public static readonly Data Data = new Data();

        static DataAssert()
        {
            Data.LoadMemoryFromElastic();
        }

        public static void Same<TSource>(Func<IQueryable<TSource>, IQueryable<TSource>> query, bool ignoreOrder = false)
        {
            Same<TSource, TSource>(query, ignoreOrder);
        }

        public static void Same<TSource, TTarget>(Func<IQueryable<TSource>, IQueryable<TTarget>> query, bool ignoreOrder = false)
        {
            var expect = query(Data.Memory<TSource>()).ToList();
            var actual = query(Data.Elastic<TSource>()).ToList();

            if (ignoreOrder)
            {
                var difference = Difference(expect, actual);
                Assert.Empty(difference);
            }
            else
                SameSequence(expect, actual);
        }

        public static void SameSequence<TTarget>(List<TTarget> expect, List<TTarget> actual)
        {
            var upperBound = Math.Min(expect.Count, actual.Count);
            for (var i = 0; i < upperBound; i++)
                Assert.Equal(expect[i], actual[i]);

            Assert.Equal(expect.Count, actual.Count);
        }

        static IEnumerable<T> Difference<T>(IEnumerable<T> left, IEnumerable<T> right)
        {
            var rightCache = new HashSet<T>(right);
            rightCache.SymmetricExceptWith(left);
            return rightCache;
        } 
    }
}