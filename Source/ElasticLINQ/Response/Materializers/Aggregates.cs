// Licensed under the Apache 2.0 License. See LICENSE.txt in the project root for more information.

using ElasticLinq.Utility;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ElasticLinq.Response.Materializers
{
    internal class AggregateColumn : IEquatable<AggregateColumn>
    {
        private readonly int hashCode;
        private readonly string name;
        private readonly string operation;

        public AggregateColumn(string name, string operation)
        {
            Argument.EnsureNotNull("name", name);
            Argument.EnsureNotNull("operation", operation);

            this.name = name;
            this.operation = operation;
            hashCode = 17 * (23 + name.GetHashCode()) * (23 + operation.GetHashCode());
        }

        public string Name { get { return name; } }
        public string Operation { get { return operation; } }

        public bool Equals(AggregateColumn other)
        {
            return other.hashCode == hashCode && other.name == name && other.Operation == operation;
        }

        public override bool Equals(object other)
        {
            return (other is AggregateColumn) && Equals((AggregateColumn)other);
        }

        public override int GetHashCode()
        {
            return hashCode;
        }
    }

    internal class AggregateField
    {
        private readonly AggregateColumn column;
        private readonly object value;

        public AggregateField(AggregateColumn column, object value)
        {
            this.column = column;
            this.value = value;
        }

        public AggregateColumn Column { get { return column; } }
        public object Value { get { return value; } }
    }

    internal class AggregateRow
    {
        private readonly object key;
        private readonly AggregateField[] fields;

        public AggregateRow(object key, IEnumerable<AggregateField> fields)
        {
            this.key = key;
            this.fields = fields.ToArray();
        }

        public object Key { get { return key; } }
        public IReadOnlyList<AggregateField> Fields { get { return fields; } }
    }
}
