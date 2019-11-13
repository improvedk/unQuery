using System;
using System.Data;
using Microsoft.Data.SqlClient.Server;

namespace unQuery.SqlTypes
{
	public class SqlDateTime : ExplicitValueType<DateTime?>
	{
		private SqlDateTime() :
			base(SqlDbType.DateTime)
		{ }

		internal SqlDateTime(DateTime? value, ParameterDirection direction) :
			base(value, SqlDbType.DateTime, direction)
		{ }

		private static readonly SqlTypeHandler typeHandler = new SqlDateTime();
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