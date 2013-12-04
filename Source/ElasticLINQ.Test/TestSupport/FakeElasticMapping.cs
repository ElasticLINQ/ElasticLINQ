// Licensed under the Apache 2.0 License. See LICENSE.txt in the project root for more information.

using ElasticLinq.Mapping;
using ElasticLinq.Request.Criteria;
using ElasticLinq.Response.Model;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace ElasticLinq.Test.TestSupport
{
    public class FakeElasticMapping : IElasticMapping
    {
        private readonly List<MemberInfo> getFieldNames = new List<MemberInfo>();
        private readonly List<Type> getTypeNames = new List<Type>();
        private readonly List<Tuple<Type, Hit>> getObjectSources = new List<Tuple<Type, Hit>>(); 

        public string GetFieldName(MemberInfo memberInfo)
        {
            getFieldNames.Add(memberInfo);
            return memberInfo.Name;
        }

        public string GetTypeName(Type type)
        {
            getTypeNames.Add(type);
            return type.Name;
        }

        public JToken GetObjectSource(Type docType, Hit hit)
        {
            getObjectSources.Add(Tuple.Create(docType, hit));
            return new JObject(
                new JProperty("type", docType.Name),
                new JProperty("hit", hit._id));
        }

        public ICriteria GetTypeSelectionCriteria(Type docType)
        {
            return null;
        }

        public IReadOnlyList<MemberInfo> GetFieldNames
        {
            get { return getFieldNames.AsReadOnly(); }
        }

        public IReadOnlyList<Type> GetTypeNames
        {
            get { return getTypeNames.AsReadOnly(); }
        }

        public IReadOnlyList<Tuple<Type, Hit>> GetObjectSources
        {
            get { return getObjectSources.AsReadOnly(); }
        }
    }
}