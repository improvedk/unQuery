using NUnit.Framework;
using System;
using System.Data;
using System.Data.SqlClient;

namespace unQuery.Tests
{
	[TestFixture]
	public class AddParametersToCommandTests
	{
		private unQueryDB db = new unQueryDB(null);

		[Test]
		public void AddToExistingParameterCollection()
		{
			var cmd = new SqlCommand();
			cmd.Parameters.Add("@A", SqlDbType.Int).Value = 52;

			db.AddParametersToCommand(cmd, new { B = 25 });

			Assert.AreEqual(2, cmd.Parameters.Count);
			Assert.AreEqual(52, cmd.Parameters["@A"].Value);
			Assert.AreEqual(25, cmd.Parameters["@B"].Value);
		}

		[Test]
		public void ClashesWithExistingParameter()
		{
			var cmd = new SqlCommand();
			cmd.Parameters.Add("@A", SqlDbType.Int).Value = 52;
			cmd.Parameters.Add("B", SqlDbType.Int).Value = 52;

			Assert.Throws<DuplicateParameterException>(() => db.AddParametersToCommand(cmd, new { A = 25 }));
			Assert.Throws<DuplicateParameterException>(() => db.AddParametersToCommand(cmd, new { B = 25 }));
		}

		[Test]
		public void ImplicitTypes()
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
		public void NonSupportedTypes()
		{
			Assert.Throws<TypeNotSupportedException>(() => db.AddParametersToCommand(new SqlCommand(), new {
				Test = "Hello"
			}));
		}
	}
}