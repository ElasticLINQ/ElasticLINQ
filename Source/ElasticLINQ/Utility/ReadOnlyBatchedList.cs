// Licensed under the Apache 2.0 License. See LICENSE.txt in the project root for more information.

using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;

namespace ElasticLinq.Utility
{
    [DebuggerDisplay("Count = {" + nameof(Count) + "}")]
    [DebuggerTypeProxy(typeof(ReadOnlyBatchedList<>.BatchedListDebugView))]
    internal class ReadOnlyBatchedList<T> : IReadOnlyList<T>
    {
        // 8192 * 8-byte (i.e. 64-bit) pointers = 65536 bytes, which is below the 85000 byte large object heap threshold. We could have a higher limit when running in 32-bit processes, but that would complicate the logic
        private const int DefaultMaxBatchCapacity = 8192;

        readonly int maxBatchCapacity;
        readonly IReadOnlyList<IReadOnlyList<T>> batches;

        public ReadOnlyBatchedList(IEnumerable<T> enumerable) : this(enumerable, DefaultMaxBatchCapacity)
        {
        }

        internal ReadOnlyBatchedList(IEnumerable<T> enumerable, int maxBatchCapacity)
        {
            if (enumerable == null)
                throw new ArgumentNullException(nameof(enumerable));
            if (maxBatchCapacity < 1)
                throw new ArgumentOutOfRangeException(nameof(maxBatchCapacity), maxBatchCapacity, "The maximum capacity of a batch must be a positive number.");

            this.maxBatchCapacity = maxBatchCapacity;

            int? totalCount = null;
            var collection = enumerable as ICollection<T>;
            if (collection != null)
                totalCount = collection.Count;

            var batches = new List<IReadOnlyList<T>>();

            var currentTotalCount = 0;
            List<T> currentBatch = null;
            foreach (var item in enumerable)
            {
                if (currentBatch == null || currentBatch.Count == maxBatchCapacity)
                {
                    var capacity = 0;
                    if (totalCount.HasValue)
                    {
                        var remainingCount = totalCount.Value - currentTotalCount;
                        capacity = Math.Min(remainingCount, maxBatchCapacity);
                    }
                    currentBatch = new List<T>(capacity);
                    batches.Add(currentBatch);
                }

                currentBatch.Add(item);
                currentTotalCount++;
            }

            this.batches = batches;
            Count = currentTotalCount;
        }

        public T this[int index]
        {
            get
            {
                if (index < 0 || index >= Count)
                    throw new ArgumentOutOfRangeException(nameof(index), index, "Index was out of range. Must be non-negative and less than the size of the collection.");

                var batchNumber = index / maxBatchCapacity;
                var batchIndex = index % maxBatchCapacity;
                return batches[batchNumber][batchIndex];
            }
        }

        public int Count { get; }

        public IEnumerator<T> GetEnumerator()
        {
            foreach (var batch in batches)
                foreach (var item in batch)
                    yield return item;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        private class BatchedListDebugView
        {
            readonly ReadOnlyBatchedList<T> list;

            public BatchedListDebugView(ReadOnlyBatchedList<T> list)
            {
                this.list = list;
            }

            [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
            public T[] Items => list.ToArray();
        }
    }
}