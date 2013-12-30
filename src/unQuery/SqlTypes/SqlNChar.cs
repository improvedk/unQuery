using System.Data;

namespace unQuery.SqlTypes
{
	public class SqlNChar : ExplicitMaxLengthType<string>
	{
		private SqlNChar() :
			base(SqlDbType.NChar)
		{ }

		public SqlNChar(string value) :
			base(value, null, SqlDbType.NChar)
		{ }

		public SqlNChar(string value, int maxLength) :
			base(value, maxLength, SqlDbType.NChar)
		{ }

		private static readonly ITypeHandler typeHandler = new SqlNChar();
		internal static ITypeHandler GetTypeHandler()
		{
			return typeHandler;
		}
	}
}