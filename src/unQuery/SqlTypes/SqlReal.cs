using System.Data;
using Microsoft.SqlServer.Server;

namespace unQuery.SqlTypes
{
	public class SqlReal : ExplicitValueType<float?>
	{
		private SqlReal() :
			base(SqlDbType.Real)
		{ }

		internal SqlReal(float? value, ParameterDirection direction) :
			base(value, SqlDbType.Real, direction)
		{ }

		private static readonly SqlTypeHandler typeHandler = new SqlReal();
		internal static SqlTypeHandler GetTypeHandler()
		{
			return typeHandler;
		}

		internal override void SetDataRecordValue(SqlDataRecord record, int ordinal)
		{
			if (InputValue == null)
				record.SetDBNull(ordinal);
			else
				record.SetFloat(ordinal, InputValue.Value);
		}
	}
}