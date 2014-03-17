// Licensed under the Apache 2.0 License. See LICENSE.txt in the project root for more information.

using ElasticLinq.Request.Criteria;
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

        /// <inheritdoc/>
        public ElasticFieldsMappingWrapper(IElasticMapping wrapped)
        {
            this.wrapped = wrapped;
        }

        /// <inheritdoc/>
        public JToken FormatValue(MemberInfo member, object value)
        {
            return wrapped.FormatValue(member, value);
        }

        /// <inheritdoc/>
        public string GetDocumentMappingPrefix(Type type)
        {
            return wrapped.GetDocumentMappingPrefix(type);
        }

        /// <inheritdoc/>
        public string GetDocumentType(Type type)
        {
            return wrapped.GetDocumentType(type);
        }

        /// <inheritdoc/>
        public string GetFieldName(string prefix, MemberInfo memberInfo)
        {
            return
                memberInfo.DeclaringType == typeof(ElasticFields)
                    ? "_" + memberInfo.Name.ToLowerInvariant()
                    : wrapped.GetFieldName(prefix, memberInfo);
        }

        /// <inheritdoc/>
        public ICriteria GetTypeExistsCriteria(Type docType)
        {
            return wrapped.GetTypeExistsCriteria(docType);
        }
    }
}