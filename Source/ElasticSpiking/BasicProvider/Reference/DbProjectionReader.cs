using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Common;

namespace ElasticSpiking.BasicProvider.Reference
{
    internal class DbProjectionReader<T>
    {
        private Enumerator enumerator;

        internal DbProjectionReader(DbDataReader reader, Func<ProjectionRow, T> projector)
        {
            enumerator = new Enumerator(reader, projector);
        }

        public IEnumerator<T> GetEnumerator()
        {
            var e = enumerator;
            if (e == null)
                throw new InvalidOperationException("Cannot enumerate more than once");
            enumerator = null;
            return e;
        }

        private class Enumerator : ProjectionRow, IEnumerator<T>
        {
            private readonly DbDataReader reader;
            private readonly Func<ProjectionRow, T> projector;
            private T current;

            internal Enumerator(DbDataReader reader, Func<ProjectionRow, T> projector)
            {
                this.reader = reader;
                this.projector = projector;
            }

            public override object GetValue(int index)
            {
                if (index < 0)
                    throw new ArgumentOutOfRangeException("index");

                return reader.IsDBNull(index)
                    ? null
                    : reader.GetValue(index);
            }

            public T Current { get { return current; } }

            object IEnumerator.Current { get { return current; }}

            public bool MoveNext()
            {
                if (reader.Read())
                {
                    current = projector(this);
                    return true;
                }
                return false;
            }

            public void Reset()
            {
            }

            public void Dispose()
            {
                reader.Dispose();
            }
        }
    }
}