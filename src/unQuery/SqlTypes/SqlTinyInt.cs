using System.Data;
using System.Data.SqlClient;

namespace unQuery.SqlTypes
{
	public class SqlTinyInt : ISqlType
	{
		private byte? value;

		public SqlTinyInt(byte? value)
		{
			this.value = value;
		}

		public SqlParameter GetParameter()
		{
			var param = new SqlParameter("", SqlDbType.TinyInt);
			param.Value = value;

			return param;
		}
	}
}