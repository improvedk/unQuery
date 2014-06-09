using System.Data;
using Microsoft.SqlServer.Server;

namespace unQuery.SqlTypes
{
	public class SqlInt : ImplicitValueType<int?>
	{
		private SqlInt() :
			base(SqlDbType.Int)
		{ }

		internal SqlInt(int? value, ParameterDirection direction) :
			base(value, SqlDbType.Int, direction)
		{ }

		private static readonly SqlTypeHandler typeHandler = new SqlInt();
		internal static SqlTypeHandler GetTypeHandler()
		{
			return typeHandler;
		}

		internal override void SetDataRecordValue(SqlDataRecord record, int ordinal)
		{
			if (InputValue == null)
				record.SetDBNull(ordinal);
			else
				record.SetInt32(ordinal, InputValue.Value);
		}
	}
}