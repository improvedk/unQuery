using System.Data;

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
	}
}