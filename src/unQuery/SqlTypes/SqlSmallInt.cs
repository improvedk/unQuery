using System.Data;

namespace unQuery.SqlTypes
{
	public class SqlSmallInt : ImplicitValueType<short?>
	{
		private SqlSmallInt() :
			base(SqlDbType.SmallInt)
		{ }

		public SqlSmallInt(short? value) :
			base(value, SqlDbType.SmallInt)
		{ }

		private static readonly ITypeHandler typeHandler = new SqlSmallInt();
		internal static ITypeHandler GetTypeHandler()
		{
			return typeHandler;
		}
	}
}