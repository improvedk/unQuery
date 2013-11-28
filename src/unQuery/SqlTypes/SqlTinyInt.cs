using System.Data;
using System.Data.SqlClient;

namespace unQuery.SqlTypes
{
	public class SqlTinyInt : ISqlType
	{
		private readonly byte? value;

		public SqlTinyInt(byte? value)
		{
			this.value = value;
		}

		public static explicit operator SqlTinyInt(long? value)
		{
			return new SqlTinyInt((byte?)value);
		}

		public SqlParameter GetParameter()
		{
			return GetParameter(value);
		}

		public static SqlParameter GetParameter(byte? value)
		{
			return new SqlParameter {
				SqlDbType = SqlDbType.TinyInt,
				Value = TypeHelper.GetDBNullableValue(value)
			};
		}
	}
}