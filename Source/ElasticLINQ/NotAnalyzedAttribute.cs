// Licensed under the Apache 2.0 License. See LICENSE.txt in the project root for more information.

using System;

namespace ElasticLinq
{
    /// <summary>
    /// Used to mark serialized fields as being "not analayzed" in Elasticsearch
    /// (and therefore not subject to value transformations like lower-casing).
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public sealed class NotAnalyzedAttribute : Attribute { }
}
