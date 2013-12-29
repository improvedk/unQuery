using System.Data;

namespace unQuery.SqlTypes
{
	public class SqlXml : ExplicitValueType<string>
	{
		private SqlXml() :
			base(SqlDbType.Xml)
		{ }

		public SqlXml(string value) :
			base(value, SqlDbType.Xml)
		{ }

		private static readonly ITypeHandler typeHandler = new SqlXml();
		internal static ITypeHandler GetTypeHandler()
		{
			return typeHandler;
		}
	}
}