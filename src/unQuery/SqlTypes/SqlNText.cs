using System.Data;
using Microsoft.Data.SqlClient.Server;

namespace unQuery.SqlTypes
{
	public class SqlNText : ExplicitMaxLengthType<string>
	{
		private SqlNText() :
			base(SqlDbType.NText)
		{ }

		internal SqlNText(string value, ParameterDirection direction) :
			base(value, SqlDbType.NText, direction, maxLength: -1)
		{ }

		private static readonly SqlTypeHandler typeHandler = new SqlNText();
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