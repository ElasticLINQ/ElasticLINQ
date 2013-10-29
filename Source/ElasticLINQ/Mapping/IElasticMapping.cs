// Copyright (c) Tier 3 Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 

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
    }
}