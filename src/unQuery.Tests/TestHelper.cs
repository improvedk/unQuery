using NUnit.Framework;
using System.Data;
using Microsoft.Data.SqlClient;

namespace unQuery.Tests
{
	public class TestHelper
	{
		internal static void AssertParameterFromValue<TValue, TParamValue>(TValue value, SqlDbType expectedType, TParamValue paramValue, int? size = null, byte? precision = null, int? scale = null)
		{
			var param = unQueryDB.ClrTypeHandlers[typeof(TValue)].CreateParamFromValue("Test", value);

			Assert.AreEqual(expectedType, param.SqlDbType);
			Assert.AreEqual(paramValue, param.Value);
			Assert.AreEqual(paramValue.GetType(), param.Value.GetType());
			Assert.AreEqual("@Test", param.ParameterName);

			if (size != null)
				Assert.AreEqual(size, param.Size);

			if (precision != null)
				Assert.AreEqual(precision, param.Precision);

			if (scale != null)
				Assert.AreEqual(scale, param.Scale);
		}

		internal static void AssertSqlParameter(SqlParameter param, SqlDbType expectedDbType, object value, int? size = null, byte? precision = null, int? scale = null)
		{
			Assert.AreEqual(expectedDbType, param.SqlDbType);
			Assert.AreEqual(value, param.Value);
			Assert.AreEqual(value.GetType(), param.Value.GetType());

			if (size != null)
				Assert.AreEqual(size, param.Size);

			if (precision != null)
				Assert.AreEqual(precision, param.Precision);

			if (scale != null)
				Assert.AreEqual(scale, param.Scale);
		}
	}
}