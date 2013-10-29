// Copyright (c) Tier 3 Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 

using ElasticLinq.Utility;
using System.Linq.Expressions;

namespace ElasticLinq.Request.Visitors
{
    internal class ConstantMemberPair
    {
        private readonly ConstantExpression constantExpression;
        private readonly MemberExpression memberExpression;

        public static ConstantMemberPair Create(Expression a, Expression b)
        {
            if (a is ConstantExpression && b is MemberExpression)
                return new ConstantMemberPair((ConstantExpression)a, (MemberExpression)b);

            if (b is ConstantExpression && a is MemberExpression)
                return new ConstantMemberPair((ConstantExpression)b, (MemberExpression)a);

            return null;
        }

        public ConstantMemberPair(ConstantExpression constantExpression, MemberExpression memberExpression)
        {
            this.constantExpression = constantExpression;
            this.memberExpression = memberExpression;
        }

        public ConstantExpression ConstantExpression
        {
            get { return constantExpression; }
        }

        public MemberExpression MemberExpression
        {
            get { return memberExpression; }
        }

        public bool IsNullTest
        {
            get
            {
                // someProperty.HasValue
                if (memberExpression.Member.Name == "HasValue"
                       && constantExpression.Type == typeof(bool)
                       && TypeHelper.IsNullableType(memberExpression.Member.DeclaringType))
                    return true;

                // something == null (for reference types or Nullable<T>
                if (constantExpression.Value == null)
                    return !memberExpression.Type.IsValueType || TypeHelper.IsNullableType(memberExpression.Type);

                return false;
            }
        }
    }
}
