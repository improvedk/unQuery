using System;
using System.Data;
using System.Data.SqlClient;
using NUnit.Framework;
using unQuery.SqlTypes;

namespace unQuery.Tests
{
	/// <summary>
	/// Exact Numerics: bigint, bit, decimal, int, money, numeric, smallint, smallmoney, tinyint
	/// Approximate Numerics: float, real
	/// Date and Time: date, datetime2, datetime, datetimeoffset, smalldatetime, time
	/// Character Strings: char, text, varchar
	/// Unicode Character Strings: nchar, ntext, nvarchar
	/// Binary Strings: binary, image, varbinary
	/// Other Data Types: cursor, hierarchyid, sql_variant, table, timestamp, uniqueidentifier, xml
	/// 
	/// Supported:
	///		bit
	///		tinyint
	///		smallint
	/// </summary>
	public class AddParametersToCommandTests : TestFixture
	{
		[Test]
		public void AddParameterToCommand_Bit()
		{
			testParameterType(true, SqlDbType.Bit);
			testParameterType((bool?)null, SqlDbType.Bit, DBNull.Value);
			testParameterType(Col.Bit(false), SqlDbType.Bit, false);
			testParameterType(Col.Bit(null), SqlDbType.Bit, DBNull.Value);
		}
		
		[Test]
		public void AddParameterToCommand_TinyInt()
		{
			testParameterType((byte)55, SqlDbType.TinyInt);
			testParameterType((byte?)null, SqlDbType.TinyInt, DBNull.Value);
			testParameterType(Col.TinyInt(55), SqlDbType.TinyInt, 55);
			testParameterType(Col.TinyInt(null), SqlDbType.TinyInt, DBNull.Value);
		}
		
		[Test]
		public void AddParameterToCommand_SmallInt()
		{
			testParameterType((short)55, SqlDbType.SmallInt);
			testParameterType((short?)null, SqlDbType.SmallInt, DBNull.Value);
			testParameterType(Col.SmallInt(55), SqlDbType.SmallInt, 55);
			testParameterType(Col.SmallInt(null), SqlDbType.SmallInt, DBNull.Value);
		}
		
		[Test]
		public void AddParameterToCommand_Int()
		{
			testParameterType(55, SqlDbType.Int);
			testParameterType((int?)null, SqlDbType.Int, DBNull.Value);
			testParameterType(Col.Int(55), SqlDbType.Int, 55);
			testParameterType(Col.Int(null), SqlDbType.Int, DBNull.Value);
		}
		
		[Test]
		public void AddParameterToCommand_BigInt()
		{
			testParameterType(55L, SqlDbType.BigInt);
			testParameterType((long?)null, SqlDbType.BigInt, DBNull.Value);
			testParameterType(Col.BigInt(55), SqlDbType.BigInt, 55);
			testParameterType(Col.BigInt(null), SqlDbType.BigInt, DBNull.Value);
		}

		private void testParameterType<TValue>(TValue value, SqlDbType expectedType)
		{
			testParameterType(value, expectedType, value);
		}

		private void testParameterType<TValue, TParamValue>(TValue value, SqlDbType expectedType, TParamValue paramValue)
		{
			var cmd = new SqlCommand();

			DB.AddParametersToCommand(cmd, new {
				Test = value
			});

			var param = cmd.Parameters[0];

			Assert.AreEqual("@Test", param.ParameterName);
			Assert.AreEqual(expectedType, param.SqlDbType);
			Assert.AreEqual(paramValue, param.Value);
		}
	}
}