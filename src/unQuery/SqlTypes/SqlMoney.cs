using System.Data;
using Microsoft.SqlServer.Server;

namespace unQuery.SqlTypes
{
	public class SqlMoney : ExplicitValueType<decimal?>
	{
		private SqlMoney() :
			base(SqlDbType.Money)
		{ }

		public SqlMoney(decimal? value) :
			base(value, SqlDbType.Money)
		{ }

		private static readonly SqlTypeHandler typeHandler = new SqlMoney();
		internal static SqlTypeHandler GetTypeHandler()
		{
			return typeHandler;
		}

		internal override void SetDataRecordValue(SqlDataRecord record, int ordinal)
		{
			if (Value == null)
				record.SetDBNull(ordinal);
			else
				record.SetDecimal(ordinal, Value.Value);
		}
	}
}