using System.Data;
using Microsoft.SqlServer.Server;

namespace unQuery.SqlTypes
{
	public class SqlReal : ExplicitValueType<float?>
	{
		private SqlReal() :
			base(SqlDbType.Real)
		{ }

		public SqlReal(float? value) :
			base(value, SqlDbType.Real)
		{ }

		private static readonly SqlTypeHandler typeHandler = new SqlReal();
		internal static SqlTypeHandler GetTypeHandler()
		{
			return typeHandler;
		}

		internal override void SetDataRecordValue(SqlDataRecord record, int ordinal)
		{
			if (Value == null)
				record.SetDBNull(ordinal);
			else
				record.SetFloat(ordinal, Value.Value);
		}
	}
}