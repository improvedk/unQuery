using System.Data;
using Microsoft.SqlServer.Server;

namespace unQuery.SqlTypes
{
	public class SqlNChar : ExplicitMaxLengthType<string>
	{
		private SqlNChar() :
			base(SqlDbType.NChar)
		{ }

		internal SqlNChar(string value, int? maxLength, ParameterDirection direction) :
			base(value, SqlDbType.NChar, direction, maxLength: maxLength)
		{ }

		private static readonly SqlTypeHandler typeHandler = new SqlNChar();
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