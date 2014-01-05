using System.Data;
using Microsoft.SqlServer.Server;

namespace unQuery.SqlTypes
{
	public class SqlSmallInt : ImplicitValueType<short?>
	{
		private SqlSmallInt() :
			base(SqlDbType.SmallInt)
		{ }

		public SqlSmallInt(short? value) :
			base(value, SqlDbType.SmallInt)
		{ }

		private static readonly ITypeHandler typeHandler = new SqlSmallInt();
		internal static ITypeHandler GetTypeHandler()
		{
			return typeHandler;
		}

		public override void SetDataRecordValue(SqlDataRecord record, int ordinal)
		{
			if (Value == null)
				record.SetDBNull(ordinal);
			else
				record.SetInt16(ordinal, Value.Value);
		}
	}
}