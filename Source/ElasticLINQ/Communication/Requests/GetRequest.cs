namespace ElasticLinq.Communication.Requests
{
    using System;
    using ElasticLinq.Communication.Attributes;

    internal class GetRequest
    {
        [ElasticRoute(Position = 0, Optional = false)]
        public string Index { get; set; }

        [ElasticRoute(Position = 1, Optional = false)]
        public string Type { get; set; }

        [ElasticRoute(Position = 2, Optional = false)]
        public string Id { get; set; }

        [ElasticParameter(Name = "realtime")]
        public bool Realtime { get; set; }

        [ElasticParameter(Name = "_source")]
        public bool Source { get; set; }

        [ElasticParameter(Name = "refresh")]
        public bool Refresh { get; set; }

        [ElasticParameter(Name = "version")]
        public Nullable<int> Version { get; set; }

        public GetRequest()
        {
            this.Realtime = false;
            this.Source = false;
            this.Refresh = false;
        }
    }
}
