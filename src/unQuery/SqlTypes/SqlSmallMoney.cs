using System.Data;
using Microsoft.SqlServer.Server;

namespace unQuery.SqlTypes
{
	public class SqlSmallMoney : ExplicitValueType<decimal?>
	{
		private SqlSmallMoney() :
			base(SqlDbType.SmallMoney)
		{ }

		public SqlSmallMoney(decimal? value) :
			base(value, SqlDbType.SmallMoney)
		{ }

		private static readonly ITypeHandler typeHandler = new SqlSmallMoney();
		internal static ITypeHandler GetTypeHandler()
		{
			return typeHandler;
		}

		public override void SetDataRecordValue(SqlDataRecord record, int ordinal)
		{
			if (Value == null)
				record.SetDBNull(ordinal);
			else
				record.SetDecimal(ordinal, Value.Value);
		}
	}
}