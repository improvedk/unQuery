using System.Data;

namespace unQuery.SqlTypes
{
	public class SqlVarBinary : ExplicitMaxLengthType<byte[]>
	{
		private SqlVarBinary() :
			base(SqlDbType.VarBinary)
		{ }

		public SqlVarBinary(byte[] value) :
			base(value, SqlDbType.VarBinary, valueLength: (value != null ? value.Length : 0))
		{ }

		public SqlVarBinary(byte[] value, int maxLength) :
			base(value, SqlDbType.VarBinary, maxLength: maxLength)
		{ }

		private static readonly ITypeHandler typeHandler = new SqlVarBinary();
		internal static ITypeHandler GetTypeHandler()
		{
			return typeHandler;
		}
	}
}