using System;
using System.Data;
using System.Data.SqlClient;

namespace unQuery.SqlTypes
{
	public class SqlSmallInt : ISqlType
	{
		private readonly short? value;

		public SqlSmallInt(short? value)
		{
			this.value = value;
		}

		public static explicit operator SqlSmallInt(short? value)
		{
			return new SqlSmallInt(value);
		}

		public static explicit operator SqlSmallInt(int? value)
		{
			return new SqlSmallInt((short?)value);
		}

		public SqlParameter GetParameter()
		{
			return GetParameter(value);
		}

		public static SqlParameter GetParameter(short? value)
		{
			return new SqlParameter {
				SqlDbType = SqlDbType.SmallInt,
				Value = TypeHelper.GetDBNullableValue(value)
			};
		}
	}
}