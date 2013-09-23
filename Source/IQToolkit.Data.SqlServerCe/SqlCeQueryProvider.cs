// Copyright (c) Microsoft Corporation.  All rights reserved.
// This source code is made available under the terms of the Microsoft Public License (MS-PL)

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.Common;
using System.Data.SqlServerCe;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace IQToolkit.Data.SqlServerCe
{
    using IQToolkit.Data.Common;

    public class SqlCeQueryProvider : DbEntityProvider
    {
        public SqlCeQueryProvider(SqlCeConnection connection, QueryMapping mapping, QueryPolicy policy)
            : base(connection, SqlCeLanguage.Default, mapping, policy)
        {
        }

        public override DbEntityProvider New(DbConnection connection, QueryMapping mapping, QueryPolicy policy)
        {
            return new SqlCeQueryProvider((SqlCeConnection)connection, mapping, policy);
        }

        public static string GetConnectionString(string databaseFile)
        {
            return string.Format(@"Data Source='{0}'", databaseFile);
        }

        protected override QueryExecutor CreateExecutor()
        {
            return new Executor(this);
        }

        new class Executor : DbEntityProvider.Executor
        {
            public Executor(SqlCeQueryProvider provider)
                : base(provider)
            {
            }

            protected override void AddParameter(DbCommand command, QueryParameter parameter, object value)
            {
                DbQueryType sqlType = (DbQueryType)parameter.QueryType;
                if (sqlType == null)
                    sqlType = (DbQueryType)this.Provider.Language.TypeSystem.GetColumnType(parameter.Type);
                var p = ((SqlCeCommand)command).Parameters.Add("@" + parameter.Name, sqlType.SqlDbType, sqlType.Length);
                if (sqlType.Precision != 0)
                    p.Precision = (byte)sqlType.Precision;
                if (sqlType.Scale != 0)
                    p.Scale = (byte)sqlType.Scale;
                p.Value = value ?? DBNull.Value;
            }
        }
    }
}