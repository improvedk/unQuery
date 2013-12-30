using System.Data;

namespace unQuery.SqlTypes
{
	public class SqlChar : ExplicitMaxLengthType<string>
	{
		private SqlChar() :
			base(SqlDbType.Char)
		{ }

		public SqlChar(string value) :
			base(value, null, SqlDbType.Char)
		{ }

		public SqlChar(string value, int maxLength) :
			base(value, maxLength, SqlDbType.Char)
		{ }

		private static readonly ITypeHandler typeHandler = new SqlChar();
		internal static ITypeHandler GetTypeHandler()
		{
			return typeHandler;
		}
	}
}