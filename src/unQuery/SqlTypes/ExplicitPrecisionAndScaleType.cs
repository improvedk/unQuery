using Microsoft.SqlServer.Server;
using System.Data;
using System.Data.SqlClient;

namespace unQuery.SqlTypes
{
	public abstract class ExplicitPrecisionAndScaleType<TValue> : SqlType, ISqlType, ITypeHandler
	{
		private readonly TValue value;
		private readonly byte? scale;
		private readonly byte? precision;
		private readonly bool hasValue;
		private readonly SqlDbType dbType;

		internal ExplicitPrecisionAndScaleType(SqlDbType dbType)
		{
			this.dbType = dbType;
		}

		internal ExplicitPrecisionAndScaleType(TValue value, byte? precision, byte? scale, SqlDbType dbType)
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
			var param = new SqlParameter {
				SqlDbType = dbType,
				Value = GetDBNullableValue(value)
			};

			if (precision != null && scale != null)
			{
				param.Precision = precision.Value;
				param.Scale = scale.Value;
			}

			return param;
		}

		SqlParameter ITypeHandler.CreateParamFromValue(object value)
		{
			throw new TypeCannotBeUsedAsAClrTypeException();
		}

		SqlMetaData ITypeHandler.CreateMetaData(string name)
		{
			if (!hasValue)
				throw new TypeCannotBeUsedAsAClrTypeException();

			if (precision == null || scale == null)
				throw new TypePropertiesMustBeSetExplicitlyException("precision & scale");

			return new SqlMetaData(name, dbType, precision.Value, scale.Value);
		}
	}
}