using System;
using System.Data;
using System.IO;
using FluentMigrator.Builders.Execute;

namespace FluentMigrator.Runner.Processors.DB2
{
	public sealed class DB2Processor : ProcessorBase
	{
		private readonly IDbFactory factory;
		public IDbConnection Connection { get; private set; }
		public IDbTransaction Transaction { get; private set; }
		public bool WasCommitted { get; private set; }

		public DB2Processor(IDbConnection connection, IMigrationGenerator generator, IAnnouncer announcer, IMigrationProcessorOptions options, IDbFactory factory)
			: base(generator, announcer, options)
		{
			this.factory = factory;
			Connection = connection;
			connection.Open();
			BeginTransaction();
		}

		public override string DatabaseType
		{
			get { return "DB2"; }
		}

		public override bool SchemaExists(string schemaName)
		{
			try {
				return Exists("SELECT 1 FROM SYSCAT.TABLES WHERE TABSCHEMA = '{0}'", FormatSqlEscape(schemaName));
			}
			catch (Exception e) {
				Console.WriteLine(e);
			}
			return false;
		}

		public override bool TableExists(string schemaName, string tableName)
		{
			try {
				return Exists("SELECT 1 FROM SYSCAT.TABLES WHERE tabname = '{0}'", FormatSqlEscape(tableName));
			}
			catch (Exception e) {
				Console.WriteLine(e);
			}
			return false;
		}

		public override bool ColumnExists(string schemaName, string tableName, string columnName)
		{
			return Exists("SELECT 1 FROM SYSCAT.COLUMNS WHERE TABNAME = '{0}' AND COLNAME = '{1}'", FormatSqlEscape(tableName), FormatSqlEscape(columnName));
		}

		public override bool ConstraintExists(string schemaName, string tableName, string constraintName)
		{
			return Exists("SELECT 1 FROM SYSCAT.CHECKS WHERE TABSCHEMA = '{0}' AND TABNAME = '{1}' AND CONSTNAME = '{2}'",
				FormatSqlEscape(schemaName), FormatSqlEscape(tableName), FormatSqlEscape(constraintName));
		}

		public override bool IndexExists(string schemaName, string tableName, string indexName)
		{
			return Exists("SELECT 1 FROM SYSCAT.INDEXES WHERE TABSCHEMA = '{0}' AND TABNAME = '{1}' AND INDNAME = '{2}'",
				FormatSqlEscape(schemaName), FormatSqlEscape(tableName),FormatSqlEscape(indexName));
		}

		public override DataSet ReadTableData(string schemaName, string tableName)
		{
			if (string.IsNullOrEmpty(schemaName))
				return Read("SELECT * FROM {0}", tableName.ToUpper());

			return Read("SELECT * FROM {0}.{1}",schemaName, tableName);
		}

		public override DataSet Read(string template, params object[] args)
		{
			if (Connection.State != ConnectionState.Open)
				Connection.Open();

			var ds = new DataSet();
			using (var command = factory.CreateCommand(String.Format(template, args), Connection, Transaction)) {
				var adapter = factory.CreateDataAdapter(command);
				adapter.Fill(ds);
				return ds;
			}
		}

		public override bool Exists(string template, params object[] args)
		{
			if (Connection.State != ConnectionState.Open)
				Connection.Open();

			using (var command = factory.CreateCommand(String.Format(template, args), Connection, Transaction))
			using (var reader = command.ExecuteReader()) {
				return reader.Read();
			}
		}

		public override void BeginTransaction()
		{
			Announcer.Say("Beginning Transaction");
			Transaction = Connection.BeginTransaction();
		}

		public override void CommitTransaction()
		{
			Announcer.Say("Committing Transaction");
			Transaction.Commit();
			WasCommitted = true;
			if (Connection.State != ConnectionState.Closed) {
				Connection.Close();
			}
		}

		public override void RollbackTransaction()
		{
			Announcer.Say("Rolling back transaction");
			Transaction.Rollback();
			WasCommitted = true;
			if (Connection.State != ConnectionState.Closed) {
				Connection.Close();
			}
		}

		public override void Execute(string template, params object[] args)
		{
			Process(String.Format(template, args));
		}

		protected override void Process(string sql)
		{
			Announcer.Sql(sql);

			if (Options.PreviewOnly || string.IsNullOrEmpty(sql))
				return;

			if (Connection.State != ConnectionState.Open) {
				Connection.Open();
			}

			ExecuteNonQuery(sql);
		}

		private void ExecuteNonQuery(string sql)
		{
			using (var command = factory.CreateCommand(sql, Connection, Transaction)) {
				try {
					command.CommandTimeout = Options.Timeout;
					command.ExecuteNonQuery();
				}
				catch (Exception ex) {
					using (var message = new StringWriter()) {
						message.WriteLine("An error occured executing the following sql:");
						message.WriteLine(sql);
						message.WriteLine("The error was {0}", ex.Message);

						throw new Exception(message.ToString(), ex);
					}
				}
			}
		}


		public override void Process(PerformDBOperationExpression expression)
		{
			if (Connection.State != ConnectionState.Open)
				Connection.Open();

			if (expression.Operation != null)
				expression.Operation(Connection, Transaction);
		}

		private static string FormatSqlEscape(string sql)
		{
			//all db2 catalog names are uppercase
			return sql.Replace("'", "''").ToUpper();
		}

	}
}
