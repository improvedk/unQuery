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

		private static readonly ITypeHandler typeHandler = new SqlNChar();
		internal static ITypeHandler GetTypeHandler()
		{
			return typeHandler;
		}

		public override void SetDataRecordValue(SqlDataRecord record, int ordinal)
		{
			if (Value == null)
				record.SetDBNull(ordinal);
			else
				record.SetString(ordinal, Value);
		}
	}
}