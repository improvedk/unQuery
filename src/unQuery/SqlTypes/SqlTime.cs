using System;
using System.Data;
using Microsoft.Data.SqlClient.Server;

namespace unQuery.SqlTypes
{
	public class SqlTime : ExplicitScaleType<TimeSpan?>
	{
		private SqlTime() :
			base(SqlDbType.Time)
		{ }

		internal SqlTime(TimeSpan? value, byte? scale, ParameterDirection direction) :
			base(value, scale, SqlDbType.Time, direction)
		{ }

		private static readonly SqlTypeHandler typeHandler = new SqlTime();
		internal static SqlTypeHandler GetTypeHandler()
		{
			return typeHandler;
		}

		internal override void SetDataRecordValue(SqlDataRecord record, int ordinal)
		{
			if (InputValue == null)
				record.SetDBNull(ordinal);
			else
				record.SetTimeSpan(ordinal, InputValue.Value);
		}
	}
}