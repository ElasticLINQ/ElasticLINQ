// Licensed under the Apache 2.0 License. See LICENSE.txt in the project root for more information.

using Newtonsoft.Json;
using System;
using System.Linq;
using Xunit;

namespace ElasticLinq.Test
{
    public class JsonNetTests
    {
        [Fact]
        public static void CustomTypes_Term()
        {
            var context = new TestableElasticContext();
            var helloIdentifier = new Identifier("hello");

            var query = context.Query<ClassWithIdentifer>().Where(x => x.id == helloIdentifier).ToElasticSearchQuery();

            Assert.Equal(@"{""filter"":{""term"":{""id"":""hello!!""}}}", query);
        }

        [Fact]
        public static void CustomTypes_Terms()
        {
            var context = new TestableElasticContext();
            var identifiers = new[] { new Identifier("value1"), new Identifier("value2") };

            var query = context.Query<ClassWithIdentifer>().Where(x => identifiers.Contains(x.id)).ToElasticSearchQuery();

            Assert.Equal(@"{""filter"":{""terms"":{""id"":[""value1!!"",""value2!!""]}}}", query);
        }

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

        class ClassWithIdentifer
        {
            public Identifier id { get; set; }
        }
    }
}
