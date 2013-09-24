// Copyright (c) Microsoft Corporation.  All rights reserved.
// This source code is made available under the terms of the Microsoft Public License (MS-PL)

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using IQToolkit.Data.Common;
using IQToolkit.Data.Mapping;

namespace IQToolkit.Data
{
    public interface IReadEntityProvider : IQueryProvider
    {
        IReadEntityTable<T> GetTable<T>(string tableId);
        IReadEntityTable GetTable(Type type, string tableId);
        bool CanBeEvaluatedLocally(Expression expression);
        bool CanBeParameter(Expression expression);
    }

    public interface IReadEntityTable : IQueryable
    {
        new IReadEntityProvider Provider { get; }
        string TableId { get; }
        object GetById(object id);
    }

    public interface IReadEntityTable<out T> : IQueryable<T>, IReadEntityTable
    {
        new T GetById(object id);
    }

    /// <summary>
    /// A LINQ IQueryable query provider that executes queries over a connection.
    /// </summary>
    public abstract class ReadEntityProvider : QueryProvider, IReadEntityProvider, ICreateExecutor
    {
        readonly QueryLanguage language;
        readonly QueryMapping mapping;
        QueryPolicy policy;
        readonly Dictionary<MappingEntity, IReadEntityTable> tables = new Dictionary<MappingEntity, IReadEntityTable>();

        protected ReadEntityProvider(QueryLanguage language, QueryMapping mapping, QueryPolicy policy = null)
        {
            if (language == null)
                throw new InvalidOperationException("Language not specified");
            if (mapping == null)
                throw new InvalidOperationException("Mapping not specified");
            if (policy == null)
                throw new InvalidOperationException("Policy not specified");
            this.language = language;
            this.mapping = mapping;
            this.policy = policy;
        }

        public QueryMapping Mapping
        {
            get { return this.mapping; }
        }

        public QueryLanguage Language
        {
            get { return this.language; }
        }

        public QueryPolicy Policy
        {
            get { return this.policy; }
            set { this.policy = value ?? QueryPolicy.Default; }
        }

        public TextWriter Log { get; set; }

        public QueryCache Cache { get; set; }

        public IReadEntityTable GetTable(MappingEntity entity)
        {
            IReadEntityTable table;
            if (!this.tables.TryGetValue(entity, out table))
            {
                table = this.CreateTable(entity);
                this.tables.Add(entity, table);
            }
            return table;
        }

        protected virtual IReadEntityTable CreateTable(MappingEntity entity)
        {
            return (IReadEntityTable)Activator.CreateInstance(
                typeof(ReadEntityTable<>).MakeGenericType(entity.ElementType),
                new object[] { this, entity }
                );
        }

        public virtual IReadEntityTable<T> GetTable<T>()
        {
            return GetTable<T>(null);
        }

        public virtual IReadEntityTable<T> GetTable<T>(string tableId)
        {
            return (IReadEntityTable<T>)this.GetTable(typeof(T), tableId);
        }

        public virtual IReadEntityTable GetTable(Type type)
        {
            return GetTable(type, null);
        }

        public virtual IReadEntityTable GetTable(Type type, string tableId)
        {
            return this.GetTable(this.Mapping.GetEntity(type, tableId));
        }

        public bool CanBeEvaluatedLocally(Expression expression)
        {
            return this.Mapping.CanBeEvaluatedLocally(expression);
        }

        public virtual bool CanBeParameter(Expression expression)
        {
            Type type = TypeHelper.GetNonNullableType(expression.Type);
            switch (Type.GetTypeCode(type))
            {
                case TypeCode.Object:
                    if (expression.Type == typeof(Byte[]) ||
                        expression.Type == typeof(Char[]))
                        return true;
                    return false;
                default:
                    return true;
            }
        }

        protected abstract QueryExecutor CreateExecutor();

        QueryExecutor ICreateExecutor.CreateExecutor()
        {
            return this.CreateExecutor();
        }

        public class ReadEntityTable<T> : Query<T>, IReadEntityTable<T>, IHaveMappingEntity
        {
            readonly MappingEntity entity;
            readonly ReadEntityProvider provider;

            public ReadEntityTable(ReadEntityProvider provider, MappingEntity entity)
                : base(provider, typeof(IReadEntityTable<T>))
            {
                this.provider = provider;
                this.entity = entity;
            }

            public MappingEntity Entity
            {
                get { return this.entity; }
            }

            new public IReadEntityProvider Provider
            {
                get { return this.provider; }
            }

            public string TableId
            {
                get { return this.entity.TableId; }
            }

            public Type EntityType
            {
                get { return this.entity.EntityType; }
            }

            public T GetById(object id)
            {
                var dbProvider = this.Provider;
                if (dbProvider != null)
                {
                    IEnumerable<object> keys = id as IEnumerable<object>;
                    if (keys == null)
                        keys = new object[] { id };
                    Expression query = ((ReadEntityProvider)dbProvider).Mapping.GetPrimaryKeyQuery(this.entity, this.Expression, keys.Select(v => Expression.Constant(v)).ToArray());
                    return this.Provider.Execute<T>(query);
                }
                return default(T);
            }

            object IReadEntityTable.GetById(object id)
            {
                return this.GetById(id);
            }
        }

        public override string GetQueryText(Expression expression)
        {
            Expression plan = this.GetExecutionPlan(expression);
            var commands = CommandGatherer.Gather(plan).Select(c => c.CommandText).ToArray();
            return string.Join("\n\n", commands);
        }

        class CommandGatherer : DbExpressionVisitor
        {
            readonly List<QueryCommand> commands = new List<QueryCommand>();

            public static IEnumerable<QueryCommand> Gather(Expression expression)
            {
                var gatherer = new CommandGatherer();
                gatherer.Visit(expression);
                return gatherer.commands.AsReadOnly();
            }

            protected override Expression VisitConstant(ConstantExpression c)
            {
                QueryCommand qc = c.Value as QueryCommand;
                if (qc != null)
                {
                    this.commands.Add(qc);
                }
                return c;
            }
        }

        public string GetQueryPlan(Expression expression)
        {
            Expression plan = this.GetExecutionPlan(expression);
            return DbExpressionWriter.WriteToString(this.Language, plan);
        }

        protected virtual QueryTranslator CreateTranslator()
        {
            return new QueryTranslator(this.language, this.mapping, this.policy);
        }

        public abstract void DoConnected(Action action);
        public abstract int ExecuteCommand(string commandText);

        /// <summary>
        /// Execute the query expression (does translation, etc.)
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        public override object Execute(Expression expression)
        {
            LambdaExpression lambda = expression as LambdaExpression;

            if (lambda == null && this.Cache != null && expression.NodeType != ExpressionType.Constant)
            {
                return this.Cache.Execute(expression);
            }

            Expression plan = this.GetExecutionPlan(expression);

            if (lambda != null)
            {
                // compile & return the execution plan so it can be used multiple times
                LambdaExpression fn = Expression.Lambda(lambda.Type, plan, lambda.Parameters);
#if NOREFEMIT
                    return ExpressionEvaluator.CreateDelegate(fn);
#else
                return fn.Compile();
#endif
            }
            else
            {
                // compile the execution plan and invoke it
                Expression<Func<object>> efn = Expression.Lambda<Func<object>>(Expression.Convert(plan, typeof(object)));
#if NOREFEMIT
                    return ExpressionEvaluator.Eval(efn, new object[] { });
#else
                Func<object> fn = efn.Compile();
                return fn();
#endif
            }
        }

        /// <summary>
        /// Convert the query expression into an execution plan
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        public virtual Expression GetExecutionPlan(Expression expression)
        {
            // strip off lambda for now
            LambdaExpression lambda = expression as LambdaExpression;
            if (lambda != null)
                expression = lambda.Body;

            QueryTranslator translator = this.CreateTranslator();

            // translate query into client & server parts
            Expression translation = translator.Translate(expression);

            var parameters = lambda != null ? lambda.Parameters : null;
            Expression provider = Find(expression, parameters, typeof(EntityProvider));
            if (provider == null)
            {
                Expression rootQueryable = Find(expression, parameters, typeof(IQueryable));
                provider = Expression.Property(rootQueryable, typeof(IQueryable).GetProperty("Provider"));
            }

            return translator.Police.BuildExecutionPlan(translation, provider);
        }

        private static Expression Find(Expression expression, IEnumerable<ParameterExpression> parameters, Type type)
        {
            if (parameters != null)
            {
                Expression found = parameters.FirstOrDefault(p => type.IsAssignableFrom(p.Type));
                if (found != null)
                    return found;
            }
            return TypedSubtreeFinder.Find(expression, type);
        }

        public static QueryMapping GetMapping(string mappingId)
        {
            if (mappingId != null)
            {
                Type type = FindLoadedType(mappingId);
                if (type != null)
                {
                    return new AttributeMapping(type);
                }
                if (File.Exists(mappingId))
                {
                    return XmlMapping.FromXml(File.ReadAllText(mappingId));
                }
            }
            return new ImplicitMapping();
        }

        public static Type GetProviderType(string providerName)
        {
            if (!string.IsNullOrEmpty(providerName))
            {
                var type = FindInstancesIn(typeof(EntityProvider), providerName).FirstOrDefault();
                if (type != null)
                    return type;
            }
            return null;
        }

        private static Type FindLoadedType(string typeName)
        {
            foreach (var assem in AppDomain.CurrentDomain.GetAssemblies())
            {
                var type = assem.GetType(typeName, false, true);
                if (type != null)
                    return type;
            }
            return null;
        }

        private static IEnumerable<Type> FindInstancesIn(Type type, string assemblyName)
        {
            Assembly assembly = GetAssemblyForNamespace(assemblyName);
            if (assembly != null)
            {
                foreach (var atype in assembly.GetTypes())
                {
                    if (string.Compare(atype.Namespace, assemblyName, true) == 0
                        && type.IsAssignableFrom(atype))
                    {
                        yield return atype;
                    }
                }
            }
        }

        private static Assembly GetAssemblyForNamespace(string nspace)
        {
            foreach (var assem in AppDomain.CurrentDomain.GetAssemblies())
            {
                if (assem.FullName.Contains(nspace))
                {
                    return assem;
                }
            }

            return Load(nspace + ".dll");
        }

        private static Assembly Load(string name)
        {
            // try to load it.
            try
            {
                return Assembly.LoadFrom(name);
            }
            catch
            {
            }
            return null;
        }
    }
}
