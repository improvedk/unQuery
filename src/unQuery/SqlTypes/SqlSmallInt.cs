using Microsoft.SqlServer.Server;
using System.Data;
using System.Data.SqlClient;

namespace unQuery.SqlTypes
{
	public class SqlSmallInt : SqlType, ISqlType, ITypeHandler
	{
		private static readonly ITypeHandler typeHandler = new SqlSmallInt();

		private readonly short? value;

		private SqlSmallInt()
		{ }

		internal static ITypeHandler GetTypeHandler()
		{
			return typeHandler;
		}

		SqlParameter ITypeHandler.CreateParamFromValue(object value)
		{
			return new SqlParameter {
				SqlDbType = SqlDbType.SmallInt,
				Value = GetDBNullableValue(value)
			};
		}

		SqlMetaData ITypeHandler.CreateMetaData(string name)
		{
			return new SqlMetaData(name, SqlDbType.SmallInt);
		}

		public SqlSmallInt(short? value)
		{
			this.value = value;
		}

		SqlParameter ISqlType.GetParameter()
		{
			return new SqlParameter {
				SqlDbType = SqlDbType.SmallInt,
				Value = GetDBNullableValue(value)
			};
		}

		object ISqlType.GetRawValue()
		{
			return value;
		}
	}
}