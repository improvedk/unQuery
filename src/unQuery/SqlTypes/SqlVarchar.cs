using System.Data;
using System.Data.SqlClient;

namespace unQuery.SqlTypes
{
	public class SqlVarChar : ISqlType
	{
		private readonly string value;
		private readonly int? size;

		public SqlVarChar(string value)
		{
			this.value = value;
		}

		public SqlVarChar(string value, int size)
		{
			this.value = value;
			this.size = size;
		}

		public static explicit operator SqlVarChar(string value)
		{
			return new SqlVarChar(value);
		}

		public SqlParameter GetParameter()
		{
			return GetParameter(value, size);
		}

		public SqlDbType GetSqlDbType()
		{
			return SqlDbType.VarChar;
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
				SqlDbType = SqlDbType.VarChar,
				Value = TypeHelper.GetDBNullableValue(value)
			};

			if (size != null)
				param.Size = size.Value;

			return param;
		}
	}
}