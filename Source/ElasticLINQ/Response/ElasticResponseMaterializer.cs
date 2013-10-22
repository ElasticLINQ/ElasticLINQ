// Copyright (c) Tier 3 Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 

using ElasticLinq.Response.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace ElasticLinq.Response
{
    /// <summary>
    /// Materializes responses from ElasticSearch.
    /// </summary>
    public class ElasticResponseMaterializer
    {
        private static readonly MethodInfo materializer =
            typeof(ElasticResponseMaterializer).GetMethod("Materialize", BindingFlags.NonPublic | BindingFlags.Static);

        public object Materialize(IEnumerable<Hit> hits, Type elementType, Func<Hit, object> projector)
        {
            return materializer
                .MakeGenericMethod(elementType)
                .Invoke(this, new object[] { hits, projector });
        }

        internal static List<T> Materialize<T>(IEnumerable<Hit> hits, Func<Hit, T> projector)
        {
            return hits.Select(projector).Cast<T>().ToList();
        }
    }
}