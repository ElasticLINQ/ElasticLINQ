// Copyright (c) Microsoft Corporation.  All rights reserved.
// This source code is made available under the terms of the Microsoft Public License (MS-PL)

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace IQToolkit.Data.Common
{
    /// <summary>
    /// Converts LINQ query operators to into custom DbExpression's
    /// </summary>
    public class QueryBinder : DbExpressionVisitor
    {
        QueryMapper mapper;
        QueryLanguage language;
        Dictionary<ParameterExpression, Expression> map;
        Dictionary<Expression, GroupByInfo> groupByMap;
        Expression root;
        IEntityTable batchUpd;

        private QueryBinder(QueryMapper mapper, Expression root)
        {
            this.mapper = mapper;
            this.language = mapper.Translator.Linguist.Language;
            this.map = new Dictionary<ParameterExpression, Expression>();
            this.groupByMap = new Dictionary<Expression, GroupByInfo>();
            this.root = root;
        }

        public static Expression Bind(QueryMapper mapper, Expression expression)
        {
            return new QueryBinder(mapper, expression).Visit(expression);
        }

        private static LambdaExpression GetLambda(Expression e)
        {
            while (e.NodeType == ExpressionType.Quote)
            {
                e = ((UnaryExpression)e).Operand;
            }
            if (e.NodeType == ExpressionType.Constant)
            {
                return ((ConstantExpression)e).Value as LambdaExpression;
            }
            return e as LambdaExpression;
        }

        internal TableAlias GetNextAlias()
        {
            return new TableAlias();
        }

        private ProjectedColumns ProjectColumns(Expression expression, TableAlias newAlias, params TableAlias[] existingAliases)
        {
            return ColumnProjector.ProjectColumns(this.language, expression, null, newAlias, existingAliases);
        }

        protected override Expression VisitMethodCall(MethodCallExpression m)
        {
            if (m.Method.DeclaringType == typeof(Queryable) || m.Method.DeclaringType == typeof(Enumerable))
            {
                switch (m.Method.Name)
                {
                    case "Where":
                        return this.BindWhere(m.Type, m.Arguments[0], GetLambda(m.Arguments[1]));
                    case "Select":
                        return this.BindSelect(m.Type, m.Arguments[0], GetLambda(m.Arguments[1]));
                    case "SelectMany":
                        if (m.Arguments.Count == 2)
                        {
                            return this.BindSelectMany(m.Type, m.Arguments[0], GetLambda(m.Arguments[1]), null);
                        }
                        else if (m.Arguments.Count == 3)
                        {
                            return this.BindSelectMany(m.Type, m.Arguments[0], GetLambda(m.Arguments[1]), GetLambda(m.Arguments[2]));
                        }
                        break;
                    case "Join":
                        return this.BindJoin(m.Type, m.Arguments[0], m.Arguments[1], GetLambda(m.Arguments[2]), GetLambda(m.Arguments[3]), GetLambda(m.Arguments[4]));
                    case "GroupJoin":
                        if (m.Arguments.Count == 5)
                        {
                            return this.BindGroupJoin(m.Method, m.Arguments[0], m.Arguments[1], GetLambda(m.Arguments[2]), GetLambda(m.Arguments[3]), GetLambda(m.Arguments[4]));
                        }
                        break;
                    case "OrderBy":
                        return this.BindOrderBy(m.Type, m.Arguments[0], GetLambda(m.Arguments[1]), OrderType.Ascending);
                    case "OrderByDescending":
                        return this.BindOrderBy(m.Type, m.Arguments[0], GetLambda(m.Arguments[1]), OrderType.Descending);
                    case "ThenBy":
                        return this.BindThenBy(m.Arguments[0], GetLambda(m.Arguments[1]), OrderType.Ascending);
                    case "ThenByDescending":
                        return this.BindThenBy(m.Arguments[0], GetLambda(m.Arguments[1]), OrderType.Descending);
                    case "GroupBy":
                        if (m.Arguments.Count == 2)
                        {
                            return this.BindGroupBy(m.Arguments[0], GetLambda(m.Arguments[1]), null, null);
                        }
                        else if (m.Arguments.Count == 3)
                        {
                            LambdaExpression lambda1 = GetLambda(m.Arguments[1]);
                            LambdaExpression lambda2 = GetLambda(m.Arguments[2]);
                            if (lambda2.Parameters.Count == 1)
                            {
                                // second lambda is element selector
                                return this.BindGroupBy(m.Arguments[0], lambda1, lambda2, null);
                            }
                            else if (lambda2.Parameters.Count == 2)
                            {
                                // second lambda is result selector
                                return this.BindGroupBy(m.Arguments[0], lambda1, null, lambda2);
                            }
                        }
                        else if (m.Arguments.Count == 4)
                        {
                            return this.BindGroupBy(m.Arguments[0], GetLambda(m.Arguments[1]), GetLambda(m.Arguments[2]), GetLambda(m.Arguments[3]));
                        }
                        break;
                    case "Distinct":
                        if (m.Arguments.Count == 1)
                        {
                            return this.BindDistinct(m.Arguments[0]);
                        }
                        break;
                    case "Skip":
                        if (m.Arguments.Count == 2)
                        {
                            return this.BindSkip(m.Arguments[0], m.Arguments[1]);
                        }
                        break;
                    case "Take":
                        if (m.Arguments.Count == 2)
                        {
                            return this.BindTake(m.Arguments[0], m.Arguments[1]);
                        }
                        break;
                    case "First":
                    case "FirstOrDefault":
                    case "Single":
                    case "SingleOrDefault":
                    case "Last":
                    case "LastOrDefault":
                        if (m.Arguments.Count == 1)
                        {
                            return this.BindFirst(m.Arguments[0], null, m.Method.Name, m == this.root);
                        }
                        else if (m.Arguments.Count == 2)
                        {
                            return this.BindFirst(m.Arguments[0], GetLambda(m.Arguments[1]), m.Method.Name, m == this.root);
                        }
                        break;
                    case "Any":
                        if (m.Arguments.Count == 1)
                        {
                            return this.BindAnyAll(m.Arguments[0], m.Method, null, m == this.root);
                        }
                        else if (m.Arguments.Count == 2)
                        {
                            return this.BindAnyAll(m.Arguments[0], m.Method, GetLambda(m.Arguments[1]), m == this.root);
                        }
                        break;
                    case "All":
                        if (m.Arguments.Count == 2)
                        {
                            return this.BindAnyAll(m.Arguments[0], m.Method, GetLambda(m.Arguments[1]), m == this.root);
                        }
                        break;
                    case "Contains":
                        if (m.Arguments.Count == 2)
                        {
                            return this.BindContains(m.Arguments[0], m.Arguments[1], m == this.root);
                        }
                        break;
                    case "Cast":
                        if (m.Arguments.Count == 1)
                        {
                            return this.BindCast(m.Arguments[0], m.Method.GetGenericArguments()[0]);
                        }
                        break;
                    case "Reverse":
                        return this.BindReverse(m.Arguments[0]);
                    case "Intersect":
                    case "Except":
                        if (m.Arguments.Count == 2)
                        {
                            return this.BindIntersect(m.Arguments[0], m.Arguments[1], m.Method.Name == "Except");
                        }
                        break;
                }
            }
            else if (typeof(Updatable).IsAssignableFrom(m.Method.DeclaringType)) 
            {
                IEntityTable upd = this.batchUpd != null
                    ? this.batchUpd
                    : (IEntityTable)((ConstantExpression)m.Arguments[0]).Value;

                switch (m.Method.Name)
                {
                    case "Insert":
                        return this.BindInsert(
                            upd,
                            m.Arguments[1], 
                            m.Arguments.Count > 2 ? GetLambda(m.Arguments[2]) : null
                            );
                    case "Update":
                        return this.BindUpdate(
                            upd,
                            m.Arguments[1], 
                            m.Arguments.Count > 2 ? GetLambda(m.Arguments[2]) : null, 
                            m.Arguments.Count > 3 ? GetLambda(m.Arguments[3]) : null
                            );
                    case "InsertOrUpdate":
                        return this.BindInsertOrUpdate(
                            upd,
                            m.Arguments[1],
                            m.Arguments.Count > 2 ? GetLambda(m.Arguments[2]) : null,
                            m.Arguments.Count > 3 ? GetLambda(m.Arguments[3]) : null
                            );
                    case "Delete":
                        if (m.Arguments.Count == 2 && GetLambda(m.Arguments[1]) != null)
                        {
                            return this.BindDelete(upd, null, GetLambda(m.Arguments[1]));
                        }
                        return this.BindDelete(
                            upd,
                            m.Arguments[1], 
                            m.Arguments.Count > 2 ? GetLambda(m.Arguments[2]) : null
                            );
                    case "Batch":
                        return this.BindBatch(
                            upd,
                            m.Arguments[1],
                            GetLambda(m.Arguments[2]),
                            m.Arguments.Count > 3 ? m.Arguments[3] : Expression.Constant(50),
                            m.Arguments.Count > 4 ? m.Arguments[4] : Expression.Constant(false)
                            );
                }
            }
            if (this.language.IsAggregate(m.Method))
            {
                return this.BindAggregate(
                    m.Arguments[0], 
                    m.Method.Name, 
                    m.Method.ReturnType, 
                    m.Arguments.Count > 1 ? GetLambda(m.Arguments[1]) : null, 
                    m == this.root
                    );
            }
            return base.VisitMethodCall(m);
        }

        protected override Expression VisitUnary(UnaryExpression u)
        {
            if ((u.NodeType == ExpressionType.Convert || u.NodeType == ExpressionType.ConvertChecked)
                && u == this.root)
            {
                this.root = u.Operand;
            }
            return base.VisitUnary(u);
        }

        private ProjectionExpression VisitSequence(Expression source)
        {
            // sure to call base.Visit in order to skip my override
            return this.ConvertToSequence(base.Visit(source));
        }

        private ProjectionExpression ConvertToSequence(Expression expr)
        {
            switch (expr.NodeType)
            {
                case (ExpressionType)DbExpressionType.Projection:
                    return (ProjectionExpression)expr;
                case ExpressionType.New:
                    NewExpression nex = (NewExpression)expr;
                    if (expr.Type.IsGenericType && expr.Type.GetGenericTypeDefinition() == typeof(Grouping<,>))
                    {
                        return (ProjectionExpression)nex.Arguments[1];
                    }
                    goto default;
                case ExpressionType.MemberAccess:
                    var bound = this.BindRelationshipProperty((MemberExpression)expr);
                    if (bound.NodeType != ExpressionType.MemberAccess)
                        return this.ConvertToSequence(bound);
                    goto default;
                default:
                    var n = this.GetNewExpression(expr);
                    if (n != null)
                    {
                        expr = n;
                        goto case ExpressionType.New;
                    }
                    throw new Exception(string.Format("The expression of type '{0}' is not a sequence", expr.Type));
            }
        }

        private Expression BindRelationshipProperty(MemberExpression mex)
        {
            EntityExpression ex = mex.Expression as EntityExpression;
            if (ex != null && this.mapper.Mapping.IsRelationship(ex.Entity, mex.Member))
            {
                return this.mapper.GetMemberExpression(mex.Expression, ex.Entity, mex.Member);
            }
            return mex;
        }

        protected override Expression Visit(Expression exp)
        {
            Expression result = base.Visit(exp);

            if (result != null)
            {
                // bindings that expect projections should have called VisitSequence, the rest will probably get annoyed if
                // the projection does not have the expected type.
                Type expectedType = exp.Type;
                ProjectionExpression projection = result as ProjectionExpression;
                if (projection != null && projection.Aggregator == null && !expectedType.IsAssignableFrom(projection.Type))
                {
                    LambdaExpression aggregator = Aggregator.GetAggregator(expectedType, projection.Type);
                    if (aggregator != null)
                    {
                        return new ProjectionExpression(projection.Select, projection.Projector, aggregator);
                    }
                }
            }

            return result;
        }

        private Expression BindWhere(Type resultType, Expression source, LambdaExpression predicate)
        {
            ProjectionExpression projection = this.VisitSequence(source);
            this.map[predicate.Parameters[0]] = projection.Projector;
            Expression where = this.Visit(predicate.Body);
            var alias = this.GetNextAlias();
            ProjectedColumns pc = this.ProjectColumns(projection.Projector, alias, projection.Select.Alias);
            return new ProjectionExpression(
                new SelectExpression(alias, pc.Columns, projection.Select, where),
                pc.Projector
                );
        }

        private Expression BindReverse(Expression source)
        {
            ProjectionExpression projection = this.VisitSequence(source);
            var alias = this.GetNextAlias();
            ProjectedColumns pc = this.ProjectColumns(projection.Projector, alias, projection.Select.Alias);
            return new ProjectionExpression(
                new SelectExpression(alias, pc.Columns, projection.Select, null).SetReverse(true),
                pc.Projector
                );
        }

        private Expression BindSelect(Type resultType, Expression source, LambdaExpression selector)
        {
            ProjectionExpression projection = this.VisitSequence(source);
            this.map[selector.Parameters[0]] = projection.Projector;
            Expression expression = this.Visit(selector.Body);
            var alias = this.GetNextAlias();
            ProjectedColumns pc = this.ProjectColumns(expression, alias, projection.Select.Alias);
            return new ProjectionExpression(
                new SelectExpression(alias, pc.Columns, projection.Select, null),
                pc.Projector
                );
        }

        protected virtual Expression BindSelectMany(Type resultType, Expression source, LambdaExpression collectionSelector, LambdaExpression resultSelector)
        {
            ProjectionExpression projection = this.VisitSequence(source);
            this.map[collectionSelector.Parameters[0]] = projection.Projector;

            Expression collection = collectionSelector.Body;

            // check for DefaultIfEmpty
            bool defaultIfEmpty = false;
            MethodCallExpression mcs = collection as MethodCallExpression;
            if (mcs != null && mcs.Method.Name == "DefaultIfEmpty" && mcs.Arguments.Count == 1 &&
                (mcs.Method.DeclaringType == typeof(Queryable) || mcs.Method.DeclaringType == typeof(Enumerable)))
            {
                collection = mcs.Arguments[0];
                defaultIfEmpty = true;
            }

            ProjectionExpression collectionProjection = (ProjectionExpression)this.VisitSequence(collection);
            bool isTable = collectionProjection.Select.From is TableExpression;
            JoinType joinType = isTable ? JoinType.CrossJoin : defaultIfEmpty ? JoinType.OuterApply : JoinType.CrossApply;
            if (joinType == JoinType.OuterApply)
            {
                collectionProjection = this.language.AddOuterJoinTest(collectionProjection);
            }
            JoinExpression join = new JoinExpression(joinType, projection.Select, collectionProjection.Select, null);

            var alias = this.GetNextAlias();
            ProjectedColumns pc;
            if (resultSelector == null)
            {
                pc = this.ProjectColumns(collectionProjection.Projector, alias, projection.Select.Alias, collectionProjection.Select.Alias);
            }
            else
            {
                this.map[resultSelector.Parameters[0]] = projection.Projector;
                this.map[resultSelector.Parameters[1]] = collectionProjection.Projector;
                Expression result = this.Visit(resultSelector.Body);
                pc = this.ProjectColumns(result, alias, projection.Select.Alias, collectionProjection.Select.Alias);
            }
            return new ProjectionExpression(
                new SelectExpression(alias, pc.Columns, join, null),
                pc.Projector
                );
        }

        protected virtual Expression BindJoin(Type resultType, Expression outerSource, Expression innerSource, LambdaExpression outerKey, LambdaExpression innerKey, LambdaExpression resultSelector)
        {
            ProjectionExpression outerProjection = this.VisitSequence(outerSource);
            ProjectionExpression innerProjection = this.VisitSequence(innerSource);
            this.map[outerKey.Parameters[0]] = outerProjection.Projector;
            Expression outerKeyExpr = this.Visit(outerKey.Body);
            this.map[innerKey.Parameters[0]] = innerProjection.Projector;
            Expression innerKeyExpr = this.Visit(innerKey.Body);
            this.map[resultSelector.Parameters[0]] = outerProjection.Projector;
            this.map[resultSelector.Parameters[1]] = innerProjection.Projector;
            Expression resultExpr = this.Visit(resultSelector.Body);
            JoinExpression join = new JoinExpression(JoinType.InnerJoin, outerProjection.Select, innerProjection.Select, outerKeyExpr.Equal(innerKeyExpr));
            var alias = this.GetNextAlias();
            ProjectedColumns pc = this.ProjectColumns(resultExpr, alias, outerProjection.Select.Alias, innerProjection.Select.Alias);
            return new ProjectionExpression(
                new SelectExpression(alias, pc.Columns, join, null),
                pc.Projector
                );
        }

        protected virtual Expression BindIntersect(Expression outerSource, Expression innerSource, bool negate)
        {
            // SELECT * FROM outer WHERE EXISTS(SELECT * FROM inner WHERE inner = outer))
            ProjectionExpression outerProjection = this.VisitSequence(outerSource);
            ProjectionExpression innerProjection = this.VisitSequence(innerSource);

            Expression exists = new ExistsExpression(
                new SelectExpression(new TableAlias(), null, innerProjection.Select, innerProjection.Projector.Equal(outerProjection.Projector))
                );
            if (negate)
                exists = Expression.Not(exists);
            var alias = this.GetNextAlias();
            ProjectedColumns pc = this.ProjectColumns(outerProjection.Projector, alias, outerProjection.Select.Alias);
            return new ProjectionExpression(
                new SelectExpression(alias, pc.Columns, outerProjection.Select, exists),
                pc.Projector, outerProjection.Aggregator
                );
        }

        protected virtual Expression BindGroupJoin(MethodInfo groupJoinMethod, Expression outerSource, Expression innerSource, LambdaExpression outerKey, LambdaExpression innerKey, LambdaExpression resultSelector)
        {
            // A database will treat this no differently than a SelectMany w/ result selector, so just use that translation instead
            Type[] args = groupJoinMethod.GetGenericArguments();

            ProjectionExpression outerProjection = this.VisitSequence(outerSource);

            this.map[outerKey.Parameters[0]] = outerProjection.Projector;
            var predicateLambda = Expression.Lambda(innerKey.Body.Equal(outerKey.Body), innerKey.Parameters[0]);
            var callToWhere = Expression.Call(typeof(Enumerable), "Where", new Type[] { args[1] }, innerSource, predicateLambda);           
            Expression group = this.Visit(callToWhere);

            this.map[resultSelector.Parameters[0]] = outerProjection.Projector;
            this.map[resultSelector.Parameters[1]] = group;
            Expression resultExpr = this.Visit(resultSelector.Body);

            var alias = this.GetNextAlias();
            ProjectedColumns pc = this.ProjectColumns(resultExpr, alias, outerProjection.Select.Alias);
            return new ProjectionExpression(
                new SelectExpression(alias, pc.Columns, outerProjection.Select, null),
                pc.Projector
                );
        }

        List<OrderExpression> thenBys;

        protected virtual Expression BindOrderBy(Type resultType, Expression source, LambdaExpression orderSelector, OrderType orderType)
        {
            List<OrderExpression> myThenBys = this.thenBys;
            this.thenBys = null;
            ProjectionExpression projection = this.VisitSequence(source);

            this.map[orderSelector.Parameters[0]] = projection.Projector;
            List<OrderExpression> orderings = new List<OrderExpression>();
            orderings.Add(new OrderExpression(orderType, this.Visit(orderSelector.Body)));

            if (myThenBys != null)
            {
                for (int i = myThenBys.Count - 1; i >= 0; i--)
                {
                    OrderExpression tb = myThenBys[i];
                    LambdaExpression lambda = (LambdaExpression)tb.Expression;
                    this.map[lambda.Parameters[0]] = projection.Projector;
                    orderings.Add(new OrderExpression(tb.OrderType, this.Visit(lambda.Body)));
                }
            }

            var alias = this.GetNextAlias();
            ProjectedColumns pc = this.ProjectColumns(projection.Projector, alias, projection.Select.Alias);
            return new ProjectionExpression(
                new SelectExpression(alias, pc.Columns, projection.Select, null, orderings.AsReadOnly(), null),
                pc.Projector
                );
        }

        protected virtual Expression BindThenBy(Expression source, LambdaExpression orderSelector, OrderType orderType)
        {
            if (this.thenBys == null)
            {
                this.thenBys = new List<OrderExpression>();
            }
            this.thenBys.Add(new OrderExpression(orderType, orderSelector));
            return this.Visit(source);
        }

        protected virtual Expression BindGroupBy(Expression source, LambdaExpression keySelector, LambdaExpression elementSelector, LambdaExpression resultSelector)
        {
            ProjectionExpression projection = this.VisitSequence(source);

            this.map[keySelector.Parameters[0]] = projection.Projector;
            Expression keyExpr = this.Visit(keySelector.Body);

            Expression elemExpr = projection.Projector;
            if (elementSelector != null)
            {
                this.map[elementSelector.Parameters[0]] = projection.Projector;
                elemExpr = this.Visit(elementSelector.Body);
            }

            // Use ProjectColumns to get group-by expressions from key expression
            ProjectedColumns keyProjection = this.ProjectColumns(keyExpr, projection.Select.Alias, projection.Select.Alias);
            var groupExprs = keyProjection.Columns.Select(c => c.Expression).ToArray();

            // make duplicate of source query as basis of element subquery by visiting the source again
            ProjectionExpression subqueryBasis = this.VisitSequence(source);

            // recompute key columns for group expressions relative to subquery (need these for doing the correlation predicate)
            this.map[keySelector.Parameters[0]] = subqueryBasis.Projector;
            Expression subqueryKey = this.Visit(keySelector.Body);

            // use same projection trick to get group-by expressions based on subquery
            ProjectedColumns subqueryKeyPC = this.ProjectColumns(subqueryKey, subqueryBasis.Select.Alias, subqueryBasis.Select.Alias);
            var subqueryGroupExprs = subqueryKeyPC.Columns.Select(c => c.Expression).ToArray();
            Expression subqueryCorrelation = this.BuildPredicateWithNullsEqual(subqueryGroupExprs, groupExprs);

            // compute element based on duplicated subquery
            Expression subqueryElemExpr = subqueryBasis.Projector;
            if (elementSelector != null)
            {
                this.map[elementSelector.Parameters[0]] = subqueryBasis.Projector;
                subqueryElemExpr = this.Visit(elementSelector.Body);
            }

            // build subquery that projects the desired element
            var elementAlias = this.GetNextAlias();
            ProjectedColumns elementPC = this.ProjectColumns(subqueryElemExpr, elementAlias, subqueryBasis.Select.Alias);
            ProjectionExpression elementSubquery =
                new ProjectionExpression(
                    new SelectExpression(elementAlias, elementPC.Columns, subqueryBasis.Select, subqueryCorrelation),
                    elementPC.Projector
                    );

            var alias = this.GetNextAlias();

            // make it possible to tie aggregates back to this group-by
            GroupByInfo info = new GroupByInfo(alias, elemExpr);
            this.groupByMap.Add(elementSubquery, info);

            Expression resultExpr;
            if (resultSelector != null)
            {
                Expression saveGroupElement = this.currentGroupElement;
                this.currentGroupElement = elementSubquery;
                // compute result expression based on key & element-subquery
                this.map[resultSelector.Parameters[0]] = keyProjection.Projector;
                this.map[resultSelector.Parameters[1]] = elementSubquery;
                resultExpr = this.Visit(resultSelector.Body);
                this.currentGroupElement = saveGroupElement;
            }
            else
            {
                // result must be IGrouping<K,E>
                resultExpr = 
                    Expression.New(
                        typeof(Grouping<,>).MakeGenericType(keyExpr.Type, subqueryElemExpr.Type).GetConstructors()[0],
                        new Expression[] { keyExpr, elementSubquery }
                        );

                resultExpr = Expression.Convert(resultExpr, typeof(IGrouping<,>).MakeGenericType(keyExpr.Type, subqueryElemExpr.Type));
            }

            ProjectedColumns pc = this.ProjectColumns(resultExpr, alias, projection.Select.Alias);

            // make it possible to tie aggregates back to this group-by
            NewExpression newResult = this.GetNewExpression(pc.Projector);
            if (newResult != null && newResult.Type.IsGenericType && newResult.Type.GetGenericTypeDefinition() == typeof(Grouping<,>))
            {
                Expression projectedElementSubquery = newResult.Arguments[1];
                this.groupByMap.Add(projectedElementSubquery, info);
            }

            return new ProjectionExpression(
                new SelectExpression(alias, pc.Columns, projection.Select, null, null, groupExprs),
                pc.Projector
                );
        }

        private NewExpression GetNewExpression(Expression expression)
        {
            // ignore converions 
            while (expression.NodeType == ExpressionType.Convert || expression.NodeType == ExpressionType.ConvertChecked)
            {
                expression = ((UnaryExpression)expression).Operand;
            }
            return expression as NewExpression;
        }

        private Expression BuildPredicateWithNullsEqual(IEnumerable<Expression> source1, IEnumerable<Expression> source2)
        {
            IEnumerator<Expression> en1 = source1.GetEnumerator();
            IEnumerator<Expression> en2 = source2.GetEnumerator();
            Expression result = null;
            while (en1.MoveNext() && en2.MoveNext())
            {
                Expression compare =
                    Expression.Or(
                        new IsNullExpression(en1.Current).And(new IsNullExpression(en2.Current)),
                        en1.Current.Equal(en2.Current)
                        );
                result = (result == null) ? compare : result.And(compare);
            }
            return result;
        }

        Expression currentGroupElement;

        class GroupByInfo
        {
            internal TableAlias Alias { get; private set; }
            internal Expression Element { get; private set; }
            internal GroupByInfo(TableAlias alias, Expression element)
            {
                this.Alias = alias;
                this.Element = element;
            }
        }

        private Expression BindAggregate(Expression source, string aggName, Type returnType, LambdaExpression argument, bool isRoot)
        {
            bool hasPredicateArg = this.language.AggregateArgumentIsPredicate(aggName);
            bool isDistinct = false;
            bool argumentWasPredicate = false;
            bool useAlternateArg = false;

            // check for distinct
            MethodCallExpression mcs = source as MethodCallExpression;
            if (mcs != null && !hasPredicateArg && argument == null)
            {
                if (mcs.Method.Name == "Distinct" && mcs.Arguments.Count == 1 &&
                    (mcs.Method.DeclaringType == typeof(Queryable) || mcs.Method.DeclaringType == typeof(Enumerable))
                    && this.language.AllowDistinctInAggregates)
                {
                    source = mcs.Arguments[0];
                    isDistinct = true;
                }
            }

            if (argument != null && hasPredicateArg)
            {
                // convert query.Count(predicate) into query.Where(predicate).Count()
                source = Expression.Call(typeof(Queryable), "Where", new [] {TypeHelper.GetElementType(source.Type)}, source, argument);
                argument = null;
                argumentWasPredicate = true;
            }

            ProjectionExpression projection = this.VisitSequence(source);

            Expression argExpr = null;
            if (argument != null)
            {
                this.map[argument.Parameters[0]] = projection.Projector;
                argExpr = this.Visit(argument.Body);
            }
            else if (!hasPredicateArg || useAlternateArg)
            {
                argExpr = projection.Projector;
            }

            var alias = this.GetNextAlias();
            var pc = this.ProjectColumns(projection.Projector, alias, projection.Select.Alias);
            Expression aggExpr = new AggregateExpression(returnType, aggName, argExpr, isDistinct);
            var colType = this.language.TypeSystem.GetColumnType(returnType);
            SelectExpression select = new SelectExpression(alias, new ColumnDeclaration[] { new ColumnDeclaration("", aggExpr, colType) }, projection.Select, null);

            if (isRoot)
            {
                ParameterExpression p = Expression.Parameter(typeof(IEnumerable<>).MakeGenericType(aggExpr.Type), "p");
                LambdaExpression gator = Expression.Lambda(Expression.Call(typeof(Enumerable), "Single", new Type[] { returnType }, p), p);
                return new ProjectionExpression(select, new ColumnExpression(returnType, this.language.TypeSystem.GetColumnType(returnType), alias, ""), gator);
            }

            ScalarExpression subquery = new ScalarExpression(returnType, select);

            // if we can find the corresponding group-info we can build a special AggregateSubquery node that will enable us to 
            // optimize the aggregate expression later using AggregateRewriter
            GroupByInfo info;
            if (!argumentWasPredicate && this.groupByMap.TryGetValue(projection, out info))
            {
                // use the element expression from the group-by info to rebind the argument so the resulting expression is one that 
                // would be legal to add to the columns in the select expression that has the corresponding group-by clause.
                if (argument != null)
                {
                    this.map[argument.Parameters[0]] = info.Element;
                    argExpr = this.Visit(argument.Body);
                }
                else if (!hasPredicateArg || useAlternateArg)
                {
                    argExpr = info.Element;
                }
                aggExpr = new AggregateExpression(returnType, aggName, argExpr, isDistinct);

                // check for easy to optimize case.  If the projection that our aggregate is based on is really the 'group' argument from
                // the query.GroupBy(xxx, (key, group) => yyy) method then whatever expression we return here will automatically
                // become part of the select expression that has the group-by clause, so just return the simple aggregate expression.
                if (projection == this.currentGroupElement)
                    return aggExpr;

                return new AggregateSubqueryExpression(info.Alias, aggExpr, subquery);
            }

            return subquery;
        }

        private Expression BindDistinct(Expression source)
        {
            ProjectionExpression projection = this.VisitSequence(source);
            SelectExpression select = projection.Select;
            var alias = this.GetNextAlias();
            ProjectedColumns pc = this.ProjectColumns(projection.Projector, alias, projection.Select.Alias);
            return new ProjectionExpression(
                new SelectExpression(alias, pc.Columns, projection.Select, null, null, null, true, null, null, false),
                pc.Projector
                );
        }

        private Expression BindTake(Expression source, Expression take)
        {
            ProjectionExpression projection = this.VisitSequence(source);
            take = this.Visit(take);
            SelectExpression select = projection.Select;
            var alias = this.GetNextAlias();
            ProjectedColumns pc = this.ProjectColumns(projection.Projector, alias, projection.Select.Alias);
            return new ProjectionExpression(
                new SelectExpression(alias, pc.Columns, projection.Select, null, null, null, false, null, take, false),
                pc.Projector
                );
        }

        private Expression BindSkip(Expression source, Expression skip)
        {
            ProjectionExpression projection = this.VisitSequence(source);
            skip = this.Visit(skip);
            SelectExpression select = projection.Select;
            var alias = this.GetNextAlias();
            ProjectedColumns pc = this.ProjectColumns(projection.Projector, alias, projection.Select.Alias);
            return new ProjectionExpression(
                new SelectExpression(alias, pc.Columns, projection.Select, null, null, null, false, skip, null, false),
                pc.Projector
                );
        }

        private Expression BindCast(Expression source, Type targetElementType)
        {
            ProjectionExpression projection = this.VisitSequence(source);
            Type elementType = this.GetTrueUnderlyingType(projection.Projector);
            if (!targetElementType.IsAssignableFrom(elementType))
            {
                throw new InvalidOperationException(string.Format("Cannot cast elements from type '{0}' to type '{1}'", elementType, targetElementType));
            }
            return projection;
        }

        private Type GetTrueUnderlyingType(Expression expression)
        {
            while (expression.NodeType == ExpressionType.Convert)
            {
                expression = ((UnaryExpression)expression).Operand;
            }
            return expression.Type;
        }

        private Expression BindFirst(Expression source, LambdaExpression predicate, string kind, bool isRoot)
        {
            ProjectionExpression projection = this.VisitSequence(source);
            Expression where = null;
            if (predicate != null)
            {
                this.map[predicate.Parameters[0]] = projection.Projector;
                where = this.Visit(predicate.Body);
            }
            bool isFirst = kind.StartsWith("First");
            bool isLast = kind.StartsWith("Last");
            Expression take = (isFirst || isLast) ? Expression.Constant(1) : null;
            if (take != null || where != null)
            {
                var alias = this.GetNextAlias();
                ProjectedColumns pc = this.ProjectColumns(projection.Projector, alias, projection.Select.Alias);
                projection = new ProjectionExpression(
                    new SelectExpression(alias, pc.Columns, projection.Select, where, null, null, false, null, take, isLast),
                    pc.Projector
                    );
            }
            if (isRoot)
            {
                Type elementType = projection.Projector.Type;
                ParameterExpression p = Expression.Parameter(typeof(IEnumerable<>).MakeGenericType(elementType), "p");
                LambdaExpression gator = Expression.Lambda(Expression.Call(typeof(Enumerable), kind, new Type[] { elementType }, p), p);
                return new ProjectionExpression(projection.Select, projection.Projector, gator);
            }
            return projection;
        }

        private Expression BindAnyAll(Expression source, MethodInfo method, LambdaExpression predicate, bool isRoot)
        {
            bool isAll = method.Name == "All";
            ConstantExpression constSource = source as ConstantExpression;
            if (constSource != null && !IsQuery(constSource))
            {
                System.Diagnostics.Debug.Assert(!isRoot);
                Expression where = null;
                foreach (object value in (IEnumerable)constSource.Value)
                {
                    Expression expr = Expression.Invoke(predicate, Expression.Constant(value, predicate.Parameters[0].Type));
                    if (where == null)
                    {
                        where = expr;
                    }
                    else if (isAll)
                    {
                        where = where.And(expr);
                    }
                    else
                    {
                        where = where.Or(expr);
                    }
                }
                return this.Visit(where);
            }
            else
            {
                if (isAll)
                {
                    predicate = Expression.Lambda(Expression.Not(predicate.Body), predicate.Parameters.ToArray());
                }
                if (predicate != null)
                {
                    source = Expression.Call(typeof(Enumerable), "Where", method.GetGenericArguments(), source, predicate);
                }
                ProjectionExpression projection = this.VisitSequence(source);
                Expression result = new ExistsExpression(projection.Select);
                if (isAll)
                {
                    result = Expression.Not(result);
                }
                if (isRoot)
                {
                    if (this.language.AllowSubqueryInSelectWithoutFrom)
                    {
                        return GetSingletonSequence(result, "SingleOrDefault");
                    }
                    else
                    {
                        // use count aggregate instead of exists
                        var colType = this.language.TypeSystem.GetColumnType(typeof(int));
                        var newSelect = projection.Select.SetColumns(
                            new[] { new ColumnDeclaration("value", new AggregateExpression(typeof(int), "Count", null, false), colType) }
                            );
                        var colx = new ColumnExpression(typeof(int), colType, newSelect.Alias, "value");
                        var exp = isAll 
                            ? colx.Equal(Expression.Constant(0))
                            : colx.GreaterThan(Expression.Constant(0));
                        return new ProjectionExpression(
                            newSelect, exp, Aggregator.GetAggregator(typeof(bool), typeof(IEnumerable<bool>))
                            );
                    }
                }
                return result;
            }
        }

        private Expression BindContains(Expression source, Expression match, bool isRoot)
        {
            ConstantExpression constSource = source as ConstantExpression;
            if (constSource != null && !IsQuery(constSource))
            {
                System.Diagnostics.Debug.Assert(!isRoot);
                List<Expression> values = new List<Expression>();
                foreach (object value in (IEnumerable)constSource.Value)
                {
                    values.Add(Expression.Constant(Convert.ChangeType(value, match.Type), match.Type));
                }
                match = this.Visit(match);
                return new InExpression(match, values);
            }
            else if (isRoot && !this.language.AllowSubqueryInSelectWithoutFrom)
            {
                var p = Expression.Parameter(TypeHelper.GetElementType(source.Type), "x");
                var predicate = Expression.Lambda(p.Equal(match), p);
                var exp = Expression.Call(typeof(Queryable), "Any", new Type[] { p.Type }, source, predicate);
                this.root = exp;
                return this.Visit(exp);
            }
            else 
            {
                ProjectionExpression projection = this.VisitSequence(source);
                match = this.Visit(match);
                Expression result = new InExpression(match, projection.Select);
                if (isRoot)
                {
                    return this.GetSingletonSequence(result, "SingleOrDefault");
                }
                return result;
            }
        }

        private Expression GetSingletonSequence(Expression expr, string aggregator)
        {
            ParameterExpression p = Expression.Parameter(typeof(IEnumerable<>).MakeGenericType(expr.Type), "p");
            LambdaExpression gator = null;
            if (aggregator != null)
            {
                gator = Expression.Lambda(Expression.Call(typeof(Enumerable), aggregator, new Type[] { expr.Type }, p), p);
            }
            var alias = this.GetNextAlias();
            var colType = this.language.TypeSystem.GetColumnType(expr.Type);
            SelectExpression select = new SelectExpression(alias, new[] { new ColumnDeclaration("value", expr, colType) }, null, null);
            return new ProjectionExpression(select, new ColumnExpression(expr.Type, colType, alias, "value"), gator);
        }

        private Expression BindInsert(IEntityTable upd, Expression instance, LambdaExpression selector)
        {
            MappingEntity entity = this.mapper.Mapping.GetEntity(instance.Type, upd.TableId);
            return this.Visit(this.mapper.GetInsertExpression(entity, instance, selector));
        }

        private Expression BindUpdate(IEntityTable upd, Expression instance, LambdaExpression updateCheck, LambdaExpression resultSelector)
        {
            MappingEntity entity = this.mapper.Mapping.GetEntity(instance.Type, upd.TableId);
            return this.Visit(this.mapper.GetUpdateExpression(entity, instance, updateCheck, resultSelector, null));
        }

        private Expression BindInsertOrUpdate(IEntityTable upd, Expression instance, LambdaExpression updateCheck, LambdaExpression resultSelector)
        {
            MappingEntity entity = this.mapper.Mapping.GetEntity(instance.Type, upd.TableId);
            return this.Visit(this.mapper.GetInsertOrUpdateExpression(entity, instance, updateCheck, resultSelector));
        }

        private Expression BindDelete(IEntityTable upd, Expression instance, LambdaExpression deleteCheck)
        {
            MappingEntity entity = this.mapper.Mapping.GetEntity(instance != null ? instance.Type : deleteCheck.Parameters[0].Type, upd.TableId);
            return this.Visit(this.mapper.GetDeleteExpression(entity, instance, deleteCheck));
        }

        private Expression BindBatch(IEntityTable upd, Expression instances, LambdaExpression operation, Expression batchSize, Expression stream)
        {
            var save = this.batchUpd;
            this.batchUpd = upd;
            var op = (LambdaExpression)this.Visit(operation);
            this.batchUpd = save;
            var items = this.Visit(instances);
            var size = this.Visit(batchSize);
            var str = this.Visit(stream);
            return new BatchExpression(items, op, size, str);
        }

        private bool IsQuery(Expression expression)
        {
            Type elementType = TypeHelper.GetElementType(expression.Type);
            return elementType != null && typeof(IQueryable<>).MakeGenericType(elementType).IsAssignableFrom(expression.Type);
        }

        protected override Expression VisitConstant(ConstantExpression c)
        {
            if (this.IsQuery(c))
            {
                IQueryable q = (IQueryable)c.Value;
                IEntityTable t = q as IEntityTable;
                if (t != null)
                {
                    IHaveMappingEntity ihme = t as IHaveMappingEntity;
                    MappingEntity entity = ihme != null ? ihme.Entity : this.mapper.Mapping.GetEntity(t.ElementType, t.TableId);
                    return this.VisitSequence(this.mapper.GetQueryExpression(entity));
                }
                else if (q.Expression.NodeType == ExpressionType.Constant)
                {
                    // assume this is also a table via some other implementation of IQueryable
                    MappingEntity entity = this.mapper.Mapping.GetEntity(q.ElementType);
                    return this.VisitSequence(this.mapper.GetQueryExpression(entity));
                }
                else
                {
                    var pev = PartialEvaluator.Eval(q.Expression, this.mapper.Mapping.CanBeEvaluatedLocally);
                    return this.Visit(pev);
                }
            }
            return c;
        }

        protected override Expression VisitParameter(ParameterExpression p)
        {
            Expression e;
            if (this.map.TryGetValue(p, out e))
            {
                return e;
            }
            return p;
        }

        protected override Expression VisitInvocation(InvocationExpression iv)
        {
            LambdaExpression lambda = iv.Expression as LambdaExpression;
            if (lambda != null)
            {
                for (int i = 0, n = lambda.Parameters.Count; i < n; i++)
                {
                    this.map[lambda.Parameters[i]] = iv.Arguments[i];
                }
                return this.Visit(lambda.Body);
            }
            return base.VisitInvocation(iv);
        }

        protected override Expression VisitMemberAccess(MemberExpression m)
        {
            if (m.Expression != null 
                && m.Expression.NodeType == ExpressionType.Parameter 
                && !this.map.ContainsKey((ParameterExpression)m.Expression)
                && this.IsQuery(m))
            {
                return this.VisitSequence(this.mapper.GetQueryExpression(this.mapper.Mapping.GetEntity(m.Member)));
            }
            Expression source = this.Visit(m.Expression);
            if (this.language.IsAggregate(m.Member) && IsRemoteQuery(source))
            {
                return this.BindAggregate(m.Expression, m.Member.Name, TypeHelper.GetMemberType(m.Member), null, m == this.root);
            }
            Expression result = BindMember(source, m.Member);
            MemberExpression mex = result as MemberExpression;
            if (mex != null && mex.Member == m.Member && mex.Expression == m.Expression)
            {
                return m;
            }
            return result;
        }

        private bool IsRemoteQuery(Expression expression)
        {
            if (expression.NodeType.IsDbExpression())
                return true;
            switch (expression.NodeType)
            {
                case ExpressionType.MemberAccess:
                    return IsRemoteQuery(((MemberExpression)expression).Expression);
                case ExpressionType.Call:
                    MethodCallExpression mc = (MethodCallExpression)expression;
                    if (mc.Object != null)
                        return IsRemoteQuery(mc.Object);
                    else if (mc.Arguments.Count > 0)
                        return IsRemoteQuery(mc.Arguments[0]);
                    break;
            }
            return false;
        }

        public static Expression BindMember(Expression source, MemberInfo member)
        {
            switch (source.NodeType)
            {
                case (ExpressionType)DbExpressionType.Entity:
                    EntityExpression ex = (EntityExpression)source;
                    var result = BindMember(ex.Expression, member);
                    MemberExpression mex = result as MemberExpression;
                    if (mex != null && mex.Expression == ex.Expression && mex.Member == member)
                    {
                        return Expression.MakeMemberAccess(source, member);
                    }
                    return result;

                case ExpressionType.Convert:
                    UnaryExpression ux = (UnaryExpression)source;
                    return BindMember(ux.Operand, member);

                case ExpressionType.MemberInit:
                    MemberInitExpression min = (MemberInitExpression)source;
                    for (int i = 0, n = min.Bindings.Count; i < n; i++)
                    {
                        MemberAssignment assign = min.Bindings[i] as MemberAssignment;
                        if (assign != null && MembersMatch(assign.Member, member))
                        {                            
                            return assign.Expression;
                        }
                    }
                    break;

                case ExpressionType.New:
                    NewExpression nex = (NewExpression)source;
                    if (nex.Members != null)
                    {
                        for (int i = 0, n = nex.Members.Count; i < n; i++)
                        {
                            if (MembersMatch(nex.Members[i], member))
                            {
                                return nex.Arguments[i];
                            }
                        }
                    }
                    else if (nex.Type.IsGenericType && nex.Type.GetGenericTypeDefinition() == typeof(Grouping<,>))
                    {
                        if (member.Name == "Key")
                        {
                            return nex.Arguments[0];
                        }
                    }
                    break;

                case (ExpressionType)DbExpressionType.Projection:
                    // member access on a projection turns into a new projection w/ member access applied
                    ProjectionExpression proj = (ProjectionExpression)source;
                    Expression newProjector = BindMember(proj.Projector, member);
                    Type mt = TypeHelper.GetMemberType(member);
                    return new ProjectionExpression(proj.Select, newProjector, Aggregator.GetAggregator(mt, typeof(IEnumerable<>).MakeGenericType(mt)));

                case (ExpressionType)DbExpressionType.OuterJoined:
                    OuterJoinedExpression oj = (OuterJoinedExpression)source;
                    Expression em = BindMember(oj.Expression, member);
                    if (em is ColumnExpression)
                    {
                        return em;
                    }
                    return new OuterJoinedExpression(oj.Test, em);

                case ExpressionType.Conditional:
                    ConditionalExpression cex = (ConditionalExpression)source;
                    return Expression.Condition(cex.Test, BindMember(cex.IfTrue, member), BindMember(cex.IfFalse, member));

                case ExpressionType.Constant:
                    ConstantExpression con = (ConstantExpression)source;
                    Type memberType = TypeHelper.GetMemberType(member);
                    if (con.Value == null)
                    {
                        return Expression.Constant(GetDefault(memberType), memberType);
                    }
                    else
                    {
                        return Expression.Constant(GetValue(con.Value, member), memberType);
                    }
            }
            return Expression.MakeMemberAccess(source, member);
        }

        private static object GetValue(object instance, MemberInfo member)
        {
            FieldInfo fi = member as FieldInfo;
            if (fi != null)
            {
                return fi.GetValue(instance);
            }
            PropertyInfo pi = member as PropertyInfo;
            if (pi != null)
            {
                return pi.GetValue(instance, null);
            }
            return null;
        }

        private static object GetDefault(Type type)
        {
            if (!type.IsValueType || TypeHelper.IsNullableType(type))
            {
                return null;
            }
            else
            {
                return Activator.CreateInstance(type);
            }
        }

        private static bool MembersMatch(MemberInfo a, MemberInfo b)
        {
            if (a.Name == b.Name)
            {
                return true;
            }
            if (a is MethodInfo && b is PropertyInfo)
            {
                return a.Name == ((PropertyInfo)b).GetGetMethod().Name;
            }
            else if (a is PropertyInfo && b is MethodInfo)
            {
                return ((PropertyInfo)a).GetGetMethod().Name == b.Name;
            }
            return false;
        }
    }
}
