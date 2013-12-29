using System.Data;

namespace unQuery.SqlTypes
{
	public class SqlText : ExplicitMaxLengthType<string>
	{
		private SqlText() :
			base(SqlDbType.Text)
		{ }

		public SqlText(string value) :
			base(value, -1, SqlDbType.Text)
		{ }

		private static readonly ITypeHandler typeHandler = new SqlText();
		internal static ITypeHandler GetTypeHandler()
		{
			return typeHandler;
		}
	}
}