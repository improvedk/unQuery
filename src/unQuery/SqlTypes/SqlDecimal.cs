using System.Data;
using Microsoft.SqlServer.Server;

namespace unQuery.SqlTypes
{
	public class SqlDecimal : ExplicitPrecisionAndScaleType<decimal?>
	{
		private SqlDecimal() :
			base(SqlDbType.Decimal)
		{ }

		public SqlDecimal(decimal? value) :
			base(value, null, null, SqlDbType.Decimal)
		{ }

		public SqlDecimal(decimal? value, byte precision, byte scale) :
			base(value, precision, scale, SqlDbType.Decimal)
		{ }

		private static readonly ITypeHandler typeHandler = new SqlDecimal();
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