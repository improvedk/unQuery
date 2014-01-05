using System.Data;
using Microsoft.SqlServer.Server;

namespace unQuery.SqlTypes
{
	public class SqlInt : ImplicitValueType<int?>
	{
		private SqlInt() :
			base(SqlDbType.Int)
		{ }

		public SqlInt(int? value) :
			base(value, SqlDbType.Int)
		{ }

		private static readonly ITypeHandler typeHandler = new SqlInt();
		internal static ITypeHandler GetTypeHandler()
		{
			return typeHandler;
		}

		public override void SetDataRecordValue(SqlDataRecord record, int ordinal)
		{
			if (Value == null)
				record.SetDBNull(ordinal);
			else
				record.SetInt32(ordinal, Value.Value);
		}
	}
}