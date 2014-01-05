using System;
using System.Data;
using Microsoft.SqlServer.Server;

namespace unQuery.SqlTypes
{
	public class SqlDate : ExplicitValueType<DateTime?>
	{
		private SqlDate() :
			base(SqlDbType.Date)
		{ }

		public SqlDate(DateTime? value) :
			base(value, SqlDbType.Date)
		{ }

		private static readonly ITypeHandler typeHandler = new SqlDate();
		internal static ITypeHandler GetTypeHandler()
		{
			return typeHandler;
		}

		public override void SetDataRecordValue(SqlDataRecord record, int ordinal)
		{
			if (Value == null)
				record.SetDBNull(ordinal);
			else
				record.SetDateTime(ordinal, Value.Value);
		}
	}
}