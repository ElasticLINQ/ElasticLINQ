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
        public static IOrderedQueryable<TSource> WhereAppliesTo<TSource>(this IQueryable<TSource> source, WhereTarget target)
        {
            var method = (MethodInfo)MethodBase.GetCurrentMethod();
            return (IOrderedQueryable<TSource>)source.Provider.CreateQuery<TSource>(
                Expression.Call(null, method.MakeGenericMethod(typeof(TSource)), source.Expression, Expression.Constant(target)));
        }

        public static IOrderedQueryable<TSource> OrderByScore<TSource>(this IQueryable<TSource> source)
        {
            return OrderBy(source, (MethodInfo)MethodBase.GetCurrentMethod());
        }

        public static IOrderedQueryable<TSource> OrderByScoreDescending<TSource>(this IQueryable<TSource> source)
        {
            return OrderBy(source, (MethodInfo)MethodBase.GetCurrentMethod());
        }

        private static IOrderedQueryable<TSource> OrderBy<TSource>(IQueryable<TSource> source, MethodInfo method )
        {
            Argument.EnsureNotNull("source", source);
            Argument.EnsureNotNull("method", method);

            return (IOrderedQueryable<TSource>)source.Provider.CreateQuery<TSource>(
                Expression.Call(null, method.MakeGenericMethod(typeof(TSource)), source.Expression));
        }

        public static IOrderedQueryable<TSource> ThenByScore<TSource>(this IOrderedQueryable<TSource> source)
        {
            return ThenBy(source, (MethodInfo)MethodBase.GetCurrentMethod());
        }

        public static IOrderedQueryable<TSource> ThenByScoreDescending<TSource>(this IOrderedQueryable<TSource> source)
        {
            return ThenBy(source, (MethodInfo)MethodBase.GetCurrentMethod());
        }

        private static IOrderedQueryable<TSource> ThenBy<TSource>(IOrderedQueryable<TSource> source, MethodInfo method)
        {
            Argument.EnsureNotNull("source", source);

            return (IOrderedQueryable<TSource>)source.Provider.CreateQuery<TSource>(
                Expression.Call(null, method.MakeGenericMethod(typeof(TSource)), source.Expression));
        }
    }
}
