// Copyright (c) Tier 3 Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 

using ElasticLinq.Response;
using ElasticLinq.Response.Model;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace ElasticLinq.Test.Response
{
    public class ElasticResponseMaterializerTests
    {
        [Fact]
        public void MaterializeOfTMaterializesHitsToT()
        {
            Func<Hit, SampleClass> materializer = h => new SampleClass { SampleField = (string)h.fields["someField"] };
            var hits = new[] { "first", "second", "third" }.Select(CreateHit).ToList();

            var materialized = ElasticResponseMaterializer.Materialize<SampleClass>(hits, materializer);

            Assert.Equal(hits.Count, materialized.Count);
            var index = 0;
            foreach (var hit in hits)
                Assert.Equal(hit.fields["someField"], materialized[index++].SampleField);
        }

        [Fact]
        public void MaterializeMaterializesHits()
        {
            Func<Hit, SampleClass> materializer = h => new SampleClass { SampleField = (string)h.fields["someField"] };
            var hits = new[] { "first", "second", "third" }.Select(CreateHit).ToList();

            var materialized = ElasticResponseMaterializer.Materialize(hits, typeof(SampleClass), materializer);

            Assert.IsType<List<SampleClass>>(materialized);

            var materializedList = (List<SampleClass>)materialized;

            Assert.Equal(hits.Count, materializedList.Count);
            var index = 0;
            foreach (var hit in hits)
                Assert.Equal(hit.fields["someField"], materializedList[index++].SampleField);
        }

        private static Hit CreateHit(string sampleField)
        {
            return new Hit { fields = new Dictionary<string, JToken> { { "someField", new JProperty("a","b").Value } } };
        }

        private class SampleClass
        {
            public string SampleField { get; set; }
        }
    }
}