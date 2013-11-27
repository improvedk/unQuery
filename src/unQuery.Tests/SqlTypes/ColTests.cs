using NUnit.Framework;
using System;
using System.Data;
using unQuery.SqlTypes;

namespace unQuery.Tests.SqlTypes
{
	public class ColTests : TestFixture
	{
		[Test]
		public void TypeParameters()
		{
			// bigint
			TestHelper.AssertParameterFromValue(Col.BigInt(5), SqlDbType.BigInt, 5L);
			TestHelper.AssertParameterFromValue(Col.BigInt(null), SqlDbType.BigInt, DBNull.Value);

			// bit
			TestHelper.AssertParameterFromValue(Col.Bit(false), SqlDbType.Bit, false);
			TestHelper.AssertParameterFromValue(Col.Bit(null), SqlDbType.Bit, DBNull.Value);

			// int
			TestHelper.AssertParameterFromValue(Col.Int(5), SqlDbType.Int, 5);
			TestHelper.AssertParameterFromValue(Col.Int(null), SqlDbType.Int, DBNull.Value);

			// nvarchar
			TestHelper.AssertParameterFromValue(Col.NVarChar("Test"), SqlDbType.NVarChar, "Test", 4);
			TestHelper.AssertParameterFromValue(Col.NVarChar("Test", 10), SqlDbType.NVarChar, "Test", 10);
			TestHelper.AssertParameterFromValue(Col.NVarChar(null), SqlDbType.NVarChar, DBNull.Value);

			// smallint
			TestHelper.AssertParameterFromValue(Col.SmallInt(5), SqlDbType.SmallInt, (short)5);
			TestHelper.AssertParameterFromValue(Col.SmallInt(null), SqlDbType.SmallInt, DBNull.Value);

			// tinyint
			TestHelper.AssertParameterFromValue(Col.TinyInt(5), SqlDbType.TinyInt, (byte)5);
			TestHelper.AssertParameterFromValue(Col.TinyInt(null), SqlDbType.TinyInt, DBNull.Value);

			// uniqueidentifier
			var guid = Guid.NewGuid();
			TestHelper.AssertParameterFromValue(Col.UniqueIdentifier(guid), SqlDbType.UniqueIdentifier, guid);
			TestHelper.AssertParameterFromValue(Col.UniqueIdentifier(null), SqlDbType.UniqueIdentifier, DBNull.Value);

			// varchar
			TestHelper.AssertParameterFromValue(Col.VarChar("Test"), SqlDbType.VarChar, "Test", 4);
			TestHelper.AssertParameterFromValue(Col.VarChar("Test", 10), SqlDbType.VarChar, "Test", 10);
			TestHelper.AssertParameterFromValue(Col.VarChar(null), SqlDbType.VarChar, DBNull.Value);
		}
	}
}