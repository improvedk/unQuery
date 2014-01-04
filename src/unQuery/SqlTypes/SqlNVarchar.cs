using System.Data;

namespace unQuery.SqlTypes
{
	public class SqlNVarChar : ExplicitMaxLengthType<string>
	{
		private SqlNVarChar() :
			base(SqlDbType.NVarChar)
		{ }

		public SqlNVarChar(string value) :
			base(value, SqlDbType.NVarChar, valueLength: (value != null ? value.Length : 0))
		{ }

		public SqlNVarChar(string value, int maxLength) :
			base(value, SqlDbType.NVarChar, maxLength: maxLength)
		{ }

		private static readonly ITypeHandler typeHandler = new SqlNVarChar();
		internal static ITypeHandler GetTypeHandler()
		{
			return typeHandler;
		}
	}
}