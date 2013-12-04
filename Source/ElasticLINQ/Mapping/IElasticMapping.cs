// Licensed under the Apache 2.0 License. See LICENSE.txt in the project root for more information.

using ElasticLinq.Request.Criteria;
using ElasticLinq.Response.Model;
using Newtonsoft.Json.Linq;
using System;
using System.Reflection;

namespace ElasticLinq.Mapping
{
    /// <summary>
    /// Interface to describe how types and properties are mapped into ElasticSearch.
    /// </summary>
    public interface IElasticMapping
    {
        string GetFieldName(MemberInfo memberInfo);
        string GetTypeName(Type type);
        JToken GetObjectSource(Type docType, Hit hit);
        ICriteria GetTypeSelectionCriteria(Type docType);
    }
}