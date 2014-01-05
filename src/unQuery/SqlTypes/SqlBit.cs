using System.Data;
using Microsoft.SqlServer.Server;

namespace unQuery.SqlTypes
{
	public class SqlBit : ImplicitValueType<bool?>
	{
		private SqlBit() :
			base(SqlDbType.Bit)
		{ }

		public SqlBit(bool? value) :
			base(value, SqlDbType.Bit)
		{ }

		private static readonly ITypeHandler typeHandler = new SqlBit();
		internal static ITypeHandler GetTypeHandler()
		{
			return typeHandler;
		}

		public override void SetDataRecordValue(SqlDataRecord record, int ordinal)
		{
			if (Value == null)
				record.SetDBNull(ordinal);
			else
				record.SetBoolean(ordinal, Value.Value);
		}
	}
}