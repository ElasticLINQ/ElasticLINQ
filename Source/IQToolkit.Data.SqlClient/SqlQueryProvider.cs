// Copyright (c) Microsoft Corporation.  All rights reserved.
// This source code is made available under the terms of the Microsoft Public License (MS-PL)

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace IQToolkit.Data.SqlClient
{
    using IQToolkit.Data.Common;

    public class SqlQueryProvider : DbEntityProvider
    {
        bool? allowMulitpleActiveResultSets;

        public SqlQueryProvider(SqlConnection connection, QueryMapping mapping, QueryPolicy policy)
            : base(connection, TSqlLanguage.Default, mapping, policy)
        {
        }

        public override DbEntityProvider New(DbConnection connection, QueryMapping mapping, QueryPolicy policy)
        {
            return new SqlQueryProvider((SqlConnection)connection, mapping, policy);
        }

        public static string GetConnectionString(string databaseFile)
        {
            return string.Format(@"Data Source=.\SQLEXPRESS;Integrated Security=True;Connect Timeout=30;User Instance=True;MultipleActiveResultSets=true;AttachDbFilename='{0}'", databaseFile);
        }

        public bool AllowsMultipleActiveResultSets
        {
            get
            {
                if (this.allowMulitpleActiveResultSets == null)
                {
                    var builder = new SqlConnectionStringBuilder(this.Connection.ConnectionString);
                    var result = builder["MultipleActiveResultSets"];
                    this.allowMulitpleActiveResultSets = (result != null && result.GetType() == typeof(bool) && (bool)result);
                }
                return (bool)this.allowMulitpleActiveResultSets;
            }
        }

        protected override QueryExecutor CreateExecutor()
        {
            return new Executor(this);
        }

        new class Executor : DbEntityProvider.Executor
        {
            SqlQueryProvider provider;

            public Executor(SqlQueryProvider provider)
                : base(provider)
            {
                this.provider = provider;
            }

            protected override bool BufferResultRows
            {
                get { return !this.provider.AllowsMultipleActiveResultSets; }
            }

            protected override void AddParameter(DbCommand command, QueryParameter parameter, object value)
            {
                DbQueryType sqlType = (DbQueryType)parameter.QueryType;
                if (sqlType == null)
                    sqlType = (DbQueryType)this.Provider.Language.TypeSystem.GetColumnType(parameter.Type);
                int len = sqlType.Length;
                if (len == 0 && DbTypeSystem.IsVariableLength(sqlType.SqlDbType))
                {
                    len = Int32.MaxValue;
                }
                var p = ((SqlCommand)command).Parameters.Add("@" + parameter.Name, sqlType.SqlDbType, len);
                if (sqlType.Precision != 0)
                    p.Precision = (byte)sqlType.Precision;
                if (sqlType.Scale != 0)
                    p.Scale = (byte)sqlType.Scale;
                p.Value = value ?? DBNull.Value;
            }

            public override IEnumerable<int> ExecuteBatch(QueryCommand query, IEnumerable<object[]> paramSets, int batchSize, bool stream)
            {
                this.StartUsingConnection();
                try
                {
                    var result = this.ExecuteBatch(query, paramSets, batchSize);
                    if (!stream || this.ActionOpenedConnection)
                    {
                        return result.ToList();
                    }
                    else
                    {
                        return new EnumerateOnce<int>(result);
                    }
                }
                finally
                {
                    this.StopUsingConnection();
                }
            }

            private IEnumerable<int> ExecuteBatch(QueryCommand query, IEnumerable<object[]> paramSets, int batchSize)
            {
                SqlCommand cmd = (SqlCommand)this.GetCommand(query, null);
                DataTable dataTable = new DataTable();
                for (int i = 0, n = query.Parameters.Count; i < n; i++)
                {
                    var qp = query.Parameters[i];
                    cmd.Parameters[i].SourceColumn = qp.Name;
                    dataTable.Columns.Add(qp.Name, TypeHelper.GetNonNullableType(qp.Type));
                }
                SqlDataAdapter dataAdapter = new SqlDataAdapter();
                dataAdapter.InsertCommand = cmd;
                dataAdapter.InsertCommand.UpdatedRowSource = UpdateRowSource.None;
                dataAdapter.UpdateBatchSize = batchSize;

                this.LogMessage("-- Start SQL Batching --");
                this.LogMessage("");
                this.LogCommand(query, null);

                IEnumerator<object[]> en = paramSets.GetEnumerator();
                using (en)
                {
                    bool hasNext = true;
                    while (hasNext)
                    {
                        int count = 0;
                        for (; count < dataAdapter.UpdateBatchSize && (hasNext = en.MoveNext()); count++)
                        {
                            var paramValues = en.Current;
                            dataTable.Rows.Add(paramValues);
                            this.LogParameters(query, paramValues);
                            this.LogMessage("");
                        }
                        if (count > 0)
                        {
                            int n = dataAdapter.Update(dataTable);
                            for (int i = 0; i < count; i++)
                            {
                                yield return (i < n) ? 1 : 0;
                            }
                            dataTable.Rows.Clear();
                        }
                    }
                }

                this.LogMessage(string.Format("-- End SQL Batching --"));
                this.LogMessage("");
            }
        }
    }
}