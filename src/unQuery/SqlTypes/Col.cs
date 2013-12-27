using System;
using System.Collections.Generic;

namespace unQuery.SqlTypes
{
	/// <summary>
	/// Helper class for easily instantiating type values
	/// </summary>
	public static class Col
	{
		/// <summary>
		/// Creates a bit value
		/// </summary>
		public static SqlBit Bit(bool? value)
		{
			return new SqlBit(value);
		}

		/// <summary>
		/// Creates a binary value
		/// </summary>
		public static SqlBinary Binary(byte[] value, int maxLength)
		{
			return new SqlBinary(value, maxLength);
		}

		/// <summary>
		/// Creates a decimal value
		/// </summary>
		public static SqlDecimal Decimal(decimal? value, byte precision, byte scale)
		{
			return new SqlDecimal(value, precision, scale);
		}

		/// <summary>
		/// Creates a tinyint value
		/// </summary>
		public static SqlTinyInt TinyInt(byte? value)
		{
			return new SqlTinyInt(value);
		}

		/// <summary>
		/// Creates a smallint value
		/// </summary>
		public static SqlSmallInt SmallInt(byte? value)
		{
			return new SqlSmallInt(value);
		}

		/// <summary>
		/// Creates an int value
		/// </summary>
		public static SqlInt Int(int? value)
		{
			return new SqlInt(value);
		}

		/// <summary>
		/// Creates a bigint value
		/// </summary>
		public static SqlBigInt BigInt(long? value)
		{
			return new SqlBigInt(value);
		}

		/// <summary>
		/// Creates an nvarchar value
		/// </summary>
		/// <param name="value">The value</param>
		/// <param name="maxLength">The max length of the column value - should match the one defined on the column</param>
		public static SqlNVarChar NVarChar(string value, int maxLength)
		{
			return new SqlNVarChar(value, maxLength);
		}

		/// <summary>
		/// Creates a structured value for use as a table valued parameter
		/// </summary>
		/// <param name="typeName">The name of the SQL Server table valued type</param>
		/// <param name="values">The values to be passed onto the type. The properties on the value must match the SQL Server type directly.</param>
		public static SqlStructured Structured(string typeName, IEnumerable<object> values)
		{
			return new SqlStructured(typeName, values);
		}

		/// <summary>
		/// Creates a varchar value
		/// </summary>
		/// <param name="value">The value</param>
		/// <param name="maxLength">The max length of the column value - should match the one defined on the column</param>
		public static SqlVarChar VarChar(string value, int maxLength)
		{
			return new SqlVarChar(value, maxLength);
		}

		/// <summary>
		/// Creates a uniqueidentifier value
		/// </summary>
		public static SqlUniqueIdentifier UniqueIdentifier(Guid? value)
		{
			return new SqlUniqueIdentifier(value);
		}
	}
}