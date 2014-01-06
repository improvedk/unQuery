using System;
using System.Data;
using Microsoft.SqlServer.Server;

namespace unQuery.SqlTypes
{
	public class SqlDateTimeOffset : ExplicitScaleType<DateTimeOffset?>
	{
		private SqlDateTimeOffset() :
			base(SqlDbType.DateTimeOffset)
		{ }

		public SqlDateTimeOffset(DateTimeOffset? value) :
			base(value, null, SqlDbType.DateTimeOffset)
		{ }

		public SqlDateTimeOffset(DateTimeOffset? value, byte scale) :
			base(value, scale, SqlDbType.DateTimeOffset)
		{ }

		private static readonly SqlTypeHandler typeHandler = new SqlDateTimeOffset();
		internal static SqlTypeHandler GetTypeHandler()
		{
			return typeHandler;
		}

		internal override void SetDataRecordValue(SqlDataRecord record, int ordinal)
		{
			if (Value == null)
				record.SetDBNull(ordinal);
			else
				record.SetDateTimeOffset(ordinal, Value.Value);
		}
	}
}