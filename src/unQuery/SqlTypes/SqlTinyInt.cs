using Microsoft.SqlServer.Server;
using System.Data;
using System.Data.SqlClient;

namespace unQuery.SqlTypes
{
	public class SqlTinyInt : SqlType, ISqlType, ITypeHandler
	{
		private static readonly ITypeHandler typeHandler = new SqlTinyInt();

		private readonly byte? value;

		private SqlTinyInt()
		{ }

		internal static ITypeHandler GetTypeHandler()
		{
			return typeHandler;
		}

		SqlParameter ITypeHandler.CreateParamFromValue(object value)
		{
			return new SqlParameter {
				SqlDbType = SqlDbType.TinyInt,
				Value = GetDBNullableValue(value)
			};
		}

		SqlMetaData ITypeHandler.CreateMetaData(string name)
		{
			return new SqlMetaData(name, SqlDbType.TinyInt);
		}

		public SqlTinyInt(byte? value)
		{
			this.value = value;
		}

		SqlParameter ISqlType.GetParameter()
		{
			return new SqlParameter {
				SqlDbType = SqlDbType.TinyInt,
				Value = GetDBNullableValue(value)
			};
		}

		object ISqlType.GetRawValue()
		{
			return value;
		}
	}
}