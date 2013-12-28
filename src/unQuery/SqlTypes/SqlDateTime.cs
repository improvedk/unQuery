using System;
using System.Data;

namespace unQuery.SqlTypes
{
	public class SqlDateTime : ExplicitValueType<DateTime?>
	{
		private SqlDateTime() :
			base(SqlDbType.DateTime)
		{ }

		public SqlDateTime(DateTime? value) :
			base(value, SqlDbType.DateTime)
		{ }

		private static readonly ITypeHandler typeHandler = new SqlDateTime();
		internal static ITypeHandler GetTypeHandler()
		{
			return typeHandler;
		}
	}
}