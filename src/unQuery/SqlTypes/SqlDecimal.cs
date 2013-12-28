using System.Data;

namespace unQuery.SqlTypes
{
	public class SqlDecimal : ExplicitPrecisionAndScaleType<decimal?>
	{
		private SqlDecimal() :
			base(SqlDbType.Decimal)
		{ }

		public SqlDecimal(decimal? value, byte precision, byte scale) :
			base(value, precision, scale, SqlDbType.Decimal)
		{ }

		private static readonly ITypeHandler typeHandler = new SqlDecimal();
		internal static ITypeHandler GetTypeHandler()
		{
			return typeHandler;
		}
	}
}