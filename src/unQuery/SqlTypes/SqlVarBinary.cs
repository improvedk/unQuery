using System.Data;

namespace unQuery.SqlTypes
{
	public class SqlVarBinary : ExplicitMaxLengthType<byte[]>
	{
		private SqlVarBinary() :
			base(SqlDbType.VarBinary)
		{ }

		public SqlVarBinary(byte[] value, int maxLength) :
			base(value, maxLength, SqlDbType.VarBinary)
		{ }

		private static readonly ITypeHandler typeHandler = new SqlVarBinary();
		internal static ITypeHandler GetTypeHandler()
		{
			return typeHandler;
		}
	}
}