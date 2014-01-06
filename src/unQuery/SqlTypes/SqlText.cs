using System.Data;
using Microsoft.SqlServer.Server;

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

		private static readonly SqlTypeHandler typeHandler = new SqlText();
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