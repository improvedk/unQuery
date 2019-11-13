using System;
using System.Data;
using Microsoft.Data.SqlClient.Server;

namespace unQuery.SqlTypes
{
	public class SqlDateTime2 : ExplicitScaleType<DateTime?>
	{
		private SqlDateTime2() :
			base(SqlDbType.DateTime2)
		{ }

		internal SqlDateTime2(DateTime? value, byte? scale, ParameterDirection direction) :
			base(value, scale, SqlDbType.DateTime2, direction)
		{ }

		private static readonly SqlTypeHandler typeHandler = new SqlDateTime2();
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