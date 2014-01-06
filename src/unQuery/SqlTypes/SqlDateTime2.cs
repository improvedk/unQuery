using System;
using System.Data;
using Microsoft.SqlServer.Server;

namespace unQuery.SqlTypes
{
	public class SqlDateTime2 : ExplicitScaleType<DateTime?>
	{
		private SqlDateTime2() :
			base(SqlDbType.DateTime2)
		{ }

		public SqlDateTime2(DateTime? value) :
			base(value, null, SqlDbType.DateTime2)
		{ }

		public SqlDateTime2(DateTime? value, byte scale) :
			base(value, scale, SqlDbType.DateTime2)
		{ }

		private static readonly SqlTypeHandler typeHandler = new SqlDateTime2();
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