using System.Data;
using System.Data.SqlClient;

namespace unQuery.SqlTypes
{
	public class SqlBit : ISqlType
	{
		private bool? value;

		public SqlBit(bool? value)
		{
			this.value = value;
		}

		public SqlParameter GetParameter()
		{
			var param = new SqlParameter("", SqlDbType.Bit);
			param.Value = value;

			return param;
		}
	}
}