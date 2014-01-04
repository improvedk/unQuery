using System.Data;

namespace unQuery.SqlTypes
{
	public class SqlChar : ExplicitMaxLengthType<string>
	{
		private SqlChar() :
			base(SqlDbType.Char)
		{ }

		public SqlChar(string value) :
			base(value, SqlDbType.Char)
		{ }

		public SqlChar(string value, int maxLength) :
			base(value, SqlDbType.Char, maxLength: maxLength)
		{ }

		private static readonly ITypeHandler typeHandler = new SqlChar();
		internal static ITypeHandler GetTypeHandler()
		{
			return typeHandler;
		}
	}
}