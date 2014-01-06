using Microsoft.SqlServer.Server;
using System.Data;
using System.Data.SqlClient;

namespace unQuery.SqlTypes
{
	public abstract class ImplicitValueType<TValue> : SqlType
	{
		protected readonly TValue Value;
		private readonly SqlDbType dbType;

		internal ImplicitValueType(SqlDbType dbType)
		{
			this.dbType = dbType;
		}

		internal ImplicitValueType(TValue value, SqlDbType dbType)
		{
			this.Value = value;
			this.dbType = dbType;
		}

		internal override object GetRawValue()
		{
			return Value;
		}

		internal override SqlParameter GetParameter()
		{
			return new SqlParameter {
				SqlDbType = dbType,
				Value = GetDBNullableValue(Value)
			};
		}

		internal override SqlParameter CreateParamFromValue(object value)
		{
			return new SqlParameter {
				SqlDbType = dbType,
				Value = GetDBNullableValue(value)
			};
		}

		internal override SqlMetaData CreateMetaData(string name)
		{
			return new SqlMetaData(name, dbType);
		}
	}
}