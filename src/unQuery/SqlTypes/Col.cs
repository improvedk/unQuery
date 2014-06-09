using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;

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
		public static SqlBigInt BigInt(long? value, ParameterDirection direction = ParameterDirection.Input)
		{
			return new SqlBigInt(value, direction);
		}

		/// <summary>
		/// Creates a bit value
		/// </summary>
		public static SqlBit Bit(bool? value, ParameterDirection direction = ParameterDirection.Input)
		{
			return new SqlBit(value, direction);
		}

		/// <summary>
		/// Creates a binary value
		/// </summary>
		/// <param name="value">The binary data</param>
		/// <param name="maxLength">The max length of the column, as specified in the database</param>
		public static SqlBinary Binary(byte[] value, int? maxLength = null, ParameterDirection direction = ParameterDirection.Input)
		{
			return new SqlBinary(value, maxLength, direction);
		}

		/// <summary>
		/// Creates a char value
		/// </summary>
		/// <param name="value">The string data</param>
		/// <param name="maxLength">The max length of the column, as specified in the database</param>
		public static SqlChar Char(string value, int? maxLength = null, ParameterDirection direction = ParameterDirection.Input)
		{
			return new SqlChar(value, maxLength, direction);
		}

		/// <summary>
		/// Creates a date value
		/// </summary>
		public static SqlDate Date(DateTime? date, ParameterDirection direction = ParameterDirection.Input)
		{
			return new SqlDate(date, direction);
		}

		/// <summary>
		/// Creates a datetime value
		/// </summary>
		public static SqlDateTime DateTime(DateTime? date, ParameterDirection direction = ParameterDirection.Input)
		{
			return new SqlDateTime(date, direction);
		}

		/// <summary>
		/// Creates a datetime2 value
		/// </summary>
		public static SqlDateTime2 DateTime2(DateTime? value, byte? scale = null, ParameterDirection direction = ParameterDirection.Input)
		{
			return new SqlDateTime2(value, scale, direction);
		}

		/// <summary>
		/// Creates a datetimeoffset value
		/// </summary>
		public static SqlDateTimeOffset DateTimeOffset(DateTimeOffset? value, byte? scale = null, ParameterDirection direction = ParameterDirection.Input)
		{
			return new SqlDateTimeOffset(value, scale, direction);
		}

		/// <summary>
		/// Creates a decimal value
		/// </summary>
		public static SqlDecimal Decimal(decimal? value, byte? precision = null, byte? scale = null, ParameterDirection direction = ParameterDirection.Input)
		{
			return new SqlDecimal(value, precision, scale, direction);
		}

		/// <summary>
		/// Creates a float value
		/// </summary>
		public static SqlFloat Float(double? value, ParameterDirection direction = ParameterDirection.Input)
		{
			return new SqlFloat(value, direction);
		}

		/// <summary>
		/// Creates a image value
		/// </summary>
		public static SqlImage Image(byte[] value, ParameterDirection direction = ParameterDirection.Input)
		{
			return new SqlImage(value, direction);
		}

		/// <summary>
		/// Creates an int value
		/// </summary>
		public static SqlInt Int(int? value, ParameterDirection direction = ParameterDirection.Input)
		{
			return new SqlInt(value, direction);
		}

		/// <summary>
		/// Creates a money value
		/// </summary>
		public static SqlMoney Money(decimal? value, ParameterDirection direction = ParameterDirection.Input)
		{
			return new SqlMoney(value, direction);
		}

		/// <summary>
		/// Creates an nchar value
		/// </summary>
		public static SqlNChar NChar(string value, int? maxLength = null, ParameterDirection direction = ParameterDirection.Input)
		{
			return new SqlNChar(value, maxLength, direction);
		}

		/// <summary>
		/// Creates a ntext value
		/// </summary>
		public static SqlNText NText(string value, ParameterDirection direction = ParameterDirection.Input)
		{
			return new SqlNText(value, direction);
		}

		/// <summary>
		/// Creates an nvarchar value
		/// </summary>
		/// <param name="value">The value</param>
		/// <param name="maxLength">The max length of the column value - should match the one defined on the column</param>
		public static SqlNVarChar NVarChar(string value, int? maxLength = null, ParameterDirection direction = ParameterDirection.Input)
		{
			return new SqlNVarChar(value, maxLength, direction);
		}

		/// <summary>
		/// Creates a smalldatetime value
		/// </summary>
		public static SqlSmallDateTime SmallDateTime(DateTime? value, ParameterDirection direction = ParameterDirection.Input)
		{
			return new SqlSmallDateTime(value, direction);
		}

		/// <summary>
		/// Creates a smallint value
		/// </summary>
		public static SqlSmallInt SmallInt(short? value, ParameterDirection direction = ParameterDirection.Input)
		{
			return new SqlSmallInt(value, direction);
		}

		/// <summary>
		/// Creates a smallmoney value
		/// </summary>
		public static SqlSmallMoney SmallMoney(decimal? value, ParameterDirection direction = ParameterDirection.Input)
		{
			return new SqlSmallMoney(value, direction);
		}

		/// <summary>
		/// Creates a structured value for use as a table valued parameter
		/// </summary>
		/// <param name="typeName">The name of the SQL Server table valued type</param>
		/// <param name="values">The values to be passed onto the type. The properties on the value must match the SQL Server type directly.</param>
		public static SqlStructured Structured(string typeName, IEnumerable values)
		{
			return new SqlStructured(typeName, values.Cast<object>());
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
		public static SqlText Text(string value, ParameterDirection direction = ParameterDirection.Input)
		{
			return new SqlText(value, direction);
		}

		/// <summary>
		/// Creates a time value
		/// </summary>
		public static SqlTime Time(TimeSpan? value, byte? scale = null, ParameterDirection direction = ParameterDirection.Input)
		{
			return new SqlTime(value, scale, direction);
		}

		/// <summary>
		/// Creates a tinyint value
		/// </summary>
		public static SqlTinyInt TinyInt(byte? value, ParameterDirection direction = ParameterDirection.Input)
		{
			return new SqlTinyInt(value, direction);
		}

		/// <summary>
		/// Creates a real value
		/// </summary>
		public static SqlReal Real(float? value, ParameterDirection direction = ParameterDirection.Input)
		{
			return new SqlReal(value, direction);
		}

		/// <summary>
		/// Creates a uniqueidentifier value
		/// </summary>
		public static SqlUniqueIdentifier UniqueIdentifier(Guid? value, ParameterDirection direction = ParameterDirection.Input)
		{
			return new SqlUniqueIdentifier(value, direction);
		}

		/// <summary>
		/// Creates a varbinary value
		/// </summary>
		public static SqlVarBinary VarBinary(byte[] value, int? maxLength = null, ParameterDirection direction = ParameterDirection.Input)
		{
			return new SqlVarBinary(value, maxLength, direction);
		}

		/// <summary>
		/// Creates a varchar value
		/// </summary>
		/// <param name="value">The value</param>
		/// <param name="maxLength">The max length of the column value - should match the one defined on the column</param>
		public static SqlVarChar VarChar(string value, int? maxLength = null, ParameterDirection direction = ParameterDirection.Input)
		{
			return new SqlVarChar(value, maxLength, direction);
		}

		/// <summary>
		/// Creates an xml value
		/// </summary>
		public static SqlXml Xml(string value, ParameterDirection direction = ParameterDirection.Input)
		{
			return new SqlXml(value, direction);
		}
	}
}