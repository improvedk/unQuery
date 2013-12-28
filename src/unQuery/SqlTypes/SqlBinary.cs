using System.Data;

namespace unQuery.SqlTypes
{
	public class SqlBinary : MaxLengthType<byte[]>
	{
		private SqlBinary() :
			base(SqlDbType.Binary)
		{ }

		public SqlBinary(byte[] value, int maxLength) :
			base(value, maxLength, SqlDbType.Binary)
		{ }

		private static readonly ITypeHandler typeHandler = new SqlBinary();
		internal static ITypeHandler GetTypeHandler()
		{
			return typeHandler;
		}
	}
}