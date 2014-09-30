namespace ElasticLinq.Communication.Responses
{
    using Newtonsoft.Json;

    internal class IndexResponse
    {
        [JsonProperty("_index")]
        public string Index { get; set; }

        [JsonProperty("_type")]
        public string Type { get; set; }

        [JsonProperty("_id")]
        public string Id { get; set; }

        [JsonProperty("_version")]
        public long Version { get; set; }

        [JsonProperty("created")]
        public bool Created { get; set; }
    }
}
