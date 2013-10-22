// Copyright (c) Tier 3 Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 

using ElasticLinq.Response;
using ElasticLinq.Response.Model;
using Newtonsoft.Json.Linq;
using System;
using System.Linq;
using Xunit;

namespace ElasticLINQ.Test.Response
{
    public class ElasticResponseMaterializerTests
    {
        [Fact]
        public void MaterializeOfTMaterializesHitsToT()
        {
            Func<Hit, SampleClass> materializer = h => new SampleClass { SampleField = (string)h.fields["someField"] };
            var hits = new[] { "first", "second", "third" }.Select(CreateHit).ToList();

            var materialized = ElasticResponseMaterializer.Materialize(hits, materializer);

            Assert.Equal(hits.Count, materialized.Count);
            var index = 0;
            foreach (var hit in hits)
                Assert.Equal(hit.fields["someField"], materialized[index++].SampleField);
        }

        private static Hit CreateHit(string sampleField)
        {
            return new Hit { fields = new JObject(new JProperty("someField", sampleField)) };
        }

        class SampleClass
        {
            public string SampleField { get; set; }
        }
    }
}