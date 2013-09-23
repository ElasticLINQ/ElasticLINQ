// Copyright (c) Microsoft Corporation.  All rights reserved.
// This source code is made available under the terms of the Microsoft Public License (MS-PL)

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.Common;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using MySql.Data.MySqlClient;

namespace IQToolkit.Data.MySqlClient
{
    using IQToolkit.Data.Common;

    public class MySqlQueryProvider : DbEntityProvider
    {
        public MySqlQueryProvider(MySqlConnection connection, QueryMapping mapping, QueryPolicy policy)
            : base(connection, MySqlLanguage.Default, mapping, policy)
        {
        }

        public override DbEntityProvider New(DbConnection connection, QueryMapping mapping, QueryPolicy policy)
        {
            return new MySqlQueryProvider((MySqlConnection)connection, mapping, policy);
        }

        public static string GetConnectionString(string databaseName)
        {
            return string.Format(@"Server=127.0.0.1;Database={0}", databaseName);
        }

        protected override QueryExecutor CreateExecutor()
        {
            return new Executor(this);
        }

        new class Executor : DbEntityProvider.Executor
        {
            MySqlQueryProvider provider;

            public Executor(MySqlQueryProvider provider)
                : base(provider)
            {
                this.provider = provider;
            }

            protected override bool BufferResultRows
            {
                get { return true; }
            }

            protected override void AddParameter(DbCommand command, QueryParameter parameter, object value)
            {
                DbQueryType sqlType = (DbQueryType)parameter.QueryType;
                if (sqlType == null)
                    sqlType = (DbQueryType)this.provider.Language.TypeSystem.GetColumnType(parameter.Type);
                var p = ((MySqlCommand)command).Parameters.Add(parameter.Name, ToMySqlDbType(sqlType.SqlDbType), sqlType.Length);
                if (sqlType.Precision != 0)
                    p.Precision = (byte)sqlType.Precision;
                if (sqlType.Scale != 0)
                    p.Scale = (byte)sqlType.Scale;
                p.Value = value ?? DBNull.Value;
            }
        }

        public static MySqlDbType ToMySqlDbType(SqlDbType dbType)
        {
            switch (dbType)
            {
                case SqlDbType.BigInt:
                    return MySqlDbType.Int64;
                case SqlDbType.Binary:
                    return MySqlDbType.Binary;
                case SqlDbType.Bit:
                    return MySqlDbType.Bit;
                case SqlDbType.NChar:
                case SqlDbType.Char:
                    return MySqlDbType.Text;
                case SqlDbType.Date:
                    return MySqlDbType.Date;
                case SqlDbType.DateTime:
                case SqlDbType.SmallDateTime:
                    return MySqlDbType.DateTime;
                case SqlDbType.Decimal:
                    return MySqlDbType.Decimal;
                case SqlDbType.Float:
                    return MySqlDbType.Float;
                case SqlDbType.Image:
                    return MySqlDbType.LongBlob;
                case SqlDbType.Int:
                    return MySqlDbType.Int32;
                case SqlDbType.Money:
                case SqlDbType.SmallMoney:
                    return MySqlDbType.Decimal;
                case SqlDbType.NVarChar:
                case SqlDbType.VarChar:
                    return MySqlDbType.VarChar;
                case SqlDbType.SmallInt:
                    return MySqlDbType.Int16;
                case SqlDbType.NText:
                case SqlDbType.Text:
                    return MySqlDbType.LongText;
                case SqlDbType.Time:
                    return MySqlDbType.Time;
                case SqlDbType.Timestamp:
                    return MySqlDbType.Timestamp;
                case SqlDbType.TinyInt:
                    return MySqlDbType.Byte;
                case SqlDbType.UniqueIdentifier:
                    return MySqlDbType.Guid;
                case SqlDbType.VarBinary:
                    return MySqlDbType.VarBinary;
                case SqlDbType.Xml:
                    return MySqlDbType.Text;
                default:
                    throw new NotSupportedException(string.Format("The SQL type '{0}' is not supported", dbType));
            }
        }
    }
}
