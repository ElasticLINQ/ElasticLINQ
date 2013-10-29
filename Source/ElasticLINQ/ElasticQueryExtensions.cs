// Copyright (c) Tier 3 Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 

using ElasticLinq.Utility;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace ElasticLinq
{
    public enum WhereTarget
    {
        Filter,
        Query
    };

    /// <summary>
    /// Various extension methods that extend LINQ functionality for ElasticSearch queries.
    /// </summary>
    /// <remarks>
    /// These can not extend ElasticQuery as Queryable extends and returns IQueryable.
    /// Using these against any other provider will fail.
    /// </remarks>
    public static class ElasticQueryExtensions
    {
        public static IQueryable<TSource> QueryString<TSource>(this IQueryable<TSource> source, string query)
        {
            return CreateQueryMethodCall(source, (MethodInfo)MethodBase.GetCurrentMethod(), Expression.Constant(query));
        }

        public static IQueryable<TSource> WhereAppliesTo<TSource>(this IQueryable<TSource> source, WhereTarget target)
        {
            return CreateQueryMethodCall(source, (MethodInfo)MethodBase.GetCurrentMethod(), Expression.Constant(target));
        }

        public static IOrderedQueryable<TSource> OrderByScore<TSource>(this IQueryable<TSource> source)
        {
            return (IOrderedQueryable<TSource>)CreateQueryMethodCall(source, (MethodInfo)MethodBase.GetCurrentMethod());
        }

        public static IOrderedQueryable<TSource> OrderByScoreDescending<TSource>(this IQueryable<TSource> source)
        {
            return (IOrderedQueryable<TSource>)CreateQueryMethodCall(source, (MethodInfo)MethodBase.GetCurrentMethod());
        }

        public static IOrderedQueryable<TSource> ThenByScore<TSource>(this IOrderedQueryable<TSource> source)
        {
            return (IOrderedQueryable<TSource>)CreateQueryMethodCall(source, (MethodInfo)MethodBase.GetCurrentMethod());
        }

        public static IOrderedQueryable<TSource> ThenByScoreDescending<TSource>(this IOrderedQueryable<TSource> source)
        {
            return (IOrderedQueryable<TSource>)CreateQueryMethodCall(source, (MethodInfo)MethodBase.GetCurrentMethod());
        }

        private static IQueryable<TSource> CreateQueryMethodCall<TSource>(IQueryable<TSource> source, MethodInfo method, params Expression[] arguments)
        {
            Argument.EnsureNotNull("source", source);
            Argument.EnsureNotNull("method", source);

            var callExpression = Expression.Call(null, method.MakeGenericMethod(typeof(TSource)), new [] { source.Expression }.Concat(arguments));
            return source.Provider.CreateQuery<TSource>(callExpression);
        }
    }
}