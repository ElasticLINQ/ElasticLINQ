// Copyright (c) Microsoft Corporation.  All rights reserved.
// This source code is made available under the terms of the Microsoft Public License (MS-PL)

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.Common;
using System.Data.OleDb;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace IQToolkit.Data.Access
{
    using IQToolkit.Data.Common;
    using IQToolkit.Data.OleDb;

    public class AccessQueryProvider : OleDb.OleDbQueryProvider
    {
        Dictionary<QueryCommand, OleDbCommand> commandCache = new Dictionary<QueryCommand, OleDbCommand>();

        public AccessQueryProvider(OleDbConnection connection, QueryMapping mapping, QueryPolicy policy)
            : base(connection, AccessLanguage.Default, mapping, policy)
        {
        }

        public override DbEntityProvider New(DbConnection connection, QueryMapping mapping, QueryPolicy policy)
        {
            return new AccessQueryProvider((OleDbConnection)connection, mapping, policy);
        }

        public static string GetConnectionString(string databaseFile) 
        {
            string dbLower = databaseFile.ToLower();
            if (dbLower.Contains(".mdb"))
            {
                return GetConnectionString(AccessOleDbProvider2000, databaseFile);
            }
            else if (dbLower.Contains(".accdb"))
            {
                return GetConnectionString(AccessOleDbProvider2007, databaseFile);
            }
            else
            {
                throw new InvalidOperationException(string.Format("Unrecognized file extension on database file '{0}'", databaseFile));
            }
        }

        private static string GetConnectionString(string provider, string databaseFile)
        {
            return string.Format("Provider={0};ole db services=0;Data Source={1}", provider, databaseFile);
        }

        public static readonly string AccessOleDbProvider2000 = "Microsoft.Jet.OLEDB.4.0";
        public static readonly string AccessOleDbProvider2007 = "Microsoft.ACE.OLEDB.12.0";

        protected override QueryExecutor CreateExecutor()
        {
            return new Executor(this);
        }

        public new class Executor : OleDbQueryProvider.Executor
        {
            AccessQueryProvider provider;

            public Executor(AccessQueryProvider provider)
                : base(provider)
            {
                this.provider = provider;
            }

            protected override DbCommand GetCommand(QueryCommand query, object[] paramValues)
            {
#if false
                OleDbCommand cmd;
                if (!this.provider.commandCache.TryGetValue(query, out cmd))
                {
                    cmd = (OleDbCommand)this.provider.Connection.CreateCommand();
                    cmd.CommandText = query.CommandText;
                    this.SetParameterValues(query, cmd, paramValues);
                    if (this.provider.Transaction != null)
                        cmd.Transaction = (OleDbTransaction)this.provider.Transaction;
                    cmd.Prepare();
                    this.provider.commandCache.Add(query, cmd);
                }
                else
                {
                    cmd = (OleDbCommand)cmd.Clone();
                    if (this.provider.Transaction != null)
                        cmd.Transaction = (OleDbTransaction)this.provider.Transaction;
                    this.SetParameterValues(query, cmd, paramValues);
                }
#else
                var cmd = (OleDbCommand)this.provider.Connection.CreateCommand();
                cmd.CommandText = query.CommandText;
                this.SetParameterValues(query, cmd, paramValues);
                if (this.provider.Transaction != null)
                    cmd.Transaction = (OleDbTransaction)this.provider.Transaction;

#endif
                return cmd;
            }

            protected override OleDbType GetOleDbType(QueryType type)
            {
                DbQueryType sqlType = type as DbQueryType;
                if (sqlType != null)
                {
                    return ToOleDbType(sqlType.SqlDbType);
                }
                return base.GetOleDbType(type);
            }
        }
    }
}