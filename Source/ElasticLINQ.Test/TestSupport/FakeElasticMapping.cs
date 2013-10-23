// Copyright (c) Tier 3 Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 

using ElasticLinq.Mapping;
using ElasticLinq.Response.Model;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace ElasticLINQ.Test.TestSupport
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

        public JToken GetObjectSource(Type type, Hit hit)
        {
            getObjectSources.Add(Tuple.Create(type, hit));
            return new JObject(
                new JProperty("type", type.Name),
                new JProperty("hit", hit._id));
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