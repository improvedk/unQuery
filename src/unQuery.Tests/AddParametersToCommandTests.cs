using NUnit.Framework;
using System;
using System.Data;
using System.Data.SqlClient;
using unQuery.SqlTypes;

namespace unQuery.Tests
{
	public partial class unQueryTests
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
		public void AddParameterToCommand_Tinyint()
		{
			testParameterType(true, SqlDbType.Bit);
			testParameterType((bool?)null, SqlDbType.Bit, DBNull.Value);
			testParameterType(Col.Bit(false), SqlDbType.Bit, false);
			testParameterType(Col.Bit(null), SqlDbType.Bit, DBNull.Value);
		}

		[Test]
		public void AddParameterToCommand_Byte() { testParameterType<byte>(55, SqlDbType.TinyInt); }

		[Test]
		public void AddParameterToCommand_NullableByte() { testParameterType<byte?>(null, SqlDbType.TinyInt); }

		[Test]
		public void AddParameterToCommand_Short() { testParameterType<short>(55, SqlDbType.SmallInt); }

		[Test]
		public void AddParameterToCommand_NullableShort() { testParameterType<short?>(null, SqlDbType.SmallInt); }

		[Test]
		public void AddParameterToCommand_Int() { testParameterType<int>(55, SqlDbType.Int); }

		[Test]
		public void AddParameterToCommand_NullableInt() { testParameterType<int?>(null, SqlDbType.Int); }

		[Test]
		public void AddParameterToCommand_Long() { testParameterType<long>(55, SqlDbType.BigInt); }

		[Test]
		public void AddParameterToCommand_NullableLong() { testParameterType<long?>(null, SqlDbType.BigInt); }

		[Test]
		public void AddParameterToCommand_Guid() { testParameterType<Guid>(new Guid("df5f737d-8730-49ee-90f2-4f60acbd1323"), SqlDbType.UniqueIdentifier); }

		[Test]
		public void AddParameterToCommand_NullableGuid() { testParameterType<Guid?>(null, SqlDbType.UniqueIdentifier); }

		[Test]
		public void AddParameterToCommand_Bool() { testParameterType<bool>(true, SqlDbType.Bit); }

		[Test]
		public void AddParameterToCommand_NullableBool() { testParameterType<bool?>(null, SqlDbType.Bit); }

		[Test]
		public void AddParameterToCommand_DateTime() { testParameterType<DateTime>(DateTime.Now, SqlDbType.DateTime); }

		[Test]
		public void AddParameterToCommand_NullableDateTime() { testParameterType<DateTime?>(null, SqlDbType.DateTime); }

		private void testParameterType<TValue>(TValue value, SqlDbType expectedType)
		{
			testParameterType(value, expectedType, value);
		}

		private void testParameterType<TValue, TParamValue>(TValue value, SqlDbType expectedType, TParamValue paramValue)
		{
			var cmd = new SqlCommand();

			db.AddParametersToCommand(cmd, new {
				Test = value
			});

			var param = cmd.Parameters[0];

			Assert.AreEqual("@Test", param.ParameterName);
			Assert.AreEqual(expectedType, param.SqlDbType);
			Assert.AreEqual(paramValue, param.Value);
		}
	}
}