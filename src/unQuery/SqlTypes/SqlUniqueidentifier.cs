using System;
using System.Data;
using Microsoft.SqlServer.Server;

namespace unQuery.SqlTypes
{
	public class SqlUniqueIdentifier : ImplicitValueType<Guid?>
	{
		private SqlUniqueIdentifier() :
			base(SqlDbType.UniqueIdentifier)
		{ }

		public SqlUniqueIdentifier(Guid? value) :
			base(value, SqlDbType.UniqueIdentifier)
		{ }

		private static readonly ITypeHandler typeHandler = new SqlUniqueIdentifier();
		internal static ITypeHandler GetTypeHandler()
		{
			return typeHandler;
		}

		public override void SetDataRecordValue(SqlDataRecord record, int ordinal)
		{
			if (Value == null)
				record.SetDBNull(ordinal);
			else
				record.SetGuid(ordinal, Value.Value);
		}
	}
}