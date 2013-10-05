// Copyright (c) Tier 3 Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 

using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace ElasticLinq
{
    public static class ElasticQueryExtensions
    {
        public static IOrderedQueryable<TSource> OrderByScore<TSource>(this IQueryable<TSource> source)
        {
            return A(source, (MethodInfo)MethodBase.GetCurrentMethod());
        }

        public static IOrderedQueryable<TSource> OrderByScoreDescending<TSource>(this IQueryable<TSource> source)
        {
            return A(source, (MethodInfo)MethodBase.GetCurrentMethod());
        }

        private static IOrderedQueryable<TSource> A<TSource>(IQueryable<TSource> source, MethodInfo method)
        {
            if (source == null)
                throw new ArgumentNullException("source");

            return (IOrderedQueryable<TSource>) source.Provider.CreateQuery<TSource>(
                Expression.Call(null, method.MakeGenericMethod(typeof (TSource)), new[] { source.Expression }));
        }

        public static IOrderedQueryable<TSource> ThenByScore<TSource>(this IOrderedQueryable<TSource> source)
        {
            return A(source, (MethodInfo)MethodBase.GetCurrentMethod());
        }

        public static IOrderedQueryable<TSource> ThenByScoreDescending<TSource>(this IOrderedQueryable<TSource> source)
        {
            return A(source, (MethodInfo)MethodBase.GetCurrentMethod());
        }

        private static IOrderedQueryable<TSource> A<TSource>(IOrderedQueryable<TSource> source, MethodInfo method)
        {
            if (source == null)
                throw new ArgumentNullException("source");

            return (IOrderedQueryable<TSource>)source.Provider.CreateQuery<TSource>(
                Expression.Call(null, method.MakeGenericMethod(typeof(TSource)), new[] { source.Expression }));
        }
    }
}
