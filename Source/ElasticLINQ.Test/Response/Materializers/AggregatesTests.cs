using System;
using ElasticLinq.Response.Materializers;
using Newtonsoft.Json.Bson;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using Xunit;

namespace ElasticLinq.Test.Response.Materializers
{
    public class AggregatesTests
    {
        private const string ExpectedKey = "Wikkit";
        private static readonly List<AggregateField> expectedFields = new List<AggregateField>
        {
            new AggregateField("Green", "Mow", JToken.Parse("\"mower\"")),
            new AggregateField("Brown", "Water", JToken.Parse("\"5\"")),
        };

        private static readonly JObject expectedFacets = JObject.Parse(
            "{ " +
                " \"multi\": { \"string\": \"piece-of\", \"integer\": 19 }, " +
                " \"single\": { \"double\": 5.01 } " +
            "}");

        [Fact]
        public void AggregateFieldConstructorSetsProperties()
        {
            const string expectedName = "Kryten";
            const string expectedOperation = "Head Rotation";
            var expectedToken = JToken.Parse("{ \"scheduled\":1 }");

            var actual = new AggregateField(expectedName, expectedOperation, expectedToken);

            Assert.Same(expectedName, actual.Name);
            Assert.Same(expectedOperation, actual.Operation);
            Assert.Same(expectedToken, actual.Token);
        }

        [Fact]
        public void AggregateTermRowConstructorSetsProperties()
        {
            var actual = new AggregateTermRow(ExpectedKey, expectedFields);

            Assert.Same(ExpectedKey, actual.Key);
            Assert.Equal(expectedFields.ToArray(), actual.Fields);
        }

        [Fact]
        public void AggregateTermRowGetValueParsesStringValue()
        {
            var expected = expectedFields[0];
            var row = new AggregateTermRow(ExpectedKey, expectedFields);

            var actual = row.GetValue(expected.Name, expected.Operation, typeof(string));

            Assert.Equal("mower", actual);
        }

        [Fact]
        public void AggregateTermRowGetValueParsesIntValue()
        {
            var expected = expectedFields[1];
            var row = new AggregateTermRow(ExpectedKey, expectedFields);

            var actual = row.GetValue(expected.Name, expected.Operation, typeof(int));

            Assert.Equal(5, actual);
        }

        [Fact]
        public void AggregateTermRowGetValueDefaultsStringValue()
        {
            var row = new AggregateTermRow(ExpectedKey, expectedFields);

            var actual = row.GetValue("missing", "string", typeof(string));

            Assert.Equal(default(string), actual);
        }

        [Fact]
        public void AggregateTermRowGetValueDefaultsIntValue()
        {
            var row = new AggregateTermRow(ExpectedKey, expectedFields);

            var actual = row.GetValue("missing", "int", typeof(int));

            Assert.Equal(default(int), actual);
        }

        [Fact]
        public void AggregateStatisticalRowGetValueParsesMultipleFacetsForString()
        {
            var row = new AggregateStatisticalRow("", expectedFacets);

            var actual = row.GetValue("multi", "string", typeof(string));

            Assert.Equal("piece-of", actual);
        }

        [Fact]
        public void AggregateStatisticalRowGetValueParsesMultipleFacetsForInt()
        {
            var row = new AggregateStatisticalRow(1, expectedFacets);

            var actual = row.GetValue("multi", "integer", typeof(int));

            Assert.Equal(19, actual);
        }

        [Fact] public void AggregateStatisticalRowGetValueParsesSingleFacetForDouble()
        {
            var row = new AggregateStatisticalRow(2.00, expectedFacets);

            var actual = row.GetValue("single", "double", typeof(double));

            Assert.Equal(5.01, actual);
        }

        [Fact]
        public void AggregateStatisticalRowGetValueDefaultsStringValue()
        {
            var row = new AggregateStatisticalRow("3", expectedFacets);

            var actual = row.GetValue("missing", "string", typeof(string));

            Assert.Equal(default(string), actual);
        }

        [Fact]
        public void AggregateStatisticalRowGetValueDefaultsIntValue()
        {
            var row = new AggregateStatisticalRow(4, expectedFacets);

            var actual = row.GetValue("missing", "int", typeof(int));

            Assert.Equal(default(int), actual);
        }

        [Fact]
        public void AggregateRowGetKeyReturnsKeyForTermRow()
        {
            var row = new AggregateTermRow("rowKey", new[] { new AggregateField("field1", "eq", new JObject()) });

            var key = AggregateRow.GetKey(row);

            Assert.Equal(row.Key, key);
        }

        [Fact]
        public void AggregateRowGetKeyReturnsKeyForStatisticalRow()
        {
            var row = new AggregateStatisticalRow("statisticalKey", new JObject());

            var key = AggregateRow.GetKey(row);

            Assert.Equal(row.Key, key);
        }

        [Fact]
        public void AggregateRowGetKeyReturnsNullForUnknownTypes()
        {
            var key = AggregateRow.GetKey(null);

            Assert.Null(key);
        }

        [Fact]
        public void AggregateRowGetValueReturnsValueForTermRow()
        {
            var expectedName = "aField";
            var expectedOperation = "count";
            var expectedValue = "aValue";

            var row = new AggregateTermRow("rowKey", new[] { new AggregateField(expectedName, expectedOperation, new JValue(expectedValue) ) });

            var actual = AggregateRow.GetValue(row, expectedName, expectedOperation, expectedValue.GetType());

            Assert.Equal(expectedValue, actual);
        }

        [Fact]
        public void AggregateRowParseValueParsesPositiveInfinity()
        {
            var token = JToken.Parse("Infinity");

            Assert.Equal(Double.PositiveInfinity, AggregateRow.ParseValue(token, typeof(Double)));
            Assert.Equal(Single.PositiveInfinity, AggregateRow.ParseValue(token, typeof(Single)));
        }

        [Fact]
        public void AggregateRowParseValueParsesNegativeInfinity()
        {
            var token = JToken.Parse("-Infinity");

            Assert.Equal(Double.NegativeInfinity, AggregateRow.ParseValue(token, typeof(Double)));
            Assert.Equal(Single.NegativeInfinity, AggregateRow.ParseValue(token, typeof(Single)));
        }

        [Fact]
        public void AggregateRowParseValueParsesNullableDecimalInfinitiesAsNull()
        {
            Assert.Null(AggregateRow.ParseValue(JToken.Parse("Infinity"), typeof(Decimal?)));
            Assert.Null(AggregateRow.ParseValue(JToken.Parse("-Infinity"), typeof(Decimal?)));
        }

        [Fact]
        public void AggregateRowParseValueParsesStringInfinitiesAsStrings()
        {
            Assert.Equal("Infinity", AggregateRow.ParseValue(JToken.Parse("Infinity"), typeof(String)));
            Assert.Equal("-Infinity", AggregateRow.ParseValue(JToken.Parse("-Infinity"), typeof(String)));
        } 
    }
}