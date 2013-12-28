using System.Data;

namespace unQuery.SqlTypes
{
	public class SqlVarChar : ExplicitMaxLengthType<string>
	{
		private SqlVarChar() :
			base(SqlDbType.VarChar)
		{ }

		public SqlVarChar(string value, int maxLength) :
			base(value, maxLength, SqlDbType.VarChar)
		{ }

		private static readonly ITypeHandler typeHandler = new SqlVarChar();
		internal static ITypeHandler GetTypeHandler()
		{
			return typeHandler;
		}
	}
}