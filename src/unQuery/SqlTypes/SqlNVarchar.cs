using System.Data;
using System.Data.SqlClient;

namespace unQuery.SqlTypes
{
	// TODO: Better handling of length. Instead of setting explicit length, try to reuse common lengths.
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
			return new SqlParameter {
				SqlDbType = SqlDbType.NVarChar,
				Value = value,
				Size = value.ToString().Length
			};
		}

		public static SqlParameter GetParameter(object value, int? size)
		{
			return new SqlParameter
			{
				SqlDbType = SqlDbType.NVarChar,
				Value = value,
				Size = size ?? value.ToString().Length
			};
		}
	}
}