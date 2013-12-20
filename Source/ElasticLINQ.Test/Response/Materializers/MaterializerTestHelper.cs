// Licensed under the Apache 2.0 License. See LICENSE.txt in the project root for more information.

using ElasticLinq.Response.Model;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ElasticLinq.Test.Response.Materializers
{
    public static class MaterializerTestHelper
    {
        public static List<Hit> CreateSampleHits(int count)
        {
            return Enumerable.Range(0, count).Select(i => i.ToString()).Select(CreateHit).ToList();
        }

        public static readonly Func<Hit, SampleClass> ItemCreator =
            h => new SampleClass { SampleField = (string)h.fields["someField"] };

        public static ElasticResponse CreateSampleResponse(int count)
        {
            return new ElasticResponse { hits = new Hits { hits = CreateSampleHits(count) } };
        }

        public static Hit CreateHit(string sampleField)
        {
            return new Hit
            {
                fields = new Dictionary<string, JToken> { { "someField", new JProperty("a", sampleField).Value } }
            };
        }
    }

    public class SampleClass
    {
        public string SampleField { get; set; }
    }
}