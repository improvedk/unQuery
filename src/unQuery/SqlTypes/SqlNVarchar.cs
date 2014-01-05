using System.Data;
using Microsoft.SqlServer.Server;

namespace unQuery.SqlTypes
{
	public class SqlNVarChar : ExplicitMaxLengthType<string>
	{
		private SqlNVarChar() :
			base(SqlDbType.NVarChar)
		{ }

		public SqlNVarChar(string value) :
			base(value, SqlDbType.NVarChar, valueLength: (value != null ? value.Length : 0))
		{ }

		public SqlNVarChar(string value, int maxLength) :
			base(value, SqlDbType.NVarChar, maxLength: maxLength)
		{ }

		private static readonly ITypeHandler typeHandler = new SqlNVarChar();
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