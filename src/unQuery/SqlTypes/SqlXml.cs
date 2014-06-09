using Microsoft.SqlServer.Server;
using System.Data;

namespace unQuery.SqlTypes
{
	public class SqlXml : ExplicitValueType<string>
	{
		private SqlXml() :
			base(SqlDbType.Xml)
		{ }

		internal SqlXml(string value, ParameterDirection direction) :
			base(value, SqlDbType.Xml, direction)
		{ }

		private static readonly SqlTypeHandler typeHandler = new SqlXml();
		internal static SqlTypeHandler GetTypeHandler()
		{
			return typeHandler;
		}

		internal override void SetDataRecordValue(SqlDataRecord record, int ordinal)
		{
			if (InputValue == null)
				record.SetDBNull(ordinal);
			else
				record.SetString(ordinal, InputValue);
		}
	}
}