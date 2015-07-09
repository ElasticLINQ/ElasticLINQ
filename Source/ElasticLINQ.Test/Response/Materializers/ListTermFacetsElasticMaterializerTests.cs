// Licensed under the Apache 2.0 License. See LICENSE.txt in the project root for more information.

using System.Collections.Generic;
using System.Linq;
using ElasticLinq.Response.Materializers;
using System;
using ElasticLinq.Response.Model;
using Newtonsoft.Json.Linq;
using Xunit;

namespace ElasticLinq.Test.Response.Materializers
{
    public class ListTermFacetsElasticMaterializerTests
    {
        static readonly Func<AggregateRow, object> defaultMaterializer = a => a;

        [Fact]
        public static void ConstructorSetsElementType()
        {
            var expectedElementType = typeof(SampleClass);

            var materializer = new ListTermFacetsElasticMaterializer(defaultMaterializer, expectedElementType,
                typeof(string));

            Assert.Same(expectedElementType, materializer.ElementType);
        }

        [Fact]
        public static void Constructor_GuardClauses()
        {
            Assert.Throws<ArgumentNullException>(() => new ListTermFacetsElasticMaterializer(null, typeof(SampleClass), typeof(string)));
            Assert.Throws<ArgumentNullException>(() => new ListTermFacetsElasticMaterializer(defaultMaterializer, null, typeof(string)));
            Assert.Throws<ArgumentNullException>(() => new ListTermFacetsElasticMaterializer(defaultMaterializer, typeof(SampleClass), null));
        }

        [Fact]
        public static void Materialize_GuardClauses()
        {
            var materializer = new ListTermFacetsElasticMaterializer(defaultMaterializer, typeof(SampleClass),
                typeof(string));

            Assert.Throws<ArgumentNullException>(() => materializer.Materialize(null));
        }

        [Fact]
        public static void MaterializeCreatesEmptyListIfFacetsIsNull()
        {
            var materializer = new ListTermFacetsElasticMaterializer(defaultMaterializer, typeof(SampleClass),
                typeof(string));

            var actual = materializer.Materialize(new ElasticResponse { facets = null });

            var actualList = Assert.IsType<List<SampleClass>>(actual);
            Assert.Empty(actualList);
        }

        [Fact]
        public static void MaterializeCreatesEmptyListIfFacetsIsEmpty()
        {
            var materializer = new ListTermFacetsElasticMaterializer(defaultMaterializer, typeof(SampleClass),
                typeof(string));

            var actual = materializer.Materialize(new ElasticResponse { facets = new JObject() });

            var actualList = Assert.IsType<List<SampleClass>>(actual);
            Assert.Empty(actualList);
        }

        [Fact]
        public static void ManyMaterializesJsonIntoAggregateFields()
        {
            // Materializer should be able to handle inefficient terms + terms_stats combination
            var facets = JObject.Parse(
                "{ \"GroupKey\": { \"_type\": \"terms\", \"terms\" : [ " +
                    "{ \"term\": \"suppliers/7\", \"count\": 5 }, " +
                    "{ \"term\": \"suppliers/8\", \"count\": 4 } ] }, " +
                " \"unitsInStock\": { \"_type\" : \"terms_stats\", \"terms\" : [ " +
                    "{ \"term\": \"suppliers/7\", \"count\": 5, \"max\": 42.0 }, " +
                    "{ \"term\": \"suppliers/8\", \"count\": 4, \"max\": 40.0 } ] } }");

            var materializer = new ListTermFacetsElasticMaterializer(defaultMaterializer, typeof(AggregateRow), typeof(string));

            var actual = materializer.Materialize(new ElasticResponse { facets = facets });

            var actualList = Assert.IsType<List<AggregateRow>>(actual);
            Assert.Equal(2, actualList.Count);
            Assert.All(actualList, a => Assert.IsType<AggregateTermRow>(a));

            var testItems = actualList.OfType<AggregateTermRow>().OrderBy(r => r.Key).ToArray();
            Assert.Equal("suppliers/7", testItems[0].Key);
            Assert.Equal(3, testItems[0].Fields.Count);
            Assert.Single(testItems[0].Fields, f => f.Name == "GroupKey" && f.Operation == "count" && f.Token.ToObject<int>() == 5);
            Assert.Single(testItems[0].Fields, f => f.Name == "unitsInStock" && f.Operation == "max" && Math.Abs(f.Token.ToObject<double>() - 42.0d) < 0.01);

            Assert.Equal("suppliers/8", testItems[1].Key);
            Assert.Equal(3, testItems[1].Fields.Count);
            Assert.Single(testItems[1].Fields, f => f.Name == "GroupKey" && f.Operation == "count" && f.Token.ToObject<int>() == 4);
            Assert.Single(testItems[1].Fields, f => f.Name == "unitsInStock" && f.Operation == "max" && Math.Abs(f.Token.ToObject<double>() - 40.0d) < 0.01);
        }

        [Fact]
        public static void CanMaterializeDateTimeKeys()
        {
            var facets = JObject.Parse(
                "{ \"GroupKey\": { \"_type\": \"terms\", \"terms\" : [ " +
                    "{ \"term\": 1428394929000, \"count\": 5 }, " +
                    "{ \"term\": 1428456720000, \"count\": 4 } ] } }");

            var materializer = new ListTermFacetsElasticMaterializer(defaultMaterializer, typeof(AggregateRow), typeof(DateTime));

            var actual = materializer.Materialize(new ElasticResponse { facets = facets });

            var actualList = Assert.IsType<List<AggregateRow>>(actual);
            Assert.Equal(2, actualList.Count);
            Assert.All(actualList, a => Assert.IsType<AggregateTermRow>(a));

            var testItems = actualList.OfType<AggregateTermRow>().OrderBy(r => r.Key).ToArray();
            Assert.Equal(new DateTime(2015,04,07,08,22,09, DateTimeKind.Utc), testItems[0].Key);
            Assert.Equal(1, testItems[0].Fields.Count);
            Assert.Single(testItems[0].Fields, f => f.Name == "GroupKey" && f.Operation == "count" && f.Token.ToObject<int>() == 5);

            Assert.Equal(new DateTime(2015, 04, 08, 01, 32, 00, DateTimeKind.Utc), testItems[1].Key);
            Assert.Equal(1, testItems[1].Fields.Count);
            Assert.Single(testItems[1].Fields, f => f.Name == "GroupKey" && f.Operation == "count" && f.Token.ToObject<int>() == 4);
        }
    }
}