using System.Data;

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

		private static readonly ITypeHandler typeHandler = new SqlMoney();
		internal static ITypeHandler GetTypeHandler()
		{
			return typeHandler;
		}
	}
}