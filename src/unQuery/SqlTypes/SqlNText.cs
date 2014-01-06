using System.Data;
using Microsoft.SqlServer.Server;

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

		private static readonly SqlTypeHandler typeHandler = new SqlNText();
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