using Microsoft.SqlServer.Server;
using System.Data;
using System.Data.SqlClient;

namespace unQuery.SqlTypes
{
	public abstract class ImplicitValueType<TValue> : SqlType, ISqlType, ITypeHandler
	{
		private readonly TValue value;
		private readonly SqlDbType dbType;

		internal ImplicitValueType(SqlDbType dbType)
		{
			this.dbType = dbType;
		}

		internal ImplicitValueType(TValue value, SqlDbType dbType)
		{
			this.value = value;
			this.dbType = dbType;
		}

		object ISqlType.GetRawValue()
		{
			return value;
		}

		SqlParameter ISqlType.GetParameter()
		{
			return new SqlParameter {
				SqlDbType = dbType,
				Value = GetDBNullableValue(value)
			};
		}

		SqlParameter ITypeHandler.CreateParamFromValue(object value)
		{
			return new SqlParameter {
				SqlDbType = dbType,
				Value = GetDBNullableValue(value)
			};
		}

		SqlMetaData ITypeHandler.CreateMetaData(string name)
		{
			return new SqlMetaData(name, dbType);
		}
	}
}