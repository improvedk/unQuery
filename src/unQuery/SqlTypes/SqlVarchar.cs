using System.Data;
using System.Data.SqlClient;

namespace unQuery.SqlTypes
{
	public class SqlVarchar : ISqlType
	{
		private readonly string value;
		private readonly int? size;

		public SqlVarchar(string value)
		{
			this.value = value;
		}

		public SqlVarchar(string value, int size)
		{
			this.value = value;
			this.size = size;
		}

		public static explicit operator SqlVarchar(string value)
		{
			return new SqlVarchar(value);
		}

		public SqlParameter GetParameter()
		{
			return GetParameter(value, size);
		}

		public static SqlParameter GetParameter(string value)
		{
			return GetParameter(value, null);
		}

		public static SqlParameter GetParameter(string value, int? size)
		{
			var param = new SqlParameter {
				SqlDbType = SqlDbType.VarChar,
				Value = TypeHelper.GetDBNullableValue(value)
			};

			if (size != null || value != null)
				param.Size = size ?? param.Value.ToString().Length;

			return param;
		}
	}
}