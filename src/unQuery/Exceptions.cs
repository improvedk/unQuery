using System;

namespace unQuery
{
	public class NoRowsException : Exception
	{
		public NoRowsException() : base("No rows were returned - no scalar value to return.")
		{ }
	}

	public class TypeNotSupportedException : Exception
	{
		public TypeNotSupportedException(Type t) : base("Type '" + t + "' not supported.")
		{ }

		public TypeNotSupportedException(string t) : base("Type '" + t + "' not supported.")
		{ }
	}

	public class MissingConnectionStringException : Exception
	{
		public MissingConnectionStringException() : base("unQuery did not find any connection strings in the ConnectionStrings configuration section.")
		{ }
	}

	public class ColumnDoesNotExistException : Exception
	{
		public ColumnDoesNotExistException(string column) : base("Column '" + column + "' does not exist.")
		{ }
	}
}