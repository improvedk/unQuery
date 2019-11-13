using Microsoft.Data.SqlClient.Server;
using System;
using System.Data;

namespace unQuery.SqlTypes
{
	public class SqlUniqueIdentifier : ImplicitValueType<Guid?>
	{
		private SqlUniqueIdentifier() :
			base(SqlDbType.UniqueIdentifier)
		{ }

		internal SqlUniqueIdentifier(Guid? value, ParameterDirection direction) :
			base(value, SqlDbType.UniqueIdentifier, direction)
		{ }

		private static readonly SqlTypeHandler typeHandler = new SqlUniqueIdentifier();
		internal static SqlTypeHandler GetTypeHandler()
		{
			return typeHandler;
		}

		internal override void SetDataRecordValue(SqlDataRecord record, int ordinal)
		{
			if (InputValue == null)
				record.SetDBNull(ordinal);
			else
				record.SetGuid(ordinal, InputValue.Value);
		}
	}
}