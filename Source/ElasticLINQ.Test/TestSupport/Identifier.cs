// Licensed under the Apache 2.0 License. See LICENSE.txt in the project root for more information.

using Newtonsoft.Json;
using System;

namespace ElasticLinq.Test.TestSupport
{
    [JsonConverter(typeof(IdentifierJsonConverter))]
    class Identifier
    {
        private readonly string value;

        public Identifier(string value)
        {
            this.value = value;
        }

        public override string ToString()
        {
            return value;
        }

        class IdentifierJsonConverter : JsonConverter
        {
            public override bool CanConvert(Type objectType)
            {
                return objectType == typeof(Identifier);
            }

            public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
            {
                if (reader.Value == null)
                    return null;

                return new Identifier(reader.Value.ToString());
            }

            public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
            {
                writer.WriteValue(value + "!!");
            }
        }
    }
}
