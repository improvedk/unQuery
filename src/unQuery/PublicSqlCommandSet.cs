using System;
using System.Data.SqlClient;
using System.Reflection;

namespace unQuery
{
	internal class PublicSqlCommandSet : IDisposable
	{
		private static readonly Type sqlCommandSetType;

		private readonly object commandSet;
		private readonly Action<SqlCommand> append;
		private readonly Action dispose;
		private readonly Func<int> executeNonQuery;
		private readonly Action<SqlConnection> setConnection;

		private int commandCount;

		/// <summary>
		/// Pulls out the internal SqlCommandSet type from the System.Data assembly
		/// </summary>
		static PublicSqlCommandSet()
		{
			var assembly = Assembly.Load("System.Data, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089");
			sqlCommandSetType = assembly.GetType("System.Data.SqlClient.SqlCommandSet");
		}

		/// <summary>
		/// Instantiates a local instance of the SqlCommandSet type, maps up the delegates and sets the connection
		/// </summary>
		internal PublicSqlCommandSet(SqlConnection conn)
		{
			commandSet = Activator.CreateInstance(sqlCommandSetType, true);
			append = (Action<SqlCommand>)Delegate.CreateDelegate(typeof(Action<SqlCommand>), commandSet, "Append");
			dispose = (Action)Delegate.CreateDelegate(typeof(Action), commandSet, "Dispose");
			executeNonQuery = (Func<int>)Delegate.CreateDelegate(typeof(Func<int>), commandSet, "ExecuteNonQuery");
			setConnection = (Action<SqlConnection>)Delegate.CreateDelegate(typeof(Action<SqlConnection>), commandSet, "set_Connection");

			Connection = conn;
		}

		/// <summary>
		/// Appends a command to the set
		/// </summary>
		internal void Append(SqlCommand command)
		{
			commandCount++;
			append(command);
		}

		/// <summary>
		/// Executes all the commands in the set
		/// </summary>
		/// <returns>The total number of modified rows across all commands in the set</returns>
		internal int ExecuteNonQuery()
		{
			return executeNonQuery();
		}

		/// <summary>
		/// Sets the connection for the set
		/// </summary>
		internal SqlConnection Connection
		{
			set { setConnection(value); }
		}

		/// <summary>
		/// The number of commands currently in the set
		/// </summary>
		internal int CommandCount
		{
			get { return commandCount; }
		}

		public void Dispose()
		{
			dispose();
		}
	}
}