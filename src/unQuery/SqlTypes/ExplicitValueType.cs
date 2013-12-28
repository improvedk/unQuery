using Microsoft.SqlServer.Server;
using System.Data;
using System.Data.SqlClient;

namespace unQuery.SqlTypes
{
	public abstract class ExplicitValueType<TValue> : SqlType, ISqlType, ITypeHandler
	{
		private readonly TValue value;
		private readonly SqlDbType dbType;
		private readonly bool hasValue;

		internal ExplicitValueType(SqlDbType dbType)
		{
			this.dbType = dbType;
		}

		internal ExplicitValueType(TValue value, SqlDbType dbType)
		{
			this.value = value;
			this.dbType = dbType;

			hasValue = true;
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
			throw new TypeCannotBeUsedAsAClrTypeException();
		}

		SqlMetaData ITypeHandler.CreateMetaData(string name)
		{
			if (!hasValue)
				throw new TypeCannotBeUsedAsAClrTypeException();

			return new SqlMetaData(name, dbType);
		}
	}
}