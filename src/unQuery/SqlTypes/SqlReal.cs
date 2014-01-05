using System.Data;
using Microsoft.SqlServer.Server;

namespace unQuery.SqlTypes
{
	public class SqlReal : ExplicitValueType<float?>
	{
		private SqlReal() :
			base(SqlDbType.Real)
		{ }

		public SqlReal(float? value) :
			base(value, SqlDbType.Real)
		{ }

		private static readonly ITypeHandler typeHandler = new SqlReal();
		internal static ITypeHandler GetTypeHandler()
		{
			return typeHandler;
		}

		public override void SetDataRecordValue(SqlDataRecord record, int ordinal)
		{
			if (Value == null)
				record.SetDBNull(ordinal);
			else
				record.SetFloat(ordinal, Value.Value);
		}
	}
}