// Licensed under the Apache 2.0 License. See LICENSE.txt in the project root for more information.

using ElasticLinq.Response.Materializers;
using ElasticLinq.Response.Model;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using Xunit;

namespace ElasticLinq.Test.Response.Materializers
{
    public class TermlessFacetsElasticMaterializerTests
    {
        private static readonly Func<AggregateRow, object> defaultMaterializer = a => a;

        [Fact]
        public static void ConstructorSetsElementType()
        {
            var expectedElementType = typeof(SampleClass);

            var materializer = new ListTermlessFacetsElasticMaterializer(defaultMaterializer, expectedElementType, "hello");

            Assert.Same(expectedElementType, materializer.ElementType);
        }

        [Fact]
        public static void Constructor_GuardClauses()
        {
            Assert.Throws<ArgumentNullException>(() => new TermlessFacetElasticMaterializer(null, typeof(SampleClass), ""));
            Assert.Throws<ArgumentNullException>(() => new TermlessFacetElasticMaterializer(defaultMaterializer, null, ""));
            Assert.DoesNotThrow(() => new TermlessFacetElasticMaterializer(defaultMaterializer, typeof(SampleClass), null));
        }

        [Fact]
        public static void Materialize_GuardClauses()
        {
            var materializer = new TermlessFacetElasticMaterializer(defaultMaterializer, typeof(SampleClass), "");

            Assert.Throws<ArgumentNullException>(() => materializer.Materialize(null));
        }

        [Fact]
        public static void MaterializeCreatesEmptyListIfFacetsIsNull()
        {
            var materializer = new TermlessFacetElasticMaterializer(defaultMaterializer, typeof(SampleClass), "");

            var actual = materializer.Materialize(new ElasticResponse { facets = null });

            Assert.Null(actual);
        }

        [Fact]
        public static void MaterializeCreatesDefaultValueIfFacetsIsEmpty()
        {
            var materializer = new TermlessFacetElasticMaterializer(defaultMaterializer, typeof(SampleClass), "");

            var actual = materializer.Materialize(new ElasticResponse { facets = new JObject() });

            Assert.Null(actual);
        }

        [Fact]
        public static void ManyMaterializesJsonIntoAggregateFields()
        {
            const string expectedKey = "some key";

            // Materializer should be able to handle inefficient filter + statistical combination
            var facets = JObject.Parse(
                "{ \"GroupKey\": { \"_type\": \"filter\", \"count\": 77 }," +
                " \"unitsInStock\": { \"_type\": \"statistical\", \"count\": 77, \"total\": 3119.0, \"max\": 125.0 } }");

            var materializer = new TermlessFacetElasticMaterializer(defaultMaterializer, typeof(AggregateRow), expectedKey);

            var actual = materializer.Materialize(new ElasticResponse { facets = facets });

            var statisticalRow = Assert.IsType<AggregateStatisticalRow>(actual);
            Assert.Equal(expectedKey, statisticalRow.Key);
            Assert.Equal(77, statisticalRow.GetValue("GroupKey", "count", typeof(int)));
            Assert.Equal(3119.0d, statisticalRow.GetValue("unitsInStock", "total", typeof(double)));
            Assert.Equal(125.0d, statisticalRow.GetValue("unitsInStock", "max", typeof(double)));
        }

        [Fact]
        public static void ManyMaterializesDefaultValueGivenNoValidFacets()
        {
            var facets = JObject.Parse(
                "{ \"GroupKey\": { \"_type\": \"term_filter\", \"count\": 77 }," +
                " \"unitsInStock\": { \"_type\": \"term_stats\", \"count\": 77, \"total\": 3119.0, \"max\": 125.0 } }");

            var materializer = new TermlessFacetElasticMaterializer(a => a.GetValue("", "", typeof(int)), typeof(int), null);

            var actual = materializer.Materialize(new ElasticResponse { facets = facets });

            Assert.Equal(default(int), actual);
        }
    }
}