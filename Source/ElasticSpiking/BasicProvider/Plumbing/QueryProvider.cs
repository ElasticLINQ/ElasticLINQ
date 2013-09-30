using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace ElasticSpiking.BasicProvider
{
    public abstract class QueryProvider : IQueryProvider
    {
        IQueryable<S> IQueryProvider.CreateQuery<S>(Expression expression)
        {
            return new Query<S>(this, expression);
        }

        IQueryable IQueryProvider.CreateQuery(Expression expression)
        {
            var elementType = TypeSystem.GetElementType(expression.Type);

            try
            {
                return (IQueryable)Activator.CreateInstance(typeof(Query<>).MakeGenericType(elementType),
                            new object[] { this, expression });
            }

            catch (TargetInvocationException tie)
            {
                throw tie.InnerException;
            }
        }


        S IQueryProvider.Execute<S>(Expression expression)
        {
            return (S)Execute(expression);
        }


        object IQueryProvider.Execute(Expression expression)
        {
            return Execute(expression);
        }

        public abstract string GetQueryText(Expression expression);

        public abstract object Execute(Expression expression);
    }
}