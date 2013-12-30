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
		/// Creates a bigint value
		/// </summary>
		public static SqlBigInt BigInt(long? value)
		{
			return new SqlBigInt(value);
		}

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
		/// <param name="value">The binary data</param>
		/// <param name="maxLength">The max length of the column, as specified in the database</param>
		public static SqlBinary Binary(byte[] value, int maxLength)
		{
			return new SqlBinary(value, maxLength);
		}

		/// <summary>
		/// Creates a binary value
		/// </summary>
		/// <param name="value">The binary data</param>
		public static SqlBinary Binary(byte[] value)
		{
			return new SqlBinary(value);
		}

		/// <summary>
		/// Creates a char value
		/// </summary>
		/// <param name="value">The string data</param>
		public static SqlChar Char(string value)
		{
			return new SqlChar(value);
		}

		/// <summary>
		/// Creates a char value
		/// </summary>
		/// <param name="value">The string data</param>
		/// <param name="maxLength">The max length of the column, as specified in the database</param>
		public static SqlChar Char(string value, int maxLength)
		{
			return new SqlChar(value, maxLength);
		}

		/// <summary>
		/// Creates a date value
		/// </summary>
		public static SqlDate Date(DateTime? date)
		{
			return new SqlDate(date);
		}

		/// <summary>
		/// Creates a datetime value
		/// </summary>
		public static SqlDateTime DateTime(DateTime? date)
		{
			return new SqlDateTime(date);
		}

		/// <summary>
		/// Creates a datetime2 value
		/// </summary>
		public static SqlDateTime2 DateTime2(DateTime? value)
		{
			return new SqlDateTime2(value);
		}

		/// <summary>
		/// Creates a datetime2 value
		/// </summary>
		public static SqlDateTime2 DateTime2(DateTime? value, byte scale)
		{
			return new SqlDateTime2(value, scale);
		}

		/// <summary>
		/// Creates a datetimeoffset value
		/// </summary>
		public static SqlDateTimeOffset DateTimeOffset(DateTimeOffset? value)
		{
			return new SqlDateTimeOffset(value);
		}

		/// <summary>
		/// Creates a datetimeoffset value
		/// </summary>
		public static SqlDateTimeOffset DateTimeOffset(DateTimeOffset? value, byte scale)
		{
			return new SqlDateTimeOffset(value, scale);
		}

		/// <summary>
		/// Creates a decimal value
		/// </summary>
		public static SqlDecimal Decimal(decimal? value)
		{
			return new SqlDecimal(value);
		}

		/// <summary>
		/// Creates a decimal value
		/// </summary>
		public static SqlDecimal Decimal(decimal? value, byte precision, byte scale)
		{
			return new SqlDecimal(value, precision, scale);
		}

		/// <summary>
		/// Creates a float value
		/// </summary>
		public static SqlFloat Float(double? value)
		{
			return new SqlFloat(value);
		}

		/// <summary>
		/// Creates a image value
		/// </summary>
		public static SqlImage Image(byte[] value)
		{
			return new SqlImage(value);
		}

		/// <summary>
		/// Creates an int value
		/// </summary>
		public static SqlInt Int(int? value)
		{
			return new SqlInt(value);
		}

		/// <summary>
		/// Creates a money value
		/// </summary>
		public static SqlMoney Money(decimal? value)
		{
			return new SqlMoney(value);
		}

		/// <summary>
		/// Creates an nchar value
		/// </summary>
		public static SqlNChar NChar(string value)
		{
			return new SqlNChar(value);
		}

		/// <summary>
		/// Creates an nchar value
		/// </summary>
		public static SqlNChar NChar(string value, int maxLength)
		{
			return new SqlNChar(value, maxLength);
		}

		/// <summary>
		/// Creates a ntext value
		/// </summary>
		public static SqlNText NText(string value)
		{
			return new SqlNText(value);
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
		/// Creates a smalldatetime value
		/// </summary>
		public static SqlSmallDateTime SmallDateTime(DateTime? value)
		{
			return new SqlSmallDateTime(value);
		}

		/// <summary>
		/// Creates a smallint value
		/// </summary>
		public static SqlSmallInt SmallInt(byte? value)
		{
			return new SqlSmallInt(value);
		}

		/// <summary>
		/// Creates a smallmoney value
		/// </summary>
		public static SqlSmallMoney SmallMoney(decimal? value)
		{
			return new SqlSmallMoney(value);
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
		/// Creates a text value
		/// </summary>
		public static SqlText Text(string value)
		{
			return new SqlText(value);
		}

		/// <summary>
		/// Creates a time value
		/// </summary>
		public static SqlTime Time(TimeSpan? value, byte scale)
		{
			return new SqlTime(value, scale);
		}

		/// <summary>
		/// Creates a tinyint value
		/// </summary>
		public static SqlTinyInt TinyInt(byte? value)
		{
			return new SqlTinyInt(value);
		}

		/// <summary>
		/// Creates a real value
		/// </summary>
		public static SqlReal Real(float? value)
		{
			return new SqlReal(value);
		}

		/// <summary>
		/// Creates a uniqueidentifier value
		/// </summary>
		public static SqlUniqueIdentifier UniqueIdentifier(Guid? value)
		{
			return new SqlUniqueIdentifier(value);
		}

		/// <summary>
		/// Creates a varbinary value
		/// </summary>
		public static SqlVarBinary VarBinary(byte[] value, int maxLength)
		{
			return new SqlVarBinary(value, maxLength);
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
		/// Creates an xml value
		/// </summary>
		public static SqlXml Xml(string value)
		{
			return new SqlXml(value);
		}
	}
}