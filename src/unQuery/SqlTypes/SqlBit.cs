using System.Data;

namespace unQuery.SqlTypes
{
	public class SqlBit : ImplicitValueType<bool?>
	{
		private SqlBit() :
			base(SqlDbType.Bit)
		{ }

		public SqlBit(bool? value) :
			base(value, SqlDbType.Bit)
		{ }

		private static readonly ITypeHandler typeHandler = new SqlBit();
		internal static ITypeHandler GetTypeHandler()
		{
			return typeHandler;
		}
	}
}