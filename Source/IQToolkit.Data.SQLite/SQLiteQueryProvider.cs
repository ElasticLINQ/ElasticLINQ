using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Text;


namespace IQToolkit.Data.SQLite
{
    using IQToolkit.Data.Common;

    public class SQLiteQueryProvider : DbEntityProvider
    {
        Dictionary<QueryCommand, SQLiteCommand> commandCache = new Dictionary<QueryCommand, SQLiteCommand>();

        public SQLiteQueryProvider(SQLiteConnection connection, QueryMapping mapping, QueryPolicy policy)
            : base(connection, SQLiteLanguage.Default, mapping, policy)
        {
        }

        public static string GetConnectionString(string databaseFile)
        {
            return string.Format("Data Source={0};", databaseFile);
        }

        public static string GetConnectionString(string databaseFile, string password)
        {
            return string.Format("Data Source={0};Password={1};", databaseFile, password);
        }

        public static string GetConnectionString(string databaseFile, bool failIfMissing)
        {
            return string.Format("Data Source={0};FailIfMissing={1};", databaseFile, failIfMissing ? bool.TrueString : bool.FalseString);
        }

        public static string GetConnectionString(string databaseFile, string password, bool failIfMissing)
        {
            return string.Format("Data Source={0};Password={1};FailIfMissing={2};", databaseFile, password, failIfMissing ? bool.TrueString : bool.FalseString);
        }

        public override DbEntityProvider New(DbConnection connection, QueryMapping mapping, QueryPolicy policy)
        {
            return new SQLiteQueryProvider((SQLiteConnection)connection, mapping, policy);
        }

        protected override QueryExecutor CreateExecutor()
        {
            return new Executor(this);
        }

        new class Executor : DbEntityProvider.Executor
        {
            SQLiteQueryProvider provider;

            public Executor(SQLiteQueryProvider provider)
                : base(provider)
            {
                this.provider = provider;
            }

            protected override DbCommand GetCommand(QueryCommand query, object[] paramValues)
            {
                SQLiteCommand cmd;
                if (!this.provider.commandCache.TryGetValue(query, out cmd))
                {
                    cmd = (SQLiteCommand)this.provider.Connection.CreateCommand();
                    cmd.CommandText = query.CommandText;
                    this.SetParameterValues(query, cmd, paramValues);
                    cmd.Prepare();
                    this.provider.commandCache.Add(query, cmd);
                    if (this.provider.Transaction != null)
                    {
                        cmd = (SQLiteCommand)cmd.Clone();
                        cmd.Transaction = (SQLiteTransaction)this.provider.Transaction;
                    }
                }
                else
                {
                    cmd = (SQLiteCommand)cmd.Clone();
                    cmd.Transaction = (SQLiteTransaction)this.provider.Transaction;
                    this.SetParameterValues(query, cmd, paramValues);
                }
                return cmd;
            }

            protected override void AddParameter(DbCommand command, QueryParameter parameter, object value)
            {
                QueryType qt = parameter.QueryType;
                if (qt == null)
                    qt = this.provider.Language.TypeSystem.GetColumnType(parameter.Type);
                var p = ((SQLiteCommand)command).Parameters.Add(parameter.Name, ((DbQueryType)qt).DbType, qt.Length);
                if (qt.Length != 0)
                {
                    p.Size = qt.Length;
                }
                else if (qt.Scale != 0)
                {
                    p.Size = qt.Scale;
                }
                p.Value = value ?? DBNull.Value;
            }
        }
    }
}
