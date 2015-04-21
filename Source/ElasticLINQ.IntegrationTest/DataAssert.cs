using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Xunit;

namespace ElasticLINQ.IntegrationTest
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
            var expect = query(Data.Memory<TSource>()).ToArray();
            var actual = query(Data.Elastic<TSource>()).ToArray();

            var upperBound = Math.Min(expect.Length, actual.Length);
            for (var i = 0; i < upperBound; i++)
                Assert.Equal(expect[i], actual[i]);
            Assert.Equal(expect.Length, actual.Length);
        }

        private static IEnumerable<T> Difference<T>(IEnumerable<T> left, IEnumerable<T> right)
        {
            var rightCache = new HashSet<T>(right);
            rightCache.SymmetricExceptWith(left);
            return rightCache;
        } 
    }
}