using System.Data;
using Microsoft.Data.SqlClient.Server;

namespace unQuery.SqlTypes
{
	public class SqlFloat : ExplicitValueType<double?>
	{
		private SqlFloat() :
			base(SqlDbType.Float)
		{ }

		internal SqlFloat(double? value, ParameterDirection direction) :
			base(value, SqlDbType.Float, direction)
		{ }

		private static readonly SqlTypeHandler typeHandler = new SqlFloat();
		internal static SqlTypeHandler GetTypeHandler()
		{
			return typeHandler;
		}

		internal override void SetDataRecordValue(SqlDataRecord record, int ordinal)
		{
			if (InputValue == null)
				record.SetDBNull(ordinal);
			else
				record.SetDouble(ordinal, InputValue.Value);
		}
	}
}