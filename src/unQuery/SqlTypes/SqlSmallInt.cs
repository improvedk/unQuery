using System.Data;
using System.Data.SqlClient;

namespace unQuery.SqlTypes
{
	public class SqlSmallInt : ISqlType
	{
		private byte? value;

		public SqlSmallInt(byte? value)
		{
			this.value = value;
		}

		public SqlParameter GetParameter()
		{
			var param = new SqlParameter("", SqlDbType.SmallInt);
			param.Value = value;

			return param;
		}
	}
}