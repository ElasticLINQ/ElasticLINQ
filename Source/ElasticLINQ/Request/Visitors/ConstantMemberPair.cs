// Licensed under the Apache 2.0 License. See LICENSE.txt in the project root for more information.

using ElasticLinq.Utility;
using System;
using System.Diagnostics;
using System.Linq.Expressions;

namespace ElasticLinq.Request.Visitors
{
    /// <summary>
    /// A pair containing one MemberExpression and one ConstantExpression that might be used
    /// in a test or assignment.
    /// </summary>
    [DebuggerDisplay("{MemberExpression,nq}, {ConstantExpression.Value}")]
    class ConstantMemberPair
    {
        readonly ConstantExpression constantExpression;
        readonly MemberExpression memberExpression;

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

        public ConstantExpression ConstantExpression => constantExpression;

        public MemberExpression MemberExpression => memberExpression;

        public bool IsNullTest
        {
            get
            {
                // someProperty.HasValue
                if (memberExpression.Member.Name == "HasValue"
                       && constantExpression.Type == typeof(bool)
                       && memberExpression.Member.DeclaringType.IsGenericOf(typeof(Nullable<>)))
                    return true;

                // something == null (for reference types or Nullable<T>)
                if (constantExpression.Value == null)
                    return memberExpression.Type.IsNullable();

                return false;
            }
        }
    }
}
