using System.Data;
using Microsoft.Data.SqlClient.Server;

namespace unQuery.SqlTypes
{
	public class SqlBigInt : ImplicitValueType<long?>
	{
		private SqlBigInt() :
			base(SqlDbType.BigInt)
		{ }

		internal SqlBigInt(long? value, ParameterDirection direction) :
			base(value, SqlDbType.BigInt, direction)
		{ }

		private static readonly SqlType typeHandler = new SqlBigInt();
		internal static SqlType GetTypeHandler()
		{
			return typeHandler;
		}

		internal override void SetDataRecordValue(SqlDataRecord record, int ordinal)
		{
			if (InputValue == null)
				record.SetDBNull(ordinal);
			else
				record.SetInt64(ordinal, InputValue.Value);
		}
	}
}