// Licensed under the Apache 2.0 License. See LICENSE.txt in the project root for more information.

using System;
using ElasticLinq.Utility;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace ElasticLinq.Response.Materializers
{
    [DebuggerDisplay("Column {Name},{Operation} ")]
    internal class AggregateColumn
    {
        private readonly string name;
        private readonly string operation;

        public AggregateColumn(string name, string operation)
        {
            Argument.EnsureNotNull("name", name);
            Argument.EnsureNotNull("operation", operation);

            this.name = name;
            this.operation = operation;
        }

        public string Name { get { return name; } }
        public string Operation { get { return operation; } }
    }

    [DebuggerDisplay("Field {Column.Name},{Column.Operation} = {Token.Value}")]
    internal class AggregateField
    {
        private readonly AggregateColumn column;
        private readonly JToken token;

        public AggregateField(AggregateColumn column, JToken token)
        {
            this.column = column;
            this.token = token;
        }

        public AggregateColumn Column { get { return column; } }
        public JToken Token { get { return token; } }
    }

    [DebuggerDisplay("Row {Key} Fields({Fields.Count})")]
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

        internal static object GetValue(AggregateRow row, string name, string operation, Type type)
        {
            var field = row.Fields.FirstOrDefault(f => f.Column.Name == name && f.Column.Operation == operation);
            return field != null ? field.Token.ToObject(type) : TypeHelper.CreateDefault(type);
        }

        internal static object GetKey(AggregateRow row)
        {
            return row.Key.ToString();
        }
    }
}
