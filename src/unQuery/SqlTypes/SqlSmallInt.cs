using System.Data;
using Microsoft.Data.SqlClient.Server;

namespace unQuery.SqlTypes
{
	public class SqlSmallInt : ImplicitValueType<short?>
	{
		private SqlSmallInt() :
			base(SqlDbType.SmallInt)
		{ }

		internal SqlSmallInt(short? value, ParameterDirection direction) :
			base(value, SqlDbType.SmallInt, direction)
		{ }

		private static readonly SqlTypeHandler typeHandler = new SqlSmallInt();
		internal static SqlTypeHandler GetTypeHandler()
		{
			return typeHandler;
		}

		internal override void SetDataRecordValue(SqlDataRecord record, int ordinal)
		{
			if (InputValue == null)
				record.SetDBNull(ordinal);
			else
				record.SetInt16(ordinal, InputValue.Value);
		}
	}
}