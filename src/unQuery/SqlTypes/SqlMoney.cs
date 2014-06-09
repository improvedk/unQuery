using System.Data;
using Microsoft.SqlServer.Server;

namespace unQuery.SqlTypes
{
	public class SqlMoney : ExplicitValueType<decimal?>
	{
		private SqlMoney() :
			base(SqlDbType.Money)
		{ }

		internal SqlMoney(decimal? value, ParameterDirection direction) :
			base(value, SqlDbType.Money, direction)
		{ }

		private static readonly SqlTypeHandler typeHandler = new SqlMoney();
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