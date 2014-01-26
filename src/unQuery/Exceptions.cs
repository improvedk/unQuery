using System;

namespace unQuery
{
	/// <summary>
	/// Thrown when rows are expected, but none were present.
	/// </summary>
	public class NoRowsException : Exception
	{
		public NoRowsException() :
			base("No rows were returned - no scalar value to return.")
		{ }
	}

	/// <summary>
	/// Thrown when a parameter type is not supported.
	/// </summary>
	public class TypeNotSupportedException : Exception
	{
		public TypeNotSupportedException(Type t) :
			base("Type '" + t + "' not supported.")
		{ }

		public TypeNotSupportedException(string t) :
			base("Type '" + t + "' not supported.")
		{ }
	}

	/// <summary>
	/// Thrown if no connection strings are defined.
	/// </summary>
	public class MissingConnectionStringException : Exception
	{
		public MissingConnectionStringException() :
			base("unQueryDB did not find any connection strings in the ConnectionStrings configuration section.")
		{ }
	}

	/// <summary>
	/// Thrown if trying to access a non-existing column.
	/// </summary>
	public class ColumnDoesNotExistException : Exception
	{
		public ColumnDoesNotExistException(string column) :
			base("Column '" + column + "' does not exist.")
		{ }
	}

	/// <summary>
	/// Thrown if a result row contains two columns with the same name.
	/// </summary>
	public class DuplicateColumnException : Exception
	{
		public DuplicateColumnException(string column) :
			base("Two or more columns share the same name '" + column + "'.")
		{ }
	}

	/// <summary>
	/// Thrown if a result contains an unnamed column.
	/// </summary>
	public class UnnamedColumnException : Exception
	{
		public UnnamedColumnException(int index) :
			base("Column with index " + index + " has no name.")
		{ }
	}

	/// <summary>
	/// Thrown if trying to add a parameter to a SqlCommand that already contains a parameter.
	/// with the same name.
	/// </summary>
	public class SqlCommandAlreadyHasParametersException : Exception
	{
		public SqlCommandAlreadyHasParametersException() :
			base("If parameters are passed to unQuery, the SqlCommand must not already have parameters added.")
		{ }
	}

	/// <summary>
	/// Thrown if properties are expected on an object, but none are present.
	/// </summary>
	public class ObjectHasNoPropertiesException : Exception
	{
		public ObjectHasNoPropertiesException(string message) :
			base(message)
		{ }
	}

	/// <summary>
	/// Thrown if a SqlType is used as if it could be auto converted from the native CLR type. E.g. valid for ints but not for strings.
	/// </summary>
	public class TypeCannotBeUsedAsAClrTypeException : InvalidOperationException
	{
		public TypeCannotBeUsedAsAClrTypeException() :
			base("This type can only be used explicitly and not implicitly from a native CLR type.")
		{ }
	}

	/// <summary>
	/// Thrown if a type is used in situations that requires types to be specified explicitly (like for structured use)
	/// </summary>
	public class TypePropertiesMustBeSetExplicitlyException : InvalidOperationException
	{
		public TypePropertiesMustBeSetExplicitlyException(string properties) :
			base(properties + " must be set explicitly.")
		{ }
	}

	/// <summary>
	/// Thrown if an IEnumerable of simple values is passed to a structured parameter, with different types being
	/// returned from the IEnumerable
	/// </summary>
	public class StructuredTypeMismatchException : ArgumentException
	{
		public StructuredTypeMismatchException(Type t)
			: base("Type " + t + " was unexpected for this structured parameter")
		{ }
	}

	/// <summary>
	/// Thrown if two types do not match
	/// </summary>
	public class TypeMismatchException : Exception
	{
		public TypeMismatchException(string msg) : base(msg)
		{ }
	}
}