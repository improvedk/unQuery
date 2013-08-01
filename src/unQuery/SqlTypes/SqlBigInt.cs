using System.Data;
using System.Data.SqlClient;

namespace unQuery.SqlTypes
{
	public class SqlBigInt : ISqlType
	{
		private readonly long? value;

		public SqlBigInt(long? value)
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
				SqlDbType = SqlDbType.BigInt,
				Value = value
			};
		}
	}
}