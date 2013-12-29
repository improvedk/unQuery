using System;
using System.Data;

namespace unQuery.SqlTypes
{
	public class SqlSmallDateTime : ExplicitValueType<DateTime?>
	{
		private SqlSmallDateTime() :
			base(SqlDbType.SmallDateTime)
		{ }

		public SqlSmallDateTime(DateTime? value) :
			base(value, SqlDbType.SmallDateTime)
		{ }

		private static readonly ITypeHandler typeHandler = new SqlSmallDateTime();
		internal static ITypeHandler GetTypeHandler()
		{
			return typeHandler;
		}
	}
}