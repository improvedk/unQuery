using System.Data;
using Microsoft.SqlServer.Server;

namespace unQuery.SqlTypes
{
	public class SqlBinary : ExplicitMaxLengthType<byte[]>
	{
		private SqlBinary() :
			base(SqlDbType.Binary)
		{ }

		public SqlBinary(byte[] value) :
			base(value, SqlDbType.Binary)
		{ }

		public SqlBinary(byte[] value, int maxLength) :
			base(value, SqlDbType.Binary, maxLength: maxLength)
		{ }

		private static readonly ITypeHandler typeHandler = new SqlBinary();
		internal static ITypeHandler GetTypeHandler()
		{
			return typeHandler;
		}

		public override void SetDataRecordValue(SqlDataRecord record, int ordinal)
		{
			if (Value == null)
				record.SetDBNull(ordinal);
			else
				record.SetBytes(ordinal, 0, Value, 0, Value.Length);
		}
	}
}