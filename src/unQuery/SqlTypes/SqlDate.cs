using System;
using System.Data;

namespace unQuery.SqlTypes
{
	public class SqlDate : ExplicitValueType<DateTime?>
	{
		private SqlDate() :
			base(SqlDbType.Date)
		{ }

		public SqlDate(DateTime? value) :
			base(value, SqlDbType.Date)
		{ }

		private static readonly ITypeHandler typeHandler = new SqlDate();
		internal static ITypeHandler GetTypeHandler()
		{
			return typeHandler;
		}
	}
}