using System.Data;

namespace unQuery.SqlTypes
{
	public class SqlNVarChar : MaxLengthType<string>
	{
		private SqlNVarChar() :
			base(SqlDbType.NVarChar)
		{ }

		public SqlNVarChar(string value, int maxLength) :
			base(value, maxLength, SqlDbType.NVarChar)
		{ }

		private static readonly ITypeHandler typeHandler = new SqlNVarChar();
		internal static ITypeHandler GetTypeHandler()
		{
			return typeHandler;
		}
	}
}