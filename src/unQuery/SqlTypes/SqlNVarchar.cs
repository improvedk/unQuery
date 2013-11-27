using System;
using System.Data;
using System.Data.SqlClient;

namespace unQuery.SqlTypes
{
	public class SqlNVarchar : ISqlType
	{
		private readonly string value;
		private readonly int? size;

		public SqlNVarchar(string value)
		{
			this.value = value;
		}

		public SqlNVarchar(string value, int size)
		{
			this.value = value;
			this.size = size;
		}

		public static explicit operator SqlNVarchar(string value)
		{
			return new SqlNVarchar(value);
		}

		public SqlParameter GetParameter()
		{
			return GetParameter(value, size);
		}

		public static SqlParameter GetParameter(object value)
		{
			return GetParameter(value, null);
		}

		public static SqlParameter GetParameter(object value, int? size)
		{
			var param = new SqlParameter {
				SqlDbType = SqlDbType.NVarChar,
				Value = value ?? DBNull.Value
			};

			if (size != null || value != null)
				param.Size = size ?? param.Value.ToString().Length;

			return param;
		}
	}
}