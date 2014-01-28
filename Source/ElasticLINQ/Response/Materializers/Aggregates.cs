// Licensed under the Apache 2.0 License. See LICENSE.txt in the project root for more information.

using ElasticLinq.Utility;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace ElasticLinq.Response.Materializers
{
    [DebuggerDisplay("Field {Name}.{Operation} = {Token.Value}")]
    internal class AggregateField
    {
        private readonly string name;
        private readonly string operation;
        private readonly JToken token;

        public AggregateField(string name, string operation, JToken token)
        {
            this.name = name;
            this.operation = operation;
            this.token = token;
        }

        public string Name { get { return name; } }
        public string Operation { get { return operation; } }
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

        internal static object GetValue(AggregateRow row, string name, string operation, Type valueType)
        {
            var field = row.Fields.FirstOrDefault(f => f.Name == name && f.Operation == operation);

            return field == null
                ? TypeHelper.CreateDefault(valueType)
                : ParseValue(field.Token, valueType);
        }

        internal static object GetKey(AggregateRow row)
        {
            return row.Key.ToString();
        }

        internal static object ParseValue(JToken token, Type valueType)
        {
            switch (token.ToString())
            {
                case "Infinity":
                    {
                        if (valueType == typeof(Double))
                            return Double.PositiveInfinity;

                        if (valueType == typeof(Single))
                            return Single.PositiveInfinity;

                        if (valueType == typeof(decimal?))
                            return null;

                        break;
                    }

                case "-Infinity":
                    {
                        if (valueType == typeof(Double))
                            return Double.NegativeInfinity;

                        if (valueType == typeof(Single))
                            return Single.NegativeInfinity;

                        if (valueType == typeof(decimal?))
                            return null;

                        break;
                    }
            }

            return token.ToObject(valueType);
        }
    }
}
