using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using ElasticApi.Attributes;

namespace ElasticApi
{
    internal static class RequestHelper
    {
        public static IEnumerable<string> GetPath<TRequest>(TRequest request)
        {
            var routeProperties = typeof(TRequest).GetProperties().Select(x => new { PropertyInfo = x, Attribute = x.GetCustomAttributes<ApiRouteAttribute>().SingleOrDefault() });

            return routeProperties.Where(x => x.Attribute != null).OrderBy(x => x.Attribute.Position).Select(x => x.PropertyInfo.GetValue(request)).Select(x => x != null ? x.ToString() : null);
        }

        public static IDictionary<string, object> GetParameters<TRequest>(TRequest request)
        {
            var paramProperties = typeof(TRequest).GetProperties().Select(x => new { PropertyInfo = x, Attribute = x.GetCustomAttributes<ApiParamAttribute>().SingleOrDefault() }).Where(x => x.Attribute != null);

            return paramProperties.Select(x => new { Value = x.PropertyInfo.GetValue(request), x.Attribute }).Where(x => x.Value != null).ToDictionary(x => x.Attribute.Name, x => x.Value);
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
