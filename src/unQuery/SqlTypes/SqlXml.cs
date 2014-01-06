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

		private static readonly SqlTypeHandler typeHandler = new SqlXml();
		internal static SqlTypeHandler GetTypeHandler()
		{
			return typeHandler;
		}

		internal override void SetDataRecordValue(SqlDataRecord record, int ordinal)
		{
			if (Value == null)
				record.SetDBNull(ordinal);
			else
				record.SetString(ordinal, Value);
		}
	}
}