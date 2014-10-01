namespace ElasticApi.Attributes
{
    using System;

    internal class ApiParamAttribute : Attribute
    {
        public string Name { get; set; }
    }
}
