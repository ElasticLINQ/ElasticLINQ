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
            ConstantExpression = constantExpression;
            MemberExpression = memberExpression;
        }

        public ConstantExpression ConstantExpression { get; }

        public MemberExpression MemberExpression { get; }

        public bool IsNullTest
        {
            get
            {
                // someProperty.HasValue
                if (MemberExpression.Member.Name == "HasValue"
                       && ConstantExpression.Type == typeof(bool)
                       && MemberExpression.Member.DeclaringType.IsGenericOf(typeof(Nullable<>)))
                    return true;

                // something == null (for reference types or Nullable<T>)
                if (ConstantExpression.Value == null)
                    return MemberExpression.Type.IsNullable();

                return false;
            }
        }
    }
}
