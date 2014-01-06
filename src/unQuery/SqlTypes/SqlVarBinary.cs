using System.Data;
using Microsoft.SqlServer.Server;

namespace unQuery.SqlTypes
{
	public class SqlVarBinary : ExplicitMaxLengthType<byte[]>
	{
		private SqlVarBinary() :
			base(SqlDbType.VarBinary)
		{ }

		public SqlVarBinary(byte[] value) :
			base(value, SqlDbType.VarBinary, valueLength: (value != null ? value.Length : 0))
		{ }

		public SqlVarBinary(byte[] value, int maxLength) :
			base(value, SqlDbType.VarBinary, maxLength: maxLength)
		{ }

		private static readonly SqlTypeHandler typeHandler = new SqlVarBinary();
		internal static SqlTypeHandler GetTypeHandler()
		{
			return typeHandler;
		}

		internal override void SetDataRecordValue(SqlDataRecord record, int ordinal)
		{
			if (Value == null)
				record.SetDBNull(ordinal);
			else
				record.SetBytes(ordinal, 0, Value, 0, Value.Length);
		}
	}
}