using System.Data;
using Microsoft.SqlServer.Server;

namespace unQuery.SqlTypes
{
	public class SqlChar : ExplicitMaxLengthType<string>
	{
		private SqlChar() :
			base(SqlDbType.Char)
		{ }

		internal SqlChar(string value, int? maxLength, ParameterDirection direction) :
			base(value, SqlDbType.Char, direction, maxLength: maxLength)
		{ }

		private static readonly SqlTypeHandler typeHandler = new SqlChar();
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