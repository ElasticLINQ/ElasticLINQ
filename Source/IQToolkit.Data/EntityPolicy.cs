// Copyright (c) Microsoft Corporation.  All rights reserved.
// This source code is made available under the terms of the Microsoft Public License (MS-PL)

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace IQToolkit.Data
{
    using Common;
    using Mapping;

    public class EntityPolicy : QueryPolicy
    {
        HashSet<MemberInfo> included = new HashSet<MemberInfo>();
        HashSet<MemberInfo> deferred = new HashSet<MemberInfo>();
        Dictionary<MemberInfo, List<LambdaExpression>> operations = new Dictionary<MemberInfo, List<LambdaExpression>>();

        public void Apply(LambdaExpression fnApply)
        {
            if (fnApply == null)
                throw new ArgumentNullException("fnApply");
            if (fnApply.Parameters.Count != 1)
                throw new ArgumentException("Apply function has wrong number of arguments.");
            this.AddOperation(TypeHelper.GetElementType(fnApply.Parameters[0].Type), fnApply);
        }

        public void Apply<TEntity>(Expression<Func<IEnumerable<TEntity>, IEnumerable<TEntity>>> fnApply)
        {
            Apply((LambdaExpression)fnApply);
        }

        public void Include(MemberInfo member)
        {
            Include(member, false);
        }

        public void Include(MemberInfo member, bool deferLoad)
        {
            this.included.Add(member);
            if (deferLoad)
                Defer(member);
        }

        public void IncludeWith(LambdaExpression fnMember)
        {
            IncludeWith(fnMember, false);
        }

        public void IncludeWith(LambdaExpression fnMember, bool deferLoad)
        {
            var rootMember = RootMemberFinder.Find(fnMember, fnMember.Parameters[0]);
            if (rootMember == null)
                throw new InvalidOperationException("Subquery does not originate with a member access");
            Include(rootMember.Member, deferLoad);
            if (rootMember != fnMember.Body)
            {
                AssociateWith(fnMember);
            }
        }

        public void IncludeWith<TEntity>(Expression<Func<TEntity, object>> fnMember)
        {
            IncludeWith((LambdaExpression)fnMember, false);
        }

        public void IncludeWith<TEntity>(Expression<Func<TEntity, object>> fnMember, bool deferLoad)
        {
            IncludeWith((LambdaExpression)fnMember, deferLoad);
        }

        private void Defer(MemberInfo member)
        {
            Type mType = TypeHelper.GetMemberType(member);
            if (mType.IsGenericType)
            {
                var gType = mType.GetGenericTypeDefinition();
                if (gType != typeof(IEnumerable<>)
                    && gType != typeof(IList<>)
                    && !typeof(IDeferLoadable).IsAssignableFrom(mType))
                {
                    throw new InvalidOperationException(string.Format("The member '{0}' cannot be deferred due to its type.", member));
                }
            }
            this.deferred.Add(member);
        }

        public void AssociateWith(LambdaExpression memberQuery)
        {
            var rootMember = RootMemberFinder.Find(memberQuery, memberQuery.Parameters[0]);
            if (rootMember == null)
                throw new InvalidOperationException("Subquery does not originate with a member access");
            if (rootMember != memberQuery.Body)
            {
                var memberParam = Expression.Parameter(rootMember.Type, "root");
                var newBody = ExpressionReplacer.Replace(memberQuery.Body, rootMember, memberParam);
                this.AddOperation(rootMember.Member, Expression.Lambda(newBody, memberParam));
            }
        }

        private void AddOperation(MemberInfo member, LambdaExpression operation)
        {
            List<LambdaExpression> memberOps;
            if (!this.operations.TryGetValue(member, out memberOps))
            {
                memberOps = new List<LambdaExpression>();
                this.operations.Add(member, memberOps);
            }
            memberOps.Add(operation);
        }

        public void AssociateWith<TEntity>(Expression<Func<TEntity, IEnumerable>> memberQuery)
        {
            AssociateWith((LambdaExpression)memberQuery);
        }

        class RootMemberFinder : ExpressionVisitor
        {
            MemberExpression found;
            ParameterExpression parameter;

            private RootMemberFinder(ParameterExpression parameter)
            {
                this.parameter = parameter;
            }

            public static MemberExpression Find(Expression query, ParameterExpression parameter)
            {
                var finder = new RootMemberFinder(parameter);
                finder.Visit(query);
                return finder.found;
            }

            protected override Expression VisitMethodCall(MethodCallExpression m)
            {
                if (m.Object != null)
                {
                    this.Visit(m.Object);
                }
                else if (m.Arguments.Count > 0)
                {
                    this.Visit(m.Arguments[0]);
                }
                return m;
            }

            protected override Expression VisitMemberAccess(MemberExpression m)
            {
                if (m.Expression == this.parameter)
                {
                    this.found = m;
                    return m;
                }
                else
                {
                    return base.VisitMemberAccess(m);
                }
            }
        }

        public override bool IsIncluded(MemberInfo member)
        {
            return this.included.Contains(member);
        }

        public override bool IsDeferLoaded(MemberInfo member)
        {
            return this.deferred.Contains(member);
        }

        public override QueryPolice CreatePolice(QueryTranslator translator)
        {
            return new Police(this, translator);
        }

        class Police : QueryPolice
        {
            EntityPolicy policy;

            public Police(EntityPolicy policy, QueryTranslator translator)
                : base(policy, translator)
            {
                this.policy = policy;
            }

            public override Expression ApplyPolicy(Expression expression, MemberInfo member)
            {
                List<LambdaExpression> ops;
                if (this.policy.operations.TryGetValue(member, out ops))
                {
                    var result = expression;
                    foreach (var fnOp in ops)
                    {
                        var pop = PartialEvaluator.Eval(fnOp, this.Translator.Mapper.Mapping.CanBeEvaluatedLocally);
                        result = this.Translator.Mapper.ApplyMapping(Expression.Invoke(pop, result));
                    }
                    var projection = (ProjectionExpression)result;
                    if (projection.Type != expression.Type)
                    {
                        var fnAgg = Aggregator.GetAggregator(expression.Type, projection.Type);
                        projection = new ProjectionExpression(projection.Select, projection.Projector, fnAgg);
                    }
                    return projection;
                }
                return expression;
            }
        }
    }
}