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
using System.Threading;

namespace IQToolkit.Data.Common
{
    public abstract class MappingTable
    {
    }

    public abstract class AdvancedMapping : BasicMapping
    {
        public abstract bool IsNestedEntity(MappingEntity entity, MemberInfo member);
        public abstract IList<MappingTable> GetTables(MappingEntity entity);
        public abstract string GetAlias(MappingTable table);
        public abstract string GetAlias(MappingEntity entity, MemberInfo member);
        public abstract string GetTableName(MappingTable table);
        public abstract bool IsExtensionTable(MappingTable table);
        public abstract string GetExtensionRelatedAlias(MappingTable table);
        public abstract IEnumerable<string> GetExtensionKeyColumnNames(MappingTable table);
        public abstract IEnumerable<MemberInfo> GetExtensionRelatedMembers(MappingTable table);

        protected AdvancedMapping()
        {
        }

        public override bool IsRelationship(MappingEntity entity, MemberInfo member)
        {
            return base.IsRelationship(entity, member)
                || this.IsNestedEntity(entity, member);
        }

        public override object CloneEntity(MappingEntity entity, object instance)
        {
            object clone = base.CloneEntity(entity, instance);

            // need to clone nested entities too
            foreach (var mi in this.GetMappedMembers(entity))
            {
                if (this.IsNestedEntity(entity, mi))
                {
                    MappingEntity nested = this.GetRelatedEntity(entity, mi);
                    var nestedValue = mi.GetValue(instance);
                    if (nestedValue != null)
                    {
                        var nestedClone = this.CloneEntity(nested, mi.GetValue(instance));
                        mi.SetValue(clone, nestedClone);
                    }
                }
            }

            return clone;
        }

        public override bool IsModified(MappingEntity entity, object instance, object original)
        {
            if (base.IsModified(entity, instance, original))
                return true;

            // need to check nested entities too
            foreach (var mi in this.GetMappedMembers(entity))
            {
                if (this.IsNestedEntity(entity, mi))
                {
                    MappingEntity nested = this.GetRelatedEntity(entity, mi);
                    if (this.IsModified(nested, mi.GetValue(instance), mi.GetValue(original)))
                        return true;
                }
            }

            return false;
        }

        public override QueryMapper CreateMapper(QueryTranslator translator)
        {
            return new AdvancedMapper(this, translator);
        }
    }

    public class AdvancedMapper : BasicMapper
    {
        AdvancedMapping mapping;

        public AdvancedMapper(AdvancedMapping mapping, QueryTranslator translator)
            : base(mapping, translator)
        {
            this.mapping = mapping;
        }

        public virtual IEnumerable<MappingTable> GetDependencyOrderedTables(MappingEntity entity)
        {
            var lookup = this.mapping.GetTables(entity).ToLookup(t => this.mapping.GetAlias(t));
            return this.mapping.GetTables(entity).Sort(t => this.mapping.IsExtensionTable(t) ? lookup[this.mapping.GetExtensionRelatedAlias(t)] : null);
        }

        public override EntityExpression GetEntityExpression(Expression root, MappingEntity entity)
        {
            // must be some complex type constructed from multiple columns
            var assignments = new List<EntityAssignment>();
            foreach (MemberInfo mi in this.mapping.GetMappedMembers(entity))
            {
                if (!this.mapping.IsAssociationRelationship(entity, mi))
                {
                    Expression me;
                    if (this.mapping.IsNestedEntity(entity, mi))
                    {
                        me = this.GetEntityExpression(root, this.mapping.GetRelatedEntity(entity, mi));
                    }
                    else
                    {
                        me = this.GetMemberExpression(root, entity, mi);
                    }
                    if (me != null)
                    {
                        assignments.Add(new EntityAssignment(mi, me));
                    }
                }
            }

            return new EntityExpression(entity, this.BuildEntityExpression(entity, assignments));
        }

        public override Expression GetMemberExpression(Expression root, MappingEntity entity, MemberInfo member)
        {
            if (this.mapping.IsNestedEntity(entity, member))
            {
                MappingEntity subEntity = this.mapping.GetRelatedEntity(entity, member);
                return this.GetEntityExpression(root, subEntity);
            }
            else 
            {
                return base.GetMemberExpression(root, entity, member);
            }
        }

        public override ProjectionExpression GetQueryExpression(MappingEntity entity)
        {
            var tables = this.mapping.GetTables(entity);
            if (tables.Count <= 1)
            {
                return base.GetQueryExpression(entity);
            }

            var aliases = new Dictionary<string, TableAlias>();
            MappingTable rootTable = tables.Single(ta => !this.mapping.IsExtensionTable(ta));
            var tex = new TableExpression(new TableAlias(), entity, this.mapping.GetTableName(rootTable));
            aliases.Add(this.mapping.GetAlias(rootTable), tex.Alias);
            Expression source = tex;

            foreach (MappingTable table in tables.Where(t => this.mapping.IsExtensionTable(t)))
            {
                TableAlias joinedTableAlias = new TableAlias();
                string extensionAlias = this.mapping.GetAlias(table);
                aliases.Add(extensionAlias, joinedTableAlias);

                List<string> keyColumns = this.mapping.GetExtensionKeyColumnNames(table).ToList();
                List<MemberInfo> relatedMembers = this.mapping.GetExtensionRelatedMembers(table).ToList();
                string relatedAlias = this.mapping.GetExtensionRelatedAlias(table);

                TableAlias relatedTableAlias;
                aliases.TryGetValue(relatedAlias, out relatedTableAlias);

                TableExpression joinedTex = new TableExpression(joinedTableAlias, entity, this.mapping.GetTableName(table));

                Expression cond = null;
                for (int i = 0, n = keyColumns.Count; i < n; i++)
                {
                    var memberType = TypeHelper.GetMemberType(relatedMembers[i]);
                    var colType = this.GetColumnType(entity, relatedMembers[i]);
                    var relatedColumn = new ColumnExpression(memberType, colType, relatedTableAlias, this.mapping.GetColumnName(entity, relatedMembers[i]));
                    var joinedColumn = new ColumnExpression(memberType, colType, joinedTableAlias, keyColumns[i]);
                    var eq = joinedColumn.Equal(relatedColumn);
                    cond = (cond != null) ? cond.And(eq) : eq;
                }

                source = new JoinExpression(JoinType.SingletonLeftOuter, source, joinedTex, cond);
            }

            var columns = new List<ColumnDeclaration>();
            this.GetColumns(entity, aliases, columns);
            SelectExpression root = new SelectExpression(new TableAlias(), columns, source, null);
            var existingAliases = aliases.Values.ToArray();

            Expression projector = this.GetEntityExpression(root, entity);
            var selectAlias = new TableAlias();
            var pc = ColumnProjector.ProjectColumns(this.Translator.Linguist.Language, projector, null, selectAlias, root.Alias);
            var proj = new ProjectionExpression(
                new SelectExpression(selectAlias, pc.Columns, root, null),
                pc.Projector
                );

            return (ProjectionExpression)this.Translator.Police.ApplyPolicy(proj, entity.ElementType);
        }

        private void GetColumns(MappingEntity entity, Dictionary<string, TableAlias> aliases, List<ColumnDeclaration> columns)
        {
            foreach (MemberInfo mi in this.mapping.GetMappedMembers(entity))
            {
                if (!this.mapping.IsAssociationRelationship(entity, mi))
                {
                    if (this.mapping.IsNestedEntity(entity, mi))
                    {
                        this.GetColumns(this.mapping.GetRelatedEntity(entity, mi), aliases, columns);
                    }
                    else if (this.mapping.IsColumn(entity, mi))
                    {
                        string name = this.mapping.GetColumnName(entity, mi);
                        string aliasName = this.mapping.GetAlias(entity, mi);
                        TableAlias alias;
                        aliases.TryGetValue(aliasName, out alias);
                        var colType = this.GetColumnType(entity, mi);
                        ColumnExpression ce = new ColumnExpression(TypeHelper.GetMemberType(mi), colType, alias, name);
                        ColumnDeclaration cd = new ColumnDeclaration(name, ce, colType);
                        columns.Add(cd);
                    }
                }
            }
        }

        public override Expression GetInsertExpression(MappingEntity entity, Expression instance, LambdaExpression selector)
        {
            var tables = this.mapping.GetTables(entity);
            if (tables.Count < 2)
            {
                return base.GetInsertExpression(entity, instance, selector);
            }

            var commands = new List<Expression>();

            var map = this.GetDependentGeneratedColumns(entity);
            var vexMap = new Dictionary<MemberInfo, Expression>();

            foreach (var table in this.GetDependencyOrderedTables(entity))
            {
                var tableAlias = new TableAlias();
                var tex = new TableExpression(tableAlias, entity, this.mapping.GetTableName(table));
                var assignments = this.GetColumnAssignments(tex, instance, entity,
                    (e, m) => this.mapping.GetAlias(e, m) == this.mapping.GetAlias(table) && !this.mapping.IsGenerated(e, m),
                    vexMap
                    );
                var totalAssignments = assignments.Concat(
                    this.GetRelatedColumnAssignments(tex, entity, table, vexMap)
                    );
                commands.Add(new InsertCommand(tex, totalAssignments));

                List<MemberInfo> members;
                if (map.TryGetValue(this.mapping.GetAlias(table), out members))
                {
                    var d = this.GetDependentGeneratedVariableDeclaration(entity, table, members, instance, vexMap);
                    commands.Add(d);
                }
            }

            if (selector != null)
            {
                commands.Add(this.GetInsertResult(entity, instance, selector, vexMap));
            }

            return new BlockCommand(commands);
        }

        private Dictionary<string, List<MemberInfo>> GetDependentGeneratedColumns(MappingEntity entity)
        {
            return
                (from xt in this.mapping.GetTables(entity).Where(t => this.mapping.IsExtensionTable(t))
                 group xt by this.mapping.GetExtensionRelatedAlias(xt))
                .ToDictionary(
                    g => g.Key,
                    g => g.SelectMany(xt => this.mapping.GetExtensionRelatedMembers(xt)).Distinct().ToList()
                );
        }

        // make a variable declaration / initialization for dependent generated values
        private CommandExpression GetDependentGeneratedVariableDeclaration(MappingEntity entity, MappingTable table, List<MemberInfo> members, Expression instance, Dictionary<MemberInfo, Expression> map)
        {
            // first make command that retrieves the generated ids if any
            DeclarationCommand genIdCommand = null;
            var generatedIds = this.mapping.GetMappedMembers(entity).Where(m => this.mapping.IsPrimaryKey(entity, m) && this.mapping.IsGenerated(entity, m)).ToList();
            if (generatedIds.Count > 0)
            {
                genIdCommand = this.GetGeneratedIdCommand(entity, members, map);

                // if that's all there is then just return the generated ids
                if (members.Count == generatedIds.Count)
                {
                    return genIdCommand;
                }
            }

            // next make command that retrieves the generated members
            // only consider members that were not generated ids
            members = members.Except(generatedIds).ToList();

            var tableAlias = new TableAlias();
            var tex = new TableExpression(tableAlias, entity, this.mapping.GetTableName(table));

            Expression where = null;
            if (generatedIds.Count > 0)
            {
                where = generatedIds.Select((m, i) =>
                    this.GetMemberExpression(tex, entity, m).Equal(map[m])
                    ).Aggregate((x, y) => x.And(y));
            }
            else
            {
                where = this.GetIdentityCheck(tex, entity, instance);
            }

            TableAlias selectAlias = new TableAlias();
            var columns = new List<ColumnDeclaration>();
            var variables = new List<VariableDeclaration>();
            foreach (var mi in members)
            {
                ColumnExpression col = (ColumnExpression)this.GetMemberExpression(tex, entity, mi);
                columns.Add(new ColumnDeclaration(this.mapping.GetColumnName(entity, mi), col, col.QueryType));
                ColumnExpression vcol = new ColumnExpression(col.Type, col.QueryType, selectAlias, col.Name);
                variables.Add(new VariableDeclaration(mi.Name, col.QueryType, vcol));
                map.Add(mi, new VariableExpression(mi.Name, col.Type, col.QueryType));
            }

            var genMembersCommand = new DeclarationCommand(variables, new SelectExpression(selectAlias, columns, tex, where));

            if (genIdCommand != null)
            {
                return new BlockCommand(genIdCommand, genMembersCommand);
            }

            return genMembersCommand;
        }

        private IEnumerable<ColumnAssignment> GetColumnAssignments(
            Expression table, Expression instance, MappingEntity entity,
            Func<MappingEntity, MemberInfo, bool> fnIncludeColumn,
            Dictionary<MemberInfo, Expression> map)
        {
            foreach (var m in this.mapping.GetMappedMembers(entity))
            {
                if (this.mapping.IsColumn(entity, m) && fnIncludeColumn(entity, m))
                {
                    yield return new ColumnAssignment(
                        (ColumnExpression)this.GetMemberExpression(table, entity, m),
                        this.GetMemberAccess(instance, m, map)
                        );
                }
                else if (this.mapping.IsNestedEntity(entity, m))
                {
                    var assignments = this.GetColumnAssignments(
                        table, 
                        Expression.MakeMemberAccess(instance, m), 
                        this.mapping.GetRelatedEntity(entity, m), 
                        fnIncludeColumn, 
                        map
                        );
                    foreach (var ca in assignments)
                    {
                        yield return ca;
                    }
                }
            }
        }

        private IEnumerable<ColumnAssignment> GetRelatedColumnAssignments(Expression expr, MappingEntity entity, MappingTable table, Dictionary<MemberInfo, Expression> map)
        {
            if (this.mapping.IsExtensionTable(table))
            {
                var keyColumns = this.mapping.GetExtensionKeyColumnNames(table).ToArray();
                var relatedMembers = this.mapping.GetExtensionRelatedMembers(table).ToArray();
                for (int i = 0, n = keyColumns.Length; i < n; i++)
                {
                    MemberInfo member = relatedMembers[i];
                    Expression exp = map[member];
                    yield return new ColumnAssignment((ColumnExpression)this.GetMemberExpression(expr, entity, member), exp);
                }
            }
        }

        private Expression GetMemberAccess(Expression instance, MemberInfo member, Dictionary<MemberInfo, Expression> map)
        {
            Expression exp;
            if (map == null || !map.TryGetValue(member, out exp))
            {
                exp = Expression.MakeMemberAccess(instance, member);
            }
            return exp;
        }

        public override Expression GetUpdateExpression(MappingEntity entity, Expression instance, LambdaExpression updateCheck, LambdaExpression selector, Expression @else)
        {
            var tables = this.mapping.GetTables(entity);
            if (tables.Count < 2)
            {
                return base.GetUpdateExpression(entity, instance, updateCheck, selector, @else);
            }

            var commands = new List<Expression>();
            foreach (var table in this.GetDependencyOrderedTables(entity))
            {
                TableExpression tex = new TableExpression(new TableAlias(), entity, this.mapping.GetTableName(table));
                var assignments = this.GetColumnAssignments(tex, instance, entity, (e, m) => this.mapping.GetAlias(e, m) == this.mapping.GetAlias(table) && this.mapping.IsUpdatable(e, m), null);
                var where = this.GetIdentityCheck(tex, entity, instance);
                commands.Add(new UpdateCommand(tex, where, assignments));
            }

            if (selector != null)
            {
                commands.Add(
                    new IFCommand(
                        this.Translator.Linguist.Language.GetRowsAffectedExpression(commands[commands.Count-1]).GreaterThan(Expression.Constant(0)),
                        this.GetUpdateResult(entity, instance, selector),
                        @else
                        )
                    );
            }
            else if (@else != null)
            {
                commands.Add(
                    new IFCommand(
                        this.Translator.Linguist.Language.GetRowsAffectedExpression(commands[commands.Count-1]).LessThanOrEqual(Expression.Constant(0)),
                        @else,
                        null
                        )
                    );
            }

            Expression block = new BlockCommand(commands);

            if (updateCheck != null)
            {
                var test = this.GetEntityStateTest(entity, instance, updateCheck);
                return new IFCommand(test, block, null);
            }

            return block;
        }

        private Expression GetIdentityCheck(TableExpression root, MappingEntity entity, Expression instance, MappingTable table)
        {
            if (this.mapping.IsExtensionTable(table))
            {
                var keyColNames = this.mapping.GetExtensionKeyColumnNames(table).ToArray();
                var relatedMembers = this.mapping.GetExtensionRelatedMembers(table).ToArray();

                Expression where = null;
                for (int i = 0, n = keyColNames.Length; i < n; i++)
                {
                    var relatedMember = relatedMembers[i];
                    var cex = new ColumnExpression(TypeHelper.GetMemberType(relatedMember), this.GetColumnType(entity, relatedMember), root.Alias, keyColNames[n]);
                    var nex = this.GetMemberExpression(instance, entity, relatedMember);
                    var eq = cex.Equal(nex);
                    where = (where != null) ? where.And(eq) : where;
                }
                return where;
            }
            else
            {
                return base.GetIdentityCheck(root, entity, instance);
            }
        }

        public override Expression GetDeleteExpression(MappingEntity entity, Expression instance, LambdaExpression deleteCheck)
        {
            var tables = this.mapping.GetTables(entity);
            if (tables.Count < 2)
            {
                return base.GetDeleteExpression(entity, instance, deleteCheck);
            }

            var commands = new List<Expression>();
            foreach (var table in this.GetDependencyOrderedTables(entity).Reverse())
            {
                TableExpression tex = new TableExpression(new TableAlias(), entity, this.mapping.GetTableName(table));
                var where = this.GetIdentityCheck(tex, entity, instance);
                commands.Add(new DeleteCommand(tex, where));
            }

            Expression block = new BlockCommand(commands);

            if (deleteCheck != null)
            {
                var test = this.GetEntityStateTest(entity, instance, deleteCheck);
                return new IFCommand(test, block, null);
            }

            return block;
        }
    }
}