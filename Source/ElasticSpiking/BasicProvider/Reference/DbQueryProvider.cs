using ElasticSpiking.BasicProvider.Reference;
using System;
using System.Data.Common;
using System.Linq.Expressions;
using System.Reflection;

namespace ElasticSpiking.BasicProvider
{
    public class DbQueryProvider : QueryProvider
    {
        private readonly DbConnection connection;

        public DbQueryProvider(DbConnection connection)
        {
            this.connection = connection;
        }

        public override object Execute(Expression expression)
        {
            var translated = Translate(expression);

            var cmd = connection.CreateCommand();
            cmd.CommandText = translated.CommandText;
            var reader = cmd.ExecuteReader();

            var elementType = TypeSystem.GetElementType(expression.Type);

            if (translated.Projector != null)
            {
                var projector = translated.Projector.Compile();
                return Activator.CreateInstance(
                    typeof(DbProjectionReader<>).MakeGenericType(elementType),
                    BindingFlags.Instance | BindingFlags.NonPublic, null,
                    new object[] { reader, projector },
                    null);
            }

            return Activator.CreateInstance(
                typeof(DbObjectReader<>).MakeGenericType(elementType),
                BindingFlags.Instance | BindingFlags.NonPublic, null,
                new object[] { reader },
                null);
        }

        public override string GetQueryText(Expression expression)
        {
            return Translate(expression).CommandText;
        }

        private static TranslateResult Translate(Expression expression)
        {
            expression = Evaluator.PartialEval(expression);
            return new DbQueryTranslator().Translate(expression);
        }
    }
}