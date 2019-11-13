using System.Data;
using Microsoft.Data.SqlClient.Server;

namespace unQuery.SqlTypes
{
	public class SqlDecimal : ExplicitPrecisionAndScaleType<decimal?>
	{
		private SqlDecimal() :
			base(SqlDbType.Decimal)
		{ }

		internal SqlDecimal(decimal? value, byte? precision, byte? scale, ParameterDirection direction) :
			base(value, precision, scale, SqlDbType.Decimal, direction)
		{ }

		private static readonly SqlTypeHandler typeHandler = new SqlDecimal();
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