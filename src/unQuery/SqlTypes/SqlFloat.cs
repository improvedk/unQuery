using System.Data;
using Microsoft.SqlServer.Server;

namespace unQuery.SqlTypes
{
	public class SqlFloat : ExplicitValueType<double?>
	{
		private SqlFloat() :
			base(SqlDbType.Float)
		{ }

		public SqlFloat(double? value) :
			base(value, SqlDbType.Float)
		{ }

		private static readonly SqlTypeHandler typeHandler = new SqlFloat();
		internal static SqlTypeHandler GetTypeHandler()
		{
			return typeHandler;
		}

		internal override void SetDataRecordValue(SqlDataRecord record, int ordinal)
		{
			if (Value == null)
				record.SetDBNull(ordinal);
			else
				record.SetDouble(ordinal, Value.Value);
		}
	}
}