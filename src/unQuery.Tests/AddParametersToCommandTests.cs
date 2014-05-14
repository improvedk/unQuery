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
		public void ClashesWithExistingParameter()
		{
			var cmd = new SqlCommand();
			cmd.Parameters.Add("@A", SqlDbType.Int).Value = 52;
			cmd.Parameters.Add("B", SqlDbType.Int).Value = 52;

			Assert.Throws<SqlCommandAlreadyHasParametersException>(() => db.AddParametersToCommand(cmd.Parameters, new { A = 25 }));
			Assert.Throws<SqlCommandAlreadyHasParametersException>(() => db.AddParametersToCommand(cmd.Parameters, new { B = 25 }));
		}

		[Test]
		public void SqlTypes()
		{
			var cmd = new SqlCommand();

			db.AddParametersToCommand(cmd.Parameters, new {
				A = Col.Int(25),
				B = Col.Bit(true),
				C = Col.Bit(null),
				D = Col.Decimal(5.27m),
				E = Col.Decimal(5.27m, 6, 4),
				F = Col.NVarChar("Hello"),
				G = Col.NVarChar("Hello", 128),
				H = Col.NVarChar(null, 128)
			});

			Assert.AreEqual(8, cmd.Parameters.Count);

			Assert.AreEqual(SqlDbType.Int, cmd.Parameters[0].SqlDbType);
			Assert.AreEqual(typeof(int), cmd.Parameters[0].Value.GetType());
			Assert.AreEqual("@A", cmd.Parameters[0].ParameterName);
			Assert.AreEqual(25, cmd.Parameters[0].Value);

			Assert.AreEqual(SqlDbType.Bit, cmd.Parameters[1].SqlDbType);
			Assert.AreEqual(typeof(bool), cmd.Parameters[1].Value.GetType());
			Assert.AreEqual("@B", cmd.Parameters[1].ParameterName);
			Assert.AreEqual(true, cmd.Parameters[1].Value);

			Assert.AreEqual(SqlDbType.Bit, cmd.Parameters[2].SqlDbType);
			Assert.AreEqual("@C", cmd.Parameters[2].ParameterName);
			Assert.AreEqual(DBNull.Value, cmd.Parameters[2].Value);

			Assert.AreEqual(SqlDbType.Decimal, cmd.Parameters[3].SqlDbType);
			Assert.AreEqual(typeof(decimal), cmd.Parameters[3].Value.GetType());
			Assert.AreEqual("@D", cmd.Parameters[3].ParameterName);
			Assert.AreEqual(5.27m, cmd.Parameters[3].Value);
			Assert.AreEqual(3, cmd.Parameters[3].Precision);
			Assert.AreEqual(2, cmd.Parameters[3].Scale);

			Assert.AreEqual(SqlDbType.Decimal, cmd.Parameters[4].SqlDbType);
			Assert.AreEqual(typeof(decimal), cmd.Parameters[4].Value.GetType());
			Assert.AreEqual("@E", cmd.Parameters[4].ParameterName);
			Assert.AreEqual(5.27m, cmd.Parameters[4].Value);
			Assert.AreEqual(6, cmd.Parameters[4].Precision);
			Assert.AreEqual(4, cmd.Parameters[4].Scale);

			Assert.AreEqual(SqlDbType.NVarChar, cmd.Parameters[5].SqlDbType);
			Assert.AreEqual(typeof(string), cmd.Parameters[5].Value.GetType());
			Assert.AreEqual("@F", cmd.Parameters[5].ParameterName);
			Assert.AreEqual("Hello", cmd.Parameters[5].Value);
			Assert.AreEqual(64, cmd.Parameters[5].Size);

			Assert.AreEqual(SqlDbType.NVarChar, cmd.Parameters[6].SqlDbType);
			Assert.AreEqual(typeof(string), cmd.Parameters[6].Value.GetType());
			Assert.AreEqual("@G", cmd.Parameters[6].ParameterName);
			Assert.AreEqual("Hello", cmd.Parameters[6].Value);
			Assert.AreEqual(128, cmd.Parameters[6].Size);

			Assert.AreEqual(SqlDbType.NVarChar, cmd.Parameters[7].SqlDbType);
			Assert.AreEqual("@H", cmd.Parameters[7].ParameterName);
			Assert.AreEqual(DBNull.Value, cmd.Parameters[7].Value);
			Assert.AreEqual(128, cmd.Parameters[7].Size);
		}

		[Test]
		public void ImplicitTypes()
		{
			var guid = Guid.NewGuid();
			var cmd = new SqlCommand();

			db.AddParametersToCommand(cmd.Parameters, new {
				A = (byte)1,
				B = (short)2,
				C = (int)3,
				D = true,
				E = guid,
				F = (byte?)null,
				G = (byte?)1,
				H = (Guid?)null,
				I = (bool?)false
			});

			Assert.AreEqual(9, cmd.Parameters.Count);

			Assert.AreEqual(SqlDbType.TinyInt, cmd.Parameters[0].SqlDbType);
			Assert.AreEqual(typeof(byte), cmd.Parameters[0].Value.GetType());
			Assert.AreEqual("@A", cmd.Parameters[0].ParameterName);
			Assert.AreEqual(1, cmd.Parameters[0].Value);

			Assert.AreEqual(SqlDbType.SmallInt, cmd.Parameters[1].SqlDbType);
			Assert.AreEqual(typeof(short), cmd.Parameters[1].Value.GetType());
			Assert.AreEqual("@B", cmd.Parameters[1].ParameterName);
			Assert.AreEqual(2, cmd.Parameters[1].Value);

			Assert.AreEqual(SqlDbType.Int, cmd.Parameters[2].SqlDbType);
			Assert.AreEqual(typeof(int), cmd.Parameters[2].Value.GetType());
			Assert.AreEqual("@C", cmd.Parameters[2].ParameterName);
			Assert.AreEqual(3, cmd.Parameters[2].Value);

			Assert.AreEqual(SqlDbType.Bit, cmd.Parameters[3].SqlDbType);
			Assert.AreEqual(typeof(bool), cmd.Parameters[3].Value.GetType());
			Assert.AreEqual("@D", cmd.Parameters[3].ParameterName);
			Assert.AreEqual(true, cmd.Parameters[3].Value);

			Assert.AreEqual(SqlDbType.UniqueIdentifier, cmd.Parameters[4].SqlDbType);
			Assert.AreEqual(typeof(Guid), cmd.Parameters[4].Value.GetType());
			Assert.AreEqual("@E", cmd.Parameters[4].ParameterName);
			Assert.AreEqual(guid, cmd.Parameters[4].Value);

			Assert.AreEqual(SqlDbType.TinyInt, cmd.Parameters[5].SqlDbType);
			Assert.AreEqual("@F", cmd.Parameters[5].ParameterName);
			Assert.AreEqual(DBNull.Value, cmd.Parameters[5].Value);

			Assert.AreEqual(SqlDbType.TinyInt, cmd.Parameters[6].SqlDbType);
			Assert.AreEqual(typeof(byte), cmd.Parameters[6].Value.GetType());
			Assert.AreEqual("@G", cmd.Parameters[6].ParameterName);
			Assert.AreEqual(1, cmd.Parameters[6].Value);

			Assert.AreEqual(SqlDbType.UniqueIdentifier, cmd.Parameters[7].SqlDbType);
			Assert.AreEqual("@H", cmd.Parameters[7].ParameterName);
			Assert.AreEqual(DBNull.Value, cmd.Parameters[7].Value);

			Assert.AreEqual(SqlDbType.Bit, cmd.Parameters[8].SqlDbType);
			Assert.AreEqual(typeof(bool), cmd.Parameters[8].Value.GetType());
			Assert.AreEqual("@I", cmd.Parameters[8].ParameterName);
			Assert.AreEqual(false, cmd.Parameters[8].Value);
		}

		[Test]
		public void MixedTypes()
		{
			var cmd = new SqlCommand();

			db.AddParametersToCommand(cmd.Parameters, new {
				A = true,
				B = Col.Bit(false),
				C = new SqlSmallInt(null),
				D = new SqlInt(5),
				E = Col.VarChar("Test", 10),
				F = (bool?)null,
				G = 5L,
				H = (int?)null,
				I = (short)2,
				J = Col.Decimal(5.27m, 6, 2),
				K = (Guid?)null
			});

			Assert.AreEqual(11, cmd.Parameters.Count);
		}

		[Test]
		public void NonSupportedTypes()
		{
			Assert.Throws<ParameterTypeNotSupportedException>(() => db.AddParametersToCommand(new SqlCommand().Parameters, new {
				Test = "Hello"
			}));
		}
	}
}