namespace ElasticLinq.Communication.Attributes
{
    using System;

    internal class ElasticRouteAttribute : Attribute
    {
        public int Position { get; set; }

        public bool Optional { get; set; }
    }
}
