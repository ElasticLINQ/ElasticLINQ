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
    /// An extended expression visitor including custom DbExpression nodes
    /// </summary>
    public abstract class DbExpressionVisitor : ExpressionVisitor
    {
        protected override Expression Visit(Expression exp)
        {
            if (exp == null)
            {
                return null;
            }
            switch ((DbExpressionType)exp.NodeType)
            {
                case DbExpressionType.Table:
                    return this.VisitTable((TableExpression)exp);
                case DbExpressionType.Column:
                    return this.VisitColumn((ColumnExpression)exp);
                case DbExpressionType.Select:
                    return this.VisitSelect((SelectExpression)exp);
                case DbExpressionType.Join:
                    return this.VisitJoin((JoinExpression)exp);
                case DbExpressionType.OuterJoined:
                    return this.VisitOuterJoined((OuterJoinedExpression)exp);
                case DbExpressionType.Aggregate:
                    return this.VisitAggregate((AggregateExpression)exp);
                case DbExpressionType.Scalar:
                case DbExpressionType.Exists:
                case DbExpressionType.In:
                    return this.VisitSubquery((SubqueryExpression)exp);
                case DbExpressionType.AggregateSubquery:
                    return this.VisitAggregateSubquery((AggregateSubqueryExpression)exp);
                case DbExpressionType.IsNull:
                    return this.VisitIsNull((IsNullExpression)exp);
                case DbExpressionType.Between:
                    return this.VisitBetween((BetweenExpression)exp);
                case DbExpressionType.RowCount:
                    return this.VisitRowNumber((RowNumberExpression)exp);
                case DbExpressionType.Projection:
                    return this.VisitProjection((ProjectionExpression)exp);
                case DbExpressionType.NamedValue:
                    return this.VisitNamedValue((NamedValueExpression)exp);
                case DbExpressionType.ClientJoin:
                    return this.VisitClientJoin((ClientJoinExpression)exp);
                case DbExpressionType.Insert:
                case DbExpressionType.Update:
                case DbExpressionType.Delete:
                case DbExpressionType.If:
                case DbExpressionType.Block:
                case DbExpressionType.Declaration:
                    return this.VisitCommand((CommandExpression)exp);
                case DbExpressionType.Batch:
                    return this.VisitBatch((BatchExpression)exp);
                case DbExpressionType.Variable:
                    return this.VisitVariable((VariableExpression)exp);
                case DbExpressionType.Function:
                    return this.VisitFunction((FunctionExpression)exp);
                case DbExpressionType.Entity:
                    return this.VisitEntity((EntityExpression)exp);
                default:
                    return base.Visit(exp);
            }
        }

        protected virtual Expression VisitEntity(EntityExpression entity)
        {
            var exp = this.Visit(entity.Expression);
            return this.UpdateEntity(entity, exp);
        }

        protected EntityExpression UpdateEntity(EntityExpression entity, Expression expression)
        {
            if (expression != entity.Expression)
            {
                return new EntityExpression(entity.Entity, expression);
            }
            return entity;
        }

        protected virtual Expression VisitTable(TableExpression table)
        {
            return table;
        }

        protected virtual Expression VisitColumn(ColumnExpression column)
        {
            return column;
        }

        protected virtual Expression VisitSelect(SelectExpression select)
        {
            var from = this.VisitSource(select.From);
            var where = this.Visit(select.Where);
            var orderBy = this.VisitOrderBy(select.OrderBy);
            var groupBy = this.VisitExpressionList(select.GroupBy);
            var skip = this.Visit(select.Skip);
            var take = this.Visit(select.Take);
            var columns = this.VisitColumnDeclarations(select.Columns);
            return this.UpdateSelect(select, from, where, orderBy, groupBy, skip, take, select.IsDistinct, select.IsReverse, columns);
        }

        protected SelectExpression UpdateSelect(
            SelectExpression select,             
            Expression from, Expression where, 
            IEnumerable<OrderExpression> orderBy, IEnumerable<Expression> groupBy,
            Expression skip, Expression take,
            bool isDistinct, bool isReverse,
            IEnumerable<ColumnDeclaration> columns
            )
        {
            if (from != select.From
                || where != select.Where
                || orderBy != select.OrderBy
                || groupBy != select.GroupBy
                || take != select.Take
                || skip != select.Skip
                || isDistinct != select.IsDistinct
                || columns != select.Columns
                || isReverse != select.IsReverse
                )
            {
                return new SelectExpression(select.Alias, columns, from, where, orderBy, groupBy, isDistinct, skip, take, isReverse);
            }
            return select;
        }

        protected virtual Expression VisitJoin(JoinExpression join)
        {
            var left = this.VisitSource(join.Left);
            var right = this.VisitSource(join.Right);
            var condition = this.Visit(join.Condition);
            return this.UpdateJoin(join, join.Join, left, right, condition);
        }

        protected JoinExpression UpdateJoin(JoinExpression join, JoinType joinType, Expression left, Expression right, Expression condition)
        {
            if (joinType != join.Join || left != join.Left || right != join.Right || condition != join.Condition)
            {
                return new JoinExpression(joinType, left, right, condition);
            }
            return join;
        }

        protected virtual Expression VisitOuterJoined(OuterJoinedExpression outer)
        {
            var test = this.Visit(outer.Test);
            var expression = this.Visit(outer.Expression);
            return this.UpdateOuterJoined(outer, test, expression);
        }

        protected OuterJoinedExpression UpdateOuterJoined(OuterJoinedExpression outer, Expression test, Expression expression)
        {
            if (test != outer.Test || expression != outer.Expression)
            {
                return new OuterJoinedExpression(test, expression);
            }
            return outer;
        }

        protected virtual Expression VisitAggregate(AggregateExpression aggregate)
        {
            var arg = this.Visit(aggregate.Argument);
            return this.UpdateAggregate(aggregate, aggregate.Type, aggregate.AggregateName, arg, aggregate.IsDistinct);
        }

        protected AggregateExpression UpdateAggregate(AggregateExpression aggregate, Type type, string aggType, Expression arg, bool isDistinct)
        {
            if (type != aggregate.Type || aggType != aggregate.AggregateName || arg != aggregate.Argument || isDistinct != aggregate.IsDistinct)
            {
                return new AggregateExpression(type, aggType, arg, isDistinct);
            }
            return aggregate;
        }

        protected virtual Expression VisitIsNull(IsNullExpression isnull)
        {
            var expr = this.Visit(isnull.Expression);
            return this.UpdateIsNull(isnull, expr);
        }

        protected IsNullExpression UpdateIsNull(IsNullExpression isnull, Expression expression)
        {
            if (expression != isnull.Expression)
            {
                return new IsNullExpression(expression);
            }
            return isnull;
        }

        protected virtual Expression VisitBetween(BetweenExpression between)
        {
            var expr = this.Visit(between.Expression);
            var lower = this.Visit(between.Lower);
            var upper = this.Visit(between.Upper);
            return this.UpdateBetween(between, expr, lower, upper);
        }

        protected BetweenExpression UpdateBetween(BetweenExpression between, Expression expression, Expression lower, Expression upper)
        {
            if (expression != between.Expression || lower != between.Lower || upper != between.Upper)
            {
                return new BetweenExpression(expression, lower, upper);
            }
            return between;
        }

        protected virtual Expression VisitRowNumber(RowNumberExpression rowNumber)
        {
            var orderby = this.VisitOrderBy(rowNumber.OrderBy);
            return this.UpdateRowNumber(rowNumber, orderby);
        }

        protected RowNumberExpression UpdateRowNumber(RowNumberExpression rowNumber, IEnumerable<OrderExpression> orderBy)
        {
            if (orderBy != rowNumber.OrderBy)
            {
                return new RowNumberExpression(orderBy);
            }
            return rowNumber;
        }

        protected virtual Expression VisitNamedValue(NamedValueExpression value)
        {
            return value;
        }

        protected virtual Expression VisitSubquery(SubqueryExpression subquery)
        {
            switch ((DbExpressionType)subquery.NodeType)
            {
                case DbExpressionType.Scalar:
                    return this.VisitScalar((ScalarExpression)subquery);
                case DbExpressionType.Exists:
                    return this.VisitExists((ExistsExpression)subquery);
                case DbExpressionType.In:
                    return this.VisitIn((InExpression)subquery);
            }
            return subquery;
        }

        protected virtual Expression VisitScalar(ScalarExpression scalar)
        {
            var select = (SelectExpression)this.Visit(scalar.Select);
            return this.UpdateScalar(scalar, select);
        }

        protected ScalarExpression UpdateScalar(ScalarExpression scalar, SelectExpression select)
        {
            if (select != scalar.Select)
            {
                return new ScalarExpression(scalar.Type, select);
            }
            return scalar;
        }

        protected virtual Expression VisitExists(ExistsExpression exists)
        {
            var select = (SelectExpression)this.Visit(exists.Select);
            return this.UpdateExists(exists, select);
        }

        protected ExistsExpression UpdateExists(ExistsExpression exists, SelectExpression select)
        {
            if (select != exists.Select)
            {
                return new ExistsExpression(select);
            }
            return exists;
        }

        protected virtual Expression VisitIn(InExpression @in)
        {
            var expr = this.Visit(@in.Expression);
            var select = (SelectExpression)this.Visit(@in.Select);
            var values = this.VisitExpressionList(@in.Values);
            return this.UpdateIn(@in, expr, select, values);
        }

        protected InExpression UpdateIn(InExpression @in, Expression expression, SelectExpression select, IEnumerable<Expression> values)
        {
            if (expression != @in.Expression || select != @in.Select || values != @in.Values)
            {
                if (select != null)
                {
                    return new InExpression(expression, select);
                }
                else
                {
                    return new InExpression(expression, values);
                }
            }
            return @in;
        }

        protected virtual Expression VisitAggregateSubquery(AggregateSubqueryExpression aggregate)
        {
            var subquery = (ScalarExpression) this.Visit(aggregate.AggregateAsSubquery);
            return this.UpdateAggregateSubquery(aggregate, subquery);
        }

        protected AggregateSubqueryExpression UpdateAggregateSubquery(AggregateSubqueryExpression aggregate, ScalarExpression subquery)
        {
            if (subquery != aggregate.AggregateAsSubquery)
            {
                return new AggregateSubqueryExpression(aggregate.GroupByAlias, aggregate.AggregateInGroupSelect, subquery);
            }
            return aggregate;
        }

        protected virtual Expression VisitSource(Expression source)
        {
            return this.Visit(source);
        }

        protected virtual Expression VisitProjection(ProjectionExpression proj)
        {
            var select = (SelectExpression)this.Visit(proj.Select);
            var projector = this.Visit(proj.Projector);
            return this.UpdateProjection(proj, select, projector, proj.Aggregator);
        }

        protected ProjectionExpression UpdateProjection(ProjectionExpression proj, SelectExpression select, Expression projector, LambdaExpression aggregator)
        {
            if (select != proj.Select || projector != proj.Projector || aggregator != proj.Aggregator)
            {
                return new ProjectionExpression(select, projector, aggregator);
            }
            return proj;
        }

        protected virtual Expression VisitClientJoin(ClientJoinExpression join)
        {
            var projection = (ProjectionExpression)this.Visit(join.Projection);
            var outerKey = this.VisitExpressionList(join.OuterKey);
            var innerKey = this.VisitExpressionList(join.InnerKey);
            return this.UpdateClientJoin(join, projection, outerKey, innerKey);
         }

        protected ClientJoinExpression UpdateClientJoin(ClientJoinExpression join, ProjectionExpression projection, IEnumerable<Expression> outerKey, IEnumerable<Expression> innerKey)
        {
            if (projection != join.Projection || outerKey != join.OuterKey || innerKey != join.InnerKey)
            {
                return new ClientJoinExpression(projection, outerKey, innerKey);
            }
            return join;
        }

        protected virtual Expression VisitCommand(CommandExpression command)
        {
            switch ((DbExpressionType)command.NodeType)
            {
                case DbExpressionType.Insert:
                    return this.VisitInsert((InsertCommand)command);
                case DbExpressionType.Update:
                    return this.VisitUpdate((UpdateCommand)command);
                case DbExpressionType.Delete:
                    return this.VisitDelete((DeleteCommand)command);
                case DbExpressionType.If:
                    return this.VisitIf((IFCommand)command);
                case DbExpressionType.Block:
                    return this.VisitBlock((BlockCommand)command);
                case DbExpressionType.Declaration:
                    return this.VisitDeclaration((DeclarationCommand)command);
                default:
                    return this.VisitUnknown(command);
            }
        }

        protected virtual Expression VisitInsert(InsertCommand insert)
        {
            var table = (TableExpression)this.Visit(insert.Table);
            var assignments = this.VisitColumnAssignments(insert.Assignments);
            return this.UpdateInsert(insert, table, assignments);
        }

        protected InsertCommand UpdateInsert(InsertCommand insert, TableExpression table, IEnumerable<ColumnAssignment> assignments)
        {
            if (table != insert.Table || assignments != insert.Assignments)
            {
                return new InsertCommand(table, assignments);
            }
            return insert;
        }

        protected virtual Expression VisitUpdate(UpdateCommand update)
        {
            var table = (TableExpression)this.Visit(update.Table);
            var where = this.Visit(update.Where);
            var assignments = this.VisitColumnAssignments(update.Assignments);
            return this.UpdateUpdate(update, table, where, assignments);
        }

        protected UpdateCommand UpdateUpdate(UpdateCommand update, TableExpression table, Expression where, IEnumerable<ColumnAssignment> assignments)
        {
            if (table != update.Table || where != update.Where || assignments != update.Assignments)
            {
                return new UpdateCommand(table, where, assignments);
            }
            return update;
        }

        protected virtual Expression VisitDelete(DeleteCommand delete)
        {
            var table = (TableExpression)this.Visit(delete.Table);
            var where = this.Visit(delete.Where);
            return this.UpdateDelete(delete, table, where);
        }

        protected DeleteCommand UpdateDelete(DeleteCommand delete, TableExpression table, Expression where)
        {
            if (table != delete.Table || where != delete.Where)
            {
                return new DeleteCommand(table, where);
            }
            return delete;
        }

        protected virtual Expression VisitBatch(BatchExpression batch)
        {
            var operation = (LambdaExpression)this.Visit(batch.Operation);
            var batchSize = this.Visit(batch.BatchSize);
            var stream = this.Visit(batch.Stream);
            return this.UpdateBatch(batch, batch.Input, operation, batchSize, stream);
        }

        protected BatchExpression UpdateBatch(BatchExpression batch, Expression input, LambdaExpression operation, Expression batchSize, Expression stream)
        {
            if (input != batch.Input || operation != batch.Operation || batchSize != batch.BatchSize || stream != batch.Stream)
            {
                return new BatchExpression(input, operation, batchSize, stream);
            }
            return batch;
        }

        protected virtual Expression VisitIf(IFCommand ifx)
        {
            var check = this.Visit(ifx.Check);
            var ifTrue = this.Visit(ifx.IfTrue);
            var ifFalse = this.Visit(ifx.IfFalse);
            return this.UpdateIf(ifx, check, ifTrue, ifFalse);
        }

        protected IFCommand UpdateIf(IFCommand ifx, Expression check, Expression ifTrue, Expression ifFalse)
        {
            if (check != ifx.Check || ifTrue != ifx.IfTrue || ifFalse != ifx.IfFalse)
            {
                return new IFCommand(check, ifTrue, ifFalse);
            }
            return ifx;
        }

        protected virtual Expression VisitBlock(BlockCommand block)
        {
            var commands = this.VisitExpressionList(block.Commands);
            return this.UpdateBlock(block, commands);
        }

        protected BlockCommand UpdateBlock(BlockCommand block, IList<Expression> commands)
        {
            if (block.Commands != commands)
            {
                return new BlockCommand(commands);
            }
            return block;
        }

        protected virtual Expression VisitDeclaration(DeclarationCommand decl)
        {
            var variables = this.VisitVariableDeclarations(decl.Variables);
            var source = (SelectExpression)this.Visit(decl.Source);
            return this.UpdateDeclaration(decl, variables, source);

        }

        protected DeclarationCommand UpdateDeclaration(DeclarationCommand decl, IEnumerable<VariableDeclaration> variables, SelectExpression source)
        {
            if (variables != decl.Variables || source != decl.Source)
            {
                return new DeclarationCommand(variables, source);
            }
            return decl;
        }

        protected virtual Expression VisitVariable(VariableExpression vex)
        {
            return vex;
        }

        protected virtual Expression VisitFunction(FunctionExpression func)
        {
            var arguments = this.VisitExpressionList(func.Arguments);
            return this.UpdateFunction(func, func.Name, arguments);
        }

        protected FunctionExpression UpdateFunction(FunctionExpression func, string name, IEnumerable<Expression> arguments)
        {
            if (name != func.Name || arguments != func.Arguments)
            {
                return new FunctionExpression(func.Type, name, arguments);
            }
            return func;
        }

        protected virtual ColumnAssignment VisitColumnAssignment(ColumnAssignment ca)
        {
            ColumnExpression c = (ColumnExpression)this.Visit(ca.Column);
            Expression e = this.Visit(ca.Expression);
            return this.UpdateColumnAssignment(ca, c, e);
        }

        protected ColumnAssignment UpdateColumnAssignment(ColumnAssignment ca, ColumnExpression c, Expression e)
        {
            if (c != ca.Column || e != ca.Expression)
            {
                return new ColumnAssignment(c, e);
            }
            return ca;
        }

        protected virtual ReadOnlyCollection<ColumnAssignment> VisitColumnAssignments(ReadOnlyCollection<ColumnAssignment> assignments)
        {
            List<ColumnAssignment> alternate = null;
            for (int i = 0, n = assignments.Count; i < n; i++)
            {
                ColumnAssignment assignment = this.VisitColumnAssignment(assignments[i]);
                if (alternate == null && assignment != assignments[i])
                {
                    alternate = assignments.Take(i).ToList();
                }
                if (alternate != null)
                {
                    alternate.Add(assignment);
                }
            }
            if (alternate != null)
            {
                return alternate.AsReadOnly();
            }
            return assignments;
        }

        protected virtual ReadOnlyCollection<ColumnDeclaration> VisitColumnDeclarations(ReadOnlyCollection<ColumnDeclaration> columns)
        {
            List<ColumnDeclaration> alternate = null;
            for (int i = 0, n = columns.Count; i < n; i++)
            {
                ColumnDeclaration column = columns[i];
                Expression e = this.Visit(column.Expression);
                if (alternate == null && e != column.Expression)
                {
                    alternate = columns.Take(i).ToList();
                }
                if (alternate != null)
                {
                    alternate.Add(new ColumnDeclaration(column.Name, e, column.QueryType));
                }
            }
            if (alternate != null)
            {
                return alternate.AsReadOnly();
            }
            return columns;
        }

        protected virtual ReadOnlyCollection<VariableDeclaration> VisitVariableDeclarations(ReadOnlyCollection<VariableDeclaration> decls)
        {
            List<VariableDeclaration> alternate = null;
            for (int i = 0, n = decls.Count; i < n; i++)
            {
                VariableDeclaration decl = decls[i];
                Expression e = this.Visit(decl.Expression);
                if (alternate == null && e != decl.Expression)
                {
                    alternate = decls.Take(i).ToList();
                }
                if (alternate != null)
                {
                    alternate.Add(new VariableDeclaration(decl.Name, decl.QueryType, e));
                }
            }
            if (alternate != null)
            {
                return alternate.AsReadOnly();
            }
            return decls;
        }

        protected virtual ReadOnlyCollection<OrderExpression> VisitOrderBy(ReadOnlyCollection<OrderExpression> expressions)
        {
            if (expressions != null)
            {
                List<OrderExpression> alternate = null;
                for (int i = 0, n = expressions.Count; i < n; i++)
                {
                    OrderExpression expr = expressions[i];
                    Expression e = this.Visit(expr.Expression);
                    if (alternate == null && e != expr.Expression)
                    {
                        alternate = expressions.Take(i).ToList();
                    }
                    if (alternate != null)
                    {
                        alternate.Add(new OrderExpression(expr.OrderType, e));
                    }
                }
                if (alternate != null)
                {
                    return alternate.AsReadOnly();
                }
            }
            return expressions;
        }
    }
}