// Licensed under the Apache 2.0 License. See LICENSE.txt in the project root for more information.

using ElasticLinq.Response.Model;
using Newtonsoft.Json.Linq;
using System;
using System.Reflection;

namespace ElasticLinq.Mapping
{
    /// <summary>
    /// Wraps an elastic mapping with one that also handles the built-in
    /// ElasticFields class that contains properties for _score etc.
    /// </summary>
    internal class ElasticFieldsMappingWrapper : IElasticMapping
    {
        private readonly IElasticMapping wrapped;

        public ElasticFieldsMappingWrapper(IElasticMapping wrapped)
        {
            this.wrapped = wrapped;
        }

        public string GetFieldName(MemberInfo memberInfo)
        {
            return memberInfo.DeclaringType == typeof(ElasticFields)
                ? ElasticFieldName(memberInfo) 
                : wrapped.GetFieldName(memberInfo);
        }

        private static string ElasticFieldName(MemberInfo memberInfo)
        {
            return "_" + memberInfo.Name.ToLowerInvariant();
        }

        public string GetTypeName(Type type)
        {
            return wrapped.GetTypeName(type);
        }

        public JToken GetObjectSource(Type docType, Hit hit)
        {
            return wrapped.GetObjectSource(docType, hit);
        }
    }
}