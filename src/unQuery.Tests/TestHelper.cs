using NUnit.Framework;
using System.Data;
using System.Data.SqlClient;

namespace unQuery.Tests
{
	public class TestHelper
	{
		internal static void AssertParameterFromValue<TValue, TParamValue>(TValue value, SqlDbType expectedType, TParamValue paramValue, int? size = null)
		{
			var param = unQueryDB.ClrTypeHandlers[typeof(TValue)].CreateParamFromValue(value);
			Assert.AreEqual(expectedType, param.SqlDbType);
			Assert.AreEqual(paramValue, param.Value);
			Assert.AreEqual(paramValue.GetType(), param.Value.GetType());

			if (size != null)
				Assert.AreEqual(size, param.Size);
		}

		internal static void AssertSqlParameter(SqlParameter param, SqlDbType expectedDbType, int? size, object value)
		{
			Assert.AreEqual(expectedDbType, param.SqlDbType);
			Assert.AreEqual(value, param.Value);
			Assert.AreEqual(value.GetType(), param.Value.GetType());

			if (size != null)
				Assert.AreEqual(size, param.Size);
		}
	}
}