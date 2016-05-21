// Licensed under the Apache 2.0 License. See LICENSE.txt in the project root for more information.

using System;
using System.Linq;
using ElasticLinq.Utility;
using Xunit;

namespace ElasticLinq.Test.Utility
{
    public class ReadOnlyBatchedListTests
    {
        [Fact]
        public void Count_ReturnsExptectedCount()
        {
            var list = new ReadOnlyBatchedList<int>(Enumerable.Range(0, 100), 16);

            var count = list.Count;

            Assert.Equal(100, count);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(99)]
        [InlineData(16)]
        [InlineData(17)]
        public void Indexer_ReturnsCorrectItem(int index)
        {
            var list = new ReadOnlyBatchedList<int>(Enumerable.Range(0, 100), 16);

            var item = list[index];

            Assert.Equal(index, item);
        }

        [Theory]
        [InlineData(-1)]
        [InlineData(100)]
        public void Indexer_ThrowsArgumentOutOfRangeException_WhenIndexIsOutOfRange(int index)
        {
            var list = new ReadOnlyBatchedList<int>(Enumerable.Range(0, 100));

            var exception = Assert.Throws<ArgumentOutOfRangeException>(() =>
            {
                var temp = list[index];
            });
            Assert.Equal("index", exception.ParamName);
            Assert.Equal(index, exception.ActualValue);
        }

        [Fact]
        public void GetEnumerator_EnumeratorIsInitializedToDefaultValue()
        {
            var list = new ReadOnlyBatchedList<int>(Enumerable.Range(1, 100), 16);

            var enumerator = list.GetEnumerator();

            Assert.Equal(default(int), enumerator.Current);
        }

        [Fact]
        public void GetEnumerator_EnumeratorCanBeAdvancedToFirstElement()
        {
            var enumerator = new ReadOnlyBatchedList<int>(Enumerable.Range(1, 100), 16).GetEnumerator();

            var result = enumerator.MoveNext();

            Assert.True(result);
            Assert.Equal(1, enumerator.Current);
        }

        [Fact]
        public void GetEnumerator_AdvancedEnumeratorReturnsSameItem()
        {
            var enumerator = new ReadOnlyBatchedList<int>(Enumerable.Range(1, 100), 16).GetEnumerator();
            enumerator.MoveNext();

            var currentA = enumerator.Current;
            var currentB = enumerator.Current;

            Assert.Equal(currentA, currentB);
        }

        [Fact]
        public void GetEnumerator_CanBeAdvancedToLastItem()
        {
            var enumerator = new ReadOnlyBatchedList<int>(Enumerable.Range(1, 100), 16).GetEnumerator();
            for (int i = 0; i < 99; i++)
            {
                enumerator.MoveNext();
            }

            var result = enumerator.MoveNext();
            var current = enumerator.Current;

            Assert.Equal(100, current);
            Assert.True(result);
        }

        [Fact]
        public void GetEnumerator_EnumeratorAdvancedPastLastItemReturnsFalse()
        {
            var enumerator = new ReadOnlyBatchedList<int>(Enumerable.Range(1, 100), 16).GetEnumerator();
            for (int i = 0; i < 100; i++)
            {
                enumerator.MoveNext();
            }

            var result = enumerator.MoveNext();

            Assert.False(result);
        }
    }
}
