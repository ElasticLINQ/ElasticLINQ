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
    /// Extended node types for custom expressions
    /// </summary>
    public enum DbExpressionType
    {
        Table = 1000, // make sure these don't overlap with ExpressionType
        ClientJoin,
        Column,
        Select,
        Projection,
        Entity,
        Join,
        Aggregate,
        Scalar,
        Exists,
        In,
        Grouping,
        AggregateSubquery,
        IsNull,
        Between,
        RowCount,
        NamedValue,
        OuterJoined,
        Insert,
        Update,
        Delete,
        Batch,
        Function,
        Block,
        If,
        Declaration,
        Variable
    }

    public static class DbExpressionTypeExtensions
    {
        public static bool IsDbExpression(this ExpressionType et)
        {
            return ((int)et) >= 1000;
        }
    }

    public abstract class DbExpression : Expression
    {
        protected DbExpression(DbExpressionType eType, Type type)
            : base((ExpressionType)eType, type)
        {
        }

        public override string ToString()
        {
            return DbExpressionWriter.WriteToString(this);
        }
    }

    public abstract class AliasedExpression : DbExpression
    {
        TableAlias alias;
        protected AliasedExpression(DbExpressionType nodeType, Type type, TableAlias alias)
            : base(nodeType, type)
        {
            this.alias = alias;
        }
        public TableAlias Alias
        {
            get { return this.alias; }
        }
    }


    /// <summary>
    /// A custom expression node that represents a table reference in a SQL query
    /// </summary>
    public class TableExpression : AliasedExpression
    {
        MappingEntity entity;
        string name;

        public TableExpression(TableAlias alias, MappingEntity entity, string name)
            : base(DbExpressionType.Table, typeof(void), alias)
        {
            this.entity = entity;
            this.name = name;
        }

        public MappingEntity Entity
        {
            get { return this.entity; }
        }

        public string Name
        {
            get { return this.name; }
        }

        public override string ToString()
        {
            return "T(" + this.Name + ")";
        }
    }

    public class EntityExpression : DbExpression
    {
        MappingEntity entity;
        Expression expression;

        public EntityExpression(MappingEntity entity, Expression expression)
            : base(DbExpressionType.Entity, expression.Type)
        {
            this.entity = entity;
            this.expression = expression;
        }

        public MappingEntity Entity
        {
            get { return this.entity; }
        }

        public Expression Expression
        {
            get { return this.expression; }
        }
    }

    /// <summary>
    /// A custom expression node that represents a reference to a column in a SQL query
    /// </summary>
    public class ColumnExpression : DbExpression, IEquatable<ColumnExpression>
    {
        TableAlias alias;
        string name;
        QueryType queryType;

        public ColumnExpression(Type type, QueryType queryType, TableAlias alias, string name)
            : base(DbExpressionType.Column, type)
        {
            if (queryType == null)
                throw new ArgumentNullException("queryType");
            if (name == null)
                throw new ArgumentNullException("name");
            this.alias = alias;
            this.name = name;
            this.queryType = queryType;
        }

        public TableAlias Alias
        {
            get { return this.alias; }
        }

        public string Name
        {
            get { return this.name; }
        }

        public QueryType QueryType
        {
            get { return this.queryType; }
        }

        public override string ToString()
        {
            return this.Alias.ToString() + ".C(" + this.name + ")";
        }

        public override int GetHashCode()
        {
            return alias.GetHashCode() + name.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as ColumnExpression);
        }

        public bool Equals(ColumnExpression other)
        {
            return other != null
                && ((object)this) == (object)other
                 || (alias == other.alias && name == other.Name);
        }
    }

    public class TableAlias
    {
        public TableAlias()
        {
        }

        public override string ToString()
        {
            return "A:" + this.GetHashCode();
        }
    }

    /// <summary>
    /// A declaration of a column in a SQL SELECT expression
    /// </summary>
    public class ColumnDeclaration
    {
        string name;
        Expression expression;
        QueryType queryType;

        public ColumnDeclaration(string name, Expression expression, QueryType queryType)
        {
            if (name == null)
                throw new ArgumentNullException("name");
            if (expression == null)
                throw new ArgumentNullException("expression");
            if (queryType == null)
                throw new ArgumentNullException("queryType");
            this.name = name;
            this.expression = expression;
            this.queryType = queryType;
        }

        public string Name
        {
            get { return this.name; }
        }

        public Expression Expression
        {
            get { return this.expression; }
        }

        public QueryType QueryType
        {
            get { return this.queryType; }
        }
    }

    /// <summary>
    /// An SQL OrderBy order type 
    /// </summary>
    public enum OrderType
    {
        Ascending,
        Descending
    }

    /// <summary>
    /// A pairing of an expression and an order type for use in a SQL Order By clause
    /// </summary>
    public class OrderExpression
    {
        OrderType orderType;
        Expression expression;
        public OrderExpression(OrderType orderType, Expression expression)
        {
            this.orderType = orderType;
            this.expression = expression;
        }
        public OrderType OrderType
        {
            get { return this.orderType; }
        }
        public Expression Expression
        {
            get { return this.expression; }
        }
    }

    /// <summary>
    /// A custom expression node used to represent a SQL SELECT expression
    /// </summary>
    public class SelectExpression : AliasedExpression
    {
        ReadOnlyCollection<ColumnDeclaration> columns;
        bool isDistinct;
        Expression from;
        Expression where;
        ReadOnlyCollection<OrderExpression> orderBy;
        ReadOnlyCollection<Expression> groupBy;
        Expression take;
        Expression skip;
        bool reverse;

        public SelectExpression(
            TableAlias alias,
            IEnumerable<ColumnDeclaration> columns,
            Expression from,
            Expression where,
            IEnumerable<OrderExpression> orderBy,
            IEnumerable<Expression> groupBy,
            bool isDistinct,
            Expression skip,
            Expression take,
            bool reverse
            )
            : base(DbExpressionType.Select, typeof(void), alias)
        {
            this.columns = columns.ToReadOnly();
            this.isDistinct = isDistinct;
            this.from = from;
            this.where = where;
            this.orderBy = orderBy.ToReadOnly();
            this.groupBy = groupBy.ToReadOnly();
            this.take = take;
            this.skip = skip;
            this.reverse = reverse;
        }
        public SelectExpression(
            TableAlias alias,
            IEnumerable<ColumnDeclaration> columns,
            Expression from,
            Expression where,
            IEnumerable<OrderExpression> orderBy,
            IEnumerable<Expression> groupBy
            )
            : this(alias, columns, from, where, orderBy, groupBy, false, null, null, false)
        {
        }
        public SelectExpression(
            TableAlias alias, IEnumerable<ColumnDeclaration> columns,
            Expression from, Expression where
            )
            : this(alias, columns, from, where, null, null)
        {
        }
        public ReadOnlyCollection<ColumnDeclaration> Columns
        {
            get { return this.columns; }
        }
        public Expression From
        {
            get { return this.from; }
        }
        public Expression Where
        {
            get { return this.where; }
        }
        public ReadOnlyCollection<OrderExpression> OrderBy
        {
            get { return this.orderBy; }
        }
        public ReadOnlyCollection<Expression> GroupBy
        {
            get { return this.groupBy; }
        }
        public bool IsDistinct
        {
            get { return this.isDistinct; }
        }
        public Expression Skip
        {
            get { return this.skip; }
        }
        public Expression Take
        {
            get { return this.take; }
        }
        public bool IsReverse
        {
            get { return this.reverse; }
        }
        public string QueryText
        {
            get { return SqlFormatter.Format(this, true); }
        }
    }

    /// <summary>
    /// A kind of SQL join
    /// </summary>
    public enum JoinType
    {
        CrossJoin,
        InnerJoin,
        CrossApply,
        OuterApply,
        LeftOuter,
        SingletonLeftOuter
    }

    /// <summary>
    /// A custom expression node representing a SQL join clause
    /// </summary>
    public class JoinExpression : DbExpression
    {
        JoinType joinType;
        Expression left;
        Expression right;
        Expression condition;

        public JoinExpression(JoinType joinType, Expression left, Expression right, Expression condition)
            : base(DbExpressionType.Join, typeof(void))
        {
            this.joinType = joinType;
            this.left = left;
            this.right = right;
            this.condition = condition;
        }
        public JoinType Join
        {
            get { return this.joinType; }
        }
        public Expression Left
        {
            get { return this.left; }
        }
        public Expression Right
        {
            get { return this.right; }
        }
        public new Expression Condition
        {
            get { return this.condition; }
        }
    }

    public class OuterJoinedExpression : DbExpression
    {
        Expression test;
        Expression expression;

        public OuterJoinedExpression(Expression test, Expression expression)
            : base(DbExpressionType.OuterJoined, expression.Type)
        {
            this.test = test;
            this.expression = expression;
        }

        public Expression Test
        {
            get { return this.test; }
        }

        public Expression Expression
        {
            get { return this.expression; }
        }
    }

    public abstract class SubqueryExpression : DbExpression
    {
        SelectExpression select;
        protected SubqueryExpression(DbExpressionType eType, Type type, SelectExpression select)
            : base(eType, type)
        {
            System.Diagnostics.Debug.Assert(eType == DbExpressionType.Scalar || eType == DbExpressionType.Exists || eType == DbExpressionType.In);
            this.select = select;
        }
        public SelectExpression Select
        {
            get { return this.select; }
        }
    }

    public class ScalarExpression : SubqueryExpression
    {
        public ScalarExpression(Type type, SelectExpression select)
            : base(DbExpressionType.Scalar, type, select)
        {
        }
    }

    public class ExistsExpression : SubqueryExpression
    {
        public ExistsExpression(SelectExpression select)
            : base(DbExpressionType.Exists, typeof(bool), select)
        {
        }
    }

    public class InExpression : SubqueryExpression
    {
        Expression expression;
        ReadOnlyCollection<Expression> values;  // either select or expressions are assigned
        public InExpression(Expression expression, SelectExpression select)
            : base(DbExpressionType.In, typeof(bool), select)
        {
            this.expression = expression;
        }
        public InExpression(Expression expression, IEnumerable<Expression> values)
            : base(DbExpressionType.In, typeof(bool), null)
        {
            this.expression = expression;
            this.values = values.ToReadOnly();
        }
        public Expression Expression
        {
            get { return this.expression; }
        }
        public ReadOnlyCollection<Expression> Values
        {
            get { return this.values; }
        }
    }

    public class AggregateExpression : DbExpression
    {
        string aggregateName;
        Expression argument;
        bool isDistinct;
        public AggregateExpression(Type type, string aggregateName, Expression argument, bool isDistinct)
            : base(DbExpressionType.Aggregate, type)
        {
            this.aggregateName = aggregateName;
            this.argument = argument;
            this.isDistinct = isDistinct;
        }
        public string AggregateName
        {
            get { return this.aggregateName; }
        }
        public Expression Argument
        {
            get { return this.argument; }
        }
        public bool IsDistinct
        {
            get { return this.isDistinct; }
        }
    }

    public class AggregateSubqueryExpression : DbExpression
    {
        TableAlias groupByAlias;
        Expression aggregateInGroupSelect;
        ScalarExpression aggregateAsSubquery;
        public AggregateSubqueryExpression(TableAlias groupByAlias, Expression aggregateInGroupSelect, ScalarExpression aggregateAsSubquery)
            : base(DbExpressionType.AggregateSubquery, aggregateAsSubquery.Type)
        {
            this.aggregateInGroupSelect = aggregateInGroupSelect;
            this.groupByAlias = groupByAlias;
            this.aggregateAsSubquery = aggregateAsSubquery;
        }
        public TableAlias GroupByAlias { get { return this.groupByAlias; } }
        public Expression AggregateInGroupSelect { get { return this.aggregateInGroupSelect; } }
        public ScalarExpression AggregateAsSubquery { get { return this.aggregateAsSubquery; } }
    }

    /// <summary>
    /// Allows is-null tests against value-types like int and float
    /// </summary>
    public class IsNullExpression : DbExpression
    {
        Expression expression;
        public IsNullExpression(Expression expression)
            : base(DbExpressionType.IsNull, typeof(bool))
        {
            this.expression = expression;
        }
        public Expression Expression
        {
            get { return this.expression; }
        }
    }

    public class BetweenExpression : DbExpression
    {
        Expression expression;
        Expression lower;
        Expression upper;
        public BetweenExpression(Expression expression, Expression lower, Expression upper)
            : base(DbExpressionType.Between, expression.Type)
        {
            this.expression = expression;
            this.lower = lower;
            this.upper = upper;
        }
        public Expression Expression
        {
            get { return this.expression; }
        }
        public Expression Lower
        {
            get { return this.lower; }
        }
        public Expression Upper
        {
            get { return this.upper; }
        }
    }

    public class RowNumberExpression : DbExpression
    {
        ReadOnlyCollection<OrderExpression> orderBy;
        public RowNumberExpression(IEnumerable<OrderExpression> orderBy)
            : base(DbExpressionType.RowCount, typeof(int))
        {
            this.orderBy = orderBy.ToReadOnly();
        }
        public ReadOnlyCollection<OrderExpression> OrderBy
        {
            get { return this.orderBy; }
        }
    }

    public class NamedValueExpression : DbExpression
    {
        string name;
        QueryType queryType;
        Expression value;

        public NamedValueExpression(string name, QueryType queryType, Expression value)
            : base(DbExpressionType.NamedValue, value.Type)
        {
            if (name == null)
                throw new ArgumentNullException("name");
            //if (queryType == null)
                //throw new ArgumentNullException("queryType");
            if (value == null)
                throw new ArgumentNullException("value");
            this.name = name;
            this.queryType = queryType;
            this.value = value;
        }

        public string Name
        {
            get { return this.name; }
        }

        public QueryType QueryType
        {
            get { return this.queryType; }
        }

        public Expression Value
        {
            get { return this.value; }
        }
    }

    /// <summary>
    /// A custom expression representing the construction of one or more result objects from a 
    /// SQL select expression
    /// </summary>
    public class ProjectionExpression : DbExpression
    {
        SelectExpression select;
        Expression projector;
        LambdaExpression aggregator;
        public ProjectionExpression(SelectExpression source, Expression projector)
            : this(source, projector, null)
        {
        }
        public ProjectionExpression(SelectExpression source, Expression projector, LambdaExpression aggregator)
            : base(DbExpressionType.Projection, aggregator != null ? aggregator.Body.Type : typeof(IEnumerable<>).MakeGenericType(projector.Type))
        {
            this.select = source;
            this.projector = projector;
            this.aggregator = aggregator;
        }
        public SelectExpression Select
        {
            get { return this.select; }
        }
        public Expression Projector
        {
            get { return this.projector; }
        }
        public LambdaExpression Aggregator
        {
            get { return this.aggregator; }
        }
        public bool IsSingleton
        {
            get { return this.aggregator != null && this.aggregator.Body.Type == projector.Type; }
        }
        public override string ToString()
        {
            return DbExpressionWriter.WriteToString(this);
        }
        public string QueryText
        {
            get { return SqlFormatter.Format(select, true); }
        }
    }

    public class ClientJoinExpression : DbExpression
    {
        ReadOnlyCollection<Expression> outerKey;
        ReadOnlyCollection<Expression> innerKey;
        ProjectionExpression projection;

        public ClientJoinExpression(ProjectionExpression projection, IEnumerable<Expression> outerKey, IEnumerable<Expression> innerKey)
            : base(DbExpressionType.ClientJoin, projection.Type)
        {
            this.outerKey = outerKey.ToReadOnly();
            this.innerKey = innerKey.ToReadOnly();
            this.projection = projection;
        }

        public ReadOnlyCollection<Expression> OuterKey
        {
            get { return this.outerKey; }
        }

        public ReadOnlyCollection<Expression> InnerKey
        {
            get { return this.innerKey; }
        }

        public ProjectionExpression Projection
        {
            get { return this.projection; }
        }
    }

    public class BatchExpression : Expression
    {
        Expression input;
        LambdaExpression operation;
        Expression batchSize;
        Expression stream;

        public BatchExpression(Expression input, LambdaExpression operation, Expression batchSize, Expression stream)
            : base((ExpressionType)DbExpressionType.Batch, typeof(IEnumerable<>).MakeGenericType(operation.Body.Type))
        {
            this.input = input;
            this.operation = operation;
            this.batchSize = batchSize;
            this.stream = stream;
        }

        public Expression Input
        {
            get { return this.input; }
        }

        public LambdaExpression Operation
        {
            get { return this.operation; }
        }

        public Expression BatchSize
        {
            get { return this.batchSize; }
        }

        public Expression Stream
        {
            get { return this.stream; }
        }
    }

    public class FunctionExpression : DbExpression
    {
        string name;
        ReadOnlyCollection<Expression> arguments;

        public FunctionExpression(Type type, string name, IEnumerable<Expression> arguments)
            : base(DbExpressionType.Function, type)
        {
            this.name = name;
            this.arguments = arguments.ToReadOnly();
        }

        public string Name
        {
            get { return this.name; }
        }

        public ReadOnlyCollection<Expression> Arguments
        {
            get { return this.arguments; }
        }
    }

    public abstract class CommandExpression : DbExpression
    {
        protected CommandExpression(DbExpressionType eType, Type type)
            : base(eType, type)
        {
        }
    }

    public class InsertCommand : CommandExpression
    {
        TableExpression table;
        ReadOnlyCollection<ColumnAssignment> assignments;

        public InsertCommand(TableExpression table, IEnumerable<ColumnAssignment> assignments)
            : base(DbExpressionType.Insert, typeof(int))
        {
            this.table = table;
            this.assignments = assignments.ToReadOnly();
        }

        public TableExpression Table
        {
            get { return this.table; }
        }

        public ReadOnlyCollection<ColumnAssignment> Assignments
        {
            get { return this.assignments; }
        }
    }

    public class ColumnAssignment
    {
        ColumnExpression column;
        Expression expression;

        public ColumnAssignment(ColumnExpression column, Expression expression)
        {
            this.column = column;
            this.expression = expression;
        }

        public ColumnExpression Column
        {
            get { return this.column; }
        }

        public Expression Expression
        {
            get { return this.expression; }
        }
    }

    public class UpdateCommand : CommandExpression
    {
        TableExpression table;
        Expression where;
        ReadOnlyCollection<ColumnAssignment> assignments;

        public UpdateCommand(TableExpression table, Expression where, IEnumerable<ColumnAssignment> assignments)
            : base(DbExpressionType.Update, typeof(int))
        {
            this.table = table;
            this.where = where;
            this.assignments = assignments.ToReadOnly();
        }

        public TableExpression Table
        {
            get { return this.table; }
        }

        public Expression Where
        {
            get { return this.where; }
        }

        public ReadOnlyCollection<ColumnAssignment> Assignments
        {
            get { return this.assignments; }
        }
    }

    public class DeleteCommand : CommandExpression
    {
        TableExpression table;
        Expression where;

        public DeleteCommand(TableExpression table, Expression where)
            : base(DbExpressionType.Delete, typeof(int))
        {
            this.table = table;
            this.where = where;
        }

        public TableExpression Table
        {
            get { return this.table; }
        }

        public Expression Where
        {
            get { return this.where; }
        }
    }


    public class IFCommand : CommandExpression
    {
        Expression check;
        Expression ifTrue;
        Expression ifFalse;

        public IFCommand(Expression check, Expression ifTrue, Expression ifFalse)
            : base(DbExpressionType.If, ifTrue.Type)
        {
            this.check = check;
            this.ifTrue = ifTrue;
            this.ifFalse = ifFalse;
        }

        public Expression Check 
        {
            get { return this.check; }
        }

        public Expression IfTrue
        {
            get { return this.ifTrue; }
        }

        public Expression IfFalse 
        {
            get { return this.ifFalse; }
        }
    }

    public class BlockCommand : CommandExpression
    {
        ReadOnlyCollection<Expression> commands;

        public BlockCommand(IList<Expression> commands)
            : base(DbExpressionType.Block, commands[commands.Count-1].Type)
        {
            this.commands = commands.ToReadOnly();
        }

        public BlockCommand(params Expression[] commands) 
            : this((IList<Expression>)commands)
        {
        }

        public ReadOnlyCollection<Expression> Commands
        {
            get { return this.commands; }
        }
    }

    public class DeclarationCommand : CommandExpression
    {
        ReadOnlyCollection<VariableDeclaration> variables;
        SelectExpression source;

        public DeclarationCommand(IEnumerable<VariableDeclaration> variables, SelectExpression source)
            : base(DbExpressionType.Declaration, typeof(void))
        {
            this.variables = variables.ToReadOnly();
            this.source = source;
        }

        public ReadOnlyCollection<VariableDeclaration> Variables
        {
            get { return this.variables; }
        }

        public SelectExpression Source
        {
            get { return this.source; }
        }
    }

    public class VariableDeclaration
    {
        string name;
        QueryType type;
        Expression expression;

        public VariableDeclaration(string name, QueryType type, Expression expression)
        {
            this.name = name;
            this.type = type;
            this.expression = expression;
        }

        public string Name
        {
            get { return this.name; }
        }

        public QueryType QueryType
        {
            get { return this.type; }
        }

        public Expression Expression
        {
            get { return this.expression; }
        }
    }

    public class VariableExpression : Expression
    {
        string name;
        QueryType queryType;

        public VariableExpression(string name, Type type, QueryType queryType)
            : base((ExpressionType)DbExpressionType.Variable, type)
        {
            this.name = name;
            this.queryType = queryType;
        }

        public string Name
        {
            get { return this.name; }
        }

        public QueryType QueryType
        {
            get { return this.queryType; }
        }
    }
}
