using System.Data;

namespace unQuery.SqlTypes
{
	public class SqlNText : MaxLengthType<string>
	{
		private SqlNText() :
			base(SqlDbType.NText)
		{ }

		public SqlNText(string value) :
			base(value, -1, SqlDbType.NText)
		{ }

		private static readonly ITypeHandler typeHandler = new SqlNText();
		internal static ITypeHandler GetTypeHandler()
		{
			return typeHandler;
		}
	}
}