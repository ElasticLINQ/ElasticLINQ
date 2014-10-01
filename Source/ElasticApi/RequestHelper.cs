namespace ElasticApi
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using ElasticApi.Attributes;

    internal static class RequestHelper
    {
        public static object Param<T>(T value)
            where T : class
        {
            return value;
        }

        public static object Param<T>(Nullable<T> value)
            where T : struct
        {
            if (value.HasValue == false)
            {
                return null;
            }

            return value;
        }

        public static string Segment(string value)
        {
            return value;
        }

        public static string Segment(IEnumerable<string> value)
        {
            return string.Join(",", value);
        }

        public static void AddCommonParameters(IDictionary<string, object> parameters)
        {
            parameters.Add("pretty", true);
            parameters.Add("human", false);
        }

        public static object GetBody<TRequest>(TRequest request)
        {
            var bodyProperty = typeof(TRequest).GetProperties().SingleOrDefault(x => x.GetCustomAttributes<ApiBodyAttribute>().Any());

            if (bodyProperty == null)
            {
                return null;
            }

            return bodyProperty.GetValue(request);
        }
    }
}
