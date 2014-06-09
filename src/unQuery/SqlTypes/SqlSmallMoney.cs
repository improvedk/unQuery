using System.Data;
using Microsoft.SqlServer.Server;

namespace unQuery.SqlTypes
{
	public class SqlSmallMoney : ExplicitValueType<decimal?>
	{
		private SqlSmallMoney() :
			base(SqlDbType.SmallMoney)
		{ }

		internal SqlSmallMoney(decimal? value, ParameterDirection direction) :
			base(value, SqlDbType.SmallMoney, direction)
		{ }

		private static readonly SqlTypeHandler typeHandler = new SqlSmallMoney();
		internal static SqlTypeHandler GetTypeHandler()
		{
			return typeHandler;
		}

		internal override void SetDataRecordValue(SqlDataRecord record, int ordinal)
		{
			if (InputValue == null)
				record.SetDBNull(ordinal);
			else
				record.SetDecimal(ordinal, InputValue.Value);
		}
	}
}