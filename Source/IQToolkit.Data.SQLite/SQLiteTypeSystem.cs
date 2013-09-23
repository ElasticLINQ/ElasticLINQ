using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace IQToolkit.Data.SQLite
{
    using IQToolkit.Data.Common;

    public class SQLiteTypeSystem : DbTypeSystem
    {
        public override SqlDbType GetSqlType(string typeName)
        {
            if (string.Compare(typeName, "TEXT", true) == 0 ||
                string.Compare(typeName, "CHAR", true) == 0 ||
                string.Compare(typeName, "CLOB", true) == 0 ||
                string.Compare(typeName, "VARYINGCHARACTER", true) == 0 ||
                string.Compare(typeName, "NATIONALVARYINGCHARACTER", true) == 0)
            {
                return SqlDbType.VarChar;
            }
            else if (string.Compare(typeName, "INT", true) == 0 ||
                string.Compare(typeName, "INTEGER", true) == 0)
            {
                return SqlDbType.BigInt;
            }
            else if (string.Compare(typeName, "BLOB", true) == 0)
            {
                return SqlDbType.Binary;
            }
            else if (string.Compare(typeName, "BOOLEAN", true) == 0)
            {
                return SqlDbType.Bit;
            }
            else if (string.Compare(typeName, "NUMERIC", true) == 0)
            {
                return SqlDbType.Decimal;
            }
            else
            {
                return base.GetSqlType(typeName);
            }
        }

        public override string GetVariableDeclaration(QueryType type, bool suppressSize)
        {
            StringBuilder sb = new StringBuilder();
            DbQueryType sqlType = (DbQueryType)type;
            SqlDbType sqlDbType = sqlType.SqlDbType;

            switch (sqlDbType)
            {
                case SqlDbType.BigInt:
                case SqlDbType.SmallInt:
                case SqlDbType.Int:
                case SqlDbType.TinyInt:
                    sb.Append("INTEGER");
                    break;
                case SqlDbType.Bit:
                    sb.Append("BOOLEAN");
                    break;
                case SqlDbType.SmallDateTime:
                    sb.Append("DATETIME");
                    break;
                case SqlDbType.Char:
                case SqlDbType.NChar:
                    sb.Append("CHAR");
                    if (type.Length > 0 && !suppressSize)
                    {
                        sb.Append("(");
                        sb.Append(type.Length);
                        sb.Append(")");
                    }
                    break;
                case SqlDbType.Variant:
                case SqlDbType.Binary:
                case SqlDbType.Image:
                case SqlDbType.UniqueIdentifier: //There is a setting to make it string, look at later
                    sb.Append("BLOB");
                    if (type.Length > 0 && !suppressSize)
                    {
                        sb.Append("(");
                        sb.Append(type.Length);
                        sb.Append(")");
                    }
                    break;
                case SqlDbType.Xml:
                case SqlDbType.NText:
                case SqlDbType.NVarChar:
                case SqlDbType.Text:
                case SqlDbType.VarBinary:
                case SqlDbType.VarChar:
                    sb.Append("TEXT");
                    if (type.Length > 0 && !suppressSize)
                    {
                        sb.Append("(");
                        sb.Append(type.Length);
                        sb.Append(")");
                    }
                    break;
                case SqlDbType.Decimal:
                case SqlDbType.Money:
                case SqlDbType.SmallMoney:
                    sb.Append("NUMERIC");
                    if (type.Precision != 0)
                    {
                        sb.Append("(");
                        sb.Append(type.Precision);
                        sb.Append(")");
                    }
                    break;
                case SqlDbType.Float:
                case SqlDbType.Real:
                    sb.Append("FLOAT");
                    if (type.Precision != 0)
                    {
                        sb.Append("(");
                        sb.Append(type.Precision);
                        sb.Append(")");
                    }
                    break;
                case SqlDbType.Date:
                case SqlDbType.DateTime:
                case SqlDbType.Timestamp:
                default:
                    sb.Append(sqlDbType);
                    break;
            }
            return sb.ToString();
        }
    }
}
