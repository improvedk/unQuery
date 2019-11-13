using System.Data;
using Microsoft.Data.SqlClient.Server;

namespace unQuery.SqlTypes
{
	public class SqlBit : ImplicitValueType<bool?>
	{
		private SqlBit() :
			base(SqlDbType.Bit)
		{ }

		internal SqlBit(bool? value, ParameterDirection direction) :
			base(value, SqlDbType.Bit, direction)
		{ }

		private static readonly SqlTypeHandler typeHandler = new SqlBit();
		internal static SqlTypeHandler GetTypeHandler()
		{
			return typeHandler;
		}

		internal override void SetDataRecordValue(SqlDataRecord record, int ordinal)
		{
			if (InputValue == null)
				record.SetDBNull(ordinal);
			else
				record.SetBoolean(ordinal, InputValue.Value);
		}
	}
}