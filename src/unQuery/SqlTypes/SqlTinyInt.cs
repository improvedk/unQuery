using System.Data;
using Microsoft.SqlServer.Server;

namespace unQuery.SqlTypes
{
	public class SqlTinyInt : ImplicitValueType<byte?>
	{
		private SqlTinyInt() :
			base(SqlDbType.TinyInt)
		{ }

		internal SqlTinyInt(byte? value, ParameterDirection direction) :
			base(value, SqlDbType.TinyInt, direction)
		{ }

		private static readonly SqlTypeHandler typeHandler = new SqlTinyInt();
		internal static SqlTypeHandler GetTypeHandler()
		{
			return typeHandler;
		}

		internal override void SetDataRecordValue(SqlDataRecord record, int ordinal)
		{
			if (InputValue == null)
				record.SetDBNull(ordinal);
			else
				record.SetByte(ordinal, InputValue.Value);
		}
	}
}