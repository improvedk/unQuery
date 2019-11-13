using Microsoft.Data.SqlClient.Server;
using System.Data;

namespace unQuery.SqlTypes
{
	public class SqlBinary : ExplicitMaxLengthType<byte[]>
	{
		private SqlBinary() :
			base(SqlDbType.Binary)
		{ }

		internal SqlBinary(byte[] value, int? maxLength, ParameterDirection direction) :
			base(value, SqlDbType.Binary, direction, maxLength: maxLength)
		{ }

		private static readonly SqlTypeHandler typeHandler = new SqlBinary();
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