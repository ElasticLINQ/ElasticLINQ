// Copyright (c) Tier 3 Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 

using ElasticLinq.Response.Model;
using Newtonsoft.Json.Linq;
using System;
using System.Reflection;

namespace ElasticLinq.Mapping
{
    internal class ElasticFieldsMappingWrapper : IElasticMapping
    {
        private readonly IElasticMapping wrapped;

        public ElasticFieldsMappingWrapper(IElasticMapping wrapped)
        {
            this.wrapped = wrapped;
        }

        public string GetFieldName(MemberInfo memberInfo)
        {
            if (memberInfo.DeclaringType == typeof(ElasticFields))
                return ElasticFieldName(memberInfo);

            return wrapped.GetFieldName(memberInfo);
        }

        private static string ElasticFieldName(MemberInfo memberInfo)
        {
            switch (memberInfo.Name)
            {
                case "Score": return "_score";
                default:
                    throw new ArgumentOutOfRangeException("memberInfo", String.Format("Unknown member {0} on ElasticFields", memberInfo.Name));
            }
        }

        public string GetTypeName(Type type)
        {
            return wrapped.GetTypeName(type);
        }

        public JToken GetObjectSource(Type type, Hit hit)
        {
            return wrapped.GetObjectSource(type, hit);
        }
    }
}