using System;
using System.Data;
using Microsoft.SqlServer.Server;

namespace unQuery.SqlTypes
{
	public class SqlSmallDateTime : ExplicitValueType<DateTime?>
	{
		private SqlSmallDateTime() :
			base(SqlDbType.SmallDateTime)
		{ }

		public SqlSmallDateTime(DateTime? value) :
			base(value, SqlDbType.SmallDateTime)
		{ }

		private static readonly ITypeHandler typeHandler = new SqlSmallDateTime();
		internal static ITypeHandler GetTypeHandler()
		{
			return typeHandler;
		}

		public override void SetDataRecordValue(SqlDataRecord record, int ordinal)
		{
			if (Value == null)
				record.SetDBNull(ordinal);
			else
				record.SetDateTime(ordinal, Value.Value);
		}
	}
}