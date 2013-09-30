// Copyright (c) Tier 3 Inc. All rights reserved.
// This source code is made available under the terms of the Microsoft Public License (MS-PL)

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
    }
}