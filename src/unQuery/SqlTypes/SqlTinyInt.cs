using System.Data;

namespace unQuery.SqlTypes
{
	public class SqlTinyInt : ImplicitValueType<byte?>
	{
		private SqlTinyInt() :
			base(SqlDbType.TinyInt)
		{ }

		public SqlTinyInt(byte? value) :
			base(value, SqlDbType.TinyInt)
		{ }

		private static readonly ITypeHandler typeHandler = new SqlTinyInt();
		internal static ITypeHandler GetTypeHandler()
		{
			return typeHandler;
		}
	}
}