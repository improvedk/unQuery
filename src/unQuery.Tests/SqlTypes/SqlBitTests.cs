using System.Data;
using NUnit.Framework;
using System;
using unQuery.SqlTypes;

namespace unQuery.Tests.SqlTypes
{
	public class SqlBitTests : TestFixture
	{
		[Test]
		public void ParameterType()
		{
			TestHelper.AssertParameterFromValue(Col.Bit(false), SqlDbType.Bit, false);
			TestHelper.AssertParameterFromValue(Col.Bit(null), SqlDbType.Bit, DBNull.Value);
		}
	}
}