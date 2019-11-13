using System.Data;
using Microsoft.Data.SqlClient.Server;

namespace unQuery.SqlTypes
{
	public class SqlText : ExplicitMaxLengthType<string>
	{
		private SqlText() :
			base(SqlDbType.Text)
		{ }

		internal SqlText(string value, ParameterDirection direction) :
			base(value, SqlDbType.Text, direction, maxLength: -1)
		{ }

		private static readonly SqlTypeHandler typeHandler = new SqlText();
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