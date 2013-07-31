using System.Data;
using System.Data.SqlClient;

namespace unQuery.SqlTypes
{
	public class SqlInt : ISqlType
	{
		private int? value;

		public SqlInt(int? value)
		{
			this.value = value;
		}

		public SqlParameter GetParameter()
		{
			var param = new SqlParameter("", SqlDbType.Int);
			param.Value = value;

			return param;
		}
	}
}