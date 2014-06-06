using NUnit.Framework;
using System;
using System.Data;
using System.Data.SqlClient;

namespace unQuery.Tests
{
	public class GetScalarTests : TestFixture
	{
		[Test]
		public void StoredProcedure()
		{
			DB.Execute("CREATE PROCEDURE usp_Test @A int AS SELECT @A AS A");

			var result = DB.GetScalar<int>("usp_Test", new {
				A = 26
			}, commandType: CommandType.StoredProcedure);

			Assert.AreEqual(26, result);
		}

		[Test]
		public void ValueWithParameter()
		{
			var result = DB.GetScalar<int>("SELECT COUNT(*) FROM Persons WHERE Age = @Age", new {
				Age = (byte)55
			});

			Assert.AreEqual(1, result);
		}

		[Test]
		public void ValueType()
		{
			var result = DB.GetScalar<int>("SELECT COUNT(*) FROM Persons");

			Assert.AreEqual(5, result);
		}

		[Test]
		public void NullNullableValueType()
		{
			var result = DB.GetScalar<DateTime?>("SELECT TOP 1 SignedUp FROM Persons WHERE SignedUp IS NULL");

			Assert.AreEqual(null, result);
		}

		[Test]
		public void NonNullNullableValueType()
		{
			var result = DB.GetScalar<DateTime?>("SELECT SignedUp FROM Persons WHERE Name = 'Daniel Gallagher'");

			Assert.AreEqual(Convert.ToDateTime("1997-11-15 21:03:54.000"), result);
		}

		[Test]
		public void NullableValueTypeWithNoRows()
		{
			Assert.Throws<NoRowsException>(() => DB.GetScalar<DateTime?>("SELECT TOP 1 SignedUp FROM Persons WHERE 1=0"));
		}

		[Test]
		public void ThrowsSqlException()
		{
			Assert.Throws<SqlException>(() => DB.Execute("x"));
		}
	}
}