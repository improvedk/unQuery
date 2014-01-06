using System;
using System.Data;
using Microsoft.SqlServer.Server;

namespace unQuery.SqlTypes
{
	public class SqlTime : ExplicitScaleType<TimeSpan?>
	{
		private SqlTime() :
			base(SqlDbType.Time)
		{ }

		public SqlTime(TimeSpan? value) :
			base(value, null, SqlDbType.Time)
		{ }

		public SqlTime(TimeSpan? value, byte scale) :
			base(value, scale, SqlDbType.Time)
		{ }

		private static readonly SqlTypeHandler typeHandler = new SqlTime();
		internal static SqlTypeHandler GetTypeHandler()
		{
			return typeHandler;
		}

		internal override void SetDataRecordValue(SqlDataRecord record, int ordinal)
		{
			if (Value == null)
				record.SetDBNull(ordinal);
			else
				record.SetTimeSpan(ordinal, Value.Value);
		}
	}
}