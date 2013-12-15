using System;
using System.Collections.Generic;

namespace unQuery.SqlTypes
{
	public static class Col
	{
		public static SqlBit Bit(bool? value)
		{
			return new SqlBit(value);
		}

		public static SqlTinyInt TinyInt(byte? value)
		{
			return new SqlTinyInt(value);
		}

		public static SqlSmallInt SmallInt(byte? value)
		{
			return new SqlSmallInt(value);
		}

		public static SqlInt Int(int? value)
		{
			return new SqlInt(value);
		}

		public static SqlBigInt BigInt(long? value)
		{
			return new SqlBigInt(value);
		}

		public static SqlNVarChar NVarChar(string value)
		{
			return new SqlNVarChar(value);
		}

		public static SqlNVarChar NVarChar(string value, int size)
		{
			return new SqlNVarChar(value, size);
		}

		public static SqlStructured Structured(string typeName, IEnumerable<object> values)
		{
			return new SqlStructured(typeName, values);
		}

		public static SqlVarChar VarChar(string value)
		{
			return new SqlVarChar(value);
		}

		public static SqlVarChar VarChar(string value, int size)
		{
			return new SqlVarChar(value, size);
		}

		public static SqlUniqueIdentifier UniqueIdentifier(Guid? value)
		{
			return new SqlUniqueIdentifier(value);
		}
	}
}