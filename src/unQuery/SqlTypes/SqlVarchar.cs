using Microsoft.Data.SqlClient.Server;
using System.Data;

namespace unQuery.SqlTypes
{
	public class SqlVarChar : ExplicitMaxLengthType<string>
	{
		private SqlVarChar() :
			base(SqlDbType.VarChar)
		{ }

		internal SqlVarChar(string value, int? maxLength, ParameterDirection direction) :
			base(value, SqlDbType.VarChar, direction, maxLength: maxLength, valueLength: (value != null ? value.Length : 0))
		{ }

		private static readonly SqlTypeHandler typeHandler = new SqlVarChar();
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