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
			// bit
			TestHelper.AssertParameterFromValue(Col.Bit(false), SqlDbType.Bit, false);
			TestHelper.AssertParameterFromValue(Col.Bit(null), SqlDbType.Bit, DBNull.Value);

			// bigint
			TestHelper.AssertParameterFromValue(Col.BigInt(5), SqlDbType.BigInt, 5L);
			TestHelper.AssertParameterFromValue(Col.BigInt(null), SqlDbType.BigInt, DBNull.Value);

			// int
			TestHelper.AssertParameterFromValue(Col.Int(5), SqlDbType.Int, 5);
			TestHelper.AssertParameterFromValue(Col.Int(null), SqlDbType.Int, DBNull.Value);

			// varchar
			TestHelper.AssertParameterFromValue(Col.Varchar("Test"), SqlDbType.VarChar, "Test", 4);
			TestHelper.AssertParameterFromValue(Col.Varchar("Test", 10), SqlDbType.VarChar, "Test", 10);
			TestHelper.AssertParameterFromValue(Col.Varchar(null), SqlDbType.VarChar, DBNull.Value);

			// nvarchar
			TestHelper.AssertParameterFromValue(Col.NVarchar("Test"), SqlDbType.NVarChar, "Test", 4);
			TestHelper.AssertParameterFromValue(Col.NVarchar("Test", 10), SqlDbType.NVarChar, "Test", 10);
			TestHelper.AssertParameterFromValue(Col.NVarchar(null), SqlDbType.NVarChar, DBNull.Value);
		}
	}
}