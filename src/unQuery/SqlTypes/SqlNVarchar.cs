using Microsoft.SqlServer.Server;
using System.Data;

namespace unQuery.SqlTypes
{
	public class SqlNVarChar : ExplicitMaxLengthType<string>
	{
		private SqlNVarChar() :
			base(SqlDbType.NVarChar)
		{ }

		internal SqlNVarChar(string value, int? maxLength, ParameterDirection direction) :
			base(value, SqlDbType.NVarChar, direction, maxLength: maxLength, valueLength: (value != null ? value.Length : 0))
		{ }

		private static readonly SqlTypeHandler typeHandler = new SqlNVarChar();
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