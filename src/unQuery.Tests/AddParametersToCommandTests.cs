using NUnit.Framework;
using System;
using System.Data;
using System.Data.SqlClient;
using unQuery.SqlTypes;

namespace unQuery.Tests
{
	[TestFixture]
	public class AddParametersToCommandTests
	{
		private readonly unQueryDB db = new unQueryDB(null);

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
			var cmd = new SqlCommand();

			db.AddParametersToCommand(cmd, new {
				A = true,
				B = Col.Bit(false),
				C = new SqlSmallInt(null),
				D = new SqlInt(5),
				E = Col.VarChar("Test", 10)
			});

			Assert.AreEqual(5, cmd.Parameters.Count);
			Assert.AreEqual(SqlDbType.Bit, cmd.Parameters[0].SqlDbType);
			Assert.AreEqual(true, cmd.Parameters[0].Value);
			Assert.AreEqual(SqlDbType.Bit, cmd.Parameters[1].SqlDbType);
			Assert.AreEqual(false, cmd.Parameters[1].Value);
			Assert.AreEqual(SqlDbType.SmallInt, cmd.Parameters[2].SqlDbType);
			Assert.AreEqual(DBNull.Value, cmd.Parameters[2].Value);
			Assert.AreEqual(SqlDbType.Int, cmd.Parameters[3].SqlDbType);
			Assert.AreEqual(5, cmd.Parameters[3].Value);
			Assert.AreEqual(SqlDbType.VarChar, cmd.Parameters[4].SqlDbType);
			Assert.AreEqual("Test", cmd.Parameters[4].Value);
			Assert.AreEqual(10, cmd.Parameters[4].Size);
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