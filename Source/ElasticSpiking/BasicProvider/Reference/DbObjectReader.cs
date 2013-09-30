using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using System.Reflection;

namespace ElasticSpiking.BasicProvider
{
    internal class DbObjectReader<T> : IEnumerable<T> where T : class, new()
    {
        Enumerator enumerator;

        internal DbObjectReader(DbDataReader reader)
        {
            enumerator = new Enumerator(reader);
        }

        public IEnumerator<T> GetEnumerator()
        {
            var e = enumerator;
            if (e == null)
                throw new InvalidOperationException("Cannot enumerate more than once");
            enumerator = null;
            return e;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        private class Enumerator : IEnumerator<T>
        {
            private readonly DbDataReader reader;
            private readonly FieldInfo[] fields;

            private int[] fieldLookup;
            private T current;

            internal Enumerator(DbDataReader reader)
            {
                this.reader = reader;
                fields = typeof(T).GetFields();
            }

            public T Current
            {
                get { return current; }
            }

            object IEnumerator.Current
            {
                get { return current; }
            }

            public bool MoveNext()
            {
                if (!reader.Read())
                    return false;

                fieldLookup = fieldLookup ?? BuildFieldLookup();

                var instance = new T();
                for (int i = 0, n = fields.Length; i < n; i++)
                {
                    var index = fieldLookup[i];
                    if (index < 0) continue;

                    var fi = fields[i];
                    fi.SetValue(instance, reader.IsDBNull(index) ? null : reader.GetValue(index));
                }

                current = instance;
                return true;
            }

            public void Reset()
            {
            }

            public void Dispose()
            {
                reader.Dispose();
            }

            private int[] BuildFieldLookup()
            {
                var map = new Dictionary<string, int>(StringComparer.InvariantCultureIgnoreCase);
                for (int i = 0, n = reader.FieldCount; i < n; i++)
                    map.Add(reader.GetName(i), i);

                var newFieldLookup = new int[fields.Length];

                for (int i = 0, n = fields.Length; i < n; i++)
                {
                    int index;
                    newFieldLookup[i] = map.TryGetValue(fields[i].Name, out index)
                        ? index
                        : -1;
                }

                return newFieldLookup;
            }
        }
    }
}