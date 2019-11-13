using System;
using System.Data;
using Microsoft.Data.SqlClient.Server;

namespace unQuery.SqlTypes
{
	public class SqlDate : ExplicitValueType<DateTime?>
	{
		private SqlDate() :
			base(SqlDbType.Date)
		{ }

		internal SqlDate(DateTime? value, ParameterDirection direction) :
			base(value, SqlDbType.Date, direction)
		{ }

		private static readonly SqlTypeHandler typeHandler = new SqlDate();
		internal static SqlTypeHandler GetTypeHandler()
		{
			return typeHandler;
		}

		internal override void SetDataRecordValue(SqlDataRecord record, int ordinal)
		{
			if (InputValue == null)
				record.SetDBNull(ordinal);
			else
				record.SetDateTime(ordinal, InputValue.Value);
		}
	}
}