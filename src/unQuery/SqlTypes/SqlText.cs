using System.Data;

namespace unQuery.SqlTypes
{
	public class SqlText : ExplicitMaxLengthType<string>
	{
		private SqlText() :
			base(SqlDbType.Text)
		{ }

		public SqlText(string value) :
			base(value, SqlDbType.Text, maxLength: -1)
		{ }

		private static readonly ITypeHandler typeHandler = new SqlText();
		internal static ITypeHandler GetTypeHandler()
		{
			return typeHandler;
		}
	}
}