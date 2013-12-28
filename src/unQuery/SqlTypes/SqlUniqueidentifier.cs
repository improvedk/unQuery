using System;
using System.Data;

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
	}
}