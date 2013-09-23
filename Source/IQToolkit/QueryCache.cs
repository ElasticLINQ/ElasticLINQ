using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading;

namespace IQToolkit
{
    public class QueryCache
    {
        MostRecentlyUsedCache<QueryCompiler.CompiledQuery> cache;
        static readonly Func<QueryCompiler.CompiledQuery, QueryCompiler.CompiledQuery, bool> fnCompareQueries = CompareQueries;
        static readonly Func<object, object, bool> fnCompareValues = CompareConstantValues;

        public QueryCache(int maxSize)
        {
            this.cache = new MostRecentlyUsedCache<QueryCompiler.CompiledQuery>(maxSize, fnCompareQueries);
        }

        private static bool CompareQueries(QueryCompiler.CompiledQuery x, QueryCompiler.CompiledQuery y)
        {
            return ExpressionComparer.AreEqual(x.Query, y.Query, fnCompareValues);
        }

        private static bool CompareConstantValues(object x, object y)
        {
            if (x == y) return true;
            if (x == null || y == null) return false;
            if (x is IQueryable && y is IQueryable && x.GetType() == y.GetType()) return true;
            return object.Equals(x, y);
        }

        public object Execute(Expression query)
        {
            object[] args;
            var cached = this.Find(query, true, out args);
            return cached.Invoke(args);
        }

        public object Execute(IQueryable query)
        {
            return this.Equals(query.Expression);
        }

        public IEnumerable<T> Execute<T>(IQueryable<T> query)
        {
            return (IEnumerable<T>)this.Execute(query.Expression);
        }

        public int Count
        {
            get { return this.cache.Count; }
        }

        public void Clear()
        {
            this.cache.Clear();
        }

        public bool Contains(Expression query)
        {
            object[] args;
            return this.Find(query, false, out args) != null;
        }

        public bool Contains(IQueryable query)
        {
            return this.Contains(query.Expression);
        }

        private QueryCompiler.CompiledQuery Find(Expression query, bool add, out object[] args)
        {
            var pq = this.Parameterize(query, out args);
            var cq = new QueryCompiler.CompiledQuery(pq);
            QueryCompiler.CompiledQuery cached;
            this.cache.Lookup(cq, add, out cached);
            return cached;
        }

        private LambdaExpression Parameterize(Expression query, out object[] arguments)
        {
            IQueryProvider provider = this.FindProvider(query);
            if (provider == null)
            {
                throw new ArgumentException("Cannot deduce query provider from query");
            }

            var ep = provider as IEntityProvider;
            Func<Expression, bool> fn = ep != null ? (Func<Expression, bool>)ep.CanBeEvaluatedLocally : null;
            List<ParameterExpression> parameters = new List<ParameterExpression>();
            List<object> values = new List<object>();

            var body = PartialEvaluator.Eval(query, fn, c =>
            {
                bool isQueryRoot = c.Value is IQueryable;
                if (!isQueryRoot && ep != null && !ep.CanBeParameter(c))
                    return c;
                var p = Expression.Parameter(c.Type, "p" + parameters.Count);
                parameters.Add(p);
                values.Add(c.Value);
                // if query root then parameterize but don't replace in the tree 
                if (isQueryRoot)
                    return c;
                return p;
            });

            if (body.Type != typeof(object))
                body = Expression.Convert(body, typeof(object));

            arguments = values.ToArray();
            if (arguments.Length < 5)
            {
                return Expression.Lambda(body, parameters.ToArray());
            }
            else
            {
                arguments = new object[] { arguments };
                return ExplicitToObjectArray.Rewrite(body, parameters);
            }
        }

        private IQueryProvider FindProvider(Expression expression)
        {
            ConstantExpression root = TypedSubtreeFinder.Find(expression, typeof(IQueryProvider)) as ConstantExpression;
            if (root == null)
            {
                root = TypedSubtreeFinder.Find(expression, typeof(IQueryable)) as ConstantExpression;
            }
            if (root != null)
            {
                IQueryProvider provider = root.Value as IQueryProvider;
                if (provider == null)
                {
                    IQueryable query = root.Value as IQueryable;
                    if (query != null)
                    {
                        provider = query.Provider;
                    }
                }
                return provider;
            }
            return null;
        }


        class ExplicitToObjectArray : ExpressionVisitor
        {
            IList<ParameterExpression> parameters;
            ParameterExpression array = Expression.Parameter(typeof(object[]), "array");

            private ExplicitToObjectArray(IList<ParameterExpression> parameters)
            {
                this.parameters = parameters;
            }

            internal static LambdaExpression Rewrite(Expression body, IList<ParameterExpression> parameters)
            {
                var visitor = new ExplicitToObjectArray(parameters);
                return Expression.Lambda(visitor.Visit(body), visitor.array);                  
            }

            protected override Expression VisitParameter(ParameterExpression p)
            {
                for (int i = 0, n = this.parameters.Count; i < n; i++)
                {
                    if (this.parameters[i] == p)
                    {
                        return Expression.Convert(Expression.ArrayIndex(this.array, Expression.Constant(i)), p.Type);
                    }
                }
                return p;
            }
        }
    }
}
