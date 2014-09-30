namespace ElasticLinq.Communication.Requests
{
    using ElasticLinq.Communication.Attributes;

    internal class UpdateRequest
    {
        [ElasticRoute(Position = 0, Optional = false)]
        public string Index { get; set; }

        [ElasticRoute(Position = 1, Optional = false)]
        public string Type { get; set; }

        [ElasticRoute(Position = 2, Optional = false)]
        public string Id { get; set; }

        [ElasticRoute(Position = 3, Optional = false)]
        public string Update { get; set; }

        public UpdateRequest()
        {
            this.Update = "_update";
        }
    }
}
