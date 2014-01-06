using System.Data;
using Microsoft.SqlServer.Server;

namespace unQuery.SqlTypes
{
	public class SqlBigInt : ImplicitValueType<long?>
	{
		private SqlBigInt() :
			base(SqlDbType.BigInt)
		{ }

		public SqlBigInt(long? value) :
			base(value, SqlDbType.BigInt)
		{ }

		private static readonly SqlType typeHandler = new SqlBigInt();
		internal static SqlType GetTypeHandler()
		{
			return typeHandler;
		}

		internal override void SetDataRecordValue(SqlDataRecord record, int ordinal)
		{
			if (Value == null)
				record.SetDBNull(ordinal);
			else
				record.SetInt64(ordinal, Value.Value);
		}
	}
}