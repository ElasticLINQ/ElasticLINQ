// Licensed under the Apache 2.0 License. See LICENSE.txt in the project root for more information.

using System;
using System.Diagnostics;
using ElasticLinq.Utility;
using System.Linq.Expressions;

namespace ElasticLinq.Request.Visitors
{
    [DebuggerDisplay("{MemberExpression,nq}, {ConstantExpression.Value}")]
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
                       && memberExpression.Member.DeclaringType.IsGenericOf(typeof(Nullable<>)))
                    return true;

                // something == null (for reference types or Nullable<T>
                if (constantExpression.Value == null)
                    return memberExpression.Type.IsNullable();

                return false;
            }
        }
    }
}
