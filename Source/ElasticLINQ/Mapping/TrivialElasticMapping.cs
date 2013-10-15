// Copyright (c) Tier 3 Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 

using System;
using System.Reflection;
using ElasticLinq.Response.Model;
using Newtonsoft.Json.Linq;

namespace ElasticLinq.Mapping
{
    /// <summary>
    /// Trivial mapping implementation to just lower case everything and pluralize type names.
    /// </summary>
    public class TrivialElasticMapping : IElasticMapping
    {
        public string GetFieldName(MemberInfo memberInfo)
        {
            return MakeCamelCase(memberInfo.Name);
        }

        public string GetTypeName(Type type)
        {
            return Plualize(MakeCamelCase(type.Name));
        }

        public JToken GetObjectSource(Type type, Hit hit)
        {
            return hit._source;
        }

        private static string MakeCamelCase(string value)
        {
            if (value.Length < 2) // Don't camelcase or pluralize 1 letter
                return value.ToLower();

            return value.Substring(0, 1).ToLower() + value.Substring(1);
        }

        private static string Plualize(string value)
        {
            return value + (value.EndsWith("s") ? "" : "s");            
        }
    }
}