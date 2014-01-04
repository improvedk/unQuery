using System.Data;

namespace unQuery.SqlTypes
{
	public class SqlNChar : ExplicitMaxLengthType<string>
	{
		private SqlNChar() :
			base(SqlDbType.NChar)
		{ }

		public SqlNChar(string value) :
			base(value, SqlDbType.NChar)
		{ }

		public SqlNChar(string value, int maxLength) :
			base(value, SqlDbType.NChar, maxLength: maxLength)
		{ }

		private static readonly ITypeHandler typeHandler = new SqlNChar();
		internal static ITypeHandler GetTypeHandler()
		{
			return typeHandler;
		}
	}
}