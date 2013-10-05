// Copyright (c) Tier 3 Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 

using ElasticLinq.Response.Model;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace ElasticLinq.Response
{
    /// <summary>
    /// Materializes responses from ElasticSearch.
    /// </summary>
    public class ElasticResponseMaterializer
    {
        private static readonly MethodInfo materializer = typeof(ElasticResponseMaterializer)
            .GetMethod("Materialize", BindingFlags.NonPublic | BindingFlags.Static);

        public object Materialize(ElasticResponse elasticResponse, Type elementType, LambdaExpression projectionExpression)
        {
            var projector = projectionExpression == null ? null : projectionExpression.Compile();

            return materializer
                .MakeGenericMethod(elementType)
                .Invoke(this, new object[] { elasticResponse, projector });
        }

        internal static List<T> Materialize<T>(ElasticResponse response, Func<JObject, T> projector)
        {
            Func<Hit, T> func;
            if (projector == null)
                func = h => h._source.ToObject<T>();
            else
                func = h => projector(h.fields);           

            var materialized = response
                .hits.hits
                .Select(func)
                .ToList();

            return materialized;
        }
    }
}