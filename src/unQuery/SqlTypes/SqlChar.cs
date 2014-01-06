using System.Data;
using Microsoft.SqlServer.Server;

namespace unQuery.SqlTypes
{
	public class SqlChar : ExplicitMaxLengthType<string>
	{
		private SqlChar() :
			base(SqlDbType.Char)
		{ }

		public SqlChar(string value) :
			base(value, SqlDbType.Char)
		{ }

		public SqlChar(string value, int maxLength) :
			base(value, SqlDbType.Char, maxLength: maxLength)
		{ }

		private static readonly SqlTypeHandler typeHandler = new SqlChar();
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