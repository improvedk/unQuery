using System.Data;
using Microsoft.SqlServer.Server;

namespace unQuery.SqlTypes
{
	public class SqlTinyInt : ImplicitValueType<byte?>
	{
		private SqlTinyInt() :
			base(SqlDbType.TinyInt)
		{ }

		public SqlTinyInt(byte? value) :
			base(value, SqlDbType.TinyInt)
		{ }

		private static readonly ITypeHandler typeHandler = new SqlTinyInt();
		internal static ITypeHandler GetTypeHandler()
		{
			return typeHandler;
		}

		public override void SetDataRecordValue(SqlDataRecord record, int ordinal)
		{
			if (Value == null)
				record.SetDBNull(ordinal);
			else
				record.SetByte(ordinal, Value.Value);
		}
	}
}