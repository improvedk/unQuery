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

		public SqlParameter GetParameter()
		{
			return GetParameter(value);
		}

		public static SqlParameter GetParameter(object value)
		{
			return new SqlParameter {
				SqlDbType = SqlDbType.SmallInt,
				Value = value
			};
		}
	}
}