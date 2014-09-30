namespace ElasticLinq.Communication.Requests
{
    using ElasticLinq.Communication.Attributes;

    internal class PostRequest
    {
        [ElasticRoute(Position = 0, Optional = false)]
        public string Index { get; set; }

        [ElasticRoute(Position = 1, Optional = false)]
        public string Type { get; set; }
    }
}
