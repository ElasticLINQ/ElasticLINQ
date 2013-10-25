// Copyright (c) Tier 3 Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 

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

        public CouchbaseElasticMapping()
        {
            typeName = "couchbaseDocument";
        }

        public CouchbaseElasticMapping(string typeName)
        {
            this.typeName = typeName;
        }        

        public string GetFieldName(MemberInfo memberInfo)
        {
            Argument.EnsureNotNull("memberInfo", memberInfo);

            return string.Join(".",
                "doc",
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