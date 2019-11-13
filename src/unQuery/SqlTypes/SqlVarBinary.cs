using Microsoft.Data.SqlClient.Server;
using System.Data;

namespace unQuery.SqlTypes
{
	public class SqlVarBinary : ExplicitMaxLengthType<byte[]>
	{
		private SqlVarBinary() :
			base(SqlDbType.VarBinary)
		{ }

		internal SqlVarBinary(byte[] value, int? maxLength, ParameterDirection direction) :
			base(value, SqlDbType.VarBinary, direction, maxLength: maxLength, valueLength: (value != null ? value.Length : 0))
		{ }

		private static readonly SqlTypeHandler typeHandler = new SqlVarBinary();
		internal static SqlTypeHandler GetTypeHandler()
		{
			return typeHandler;
		}

		internal override void SetDataRecordValue(SqlDataRecord record, int ordinal)
		{
			if (InputValue == null)
				record.SetDBNull(ordinal);
			else
				record.SetBytes(ordinal, 0, InputValue, 0, InputValue.Length);
		}
	}
}