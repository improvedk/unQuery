using System.Data;

namespace unQuery.SqlTypes
{
	public class SqlReal : ExplicitValueType<float?>
	{
		private SqlReal() :
			base(SqlDbType.Real)
		{ }

		public SqlReal(float? value) :
			base(value, SqlDbType.Real)
		{ }

		private static readonly ITypeHandler typeHandler = new SqlReal();
		internal static ITypeHandler GetTypeHandler()
		{
			return typeHandler;
		}
	}
}