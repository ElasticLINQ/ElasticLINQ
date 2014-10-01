namespace ElasticLinq.Communication.Responses
{
    using System;
    using System.Collections.Generic;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;

    public class GetResponse
    {
        [JsonProperty("_index")]
        public string Index { get; set; }

        [JsonProperty("_type")]
        public string Type { get; set; }

        [JsonProperty("_id")]
        public string Id { get; set; }

        [JsonProperty("_version")]
        public long Version { get; set; }

        [JsonProperty("found")]
        public bool Found { get; set; }

        [JsonProperty("_source")]
        public JObject Source { get; set; }

        //[JsonProperty("found")]
        //public Dictionary<String, JToken> Fields { get; set; }
    }
}
