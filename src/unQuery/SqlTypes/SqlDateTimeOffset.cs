using System;
using System.Data;

namespace unQuery.SqlTypes
{
	public class SqlDateTimeOffset : ExplicitScaleType<DateTimeOffset?>
	{
		private SqlDateTimeOffset() :
			base(SqlDbType.DateTimeOffset)
		{ }

		public SqlDateTimeOffset(DateTimeOffset? value) :
			base(value, null, SqlDbType.DateTimeOffset)
		{ }

		public SqlDateTimeOffset(DateTimeOffset? value, byte scale) :
			base(value, scale, SqlDbType.DateTimeOffset)
		{ }

		private static readonly ITypeHandler typeHandler = new SqlDateTimeOffset();
		internal static ITypeHandler GetTypeHandler()
		{
			return typeHandler;
		}
	}
}