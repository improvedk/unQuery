using System.Data;
using System.Data.SqlClient;

namespace unQuery.SqlTypes
{
	public class SqlNVarChar : ISqlType
	{
		private readonly string value;
		private readonly int? size;

		public SqlNVarChar(string value)
		{
			this.value = value;
		}

		public SqlNVarChar(string value, int size)
		{
			this.value = value;
			this.size = size;
		}

		public static explicit operator SqlNVarChar(string value)
		{
			return new SqlNVarChar(value);
		}

		public SqlParameter GetParameter()
		{
			return GetParameter(value, size);
		}

		public SqlDbType GetDbType()
		{
			return SqlDbType.NVarChar;
		}

		public object GetRawValue()
		{
			return value;
		}

		public static SqlParameter GetParameter(string value)
		{
			return GetParameter(value, null);
		}

		public static SqlParameter GetParameter(string value, int? size)
		{
			var param = new SqlParameter {
				SqlDbType = SqlDbType.NVarChar,
				Value = TypeHelper.GetDBNullableValue(value)
			};

			if (size != null)
				param.Size = size.Value;

			return param;
		}
	}
}