using System;
using System.Collections.Generic;
using System.Linq;
using ElasticLinq.IntegrationTest.Models;
using Xunit;

namespace ElasticLinq.IntegrationTest
{
    public class GroupByTests
    {
        [Fact]
        public void GroupByInt()
        {
            DataAssert.Same((IQueryable<WebUser> q) => q.GroupBy(w => w.Id).Select(g => KeyValuePair.Create(g.Key, g.Count())), true);
        }

        [Fact]
        public void GroupByDateTime()
        {
            DataAssert.Same((IQueryable<WebUser> q) => q.GroupBy(w => w.Joined).Select(g => KeyValuePair.Create(g.Key, g.Count())), true);
        }
    }

    static class KeyValuePair
    {
        public static KeyValuePair<TKey, TValue> Create<TKey, TValue>(TKey key, TValue value) where TKey : IComparable<TKey>
        {
            return new KeyValuePair<TKey, TValue>(key, value);
        }
    }
}