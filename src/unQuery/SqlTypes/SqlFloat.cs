using System.Data;

namespace unQuery.SqlTypes
{
	public class SqlFloat : ExplicitValueType<double?>
	{
		private SqlFloat() :
			base(SqlDbType.Float)
		{ }

		public SqlFloat(double? value) :
			base(value, SqlDbType.Float)
		{ }

		private static readonly ITypeHandler typeHandler = new SqlFloat();
		internal static ITypeHandler GetTypeHandler()
		{
			return typeHandler;
		}
	}
}