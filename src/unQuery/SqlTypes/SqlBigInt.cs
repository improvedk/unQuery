using System.Data;

namespace unQuery.SqlTypes
{
	public class SqlBigInt : ImplicitValueType<long?>
	{
		private SqlBigInt() :
			base(SqlDbType.BigInt)
		{ }

		public SqlBigInt(long? value) :
			base(value, SqlDbType.BigInt)
		{ }

		private static readonly ITypeHandler typeHandler = new SqlBigInt();
		internal static ITypeHandler GetTypeHandler()
		{
			return typeHandler;
		}
	}
}