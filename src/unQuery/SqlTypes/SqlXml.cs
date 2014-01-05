using System.Data;
using Microsoft.SqlServer.Server;

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

		public override void SetDataRecordValue(SqlDataRecord record, int ordinal)
		{
			if (Value == null)
				record.SetDBNull(ordinal);
			else
				record.SetString(ordinal, Value);
		}
	}
}