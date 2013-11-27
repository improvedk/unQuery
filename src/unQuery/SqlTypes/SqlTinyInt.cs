using System;
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

		public static explicit operator SqlTinyInt(byte? value)
		{
			return new SqlTinyInt(value);
		}

		public SqlParameter GetParameter()
		{
			return GetParameter(value);
		}

		public static SqlParameter GetParameter(object value)
		{
			return new SqlParameter {
				SqlDbType = SqlDbType.TinyInt,
				Value = value ?? DBNull.Value
			};
		}
	}
}