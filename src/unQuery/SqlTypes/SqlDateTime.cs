using System;
using System.Data;
using Microsoft.SqlServer.Server;

namespace unQuery.SqlTypes
{
	public class SqlDateTime : ExplicitValueType<DateTime?>
	{
		private SqlDateTime() :
			base(SqlDbType.DateTime)
		{ }

		public SqlDateTime(DateTime? value) :
			base(value, SqlDbType.DateTime)
		{ }

		private static readonly SqlTypeHandler typeHandler = new SqlDateTime();
		internal static SqlTypeHandler GetTypeHandler()
		{
			return typeHandler;
		}

		internal override void SetDataRecordValue(SqlDataRecord record, int ordinal)
		{
			if (Value == null)
				record.SetDBNull(ordinal);
			else
				record.SetDateTime(ordinal, Value.Value);
		}
	}
}