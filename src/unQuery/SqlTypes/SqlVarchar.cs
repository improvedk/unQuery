using System.Data;
using Microsoft.SqlServer.Server;

namespace unQuery.SqlTypes
{
	public class SqlVarChar : ExplicitMaxLengthType<string>
	{
		private SqlVarChar() :
			base(SqlDbType.VarChar)
		{ }

		public SqlVarChar(string value) :
			base(value, SqlDbType.VarChar, valueLength: (value != null ? value.Length : 0))
		{ }

		public SqlVarChar(string value, int maxLength) :
			base(value, SqlDbType.VarChar, maxLength: maxLength)
		{ }

		private static readonly ITypeHandler typeHandler = new SqlVarChar();
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