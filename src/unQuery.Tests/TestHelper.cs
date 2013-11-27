using NUnit.Framework;
using System.Data;
using System.Data.SqlClient;

namespace unQuery.Tests
{
	public class TestHelper
	{
		internal static unQuery DB = new TestDB();

		internal static void AssertParameterFromValue<TValue, TParamValue>(TValue value, SqlDbType expectedType, TParamValue paramValue, int? size = null)
		{
			var cmd = new SqlCommand();

			DB.AddParametersToCommand(cmd, new {
				Test = value
			});

			var param = cmd.Parameters[0];

			Assert.AreEqual("@Test", param.ParameterName);
			Assert.AreEqual(expectedType, param.SqlDbType);
			Assert.AreEqual(paramValue, param.Value);
			Assert.AreEqual(paramValue.GetType(), param.Value.GetType());

			if (size != null)
				Assert.AreEqual(size, param.Size);
		}

		internal static void AssertParameterFromValue(SqlParameter param, SqlDbType expectedDbType, int? size, object value)
		{
			Assert.AreEqual(expectedDbType, param.SqlDbType);
			Assert.AreEqual(size ?? 0, param.Size);
			Assert.AreEqual(value, param.Value);
		}
	}
}