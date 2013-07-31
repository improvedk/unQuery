using System;
using System.Data.SqlClient;
using NUnit.Framework;

namespace unQuery.Tests
{
	public class GetScalarTests : TestFixture
	{
		[Test]
		public void GetScalar_ValueWithParameter()
		{
			var result = DB.GetScalar<int>("SELECT COUNT(*) FROM Persons WHERE Age = @Age", new {
				Age = (byte)55
			});

			Assert.AreEqual(1, result);
		}

		[Test]
		public void GetScalar_ValueType()
		{
			var result = DB.GetScalar<int>("SELECT COUNT(*) FROM Persons");

			Assert.AreEqual(5, result);
		}

		[Test]
		public void GetScalar_NullNullableValueType()
		{
			var result = DB.GetScalar<DateTime?>("SELECT TOP 1 SignedUp FROM Persons WHERE SignedUp IS NULL");

			Assert.AreEqual(null, result);
		}

		[Test]
		public void GetScalar_NonNullNullableValueType()
		{
			var result = DB.GetScalar<DateTime?>("SELECT SignedUp FROM Persons WHERE Name = 'Daniel Gallagher'");

			Assert.AreEqual(Convert.ToDateTime("1997-11-15 21:03:54.000"), result);
		}

		[Test]
		public void GetScalar_NullableValueTypeWithNoRows()
		{
			Assert.Throws<NoRowsException>(() => DB.GetScalar<DateTime?>("SELECT TOP 1 SignedUp FROM Persons WHERE 1=0"));
		}

		[Test]
		public void GetScalar_ThrowsSqlException()
		{
			Assert.Throws<SqlException>(() => DB.Execute("x"));
		}
	}
}