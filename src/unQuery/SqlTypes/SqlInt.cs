using System.Data;

namespace unQuery.SqlTypes
{
	public class SqlInt : ImplicitValueType<int?>
	{
		private SqlInt() :
			base(SqlDbType.Int)
		{ }

		public SqlInt(int? value) :
			base(value, SqlDbType.Int)
		{ }

		private static readonly ITypeHandler typeHandler = new SqlInt();
		internal static ITypeHandler GetTypeHandler()
		{
			return typeHandler;
		}
	}
}