// Licensed under the Apache 2.0 License. See LICENSE.txt in the project root for more information.

using ElasticLinq.Response.Model;
using ElasticLinq.Utility;
using Newtonsoft.Json.Linq;
using System;
using System.Reflection;

namespace ElasticLinq.Mapping
{
    /// <summary>
    /// Default mapping for a common import of Couchbase documents.
    /// </summary>
    public class CouchbaseElasticMapping : IElasticMapping
    {
        private readonly string typeName;
        private readonly string fieldPrefix;

        public CouchbaseElasticMapping()
            : this("couchbaseDocument", false)
        {
        }

        public CouchbaseElasticMapping(string typeName, bool qualifyFieldNames)
        {
            Argument.EnsureNotBlank("typeName", typeName);

            this.typeName = typeName;
            fieldPrefix = qualifyFieldNames ? typeName + ".doc" : "doc";
        }        

        public string GetFieldName(MemberInfo memberInfo)
        {
            Argument.EnsureNotNull("memberInfo", memberInfo);

            return string.Join(".",
                fieldPrefix,
                GetDocTypeName(memberInfo.ReflectedType),
                memberInfo.Name.ToCamelCase());
        }

        public string GetTypeName(Type type)
        {
            return typeName;
        }

        public JToken GetObjectSource(Type docType, Hit hit)
        {
            Argument.EnsureNotNull("type", docType);
            Argument.EnsureNotNull("hit", hit);

            return hit._source["doc"][GetDocTypeName(docType)];
        }

        private static string GetDocTypeName(Type type)
        {
            return type.Name.ToCamelCase();
        }
    }
}