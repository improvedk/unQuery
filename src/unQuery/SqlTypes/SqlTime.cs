using System;
using System.Data;

namespace unQuery.SqlTypes
{
	public class SqlTime : ExplicitScaleType<TimeSpan?>
	{
		private SqlTime() :
			base(SqlDbType.Time)
		{ }

		public SqlTime(TimeSpan? value, byte scale) :
			base(value, scale, SqlDbType.Time)
		{ }

		private static readonly ITypeHandler typeHandler = new SqlTime();
		internal static ITypeHandler GetTypeHandler()
		{
			return typeHandler;
		}
	}
}