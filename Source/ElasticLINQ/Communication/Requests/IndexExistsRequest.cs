namespace ElasticLinq.Communication.Requests
{
    using ElasticLinq.Communication.Attributes;

    internal class IndexExistsRequest
    {
        [ElasticRoute(Position = 0, Optional = false)]
        public string Index { get; set; }
    }
}
