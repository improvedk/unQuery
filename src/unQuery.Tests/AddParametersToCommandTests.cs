using NUnit.Framework;
using System;
using System.Data;
using System.Data.SqlClient;

namespace unQuery.Tests
{
	public class AddParametersToCommandTests : TestFixture
	{
		[Test]
		public void AddParameterToCommand_ImplicitTypes()
		{
			// bool / bit
			TestHelper.AssertParameterFromValue(true, SqlDbType.Bit, true);
			TestHelper.AssertParameterFromValue((bool?)null, SqlDbType.Bit, DBNull.Value);

			// byte / tinyint
			TestHelper.AssertParameterFromValue((byte)55, SqlDbType.TinyInt, (byte)55);
			TestHelper.AssertParameterFromValue((byte?)null, SqlDbType.TinyInt, DBNull.Value);

			// short / smallint
			TestHelper.AssertParameterFromValue((short)55, SqlDbType.SmallInt, (short)55);
			TestHelper.AssertParameterFromValue((short?)null, SqlDbType.SmallInt, DBNull.Value);

			// int / int
			TestHelper.AssertParameterFromValue(55, SqlDbType.Int, 55);
			TestHelper.AssertParameterFromValue((int?)null, SqlDbType.Int, DBNull.Value);

			// long / bigint
			TestHelper.AssertParameterFromValue(55L, SqlDbType.BigInt, 55L);
			TestHelper.AssertParameterFromValue((long?)null, SqlDbType.BigInt, DBNull.Value);

			// Guid / uniqueidentifier
			Guid guid = Guid.NewGuid();
			TestHelper.AssertParameterFromValue(guid, SqlDbType.UniqueIdentifier, guid);
			TestHelper.AssertParameterFromValue((Guid?)null, SqlDbType.UniqueIdentifier, DBNull.Value);
		}

		[Test]
		public void AddParameterToCommand_NonSupportedTypes()
		{
			Assert.Throws<TypeNotSupportedException>(() => DB.AddParametersToCommand(new SqlCommand(), new {
				Test = "Hello"
			}));
		}
		
		[Test]
		public void AddParameterToCommand_TinyInt()
		{
			//testAutoParameterType(Col.TinyInt(55), SqlDbType.TinyInt, 55);
			//testAutoParameterType(Col.TinyInt(null), SqlDbType.TinyInt, DBNull.Value);
		}
		
		[Test]
		public void AddParameterToCommand_SmallInt()
		{
			//testAutoParameterType(Col.SmallInt(55), SqlDbType.SmallInt, 55);
			//testAutoParameterType(Col.SmallInt(null), SqlDbType.SmallInt, DBNull.Value);
		}
		
		[Test]
		public void AddParameterToCommand_Int()
		{
			//testAutoParameterType(Col.Int(55), SqlDbType.Int, 55);
			//testAutoParameterType(Col.Int(null), SqlDbType.Int, DBNull.Value);
		}
		
		[Test]
		public void AddParameterToCommand_BigInt()
		{
			//testAutoParameterType(Col.BigInt(55), SqlDbType.BigInt, 55);
			//testAutoParameterType(Col.BigInt(null), SqlDbType.BigInt, DBNull.Value);
		}

		[Test]
		public void AddParameterToCommand_UniqueIdentifier()
		{
			//testAutoParameterType(Col.UniqueIdentifier(guid), SqlDbType.UniqueIdentifier, guid);
			//testAutoParameterType(Col.UniqueIdentifier(null), SqlDbType.UniqueIdentifier, DBNull.Value);
		}
	}
}