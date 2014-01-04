using System.Data;

namespace unQuery.SqlTypes
{
	public class SqlNText : ExplicitMaxLengthType<string>
	{
		private SqlNText() :
			base(SqlDbType.NText)
		{ }

		public SqlNText(string value) :
			base(value, SqlDbType.NText, maxLength: -1)
		{ }

		private static readonly ITypeHandler typeHandler = new SqlNText();
		internal static ITypeHandler GetTypeHandler()
		{
			return typeHandler;
		}
	}
}