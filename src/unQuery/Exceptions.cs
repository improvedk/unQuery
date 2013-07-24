using System;

namespace unQuery
{
	public class NoRowsException : Exception
	{
		public NoRowsException() : base("No rows were returned - no scalar value to return.")
		{ }
	}
}