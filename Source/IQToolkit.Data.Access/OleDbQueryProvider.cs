using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Data;
using System.Data.Common;
using System.Data.OleDb;
using System.IO;

namespace IQToolkit.Data.OleDb
{
    using IQToolkit.Data.Common;

    public class OleDbQueryProvider : DbEntityProvider
    {
        public OleDbQueryProvider(OleDbConnection connection, QueryLanguage language, QueryMapping mapping, QueryPolicy policy)
            : base(connection, language, mapping, policy)
        {
        }

        protected override QueryExecutor CreateExecutor()
        {
            return new Executor(this);
        }

        public new class Executor : DbEntityProvider.Executor
        {
            OleDbQueryProvider provider;

            public Executor(OleDbQueryProvider provider)
                : base(provider)
            {
                this.provider = provider;
            }

            protected override void AddParameter(DbCommand command, QueryParameter parameter, object value)
            {
                QueryType qt = parameter.QueryType;
                if (qt == null)
                    qt = this.provider.Language.TypeSystem.GetColumnType(parameter.Type);
                var p = ((OleDbCommand)command).Parameters.Add(parameter.Name, this.GetOleDbType(qt), qt.Length);
                if (qt.Precision != 0)
                    p.Precision = (byte)qt.Precision;
                if (qt.Scale != 0)
                    p.Scale = (byte)qt.Scale;
                p.Value = value ?? DBNull.Value;
            }

            protected virtual OleDbType GetOleDbType(QueryType type)
            {
                return ToOleDbType(((DbQueryType)type).SqlDbType);
            }
        }

        public static OleDbType ToOleDbType(SqlDbType dbType)
        {
            switch (dbType)
            {
                case SqlDbType.BigInt:
                    return OleDbType.BigInt;
                case SqlDbType.Binary:
                    return OleDbType.Binary;
                case SqlDbType.Bit:
                    return OleDbType.Boolean;
                case SqlDbType.Char:
                    return OleDbType.Char;
                case SqlDbType.Date:
                    return OleDbType.Date;
                case SqlDbType.DateTime:
                case SqlDbType.SmallDateTime:
                case SqlDbType.DateTime2:
                case SqlDbType.DateTimeOffset:
                    return OleDbType.DBTimeStamp;
                case SqlDbType.Decimal:
                    return OleDbType.Decimal;
                case SqlDbType.Float:
                case SqlDbType.Real:
                    return OleDbType.Double;
                case SqlDbType.Image:
                    return OleDbType.LongVarBinary;
                case SqlDbType.Int:
                    return OleDbType.Integer;
                case SqlDbType.Money:
                case SqlDbType.SmallMoney:
                    return OleDbType.Currency;
                case SqlDbType.NChar:
                    return OleDbType.WChar;
                case SqlDbType.NText:
                    return OleDbType.LongVarChar;
                case SqlDbType.NVarChar:
                    return OleDbType.VarWChar;
                case SqlDbType.SmallInt:
                    return OleDbType.SmallInt;
                case SqlDbType.Text:
                    return OleDbType.LongVarChar;
                case SqlDbType.Time:
                    return OleDbType.DBTime;
                case SqlDbType.Timestamp:
                    return OleDbType.Binary;
                case SqlDbType.TinyInt:
                    return OleDbType.TinyInt;
                case SqlDbType.Udt:
                    return OleDbType.Variant;
                case SqlDbType.UniqueIdentifier:
                    return OleDbType.Guid;
                case SqlDbType.VarBinary:
                    return OleDbType.VarBinary;
                case SqlDbType.VarChar:
                    return OleDbType.VarChar;
                case SqlDbType.Variant:
                    return OleDbType.Variant;
                case SqlDbType.Xml:
                    return OleDbType.VarWChar;
                default:
                    throw new InvalidOperationException(string.Format("Unhandled sql type: {0}", dbType));
            }
        }

        public static OleDbType ToOleDbType(DbType type)
        {
            switch (type)
            {
                case DbType.AnsiString:
                    return OleDbType.VarChar;
                case DbType.AnsiStringFixedLength:
                    return OleDbType.Char;
                case DbType.Binary:
                    return OleDbType.Binary;
                case DbType.Boolean:
                    return OleDbType.Boolean;
                case DbType.Byte:
                    return OleDbType.UnsignedTinyInt;
                case DbType.Currency:
                    return OleDbType.Currency;
                case DbType.Date:
                    return OleDbType.Date;
                case DbType.DateTime:
                case DbType.DateTime2:
                case DbType.DateTimeOffset:
                    return OleDbType.DBTimeStamp;
                case DbType.Decimal:
                    return OleDbType.Decimal;
                case DbType.Double:
                    return OleDbType.Double;
                case DbType.Guid:
                    return OleDbType.Guid;
                case DbType.Int16:
                    return OleDbType.SmallInt;
                case DbType.Int32:
                    return OleDbType.Integer;
                case DbType.Int64:
                    return OleDbType.BigInt;
                case DbType.Object:
                    return OleDbType.Variant;
                case DbType.SByte:
                    return OleDbType.TinyInt;
                case DbType.Single:
                    return OleDbType.Single;
                case DbType.String:
                    return OleDbType.VarWChar;
                case DbType.StringFixedLength:
                    return OleDbType.WChar;
                case DbType.Time:
                    return OleDbType.DBTime;
                case DbType.UInt16:
                    return OleDbType.UnsignedSmallInt;
                case DbType.UInt32:
                    return OleDbType.UnsignedInt;
                case DbType.UInt64:
                    return OleDbType.UnsignedBigInt;
                case DbType.VarNumeric:
                    return OleDbType.Numeric;
                case DbType.Xml:
                    return OleDbType.VarWChar;
                default:
                    throw new InvalidOperationException(string.Format("Unhandled db type '{0}'.", type));
            }
        }
    }
}