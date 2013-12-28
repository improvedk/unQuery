using Microsoft.SqlServer.Server;
using System.Data;
using System.Data.SqlClient;

namespace unQuery.SqlTypes
{
	public abstract class ExplicitPrecisionAndScaleType<TValue> : SqlType, ISqlType, ITypeHandler
	{
		private readonly TValue value;
		private readonly byte scale;
		private readonly byte precision;
		private readonly bool hasValue;
		private readonly SqlDbType dbType;

		internal ExplicitPrecisionAndScaleType(SqlDbType dbType)
		{
			this.dbType = dbType;
		}

		internal ExplicitPrecisionAndScaleType(TValue value, byte precision, byte scale, SqlDbType dbType)
		{
			this.value = value;
			this.precision = precision;
			this.scale = scale;
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
				Value = GetDBNullableValue(value),
				Precision = precision,
				Scale = scale
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

			return new SqlMetaData(name, dbType, precision, scale);
		}
	}
}