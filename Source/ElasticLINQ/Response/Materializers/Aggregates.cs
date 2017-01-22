// Licensed under the Apache 2.0 License. See LICENSE.txt in the project root for more information.

using ElasticLinq.Utility;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;

namespace ElasticLinq.Response.Materializers
{
    [DebuggerDisplay("Field {Name,nq}.{Operation,nq} = {Token}")]
    class AggregateField
    {
        public AggregateField(string name, string operation, JToken token)
        {
            Name = name;
            Operation = operation;
            Token = token;
        }

        public string Name { get; }

        public string Operation { get; }

        public JToken Token { get; }
    }

    abstract class AggregateRow
    {
        static readonly DateTime epocStartDateTime = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        static readonly DateTimeOffset epocStartDateTimeOffset = new DateTimeOffset(1970, 1, 1, 0, 0, 0, TimeSpan.Zero);

        internal static object GetValue(AggregateRow row, string name, string operation, Type valueType)
        {
            return row.GetValue(name, operation, valueType);
        }

        internal abstract object GetValue(string name, string operation, Type valueType);

        internal static object GetKey(AggregateRow row)
        {
            if (row is AggregateTermRow)
                return ((AggregateTermRow)row).Key;

            if (row is AggregateStatisticalRow)
                return ((AggregateStatisticalRow)row).Key;

            return null;
        }

        internal static object ParseValue(JToken token, Type valueType)
        {
            switch (token.ToString())
            {
                case "Infinity":
                case "∞":
                    {
                        if (valueType == typeof(double))
                            return double.PositiveInfinity;

                        if (valueType == typeof(float))
                            return float.PositiveInfinity;

                        if (valueType == typeof(decimal?))
                            return null;

                        break;
                    }

                case "-Infinity":
                case "-∞":
                    {
                        if (valueType == typeof(double))
                            return double.NegativeInfinity;

                        if (valueType == typeof(float))
                            return float.NegativeInfinity;

                        if (valueType == typeof(decimal?))
                            return null;

                        break;
                    }
            }

            // Elasticsearch returns dates as milliseconds since epoch when using facets
            if (token.Type == JTokenType.Float || token.Type == JTokenType.Integer)
            {
                if (valueType == typeof(DateTime))
                    return epocStartDateTime.AddMilliseconds((double)token);
                if (valueType == typeof(DateTimeOffset))
                    return epocStartDateTimeOffset.AddMilliseconds((double)token);
            }

            return token.ToObject(valueType);
        }
    }

    [DebuggerDisplay("Statistical Row")]
    class AggregateStatisticalRow : AggregateRow
    {
        readonly JObject facets;

        public AggregateStatisticalRow(object key, JObject facets)
        {
            Key = key;
            this.facets = facets;
        }

        internal override object GetValue(string name, string operation, Type valueType)
        {
            JToken facetObject, operationObject;
            return facets.TryGetValue(name, out facetObject)
                   && facetObject is JObject
                   && ((JObject)facetObject).TryGetValue(operation, out operationObject)
                ? ParseValue(operationObject, valueType)
                : TypeHelper.CreateDefault(valueType);
        }

        public object Key { get; }
    }

    [DebuggerDisplay("Term Row {Key} Fields({Fields.Count})")]
    class AggregateTermRow : AggregateRow
    {
        public AggregateTermRow(object key, IEnumerable<AggregateField> fields)
        {
            Key = key;
            Fields = new ReadOnlyCollection<AggregateField>(fields.ToArray());
        }

        public object Key { get; }

        public ReadOnlyCollection<AggregateField> Fields { get; }

        internal override object GetValue(string name, string operation, Type valueType)
        {
            var field = Fields.FirstOrDefault(f => f.Name == name && f.Operation == operation);

            return field == null
                ? TypeHelper.CreateDefault(valueType)
                : ParseValue(field.Token, valueType);
        }
    }
}