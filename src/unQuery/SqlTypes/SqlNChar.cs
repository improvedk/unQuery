using System.Data;
using Microsoft.SqlServer.Server;

namespace unQuery.SqlTypes
{
	public class SqlNChar : ExplicitMaxLengthType<string>
	{
		private SqlNChar() :
			base(SqlDbType.NChar)
		{ }

		public SqlNChar(string value) :
			base(value, SqlDbType.NChar)
		{ }

		public SqlNChar(string value, int maxLength) :
			base(value, SqlDbType.NChar, maxLength: maxLength)
		{ }

		private static readonly SqlTypeHandler typeHandler = new SqlNChar();
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