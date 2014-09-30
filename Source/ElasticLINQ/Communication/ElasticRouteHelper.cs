namespace ElasticLinq.Communication
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using ElasticLinq.Communication.Attributes;

    internal static class ElasticRouteHelper
    {
        public static string GetRoute<TRequest>(TRequest request)
        {
            var routeProperties = typeof(TRequest).GetProperties().Select(x => new { PropertyInfo = x, Attribute = x.GetCustomAttributes<ElasticRouteAttribute>().SingleOrDefault() });

            var route = string.Join("/", routeProperties.Where(x => x.Attribute != null).OrderBy(x => x.Attribute.Position).Select(x => x.PropertyInfo.GetValue(request))/*.Where(x => x != null)*/);

            return route;
        }

        public static IDictionary<string, object> GetParam<TRequest>(TRequest request)
        {
            var paramProperties = typeof(TRequest).GetProperties().Select(x => new { PropertyInfo = x, Attribute = x.GetCustomAttributes<ElasticParameterAttribute>().SingleOrDefault() });

            return paramProperties.Where(x => x.Attribute != null).ToDictionary(x => x.Attribute.Name, x => x.PropertyInfo.GetValue(request));
        }
    }
}
