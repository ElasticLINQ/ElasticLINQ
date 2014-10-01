namespace ElasticApi.Attributes
{
    using System;

    public class ApiRouteAttribute : Attribute
    {
        public string Part { get; set; }

        public int Position { get; set; }

        public bool Required { get; set; }
    }
}
