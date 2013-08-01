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
		public TypeNotSupportedException(Type t) : base("Type " + t + " not supported.")
		{ }
	}
}