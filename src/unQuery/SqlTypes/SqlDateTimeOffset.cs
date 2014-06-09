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

		internal SqlDateTimeOffset(DateTimeOffset? value, byte? scale, ParameterDirection direction) :
			base(value, scale, SqlDbType.DateTimeOffset, direction)
		{ }

		private static readonly SqlTypeHandler typeHandler = new SqlDateTimeOffset();
		internal static SqlTypeHandler GetTypeHandler()
		{
			return typeHandler;
		}

		internal override void SetDataRecordValue(SqlDataRecord record, int ordinal)
		{
			if (InputValue == null)
				record.SetDBNull(ordinal);
			else
				record.SetDateTimeOffset(ordinal, InputValue.Value);
		}
	}
}