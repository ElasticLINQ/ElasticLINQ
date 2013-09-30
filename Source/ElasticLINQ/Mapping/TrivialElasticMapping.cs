// Copyright (c) Tier 3 Inc. All rights reserved.
// This source code is made available under the terms of the Microsoft Public License (MS-PL)

using System;
using System.Reflection;

namespace ElasticLinq.Mapping
{
    /// <summary>
    /// Trivial mapping implementation to just lower case everything and pluralize type names.
    /// </summary>
    public class TrivialElasticMapping : IElasticMapping
    {
        public string GetFieldName(MemberInfo memberInfo)
        {
            return memberInfo.Name.ToLowerInvariant();
        }

        public string GetTypeName(Type type)
        {
            // TODO: Implement better pluralizer
            var lowered = type.Name.ToLower();
            if (!lowered.EndsWith("s"))
                lowered += "s";
            return lowered;
        }
    }
}