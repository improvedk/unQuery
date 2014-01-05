using Microsoft.SqlServer.Server;
using System.Data;
using System.Data.SqlClient;

namespace unQuery.SqlTypes
{
	public abstract class ImplicitValueType<TValue> : SqlType, ISqlType, ITypeHandler
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

		public abstract void SetDataRecordValue(SqlDataRecord record, int ordinal);

		object ISqlType.GetRawValue()
		{
			return Value;
		}

		SqlParameter ISqlType.GetParameter()
		{
			return new SqlParameter {
				SqlDbType = dbType,
				Value = GetDBNullableValue(Value)
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