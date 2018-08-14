// Licensed under the Apache 2.0 License. See LICENSE.txt in the project root for more information.

using ElasticLinq.Response.Model;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace ElasticLinq.Test.Response.Materializers
{
    public static class MaterializerTestHelper
    {
        public static List<Hit> CreateSampleHits(int count)
        {
            return Enumerable.Range(0, count).Select(i => i.ToString(CultureInfo.InvariantCulture)).Select(CreateHit).ToList();
        }

        public static List<Hit> CreateSampleHitsWithHighlight(int count)
        {
            return Enumerable.Range(0, count).Select(i => i.ToString(CultureInfo.InvariantCulture)).Select(CreateHitWithHighlight).ToList();
        }

        public static readonly Func<Hit, SampleClass> ItemCreator =
            h => new SampleClass { SampleField = (string)h._source["someField"] };

        internal static ElasticResponse CreateSampleResponse(int count)
        {
            return new ElasticResponse { hits = new Hits { hits = CreateSampleHits(count), total = count } };
        }

        internal static ElasticResponse CreateSampleResponseWithHighlight(int count)
        {
            return new ElasticResponse { hits = new Hits { hits = CreateSampleHitsWithHighlight(count), total = count } };
        }

        public static Hit CreateHit(string sampleField)
        {
            return new Hit
            {
                _source = new JObject(new Dictionary<string, JToken> { { "someField", new JProperty("a", sampleField).Value } })
            };
        }

        public static Hit CreateHitWithHighlight(string sampleField)
        {
            return new Hit
            {
                _source = new JObject(new JProperty("someField", new JProperty("a", sampleField).Value)),
                highlight = new JObject(new JProperty("sampleField", new JArray("a", "b")))
            };
        }
    }

    public class SampleClass
    {
        public string SampleField { get; set; }
    }
}