using System;
using System.Data;
using Microsoft.Data.SqlClient.Server;

namespace unQuery.SqlTypes
{
	public class SqlSmallDateTime : ExplicitValueType<DateTime?>
	{
		private SqlSmallDateTime() :
			base(SqlDbType.SmallDateTime)
		{ }

		internal SqlSmallDateTime(DateTime? value, ParameterDirection direction) :
			base(value, SqlDbType.SmallDateTime, direction)
		{ }

		private static readonly SqlTypeHandler typeHandler = new SqlSmallDateTime();
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